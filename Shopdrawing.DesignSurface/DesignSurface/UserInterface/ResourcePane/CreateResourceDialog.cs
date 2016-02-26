// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.CreateResourceDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Documents.Commands;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class CreateResourceDialog : Dialog
  {
    private CreateResourceModel model;
    protected DesignerContext designerContext;
    private Button newResourceDictionaryButton;
    private MessageBubbleHelper keyNameWarningMessageBubble;
    private RadioButton applicationRootRadioButton;
    private RadioButton thisDocumentRadioButton;
    private RadioButton otherDocumentRadioButton;
    protected FrameworkElement content;

    public CreateResourceModel Model
    {
      get
      {
        return this.model;
      }
    }

    internal CreateResourceDialog(DesignerContext designerContext, CreateResourceModel model)
    {
      this.model = model;
      this.designerContext = designerContext;
      this.content = Microsoft.Expression.DesignSurface.FileTable.GetElement("Resources\\ResourcePane\\CreateResourceDialog.xaml");
      this.content.DataContext = (object) model;
      this.DialogContent = (UIElement) this.content;
      this.newResourceDictionaryButton = (Button) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "NewResourceDialog");
      this.newResourceDictionaryButton.Click += new RoutedEventHandler(this.NewResourceDictionaryButton_Click);
      ((UIElement) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "DefineIn_WhichResDict")).GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.defineInThisDocumentComboBox_GotKeyboardFocus);
      ((UIElement) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "DefineIn_WhichDocument")).GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.defineInOtherDocumentComboBox_GotKeyboardFocus);
      this.applicationRootRadioButton = (RadioButton) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "DefineIn_ApplicationRoot");
      this.applicationRootRadioButton.Checked += new RoutedEventHandler(this.OnDefineRadioButtonCheck);
      this.thisDocumentRadioButton = (RadioButton) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "DefineIn_ThisDocument");
      this.thisDocumentRadioButton.Checked += new RoutedEventHandler(this.OnDefineRadioButtonCheck);
      this.otherDocumentRadioButton = (RadioButton) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "DefineIn_OtherDocument");
      this.otherDocumentRadioButton.Checked += new RoutedEventHandler(this.OnDefineRadioButtonCheck);
      this.Title = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.model.CanPickResourceOrName ? StringTable.CreateResourceDialogTitleResourceOptional : StringTable.CreateResourceDialogTitle, new object[1]
      {
        (object) this.model.ResourceType.Name
      });
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.keyNameWarningMessageBubble = new MessageBubbleHelper(LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "Key_IsNamed") as UIElement, (IMessageBubbleValidator) new CreateResourceDialog.KeyNameValidator(this.model));
      if (!this.model.CanPickScope)
        return;
      if (this.model.SelectedLocation == this.model.ThisDocumentResourceDictionaries)
        this.thisDocumentRadioButton.IsChecked = new bool?(true);
      else if (this.model.SelectedLocation == this.model.ApplicationDocument)
        this.applicationRootRadioButton.IsChecked = new bool?(true);
      else
        this.otherDocumentRadioButton.IsChecked = new bool?(true);
    }

    private void defineInThisDocumentComboBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.thisDocumentRadioButton.IsChecked = new bool?(true);
    }

    private void defineInOtherDocumentComboBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      this.otherDocumentRadioButton.IsChecked = new bool?(true);
    }

    private void NewResourceDictionaryButton_Click(object sender, RoutedEventArgs e)
    {
      IProjectItem newResourceItem = (IProjectItem) null;
      EventHandler<ProjectItemEventArgs> eventHandler = (EventHandler<ProjectItemEventArgs>) ((o, args) => newResourceItem = args.ProjectItem);
      IProject activeProject = this.designerContext.ActiveProject;
      if (activeProject != null)
      {
        AddNewItemCommand addNewItemCommand = new AddNewItemCommand(this.designerContext, 0 != 0, new string[1]
        {
          "ResourceDictionary.xaml"
        });
        addNewItemCommand.SetProperty("TemporarilyStopProjectPaneActivation", (object) true);
        activeProject.ItemAdded += eventHandler;
        activeProject.ItemChanged += eventHandler;
        addNewItemCommand.ExecuteWithProject(activeProject);
        activeProject.ItemAdded -= eventHandler;
        activeProject.ItemChanged -= eventHandler;
      }
      if (newResourceItem == null || this.designerContext.ResourceManager.TopLevelResourceContainer == null)
        return;
      this.model.SelectedLocation = (object) this.model.OtherDocuments;
      this.otherDocumentRadioButton.IsChecked = new bool?(true);
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() =>
      {
        this.model.UpdateResourceList(this.designerContext.ResourceManager);
        this.model.SelectedExternalResourceDictionaryFile = this.designerContext.ResourceManager.FindResourceContainer(newResourceItem.DocumentReference.Path);
      }));
    }

    public void OnDefineRadioButtonCheck(object sender, RoutedEventArgs args)
    {
      RadioButton radioButton = (RadioButton) sender;
      if (radioButton.Tag == null)
        return;
      this.model.SelectedLocation = radioButton.Tag;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      string[] strArray = new string[5]
      {
        "Key_IsNamed",
        "Key_ApplyAuto",
        "DefineIn_ApplicationRoot",
        "DefineIn_ThisDocument",
        "DefineIn_OtherDocument"
      };
      foreach (string elementName in strArray)
        ((RadioButton) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, elementName)).GroupName = (string) null;
      base.OnClosing(e);
    }

    protected override void OnAcceptButtonExecute()
    {
      SceneDocument externalDocument = this.model.ExternalDocument;
      if (externalDocument != null)
      {
        SceneViewModel viewModel = this.designerContext.ActiveSceneViewModel.GetViewModel(externalDocument.DocumentRoot, false);
        if (viewModel == null || !viewModel.DefaultView.IsValid)
        {
          this.designerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ResourceItemAddFailed_TargetDictionaryHasErrorsMessage, new object[1]
          {
            (object) Path.GetFileName(this.model.ExternalDocument.DocumentReference.DisplayName)
          }));
          return;
        }
      }
      this.Close(new bool?(true));
    }

    private class KeyNameValidator : IMessageBubbleValidator
    {
      private CreateResourceModel model;
      private IMessageBubbleHelper helper;

      public KeyNameValidator(CreateResourceModel model)
      {
        this.model = model;
        this.model.PropertyChanged += new PropertyChangedEventHandler(this.model_PropertyChanged);
      }

      private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (!(e.PropertyName == "KeyStringWarningText"))
          return;
        string stringWarningText = this.model.KeyStringWarningText;
        if (string.IsNullOrEmpty(stringWarningText))
          this.helper.SetContent((MessageBubbleContent) null);
        else
          this.helper.SetContent(new MessageBubbleContent(stringWarningText, MessageBubbleType.Error));
      }

      public void Initialize(UIElement targetElement, IMessageBubbleHelper helper)
      {
        this.helper = helper;
      }
    }
  }
}
