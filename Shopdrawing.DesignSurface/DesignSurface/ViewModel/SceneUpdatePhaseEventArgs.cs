// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneUpdatePhaseEventArgs
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class SceneUpdatePhaseEventArgs : EventArgs
  {
    private SceneViewModel viewModel;
    private DocumentNodeChangeList documentChanges;
    private SceneViewModel.ViewStateBits dirtyViewState;
    private SceneViewModel.PipelineCalcBits dirtyPipelineCalcState;
    private uint viewStateChangeStamp;
    private uint pipelineCalcChangeStamp;
    private uint documentChangeStamp;
    private bool sceneSwitched;
    private bool rootNodeChanged;

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public SceneViewModel.ViewStateBits DirtyViewState
    {
      get
      {
        return this.dirtyViewState;
      }
    }

    public SceneDocument Document
    {
      get
      {
        if (this.viewModel != null)
          return this.viewModel.Document;
        return (SceneDocument) null;
      }
    }

    public uint DocumentChangeStamp
    {
      get
      {
        return this.documentChangeStamp;
      }
    }

    public DocumentNodeChangeList DocumentChanges
    {
      get
      {
        return this.documentChanges;
      }
    }

    public bool SceneSwitched
    {
      get
      {
        return this.sceneSwitched;
      }
    }

    public bool RootNodeChanged
    {
      get
      {
        return this.rootNodeChanged;
      }
    }

    public bool IsRadicalChange
    {
      get
      {
        if (!this.sceneSwitched)
          return this.rootNodeChanged;
        return true;
      }
    }

    public SceneUpdatePhaseEventArgs(SceneViewModel viewModel, bool sceneSwitched, bool rootNodeChanged)
    {
      this.viewModel = viewModel;
      this.documentChanges = new DocumentNodeChangeList();
      if (this.viewModel != null)
      {
        this.dirtyViewState = viewModel.DirtyState;
        this.viewStateChangeStamp = viewModel.ChangeStamp;
        this.documentChanges.Merge((DocumentNodeMarkerSortedListOf<DocumentNodeChange>) viewModel.Damage, false);
        this.documentChangeStamp = viewModel.XamlDocument.ChangeStamp;
        this.viewModel.AnnotateDamage(this.documentChanges);
        this.dirtyPipelineCalcState = viewModel.DirtyPipelineCalcState;
        this.pipelineCalcChangeStamp = viewModel.PipelineCalcChangeStamp;
      }
      this.sceneSwitched = sceneSwitched;
      this.rootNodeChanged = rootNodeChanged;
      if (!this.sceneSwitched && !this.rootNodeChanged)
        return;
      this.dirtyViewState = SceneViewModel.ViewStateBits.EntireScene;
      this.dirtyPipelineCalcState = SceneViewModel.PipelineCalcBits.EntireScene;
    }

    public bool IsDirtyViewState(SceneViewModel.ViewStateBits bits)
    {
      return (this.dirtyViewState & bits) != SceneViewModel.ViewStateBits.None;
    }

    public bool IsDirtyPipelineCalcState(SceneViewModel.PipelineCalcBits bits)
    {
      return (this.dirtyPipelineCalcState & bits) != SceneViewModel.PipelineCalcBits.None;
    }

    public void Refresh(bool viewSwitched, bool rootNodeChanged)
    {
      this.sceneSwitched |= viewSwitched;
      this.rootNodeChanged |= rootNodeChanged;
      if (viewSwitched || rootNodeChanged)
      {
        this.dirtyViewState |= SceneViewModel.ViewStateBits.EntireScene;
        this.dirtyPipelineCalcState |= SceneViewModel.PipelineCalcBits.EntireScene;
      }
      if (this.viewModel == null || this.viewModel.Document == null || this.viewModel.XamlDocument == null)
        return;
      if ((int) this.viewModel.ChangeStamp != (int) this.viewStateChangeStamp)
      {
        this.viewStateChangeStamp = this.viewModel.ChangeStamp;
        this.dirtyViewState |= this.viewModel.DirtyState;
      }
      if ((int) this.viewModel.XamlDocument.ChangeStamp != (int) this.documentChangeStamp)
      {
        this.documentChangeStamp = this.viewModel.XamlDocument.ChangeStamp;
        this.documentChanges.Merge((DocumentNodeMarkerSortedListOf<DocumentNodeChange>) this.viewModel.Damage, true);
        this.viewModel.AnnotateDamage(this.documentChanges);
      }
      if ((int) this.viewModel.PipelineCalcChangeStamp == (int) this.pipelineCalcChangeStamp)
        return;
      this.pipelineCalcChangeStamp = this.viewModel.PipelineCalcChangeStamp;
      this.dirtyPipelineCalcState |= this.viewModel.DirtyPipelineCalcState;
    }
  }
}
