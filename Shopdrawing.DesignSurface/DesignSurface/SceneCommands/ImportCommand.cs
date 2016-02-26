// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ImportCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Import;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ImportCommand : SceneCommandBase, IDisposable
  {
    private ImportManager importManager;
    private string[] filterImportersID;
    private AsyncExecutionHelper helper;

    private string FilePath { get; set; }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return ImportManager.GetImportInsertionPoint(this.SceneViewModel) != null;
        return false;
      }
    }

    public ImportCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public ImportCommand(SceneViewModel viewModel, string[] filterImportersID)
      : base(viewModel)
    {
      this.filterImportersID = filterImportersID;
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "FilePath")
        return (object) this.FilePath;
      return base.GetProperty(propertyName);
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "FilePath")
        this.FilePath = propertyValue as string;
      base.SetProperty(propertyName, propertyValue);
    }

    public override void Execute()
    {
      if ((SceneElement) this.SceneViewModel.FindPanelClosestToRoot() == null)
        return;
      if (this.importManager == null)
      {
        IServices services = this.SceneViewModel.DesignerContext.Services;
        this.importManager = new ImportManager(this.SceneViewModel.DesignerContext.ImportService, services.GetService<IPrototypingService>(), services.GetService<IMessageDisplayService>());
      }
      DoWorkEventArgs args = new DoWorkEventArgs((object) null);
      args.Result = (object) AsyncProcessResult.Done;
      IImportManagerContext importManagerContext = (IImportManagerContext) new ImportManagerContext((string) null, this.SceneViewModel, (IProject) null, true);
      importManagerContext.FileName = this.FilePath;
      this.importManager.Import(importManagerContext, this.filterImportersID, args);
      if (this.helper == null)
        this.helper = new AsyncExecutionHelper();
      if (!args.Result.Equals((object) AsyncProcessResult.StillGoing))
        return;
      this.helper.Clear();
      this.helper.StartAsyncProcess(this.SceneViewModel.DesignerContext.ExpressionInformationService, new Action<object, DoWorkEventArgs>(this.DoWork), new EventHandler(this.OnBeginTask), new EventHandler(this.OnCompleteTask), (EventHandler) null, new EventHandler(this.OnKillTask));
    }

    internal void OnBeginTask(object sender, EventArgs e)
    {
      ((DelegateAsyncProcess) sender).DelegateStatusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ImportingFileProgressDialog, new object[1]
      {
        (object) this.importManager.ImportContext.FileName
      });
    }

    internal void OnCompleteTask(object sender, EventArgs e)
    {
    }

    internal void OnKillTask(object sender, EventArgs e)
    {
      this.importManager.CancelImport();
    }

    internal void DoWork(object sender, DoWorkEventArgs args)
    {
      this.importManager.ResumeImport(args);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.importManager.Dispose();
    }
  }
}
