// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ReferenceChecker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class ReferenceChecker
  {
    private DesignerContext designerContext;
    private ReferenceVerifierProcessor processor;
    private bool buildComplete;
    private bool processorComplete;
    private IList<InvalidReferenceModel> invalidReferences;

    private DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public ReferenceChecker(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      if (this.DesignerContext.ProjectManager == null)
        return;
      this.DesignerContext.ProjectManager.BuildManager.BuildStarting += new EventHandler<BuildStartingEventArgs>(this.OnBuildStarting);
      this.DesignerContext.ProjectManager.BuildManager.BuildCompleted += new EventHandler<BuildCompletedEventArgs>(this.OnBuildCompleted);
    }

    public void Unhook()
    {
      if (this.processor != null)
      {
        this.processor.Kill();
        this.processor = (ReferenceVerifierProcessor) null;
      }
      if (this.DesignerContext.ProjectManager == null)
        return;
      this.DesignerContext.ProjectManager.BuildManager.BuildStarting -= new EventHandler<BuildStartingEventArgs>(this.OnBuildStarting);
      this.DesignerContext.ProjectManager.BuildManager.BuildCompleted -= new EventHandler<BuildCompletedEventArgs>(this.OnBuildCompleted);
    }

    private void OnBuildStarting(object sender, BuildStartingEventArgs e)
    {
      this.processorComplete = false;
      this.buildComplete = false;
      this.processor = new ReferenceVerifierProcessor(this.DesignerContext, DispatcherPriority.Background);
      this.processor.Complete += new EventHandler(this.OnProcessorComplete);
      this.processor.Begin();
    }

    private void OnProcessorComplete(object sender, EventArgs e)
    {
      this.processorComplete = true;
      if (this.processor == null)
        return;
      this.invalidReferences = this.processor.InvalidReferences;
      this.processor = (ReferenceVerifierProcessor) null;
      this.ReportErrorsIfAppropriate();
    }

    private void OnBuildCompleted(object sender, BuildCompletedEventArgs e)
    {
      this.buildComplete = true;
      this.ReportErrorsIfAppropriate();
    }

    private void ReportErrorsIfAppropriate()
    {
      if (!this.buildComplete || !this.processorComplete)
        return;
      foreach (InvalidReferenceModel invalidReferenceModel in (IEnumerable<InvalidReferenceModel>) this.invalidReferences)
      {
        string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CouldNotResolveReferenceWarningMessage, (object) invalidReferenceModel.InvalidPropertyValue, (object) invalidReferenceModel.InvalidProperty.Name, string.IsNullOrEmpty(invalidReferenceModel.InvalidNodeName) ? (object) invalidReferenceModel.InvalidNodeType.Name : (object) invalidReferenceModel.InvalidNodeName);
        this.DesignerContext.ProjectManager.ActiveBuildTarget.BuildErrors.Add((IErrorTask) new SceneView.DocumentErrorTask(invalidReferenceModel.InvalidValueNode, str, str, ErrorSeverity.Warning));
      }
      if (this.invalidReferences.Count > 0)
        this.DesignerContext.ErrorManager.DisplayErrors();
      this.buildComplete = this.processorComplete = false;
    }
  }
}
