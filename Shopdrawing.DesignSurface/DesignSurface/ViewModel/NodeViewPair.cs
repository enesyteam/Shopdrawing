// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.NodeViewPair
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Text;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class NodeViewPair
  {
    private WeakReference view;
    private DocumentNodePath nodePath;
    private IPropertyId propertyKey;

    internal SceneView View
    {
      get
      {
        if (this.view == null || !this.view.IsAlive)
          return (SceneView) null;
        return this.view.Target as SceneView;
      }
    }

    internal DocumentNodePath NodePath
    {
      get
      {
        if (this.nodePath != null && !this.nodePath.Node.IsInDocument)
          return (DocumentNodePath) null;
        return this.nodePath;
      }
    }

    internal IPropertyId PropertyKey
    {
      get
      {
        return this.propertyKey;
      }
    }

    public NodeViewPair(SceneView view, DocumentNodePath nodePath)
      : this(view, nodePath, (IPropertyId) null)
    {
    }

    public NodeViewPair(SceneView view, DocumentNodePath nodePath, IPropertyId propertyKey)
    {
      this.view = new WeakReference((object) view);
      this.nodePath = nodePath;
      this.propertyKey = propertyKey;
    }

    internal SceneViewModel GetViewModel(DesignerContext designerContext)
    {
      SceneView view = this.View;
      if (designerContext.ViewService.Views.Contains((IView) view))
        return view.ViewModel;
      return (SceneViewModel) null;
    }

    internal static bool EvaluateExpression(SceneViewModel parentViewModel, DocumentNodePath parentPath, IPropertyId parentPropertyKey, DocumentNodePath childPath)
    {
      bool flag = true;
      if (parentPropertyKey != null)
      {
        DocumentCompositeNode documentCompositeNode = parentPath.Node as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          DocumentNode expression = documentCompositeNode.Properties[parentPropertyKey];
          if (expression != null && expression.Type.IsExpression)
            expression = new ExpressionEvaluator(parentViewModel.DocumentRootResolver).EvaluateExpression(parentPath, expression);
          else if (expression == null && documentCompositeNode.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsImplicitStyles) && BaseFrameworkElement.StyleProperty.Equals((object) parentPropertyKey))
          {
            ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator(parentViewModel.DocumentRootResolver);
            DocumentNode node = documentCompositeNode.Context.CreateNode(typeof (Type), (object) documentCompositeNode.Type.RuntimeType);
            expression = expressionEvaluator.EvaluateResource(parentPath, node);
          }
          if (expression == null || expression != childPath.Node)
            flag = false;
        }
      }
      return flag;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("View: ");
      stringBuilder.Append(this.View.Caption);
      stringBuilder.Append(", NodePath: ");
      stringBuilder.Append((object) this.NodePath);
      stringBuilder.Append(", PropertyKey: ");
      stringBuilder.Append(this.PropertyKey != null ? this.PropertyKey.ToString() : "null");
      return "{" + stringBuilder.ToString() + "}";
    }
  }
}
