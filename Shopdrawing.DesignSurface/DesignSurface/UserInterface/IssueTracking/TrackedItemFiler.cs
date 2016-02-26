// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.IssueTracking.TrackedItemFiler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.IssueTracking;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.IssueTracking
{
  public class TrackedItemFiler
  {
    private static readonly int InvalidWorkItemId = -1;
    private IIssueTrackingProvider issueTrackingProvider;
    private IServiceProvider services;
    private string defaultTitle;
    private string defaultDescription;
    private string[] defaultFileAttachments;
    private string currentProject;
    private TrackedItemFiler.PromptState state;
    private ITrackedItemType activeType;
    private IList<ITrackedItemType> itemTypes;
    private List<ITrackedItemType> filteredItemTypes;
    private Uri viewableUri;
    private bool cancelOperation;
    private RegisterItemDialog preservedDialog;

    private IMessageDisplayService MessageDisplayService
    {
      get
      {
        return (IMessageDisplayService) this.services.GetService(typeof (IMessageDisplayService));
      }
    }

    private IConfigurationObject SolutionSettings
    {
      get
      {
        return ((IProjectManager) this.services.GetService(typeof (IProjectManager))).CurrentSolution.SolutionSettingsManager.SolutionSettings;
      }
    }

    private IList<ITrackedItemType> FilteredItemTypes
    {
      get
      {
        if (this.filteredItemTypes == null)
        {
          this.filteredItemTypes = new List<ITrackedItemType>();
          if (this.itemTypes != null)
          {
            foreach (ITrackedItemType trackedItemType in (IEnumerable<ITrackedItemType>) this.itemTypes)
            {
              if (trackedItemType.GetField(ItemField.Title) != null && trackedItemType.GetField(ItemField.Description) != null)
                this.filteredItemTypes.Add(trackedItemType);
            }
          }
        }
        return (IList<ITrackedItemType>) this.filteredItemTypes;
      }
    }

    public TrackedItemFiler(string defaultTitle, string defaultDescription, string[] defaultFileAttachments, string currentProject, IServiceProvider services)
    {
      this.defaultTitle = defaultTitle;
      this.defaultDescription = defaultDescription;
      this.defaultFileAttachments = defaultFileAttachments;
      this.currentProject = currentProject;
      this.services = (IServiceProvider) (services as IServices);
      IIssueTrackingService issueTrackingService = this.services.GetService(typeof (IIssueTrackingService)) as IIssueTrackingService;
      if (issueTrackingService == null)
        return;
      this.issueTrackingProvider = issueTrackingService.ActiveProvider;
    }

    public int FileWorkItem()
    {
      this.preservedDialog = (RegisterItemDialog) null;
      int workItemId = TrackedItemFiler.InvalidWorkItemId;
      if (this.issueTrackingProvider == null)
        return workItemId;
      this.itemTypes = this.issueTrackingProvider.GetItemTypes(this.currentProject);
      if (this.itemTypes == null)
        return -1;
      while (this.state != TrackedItemFiler.PromptState.Quit)
      {
        switch (this.state)
        {
          case TrackedItemFiler.PromptState.GetTemplate:
            this.state = this.AttemptToDetermineDefaultTemplate() ? TrackedItemFiler.PromptState.ShowRegistrationForm : TrackedItemFiler.PromptState.GetTemplateDialog;
            continue;
          case TrackedItemFiler.PromptState.GetTemplateDialog:
            this.DetermineItemType();
            this.state = this.FilteredItemTypes.Count == 0 ? TrackedItemFiler.PromptState.TemplatesNotFoundDialog : (this.activeType == null ? TrackedItemFiler.PromptState.Quit : TrackedItemFiler.PromptState.ShowRegistrationForm);
            continue;
          case TrackedItemFiler.PromptState.TemplatesNotFoundDialog:
            this.state = TrackedItemFiler.PromptState.Quit;
            this.MessageDisplayService.ShowError(StringTable.WorkItemTemplateNotFound);
            continue;
          case TrackedItemFiler.PromptState.ShowRegistrationForm:
            workItemId = this.RegisterWorkItem();
            this.state = this.cancelOperation ? TrackedItemFiler.PromptState.Quit : (workItemId == TrackedItemFiler.InvalidWorkItemId ? TrackedItemFiler.PromptState.ShowFailureDialog : TrackedItemFiler.PromptState.ShowSuccessDialog);
            continue;
          case TrackedItemFiler.PromptState.ShowSuccessDialog:
            int num = (int) this.MessageDisplayService.ShowMessage(new MessageBoxArgs()
            {
              AutomationId = "WorkItemSuccess",
              Message = this.GetSuccessMessage(workItemId),
              Button = MessageBoxButton.OK,
              HyperlinkUri = this.viewableUri,
              HyperlinkMessage = StringTable.WorkItemHyperlink,
              Image = MessageBoxImage.Asterisk
            });
            this.state = TrackedItemFiler.PromptState.Quit;
            continue;
          case TrackedItemFiler.PromptState.ShowFailureDialog:
            this.state = TrackedItemFiler.PromptState.ShowRegistrationForm;
            continue;
          default:
            continue;
        }
      }
      return workItemId;
    }

    private bool AttemptToDetermineDefaultTemplate()
    {
      string workItemTypeFromSuo = this.GetDefaultWorkItemTypeFromSuo();
      bool flag = false;
      foreach (ITrackedItemType trackedItemType in (IEnumerable<ITrackedItemType>) this.FilteredItemTypes)
      {
        if (trackedItemType.Name == workItemTypeFromSuo)
        {
          flag = true;
          this.activeType = trackedItemType;
          break;
        }
      }
      if (!flag)
        this.activeType = (ITrackedItemType) null;
      return flag;
    }

    private string GetSuccessMessage(int workItemId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.WorkItemFiledSuccessfullyMessage, new object[1]
      {
        (object) workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      });
    }

    private void DetermineItemType()
    {
      if (this.FilteredItemTypes.Count <= 0)
        return;
      SelectTemplateDialog selectTemplateDialog = new SelectTemplateDialog((IEnumerable<ITrackedItemType>) this.FilteredItemTypes, this.services);
      selectTemplateDialog.SelectedTemplate = this.activeType;
      bool? nullable = selectTemplateDialog.ShowDialog();
      this.activeType = (nullable.HasValue ? (nullable.GetValueOrDefault() ? true : false) : 0) != 0 ? selectTemplateDialog.SelectedTemplate : (ITrackedItemType) null;
    }

    private int RegisterWorkItem()
    {
      RegisterItemDialog registerItemDialog = new RegisterItemDialog(this.services, this.activeType, (IList<ITrackedItemType>) this.filteredItemTypes, this.defaultTitle, this.defaultDescription, this.defaultFileAttachments);
      if (this.preservedDialog != null && this.preservedDialog.Item.ItemType == this.activeType)
        registerItemDialog.Item = this.preservedDialog.Item;
      this.preservedDialog = registerItemDialog;
      bool? nullable = registerItemDialog.ShowDialog();
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? true : false) : 0) != 0)
      {
        if (this.issueTrackingProvider.SaveWorkItem(registerItemDialog.RegistrationForm))
        {
          int itemId = registerItemDialog.RegistrationForm.ItemId;
          if (itemId > TrackedItemFiler.InvalidWorkItemId)
          {
            this.viewableUri = registerItemDialog.RegistrationForm.TrackedItem.ViewableUri;
            return itemId;
          }
        }
      }
      else
        this.cancelOperation = true;
      return TrackedItemFiler.InvalidWorkItemId;
    }

    private string GetDefaultWorkItemTypeFromSuo()
    {
      return this.SolutionSettings.GetProperty("DefaultTfsWorkItemTemplate", (object) string.Empty).ToString();
    }

    private enum PromptState
    {
      GetTemplate,
      GetTemplateDialog,
      TemplatesNotFoundDialog,
      ShowRegistrationForm,
      ShowSuccessDialog,
      ShowFailureDialog,
      Quit,
    }
  }
}
