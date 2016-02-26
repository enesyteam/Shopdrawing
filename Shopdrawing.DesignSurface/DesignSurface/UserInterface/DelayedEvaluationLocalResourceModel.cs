// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DelayedEvaluationLocalResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class DelayedEvaluationLocalResourceModel : LocalResourceModel
  {
    private bool evaluated;
    private IDocumentContext documentContext;
    private DesignerContext designerContext;

    public override object ResourceValue
    {
      get
      {
        DocumentNode resourceNode = this.ResourceNode;
        if (!this.evaluated && resourceNode.IsInDocument)
        {
          using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(this.documentContext, this.designerContext))
          {
            using (instanceBuilderContext.DisablePostponedResourceEvaluation())
            {
              instanceBuilderContext.ViewNodeManager.RootNodePath = new DocumentNodePath(resourceNode.DocumentRoot.RootNode, resourceNode);
              instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
              object validRootInstance = instanceBuilderContext.ViewNodeManager.ValidRootInstance;
              this.ResourceValue = this.designerContext.PlatformConverter.ConvertToWpf(resourceNode.Context, validRootInstance);
            }
          }
          this.evaluated = true;
        }
        return base.ResourceValue;
      }
    }

    internal DelayedEvaluationLocalResourceModel(IDocumentContext documentContext, DesignerContext designContext, DocumentNode key, Type type, DocumentNode node)
      : base(key, type, node, (object) null)
    {
      this.documentContext = documentContext;
      this.designerContext = designContext;
    }
  }
}
