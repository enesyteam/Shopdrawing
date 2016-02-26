// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.WorkspaceService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class WorkspaceService : CommandTarget, IWorkspaceService, ICommandTarget
  {
    private List<string> factoryWorkspaceNames = new List<string>();
    private SortedArrayList<string> customWorkspaceNames = new SortedArrayList<string>((IComparer<string>) StringLogicalComparer.Instance);
    private List<Uri> factoryWorkspaceConfigResources = new List<Uri>();
    public const int CustomWorkspaceMenuLimit = 10;
    public const int WorkspaceNameLengthLimit = 50;
    private const string defaultConfigExtension = ".default.xaml";
    private const string modifiedConfigExtension = ".modified.xaml";
    private IConfigurationObject configurationObject;
    private string workspacesPath;
    private IMessageDisplayService messageDisplayService;
    private PaletteRegistry paletteRegistry;
    private IWorkspace activeWorkspace;

    public FrameworkElement Content { get; set; }

    public IWorkspace ActiveWorkspace
    {
      get
      {
        return this.activeWorkspace;
      }
      private set
      {
        if (this.activeWorkspace == value)
          return;
        Workspace newWorkspace = value as Workspace;
        this.AttachPalettes(newWorkspace);
        this.activeWorkspace = (IWorkspace) newWorkspace;
        ViewManager.Instance.WindowProfile = newWorkspace.WindowProfile;
        this.OnActiveWorkspaceChanged();
      }
    }

    public ReadOnlyList<string> FactoryWorkspaceNames
    {
      get
      {
        return new ReadOnlyList<string>((IList<string>) this.factoryWorkspaceNames);
      }
    }

    public ReadOnlyList<string> CustomWorkspaceNames
    {
      get
      {
        return new ReadOnlyList<string>((IList<string>) this.customWorkspaceNames);
      }
    }

    public event CancelEventHandler ActiveWorkspaceChanging;

    public event EventHandler ActiveWorkspaceChangingCanceled;

    public event EventHandler ActiveWorkspaceChanged;

    public event EventHandler LoadedWorkspace;

    public event EventHandler SavingWorkspace;

    public WorkspaceService(IConfigurationObject configurationObject, string workspacesPath, IEnumerable<string> factoryWorkspaceNames, IEnumerable<Uri> factoryWorkspaceConfigResources, IMessageDisplayService messageDisplayService)
    {
      this.configurationObject = configurationObject;
      this.messageDisplayService = messageDisplayService;
      this.workspacesPath = workspacesPath;
      this.CreateWorkspacesFolder(workspacesPath);
      int num = 0;
      foreach (string str in factoryWorkspaceNames)
      {
        string workspaceName = WorkspaceService.StripUnderscore(str);
        this.factoryWorkspaceNames.Add(workspaceName);
        this.AddCommand("Windows_Workspace_" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture), (ICommand) new WorkspaceService.ActivateWorkspaceCommand(this, workspaceName, str));
        ++num;
      }
      this.factoryWorkspaceConfigResources.AddRange(factoryWorkspaceConfigResources);
      for (int index = 0; index < 10; ++index)
        this.AddCommand("Windows_CustomWorkspace_" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture), (ICommand) new WorkspaceService.ActivateCustomWorkspaceCommand(this, index));
      this.AddCommand("Windows_ResetWorkspace", (ICommand) new WorkspaceService.ResetActiveWorkspaceCommand(this));
      this.AddCommand("Windows_SaveAsNewWorkspace", (ICommand) new WorkspaceService.SaveAsNewWorkspaceCommand(this));
      this.AddCommand("Windows_ManageWorkspaces", (ICommand) new WorkspaceService.ManageWorkspacesCommand(this, this.messageDisplayService));
      this.AddCommand("Windows_ToggleWorkspacePalettes", (ICommand) new WorkspaceService.ToggleWorkspacePalettesCommand());
      this.AddCommand("Windows_CycleWorkspace", (ICommand) new WorkspaceService.CycleWorkspaceCommand(this));
      this.AddCommand("Windows_AllWorkspaces", (ICommand) new WorkspaceService.MoreWorkspacesCommand(this));
    }

    private static string StripUnderscore(string input)
    {
      int length = input.IndexOf('_');
      if (length < 0)
        return input;
      return input.Substring(0, length) + input.Substring(length + 1);
    }

    private bool OnActiveWorkspaceChanging()
    {
      if (this.ActiveWorkspaceChanging != null)
      {
        CancelEventArgs e = new CancelEventArgs();
        this.ActiveWorkspaceChanging((object) this, e);
        if (e.Cancel)
          return false;
      }
      return true;
    }

    private void OnActiveWorkspaceChangingCanceled()
    {
      if (this.ActiveWorkspaceChangingCanceled == null)
        return;
      this.ActiveWorkspaceChangingCanceled((object) this, EventArgs.Empty);
    }

    private void OnActiveWorkspaceChanged()
    {
      if (this.ActiveWorkspaceChanged == null)
        return;
      this.ActiveWorkspaceChanged((object) this, EventArgs.Empty);
    }

    public void SetPaletteRegistry(PaletteRegistry paletteRegistry)
    {
      this.paletteRegistry = paletteRegistry;
    }

    public void LoadConfiguration()
    {
      this.LoadConfiguration(false);
    }

    public void LoadConfiguration(bool useDefaultWorkspace)
    {
      string str = (string) this.configurationObject.GetProperty("UserWorkspaces", (object) string.Empty);
      if (!string.IsNullOrEmpty(str))
        this.customWorkspaceNames.AddRange((IEnumerable<string>) str.Split('|'));
      string name;
      if (useDefaultWorkspace)
      {
        name = this.factoryWorkspaceNames[0];
      }
      else
      {
        name = (string) this.configurationObject.GetProperty("ActiveWorkspace", (object) this.factoryWorkspaceNames[0]);
        if (!this.factoryWorkspaceNames.Contains(name) && !this.customWorkspaceNames.Contains(name))
          name = this.factoryWorkspaceNames[0];
      }
      Workspace workspace = new Workspace(name, this.Content);
      WorkspaceService.WorkspaceOperationResult workspaceOperationResult = this.LoadWorkspaceConfiguration((IWorkspace) workspace, useDefaultWorkspace);
      if (!workspaceOperationResult.Success && name != this.factoryWorkspaceNames[0])
      {
        workspace = new Workspace(this.factoryWorkspaceNames[0], this.Content);
        workspaceOperationResult = this.LoadWorkspaceConfiguration((IWorkspace) workspace, false);
      }
      if (!workspaceOperationResult.Success)
        throw new InvalidOperationException("There is no workspace configuration on the machine that could be loaded.");
      this.ActiveWorkspace = (IWorkspace) workspace;
    }

    public void SaveConfiguration()
    {
      this.SaveConfiguration(true);
    }

    public void SaveConfiguration(bool displayErrorMessages)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      for (int index = 0; index < this.customWorkspaceNames.Count; ++index)
      {
        if (flag)
          stringBuilder.Append('|');
        stringBuilder.Append(this.customWorkspaceNames[index]);
        flag = true;
      }
      this.configurationObject.SetProperty("UserWorkspaces", (object) stringBuilder.ToString(), (object) string.Empty);
      this.configurationObject.SetProperty("ActiveWorkspace", (object) this.activeWorkspace.Name);
      WorkspaceService.WorkspaceOperationResult workspaceOperationResult = this.SaveWorkspaceConfiguration(this.activeWorkspace, true);
      if (workspaceOperationResult.Success || !displayErrorMessages || this.messageDisplayService == null)
        return;
      int num = (int) this.messageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = workspaceOperationResult.Message,
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Hand
      });
    }

    public bool SwitchToWorkspace(string workspaceName)
    {
      using (PerformanceUtility.PerformanceSequence(PerformanceEvent.SwitchWorkspaces))
      {
        if (!this.factoryWorkspaceNames.Contains(workspaceName) && !this.customWorkspaceNames.Contains(workspaceName) || this.activeWorkspace.Name == workspaceName || !this.OnActiveWorkspaceChanging())
          return false;
        WorkspaceService.WorkspaceOperationResult workspaceOperationResult1 = this.SaveWorkspaceConfiguration(this.activeWorkspace, true);
        if (!workspaceOperationResult1.Success)
        {
          if (this.messageDisplayService != null)
          {
            if (this.messageDisplayService.ShowMessage(new MessageBoxArgs()
            {
              Message = workspaceOperationResult1.Message + " " + StringTable.WorkspaceConfigurationProceedSwitching,
              Button = MessageBoxButton.OKCancel,
              Image = MessageBoxImage.Exclamation
            }) != MessageBoxResult.OK)
            {
              this.OnActiveWorkspaceChangingCanceled();
              return false;
            }
          }
          else
          {
            this.OnActiveWorkspaceChangingCanceled();
            return false;
          }
        }
        Workspace workspace = new Workspace(workspaceName, this.Content);
        WorkspaceService.WorkspaceOperationResult workspaceOperationResult2 = this.LoadWorkspaceConfiguration((IWorkspace) workspace, false);
        if (!workspaceOperationResult2.Success)
        {
          if (this.messageDisplayService != null)
          {
            int num = (int) this.messageDisplayService.ShowMessage(new MessageBoxArgs()
            {
              Message = workspaceOperationResult2.Message,
              Button = MessageBoxButton.OK,
              Image = MessageBoxImage.Hand
            });
          }
          if (this.customWorkspaceNames.Contains(workspaceName))
            this.customWorkspaceNames.Remove(workspaceName);
          this.OnActiveWorkspaceChangingCanceled();
          return false;
        }
        this.ActiveWorkspace = (IWorkspace) workspace;
      }
      return true;
    }

    public bool SaveAsNewWorkspace(string workspaceName)
    {
      using (PerformanceUtility.PerformanceSequence(PerformanceEvent.SaveWorkspace))
      {
        if (this.factoryWorkspaceNames.Contains(workspaceName))
          throw new InvalidOperationException("Cannot overwrite factory workspace configuration.");
        if (!this.OnActiveWorkspaceChanging())
          return false;
        Workspace workspace = new Workspace(workspaceName, this.Content);
        workspace.CopyConfiguration(this.activeWorkspace);
        WorkspaceService.WorkspaceOperationResult workspaceOperationResult = this.SaveWorkspaceConfiguration((IWorkspace) workspace, false);
        if (!workspaceOperationResult.Success)
        {
          if (this.messageDisplayService != null)
          {
            int num = (int) this.messageDisplayService.ShowMessage(new MessageBoxArgs()
            {
              Message = workspaceOperationResult.Message,
              Button = MessageBoxButton.OK,
              Image = MessageBoxImage.Hand
            });
          }
          this.OnActiveWorkspaceChangingCanceled();
          return false;
        }
        if (!this.customWorkspaceNames.Contains(workspaceName))
          this.customWorkspaceNames.Add(workspaceName);
        this.ActiveWorkspace = (IWorkspace) workspace;
      }
      return true;
    }

    public bool DeleteWorkspace(string workspaceName)
    {
      if (this.factoryWorkspaceNames.Contains(workspaceName) || !this.customWorkspaceNames.Contains(workspaceName) || workspaceName == this.ActiveWorkspace.Name && !this.OnActiveWorkspaceChanging())
        return false;
      this.DeleteConfig(workspaceName);
      this.customWorkspaceNames.Remove(workspaceName);
      if (workspaceName == this.ActiveWorkspace.Name)
      {
        Workspace workspace = new Workspace(this.factoryWorkspaceNames[0], this.Content);
        this.LoadWorkspaceConfiguration((IWorkspace) workspace, false);
        this.ActiveWorkspace = (IWorkspace) workspace;
      }
      return true;
    }

    public bool RenameWorkspace(string oldName, string newName)
    {
      if (this.factoryWorkspaceNames.Contains(oldName) || !this.customWorkspaceNames.Contains(oldName) || oldName == this.ActiveWorkspace.Name && !this.OnActiveWorkspaceChanging())
        return false;
      string str1 = Path.Combine(this.workspacesPath, oldName + ".default.xaml");
      string destFileName1 = Path.Combine(this.workspacesPath, newName + ".default.xaml");
      string str2 = Path.Combine(this.workspacesPath, oldName + ".modified.xaml");
      string destFileName2 = Path.Combine(this.workspacesPath, newName + ".modified.xaml");
      try
      {
        bool flag1 = Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str1);
        bool flag2 = Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str2);
        if (flag1)
          File.Copy(str1, destFileName1);
        if (flag2)
          File.Copy(str2, destFileName2);
        if (flag1)
          File.Delete(str1);
        if (flag2)
          File.Delete(str2);
      }
      catch (IOException ex)
      {
        return false;
      }
      this.customWorkspaceNames.Remove(oldName);
      this.customWorkspaceNames.Add(newName);
      if (oldName == this.ActiveWorkspace.Name)
      {
        Workspace workspace = new Workspace(newName, this.Content);
        this.LoadWorkspaceConfiguration((IWorkspace) workspace, false);
        this.ActiveWorkspace = (IWorkspace) workspace;
      }
      return true;
    }

    public bool ResetActiveWorkspace()
    {
      if (!this.OnActiveWorkspaceChanging())
        return false;
      using (PerformanceUtility.PerformanceSequence(PerformanceEvent.ResetWorkspace))
      {
        Workspace workspace = new Workspace(this.activeWorkspace.Name, this.Content);
        WorkspaceService.WorkspaceOperationResult workspaceOperationResult = this.LoadWorkspaceConfiguration((IWorkspace) workspace, true);
        if (!workspaceOperationResult.Success)
        {
          if (this.messageDisplayService != null)
          {
            int num = (int) this.messageDisplayService.ShowMessage(new MessageBoxArgs()
            {
              Message = workspaceOperationResult.Message,
              Button = MessageBoxButton.OK,
              Image = MessageBoxImage.Hand
            });
          }
          this.OnActiveWorkspaceChangingCanceled();
          return false;
        }
        this.ActiveWorkspace = (IWorkspace) workspace;
      }
      return true;
    }

    public bool IsValidNameForNewWorkspace(string name, out string error)
    {
      error = string.Empty;
      if (this.IsExistingWorkspaceName(name))
      {
        error = StringTable.WorkspaceConfigurationErrorExistingNameDialogText;
        return false;
      }
      error = Microsoft.Expression.Framework.Documents.PathHelper.ValidateFileOrDirectoryName(name);
      if (error == null && name.Length >= 50)
        error = StringTable.WorkspaceNameIsTooLong;
      return error == null;
    }

    public bool IsExistingWorkspaceName(string name)
    {
      foreach (string strB in (ReadOnlyCollection<string>) this.FactoryWorkspaceNames)
      {
        if (string.Compare(name, strB, StringComparison.OrdinalIgnoreCase) == 0)
          return true;
      }
      foreach (string strB in (ReadOnlyCollection<string>) this.CustomWorkspaceNames)
      {
        if (string.Compare(name, strB, StringComparison.OrdinalIgnoreCase) == 0)
          return true;
      }
      return false;
    }

    private void AttachPalettes(Workspace newWorkspace)
    {
      Workspace workspace = this.ActiveWorkspace as Workspace;
      if (workspace != null)
      {
        foreach (Microsoft.VisualStudio.PlatformUI.Shell.View view in workspace.WindowProfile.FindAll((Predicate<ViewElement>) (v => v is Microsoft.VisualStudio.PlatformUI.Shell.View)))
        {
          if (this.paletteRegistry[view.Name] != null)
            view.Content = (object) null;
        }
      }
      using (ViewManager.Instance.DeferActiveViewChanges())
      {
        foreach (PaletteRegistryEntry paletteRegistryEntry in (IEnumerable<PaletteRegistryEntry>) this.paletteRegistry.PaletteRegistryEntries)
        {
          Microsoft.VisualStudio.PlatformUI.Shell.View view = newWorkspace.FindPalette(paletteRegistryEntry.Name) ?? Microsoft.VisualStudio.PlatformUI.Shell.View.Create(newWorkspace.WindowProfile, paletteRegistryEntry.Name, paletteRegistryEntry.ViewProperties.ViewType);
          ExpressionView expressionView = view as ExpressionView;
          if (expressionView == null || !paletteRegistryEntry.ViewProperties.ViewType.IsAssignableFrom(view.GetType()))
          {
            expressionView = Microsoft.VisualStudio.PlatformUI.Shell.View.Create(newWorkspace.WindowProfile, paletteRegistryEntry.Name, paletteRegistryEntry.ViewProperties.ViewType) as ExpressionView;
            foreach (PropertyInfo propertyInfo in view.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
              PropertyInfo property = expressionView.GetType().GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);
              if (property != (PropertyInfo) null && property.CanWrite && property.Name != "Parent")
              {
                object obj = propertyInfo.GetValue((object) view, (object[]) null);
                property.SetValue((object) expressionView, obj, (object[]) null);
              }
            }
            expressionView.Detach();
            expressionView.IsDesiredVisible = view.IsVisible;
            IList<ViewElement> list = (IList<ViewElement>) view.Parent.Children;
            int index = list.IndexOf((ViewElement) view);
            list[index] = (ViewElement) expressionView;
          }
          expressionView.Content = (object) paletteRegistryEntry.Content;
          expressionView.Title = (object) paletteRegistryEntry.Caption;
          if (double.IsNaN(expressionView.FloatingLeft) || double.IsNaN(expressionView.FloatingTop))
          {
            Size? defaultFloatSize = paletteRegistryEntry.ViewProperties.DefaultFloatSize;
            if (defaultFloatSize.HasValue && defaultFloatSize.HasValue)
            {
              expressionView.FloatingWidth = defaultFloatSize.Value.Width;
              expressionView.FloatingHeight = defaultFloatSize.Value.Height;
              FloatSite ancestor = ExtensionMethods.FindAncestor<FloatSite>((ViewElement) expressionView);
              if (ancestor != null)
              {
                bool flag = true;
                foreach (ViewElement viewElement in ancestor.FindAll((Predicate<ViewElement>) (value => value != expressionView)))
                {
                  if (!double.IsNaN(viewElement.FloatingLeft) && !double.IsNaN(viewElement.FloatingTop))
                  {
                    flag = false;
                    break;
                  }
                }
                if (flag)
                {
                  ancestor.FloatingWidth = expressionView.FloatingWidth;
                  ancestor.FloatingHeight = expressionView.FloatingHeight;
                }
              }
            }
          }
          paletteRegistryEntry.ViewProperties.Apply(expressionView);
        }
      }
      foreach (ViewElement viewElement in Enumerable.ToArray<ViewElement>(newWorkspace.WindowProfile.FindAll((Predicate<ViewElement>) (v =>
      {
        Microsoft.VisualStudio.PlatformUI.Shell.View view = v as Microsoft.VisualStudio.PlatformUI.Shell.View;
        if (view != null)
          return view.Content == null;
        return false;
      }))))
        viewElement.Detach();
    }

    private WorkspaceService.WorkspaceOperationResult LoadWorkspaceConfiguration(IWorkspace workspace, bool forceDefault)
    {
      string filePath = (string) null;
      if (forceDefault)
      {
        this.DeleteModifiedConfig(workspace.Name);
      }
      else
      {
        try
        {
          using (Stream configStream = this.GetConfigStream(workspace.Name, true, FileAccess.Read, out filePath))
          {
            if (configStream != null)
            {
              if (workspace.LoadConfiguration(configStream))
              {
                this.OnLoadedWorkspaceConfiguration();
                return new WorkspaceService.WorkspaceOperationResult()
                {
                  Success = true
                };
              }
            }
          }
        }
        catch (IOException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
      string str = (string) null;
      try
      {
        using (Stream configStream = this.GetConfigStream(workspace.Name, false, FileAccess.Read, out filePath))
        {
          if (configStream != null)
          {
            if (workspace.LoadConfiguration(configStream))
            {
              this.OnLoadedWorkspaceConfiguration();
              return new WorkspaceService.WorkspaceOperationResult()
              {
                Success = true
              };
            }
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationLoadError, new object[1]
            {
              (object) filePath
            });
          }
          else
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationLoadConfigNotFound, new object[1]
            {
              (object) filePath
            });
        }
      }
      catch (IOException ex)
      {
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationLoadIOError, new object[1]
        {
          (object) filePath
        });
      }
      catch (UnauthorizedAccessException ex)
      {
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationLoadAccessError, new object[1]
        {
          (object) filePath
        });
      }
      return new WorkspaceService.WorkspaceOperationResult()
      {
        Success = false,
        Message = str
      };
    }

    private void OnLoadedWorkspaceConfiguration()
    {
      if (this.LoadedWorkspace == null)
        return;
      this.LoadedWorkspace((object) this, (EventArgs) null);
    }

    private WorkspaceService.WorkspaceOperationResult SaveWorkspaceConfiguration(IWorkspace workspace, bool modified)
    {
      if (this.SavingWorkspace != null)
        this.SavingWorkspace((object) this, (EventArgs) null);
      string filePath = (string) null;
      string str = (string) null;
      try
      {
        using (Stream configStream = this.GetConfigStream(workspace.Name, modified, FileAccess.Write, out filePath))
        {
          if (workspace.SaveConfiguration(configStream))
            return new WorkspaceService.WorkspaceOperationResult()
            {
              Success = true
            };
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationSaveError, new object[1]
          {
            (object) filePath
          });
        }
      }
      catch (IOException ex)
      {
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationSaveIOError, new object[1]
        {
          (object) filePath
        });
      }
      catch (UnauthorizedAccessException ex)
      {
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WorkspaceConfigurationSaveAccessError, new object[1]
        {
          (object) filePath
        });
      }
      return new WorkspaceService.WorkspaceOperationResult()
      {
        Success = false,
        Message = str
      };
    }

    private void CreateWorkspacesFolder(string workspacesPath)
    {
      Exception exception = (Exception) null;
      try
      {
        if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(workspacesPath))
          Directory.CreateDirectory(workspacesPath);
      }
      catch (UnauthorizedAccessException ex)
      {
        exception = (Exception) ex;
      }
      catch (IOException ex)
      {
        exception = (Exception) ex;
      }
      if (exception == null)
        return;
      this.messageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.CannotCreateWorkspacesFolder, new object[1]
      {
        (object) exception.Message
      }));
    }

    private Stream GetConfigStream(string workspaceName, bool modified, FileAccess access, out string filePath)
    {
      int index = this.factoryWorkspaceNames.IndexOf(workspaceName);
      if (!modified && index != -1)
      {
        filePath = this.factoryWorkspaceConfigResources[index].OriginalString;
        return Application.GetResourceStream(this.factoryWorkspaceConfigResources[index]).Stream;
      }
      filePath = Path.Combine(this.workspacesPath, workspaceName + (modified ? ".modified.xaml" : ".default.xaml"));
      if (access == FileAccess.Read)
      {
        if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(filePath))
          return (Stream) null;
        return (Stream) File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
      }
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.workspacesPath))
        Directory.CreateDirectory(this.workspacesPath);
      return (Stream) File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
    }

    private void DeleteConfig(string workspaceName)
    {
      this.DeleteHelper(Path.Combine(this.workspacesPath, workspaceName + ".modified.xaml"));
      if (!this.customWorkspaceNames.Contains(workspaceName))
        return;
      this.DeleteHelper(Path.Combine(this.workspacesPath, workspaceName + ".default.xaml"));
    }

    private void DeleteModifiedConfig(string workspaceName)
    {
      this.DeleteHelper(Path.Combine(this.workspacesPath, workspaceName + ".modified.xaml"));
    }

    private void DeleteHelper(string filePath)
    {
      if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(filePath))
        return;
      try
      {
        File.Delete(filePath);
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    private class WorkspaceOperationResult
    {
      public bool Success { get; set; }

      public string Message { get; set; }
    }

    private class ActivateWorkspaceCommand : Command
    {
      private WorkspaceService workspaceService;
      private string workspaceName;
      private string menuText;

      public ActivateWorkspaceCommand(WorkspaceService workspaceService, string workspaceName, string menuText)
      {
        this.workspaceService = workspaceService;
        this.workspaceName = workspaceName;
        this.menuText = menuText;
      }

      public override void Execute()
      {
      }

      public override object GetProperty(string propertyName)
      {
        switch (propertyName)
        {
          case "Text":
            return (object) this.menuText;
          case "IsChecked":
            return (object) (bool) (this.workspaceService.ActiveWorkspace.Name == this.workspaceName ? true : false);
          default:
            return base.GetProperty(propertyName);
        }
      }

      public override void SetProperty(string propertyName, object propertyValue)
      {
        switch (propertyName)
        {
          case "IsChecked":
            if (!(bool) propertyValue)
              break;
            this.workspaceService.SwitchToWorkspace(this.workspaceName);
            break;
          default:
            base.SetProperty(propertyName, propertyValue);
            break;
        }
      }
    }

    private sealed class ActivateCustomWorkspaceCommand : Command
    {
      private WorkspaceService workspaceService;
      private int index;

      public ActivateCustomWorkspaceCommand(WorkspaceService workspaceService, int index)
      {
        this.workspaceService = workspaceService;
        this.index = index;
      }

      public override void Execute()
      {
      }

      public override object GetProperty(string propertyName)
      {
        switch (propertyName)
        {
          case "Text":
            if (this.index >= this.workspaceService.CustomWorkspaceNames.Count)
              return (object) string.Empty;
            if (this.index >= 9)
              return (object) ("1_0 " + this.workspaceService.CustomWorkspaceNames[this.index]);
            return (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "_{0} {1}", new object[2]
            {
              (object) (this.index + 1),
              (object) this.workspaceService.CustomWorkspaceNames[this.index]
            });
          case "IsEnabled":
          case "IsVisible":
            return (object) (bool) (this.index < this.workspaceService.CustomWorkspaceNames.Count ? true : false);
          case "IsChecked":
            return (object) (bool) (this.index >= this.workspaceService.CustomWorkspaceNames.Count ? false : (this.workspaceService.CustomWorkspaceNames[this.index] == this.workspaceService.ActiveWorkspace.Name ? true : false));
          default:
            return base.GetProperty(propertyName);
        }
      }

      public override void SetProperty(string propertyName, object propertyValue)
      {
        switch (propertyName)
        {
          case "IsChecked":
            if (this.index >= this.workspaceService.CustomWorkspaceNames.Count || !(bool) propertyValue)
              break;
            this.workspaceService.SwitchToWorkspace(this.workspaceService.CustomWorkspaceNames[this.index]);
            break;
          default:
            base.SetProperty(propertyName, propertyValue);
            break;
        }
      }
    }

    private class MoreWorkspacesCommand : Command
    {
      private WorkspaceService workspaceService;

      public override bool IsEnabled
      {
        get
        {
          return this.IsAvailable;
        }
      }

      public override bool IsAvailable
      {
        get
        {
          return this.workspaceService.CustomWorkspaceNames.Count > 10;
        }
      }

      public MoreWorkspacesCommand(WorkspaceService workspaceService)
      {
        this.workspaceService = workspaceService;
      }

      public override void Execute()
      {
        MoreWorkspacesDialog workspacesDialog = new MoreWorkspacesDialog(this.workspaceService);
        bool? nullable = workspacesDialog.ShowDialog();
        if (!nullable.HasValue || !nullable.Value || workspacesDialog.SelectedWorkspace == null)
          return;
        this.workspaceService.SwitchToWorkspace(workspacesDialog.SelectedWorkspace);
      }
    }

    private class ResetActiveWorkspaceCommand : Command
    {
      private WorkspaceService workspaceService;

      public override bool IsEnabled
      {
        get
        {
          return this.workspaceService.ActiveWorkspace != null;
        }
      }

      public ResetActiveWorkspaceCommand(WorkspaceService workspaceService)
      {
        this.workspaceService = workspaceService;
      }

      public override void Execute()
      {
        this.workspaceService.ResetActiveWorkspace();
      }
    }

    private class SaveAsNewWorkspaceCommand : Command
    {
      private WorkspaceService workspaceService;

      public override bool IsEnabled
      {
        get
        {
          return this.workspaceService.ActiveWorkspace != null;
        }
      }

      public SaveAsNewWorkspaceCommand(WorkspaceService workspaceService)
      {
        this.workspaceService = workspaceService;
      }

      public override void Execute()
      {
        SaveConfigurationDialog configurationDialog = new SaveConfigurationDialog(this.workspaceService);
        bool? nullable = configurationDialog.ShowDialog();
        if (!nullable.HasValue || !nullable.Value)
          return;
        this.workspaceService.SaveAsNewWorkspace(configurationDialog.WorkspaceName);
      }
    }

    private class ManageWorkspacesCommand : Command
    {
      private WorkspaceService workspaceService;
      private IMessageDisplayService messageService;

      public override bool IsEnabled
      {
        get
        {
          return this.workspaceService.CustomWorkspaceNames.Count != 0;
        }
      }

      public ManageWorkspacesCommand(WorkspaceService workspaceService, IMessageDisplayService messageService)
      {
        this.workspaceService = workspaceService;
        this.messageService = messageService;
      }

      public override void Execute()
      {
        ManageWorkspaceConfigurationsDialog configurationsDialog = new ManageWorkspaceConfigurationsDialog(this.workspaceService, this.messageService);
        bool? nullable = configurationsDialog.ShowDialog();
        if (!nullable.HasValue || !nullable.Value)
          return;
        configurationsDialog.CommitChanges();
      }
    }

    private class ToggleWorkspacePalettesCommand : CheckCommandBase
    {
      private Microsoft.VisualStudio.PlatformUI.Shell.View SavedActiveView { get; set; }

      private IEnumerable<ViewElement> HideableViews
      {
        get
        {
          ExpressionView expressionView;
          return Enumerable.Where<ViewElement>(Enumerable.Where<ViewElement>(Enumerable.Where<ViewElement>(ViewManager.Instance.WindowProfile.FindAll((Predicate<ViewElement>) (element =>
          {
            if (element.IsVisible)
              return element is Microsoft.VisualStudio.PlatformUI.Shell.View;
            return false;
          })), (Func<ViewElement, bool>) (element => !AutoHideChannel.IsAutoHidden(element))), (Func<ViewElement, bool>) (element => DockOperations.CanAutoHide(element))), (Func<ViewElement, bool>) (element =>
          {
            if ((expressionView = element as ExpressionView) != null)
              return expressionView.IsAutoHideable;
            return true;
          }));
        }
      }

      private IEnumerable<ViewElement> AutoHiddenViews
      {
        get
        {
          return ViewManager.Instance.WindowProfile.FindAll((Predicate<ViewElement>) (element => AutoHideChannel.IsAutoHidden(element)));
        }
      }

      public override bool IsAvailable
      {
        get
        {
          return true;
        }
      }

      public override bool IsEnabled
      {
        get
        {
          if (!Enumerable.Any<ViewElement>(this.HideableViews))
            return Enumerable.Any<ViewElement>(this.AutoHiddenViews);
          return true;
        }
      }

      public override void Execute()
      {
        if (Enumerable.Any<ViewElement>(this.HideableViews))
        {
          this.HidePalettes();
        }
        else
        {
          if (!Enumerable.Any<ViewElement>(this.AutoHiddenViews))
            return;
          this.RestorePalettes();
        }
      }

      private void HidePalettes()
      {
        List<ViewElement> list = Enumerable.ToList<ViewElement>(this.HideableViews);
        if (!Enumerable.Any<ViewElement>((IEnumerable<ViewElement>) list))
          return;
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.TogglePalettes);
        Microsoft.VisualStudio.PlatformUI.Shell.View activeView = ViewManager.Instance.ActiveView;
        if (list.Contains((ViewElement) activeView))
          this.SavedActiveView = activeView;
        foreach (ViewElement viewElement in list)
        {
          ExpressionView expressionView = viewElement as ExpressionView;
          if (expressionView != null)
            expressionView.WasSelectedBeforeAutoHide = expressionView.IsSelected;
        }
        foreach (ViewElement viewElement in list)
          DockOperations.AutoHide(viewElement);
      }

      protected override bool IsChecked()
      {
        if (!Enumerable.Any<ViewElement>(this.HideableViews))
          return Enumerable.Any<ViewElement>(this.AutoHiddenViews);
        return false;
      }

      protected override void OnCheckedChanged(bool isChecked)
      {
        this.Execute();
      }

      private void RestorePalettes()
      {
        List<ViewElement> list = Enumerable.ToList<ViewElement>(this.AutoHiddenViews);
        if (!Enumerable.Any<ViewElement>((IEnumerable<ViewElement>) list))
          return;
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.TogglePalettes);
        foreach (ViewElement element in list)
          DockOperations.SnapToBookmark(element);
        foreach (ViewElement viewElement in list)
        {
          ExpressionView expressionView = viewElement as ExpressionView;
          if (expressionView != null)
            expressionView.IsSelected = expressionView.WasSelectedBeforeAutoHide;
        }
        if (this.SavedActiveView != null && this.SavedActiveView.WindowProfile == ViewManager.Instance.WindowProfile)
          ViewManager.Instance.ActiveView = this.SavedActiveView;
        this.SavedActiveView = (Microsoft.VisualStudio.PlatformUI.Shell.View) null;
      }
    }

    private class CycleWorkspaceCommand : Command
    {
      private WorkspaceService workspaceService;

      public override bool IsEnabled
      {
        get
        {
          return this.workspaceService.FactoryWorkspaceNames.Count + this.workspaceService.CustomWorkspaceNames.Count >= 2;
        }
      }

      public CycleWorkspaceCommand(WorkspaceService workspaceService)
      {
        this.workspaceService = workspaceService;
      }

      public override void Execute()
      {
        int num = this.workspaceService.FactoryWorkspaceNames.IndexOf(this.workspaceService.ActiveWorkspace.Name);
        if (num == -1)
          num = this.workspaceService.CustomWorkspaceNames.IndexOf(this.workspaceService.ActiveWorkspace.Name) + this.workspaceService.FactoryWorkspaceNames.Count;
        int index = (num + 1) % (this.workspaceService.FactoryWorkspaceNames.Count + this.workspaceService.CustomWorkspaceNames.Count);
        this.workspaceService.SwitchToWorkspace(index >= this.workspaceService.FactoryWorkspaceNames.Count ? this.workspaceService.CustomWorkspaceNames[index - this.workspaceService.FactoryWorkspaceNames.Count] : this.workspaceService.FactoryWorkspaceNames[index]);
      }
    }
  }
}
