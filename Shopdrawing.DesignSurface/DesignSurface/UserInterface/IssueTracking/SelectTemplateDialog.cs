// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.IssueTracking.SelectTemplateDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.IssueTracking;
using Microsoft.Expression.Project;
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
  public class SelectTemplateDialog : Dialog, IComponentConnector
  {
    private IEnumerable<ITrackedItemType> types;
    private IServiceProvider services;
    internal ComboBox templateComboBox;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public IList<ITrackedItemType> AvailableTemplates
    {
      get
      {
        return (IList<ITrackedItemType>) new List<ITrackedItemType>(this.types);
      }
    }

    public ITrackedItemType SelectedTemplate
    {
      get
      {
        return this.templateComboBox.SelectedItem as ITrackedItemType;
      }
      set
      {
        if (this.types == null)
          return;
        int num = 0;
        foreach (ITrackedItemType trackedItemType in this.types)
        {
          if (trackedItemType == value)
          {
            this.templateComboBox.SelectedIndex = num;
            break;
          }
          ++num;
        }
      }
    }

    private IConfigurationObject SolutionSettings
    {
      get
      {
        return ((IProjectManager) this.services.GetService(typeof (IProjectManager))).CurrentSolution.SolutionSettingsManager.SolutionSettings;
      }
    }

    public SelectTemplateDialog(IEnumerable<ITrackedItemType> types, IServiceProvider services)
    {
      this.InitializeComponent();
      this.DataContext = (object) this;
      this.types = types;
      this.services = services;
      this.AcceptButton.Click += new RoutedEventHandler(this.Button_Click);
      this.CancelButton.Click += new RoutedEventHandler(this.Button_Click);
      this.Title = ((IExpressionInformationService) services.GetService(typeof (IExpressionInformationService))).DefaultDialogTitle;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = new bool?(sender == this.AcceptButton);
      if (this.DialogResult.Value && this.SelectedTemplate != null)
        this.SetDefaultWorkItemTypeInSuo(this.SelectedTemplate.Name);
      this.Close();
    }

    private void SetDefaultWorkItemTypeInSuo(string templateName)
    {
      this.SolutionSettings.SetProperty("DefaultTfsWorkItemTemplate", (object) templateName);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/issuetracking/selecttemplatedialog.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.templateComboBox = (ComboBox) target;
          break;
        case 2:
          this.AcceptButton = (Button) target;
          break;
        case 3:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
