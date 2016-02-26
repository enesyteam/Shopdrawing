// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.IssueTracking.RegisterItemDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Extensions.ServiceProvider;
using Microsoft.Expression.Framework.IssueTracking;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.IssueTracking
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class RegisterItemDialog : Dialog, IComponentConnector
  {
    private const string heightConfigPropertyName = "Height";
    private const string widthConfigPropertyName = "Width";
    private const int defaultWidth = 750;
    private const int defaultHeight = 645;
    private ITrackedItemType type;
    private IServiceProvider services;
    private IConfigurationObject configuration;
    private IList<ITrackedItemType> allTypes;
    private string defaultTitle;
    private string defaultDescription;
    private string[] fileAttachments;
    internal Border BugForm;
    internal Button ChooseButton;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public bool DialogCanceled { get; private set; }

    public ITrackedItem Item
    {
      get
      {
        return this.RegistrationForm.TrackedItem;
      }
      set
      {
        this.RegistrationForm.TrackedItem = value;
      }
    }

    public TrackedItemFormBase RegistrationForm { get; private set; }

    public RegisterItemDialog(IServiceProvider services, ITrackedItemType type, IList<ITrackedItemType> allTypes, string defaultTitle, string defaultDescription, string[] fileAttachments)
    {
      this.services = services;
      this.configuration = ServiceProviderExtensions.GetService<IConfigurationService>(this.services)["SaveTFSWorkItem"];
      this.defaultTitle = defaultTitle;
      this.defaultDescription = defaultDescription;
      this.fileAttachments = fileAttachments;
      this.allTypes = allTypes;
      this.InitializeComponent();
      this.DataContext = (object) this;
      this.Title = StringTable.RegisterTFSItemDialogTitle;
      this.type = type;
      this.AcceptButton.Click += new RoutedEventHandler(this.Button_Click);
      this.CancelButton.Click += new RoutedEventHandler(this.Button_Click);
      this.ChooseButton.Click += new RoutedEventHandler(this.Button_Click);
      this.RegenerateForm(type);
      this.Height = (double) this.configuration.GetProperty("Height", (object) 645.0);
      this.Width = (double) this.configuration.GetProperty("Width", (object) 750.0);
      this.SizeChanged += new SizeChangedEventHandler(this.RegisterItemDialog_SizeChanged);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);
      this.Owner.Activate();
      this.configuration.SetProperty("Height", (object) this.Height);
      this.configuration.SetProperty("Width", (object) this.Width);
    }

    private void RegisterItemDialog_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.RegistrationForm == null)
        return;
      FrameworkElement frameworkElement = this.RegistrationForm.Content as FrameworkElement;
      if (frameworkElement == null)
        return;
      frameworkElement.Width = this.BugForm.ActualWidth;
      frameworkElement.Height = this.BugForm.ActualHeight;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (sender == this.ChooseButton)
      {
        SelectTemplateDialog selectTemplateDialog = new SelectTemplateDialog((IEnumerable<ITrackedItemType>) this.allTypes, this.services);
        selectTemplateDialog.SelectedTemplate = this.type;
        bool? nullable = selectTemplateDialog.ShowDialog();
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? true : false) : 0) == 0)
          return;
        this.RegenerateForm(selectTemplateDialog.SelectedTemplate);
      }
      else
      {
        this.DialogResult = new bool?(sender != this.CancelButton);
        this.Close();
      }
    }

    private void RegenerateForm(ITrackedItemType newType)
    {
      Dictionary<IField, object> initialValues = new Dictionary<IField, object>();
      this.type = newType;
      IField field1 = newType.GetField(ItemField.Title);
      if (field1 != null)
        initialValues.Add(field1, (object) this.defaultTitle);
      IField field2 = newType.GetField(ItemField.Description);
      if (field2 != null)
        initialValues.Add(field2, (object) this.defaultDescription);
      IField field3 = newType.GetField(ItemField.ReproSteps);
      if (field3 != null)
        initialValues.Add(field3, (object) this.defaultDescription.Replace(Environment.NewLine, "<br />"));
      this.RegistrationForm = newType.ProductGroup.GenerateWorkItemForm(newType, initialValues, this.fileAttachments);
      this.BugForm.Child = (UIElement) this.RegistrationForm;
      this.RegisterItemDialog_SizeChanged((object) null, (SizeChangedEventArgs) null);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/issuetracking/registeritemdialog.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.BugForm = (Border) target;
          break;
        case 2:
          this.ChooseButton = (Button) target;
          break;
        case 3:
          this.AcceptButton = (Button) target;
          break;
        case 4:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
