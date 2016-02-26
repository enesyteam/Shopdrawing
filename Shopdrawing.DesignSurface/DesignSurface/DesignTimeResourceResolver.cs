// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DesignTimeResourceResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface
{
  internal class DesignTimeResourceResolver : IDesignTimeResourceResolver
  {
    private Queue<Action> resolutionActionQueue = new Queue<Action>();
    private DesignerContext designerContext;

    public DesignTimeResourceResolver(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public bool Resolve(IDocumentContext documentContext, EditDesignTimeResourceModelMode mode, string missingResourceName)
    {
      if (documentContext == null)
        throw new ArgumentNullException("documentContext");
      bool success = false;
      UIThreadDispatcherHelper.Invoke(DispatcherPriority.ApplicationIdle, (Action) (() =>
      {
        EditDesignTimeResourceModel model = this.CreateModel(documentContext, mode, missingResourceName);
        if (model == null || !model.CanResolveDesignTimeResources)
          return;
        if (mode != EditDesignTimeResourceModelMode.Manual)
        {
          bool? doNotAskAgain = model.DoNotAskAgain;
          if ((doNotAskAgain.GetValueOrDefault() ? 0 : (doNotAskAgain.HasValue ? true : false)) == 0)
            return;
        }
        bool? nullable = new EditDesignTimeResourcesDialog(model).ShowDialog();
        success = nullable.GetValueOrDefault() && nullable.HasValue;
      }));
      return success;
    }

    public void Enqueue(Action resolutionAction)
    {
      if (this.resolutionActionQueue.Contains(resolutionAction))
        return;
      this.resolutionActionQueue.Enqueue(resolutionAction);
      if (this.resolutionActionQueue.Count != 1)
        return;
      this.ResolveNext();
    }

    internal bool CanResolve(IDocumentContext documentContext)
    {
      EditDesignTimeResourceModel model = this.CreateModel(documentContext, EditDesignTimeResourceModelMode.Manual, (string) null);
      if (model == null)
        return false;
      return model.CanResolveDesignTimeResources;
    }

    private EditDesignTimeResourceModel CreateModel(IDocumentContext documentContext, EditDesignTimeResourceModelMode mode, string missingResourceName)
    {
      IProject rootProject = (IProject) null;
      if (documentContext.ApplicationRoot != null)
        rootProject = this.designerContext.ProjectManager.CurrentSolution.FindProjectContainingItem(DocumentReference.Create(documentContext.ApplicationRoot.DocumentContext.DocumentUrl));
      if (rootProject == null)
        rootProject = this.designerContext.ProjectManager.CurrentSolution.FindProjectContainingItem(DocumentReference.Create(documentContext.DocumentUrl));
      if (rootProject == null)
        return (EditDesignTimeResourceModel) null;
      return new EditDesignTimeResourceModel(rootProject, this.designerContext, Path.GetFileName(documentContext.DocumentUrl), mode, missingResourceName);
    }

    private void ResolveNext()
    {
      this.resolutionActionQueue.Peek()();
      this.resolutionActionQueue.Dequeue();
      if (!Enumerable.Any<Action>((IEnumerable<Action>) this.resolutionActionQueue))
        return;
      this.ResolveNext();
    }

    bool IDesignTimeResourceResolver.Resolve(IDocumentContext documentContext, string missingResourceName)
    {
      return this.Resolve(documentContext, EditDesignTimeResourceModelMode.Warning, missingResourceName);
    }
  }
}
