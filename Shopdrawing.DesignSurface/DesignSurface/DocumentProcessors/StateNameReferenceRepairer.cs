// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.StateNameReferenceRepairer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class StateNameReferenceRepairer : ReferenceRepairer
  {
    private StateNameChangeModel stateNameChangeModel;
    private ITypeId referentialObjectType;
    private IPropertyId referentialProperty;

    public override Predicate<DocumentNode> AppliesTo
    {
      get
      {
        return new Predicate<DocumentNode>(this.AppliesToImpl);
      }
    }

    public StateNameReferenceRepairer(StateNameChangeModel stateNameChangeModel, ITypeId referentialObjectType, IPropertyId referentialProperty)
    {
      this.stateNameChangeModel = stateNameChangeModel;
      this.referentialObjectType = referentialObjectType;
      this.referentialProperty = referentialProperty;
    }

    public override void Repair(DocumentNode node)
    {
      DocumentCompositeNode compositeNode = node as DocumentCompositeNode;
      IType codeBehindClass = this.stateNameChangeModel.ChangedNode.ViewModel.XamlDocument.CodeBehindClass;
      if (compositeNode == null)
        return;
      string valueAsString = compositeNode.GetValueAsString(this.referentialProperty);
      DocumentNode targetElement = GoToStateActionNode.FindTargetElement(node, false);
      if (targetElement == null || !(this.stateNameChangeModel.OldReferenceValue == valueAsString))
        return;
      if (targetElement.DocumentRoot.CodeBehindClass == null && codeBehindClass == null || targetElement.DocumentRoot.CodeBehindClass.Equals((object) codeBehindClass) && targetElement.DocumentRoot.RootNode == this.stateNameChangeModel.ChangedNode.DocumentNode.DocumentRoot.RootNode)
      {
        this.stateNameChangeModel.NodesForLocalUpdate.Add((ChangedNodeInfo) new ChangedStateNameNodeInfo(compositeNode, this.referentialProperty));
      }
      else
      {
        if (codeBehindClass == null || !targetElement.Type.IsAssignableFrom((ITypeId) codeBehindClass))
          return;
        this.stateNameChangeModel.NodesForExternalUpdate.Add((ChangedNodeInfo) new ChangedStateNameNodeInfo(compositeNode, this.referentialProperty));
      }
    }

    private bool AppliesToImpl(DocumentNode node)
    {
      return this.referentialObjectType.IsAssignableFrom((ITypeId) node.Type);
    }
  }
}
