// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.StateNameBuilder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class StateNameBuilder : DocumentProcessor
  {
    private Dictionary<IType, List<string>> stateNamesByType;
    private Dictionary<DocumentNode, List<string>> stateNamesByNode;

    protected override DocumentSearchScope SearchScope
    {
      get
      {
        return DocumentSearchScope.AllDocuments;
      }
    }

    public StateNameBuilder(DesignerContext designerContext)
      : base(designerContext, (IAsyncMechanism) new SynchronousAsyncMechanism())
    {
      this.stateNamesByType = new Dictionary<IType, List<string>>();
      this.stateNamesByNode = new Dictionary<DocumentNode, List<string>>();
    }

    public IEnumerable<string> GetStateNamesForNode(DocumentNode node)
    {
      IType nodeType = StateNameBuilder.GetNodeType(node);
      List<string> list;
      if (!this.stateNamesByNode.TryGetValue(node, out list) && !this.stateNamesByType.TryGetValue(nodeType, out list))
        list = new List<string>();
      return (IEnumerable<string>) list;
    }

    protected override void ProcessDocument(SceneDocument document)
    {
      foreach (DocumentNode documentNode in document.DocumentRoot.RootNode.SelectDescendantNodes(ProjectNeutralTypes.VisualState))
      {
        DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          string valueAsString = documentCompositeNode.GetValueAsString((IPropertyId) documentCompositeNode.NameProperty);
          this.AddStateName((DocumentNode) documentCompositeNode, valueAsString);
        }
      }
    }

    private void AddStateName(DocumentNode stateNode, string stateName)
    {
      DocumentNode parentNode = StateNameBuilder.GetParentNode(stateNode);
      if (parentNode == null)
        return;
      IType nodeType = StateNameBuilder.GetNodeType(parentNode);
      List<string> list;
      if (nodeType.XamlSourcePath != null)
      {
        if (!this.stateNamesByType.TryGetValue(nodeType, out list))
        {
          list = new List<string>();
          this.stateNamesByType[nodeType] = list;
        }
      }
      else if (!this.stateNamesByNode.TryGetValue(parentNode, out list))
      {
        list = new List<string>();
        this.stateNamesByNode[parentNode] = list;
      }
      list.Add(stateName);
    }

    private static DocumentNode GetParentNode(DocumentNode stateNode)
    {
      DocumentNode childNode = BehaviorHelper.ValidateNodeTypeAndGetParent(BehaviorHelper.ValidateNodeTypeAndGetParent(BehaviorHelper.ValidateNodeTypeAndGetParent(BehaviorHelper.ValidateNodeTypeAndGetParent((DocumentNode) stateNode.Parent, PlatformTypes.IList), ProjectNeutralTypes.VisualStateGroup), PlatformTypes.IList), PlatformTypes.FrameworkElement);
      if (BehaviorHelper.ValidateNodeTypeAndGetParent(childNode, PlatformTypes.ICollection) != null && childNode.Parent != null)
        childNode = (DocumentNode) childNode.Parent;
      return childNode;
    }

    private static IType GetNodeType(DocumentNode node)
    {
      IType type = node.Type;
      if (node == node.DocumentRoot.RootNode && node.DocumentRoot.DocumentContext != null && node.DocumentRoot.CodeBehindClass != null)
        type = node.DocumentRoot.CodeBehindClass;
      return type;
    }
  }
}
