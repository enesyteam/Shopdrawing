// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ReferenceVerifierProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class ReferenceVerifierProcessor : DocumentProcessor
  {
    private IList<ReferenceVerifier> referenceVerifiers;
    private List<InvalidReferenceModel> invalidReferences;

    public IList<InvalidReferenceModel> InvalidReferences
    {
      get
      {
        return (IList<InvalidReferenceModel>) this.invalidReferences;
      }
    }

    protected override DocumentSearchScope SearchScope
    {
      get
      {
        return DocumentSearchScope.AllDocuments;
      }
    }

    public ReferenceVerifierProcessor(DesignerContext designerContext, DispatcherPriority priority)
      : base(designerContext, priority)
    {
      this.invalidReferences = new List<InvalidReferenceModel>();
      this.Initialize();
    }

    protected override void ProcessDocument(SceneDocument document)
    {
      DocumentNode rootNode = document.DocumentRoot.RootNode;
      foreach (ReferenceVerifier referenceVerifier in (IEnumerable<ReferenceVerifier>) this.referenceVerifiers)
      {
        foreach (DocumentNode node in rootNode.SelectDescendantNodes(referenceVerifier.ShouldVerify))
        {
          if (!referenceVerifier.Verify(node))
            this.invalidReferences.Add(referenceVerifier.CreateInvalidReferenceModel(node));
        }
      }
    }

    private void Initialize()
    {
      this.referenceVerifiers = (IList<ReferenceVerifier>) new List<ReferenceVerifier>();
      this.referenceVerifiers.Add((ReferenceVerifier) new TargetNameReferenceVerifier());
      this.referenceVerifiers.Add((ReferenceVerifier) new SourceNameReferenceVerifier());
      this.referenceVerifiers.Add((ReferenceVerifier) new BindingObjectReferenceVerifier(ProjectNeutralTypes.BehaviorTargetedTriggerAction, BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty));
      this.referenceVerifiers.Add((ReferenceVerifier) new BindingObjectReferenceVerifier(ProjectNeutralTypes.BehaviorEventTriggerBase, BehaviorEventTriggerBaseNode.BehaviorSourceObjectProperty));
      this.referenceVerifiers.Add((ReferenceVerifier) new StateNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.GoToStateAction, GoToStateActionNode.StateNameProperty));
      this.referenceVerifiers.Add((ReferenceVerifier) new StateNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.NavigationMenuAction, NavigationMenuActionNode.ActiveStateProperty));
      this.referenceVerifiers.Add((ReferenceVerifier) new StateNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.NavigationMenuAction, NavigationMenuActionNode.InactiveStateProperty));
      this.referenceVerifiers.Add((ReferenceVerifier) new ScreenNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.NavigationMenuAction, NavigationMenuActionNode.TargetScreenProperty, false));
      this.referenceVerifiers.Add((ReferenceVerifier) new ScreenNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.PlaySketchFlowAnimationAction, PlaySketchFlowAnimationActionNode.TargetScreenProperty, true));
      this.referenceVerifiers.Add((ReferenceVerifier) new StateNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.DataStateBehavior, DataStateBehaviorNode.TrueStateProperty));
      this.referenceVerifiers.Add((ReferenceVerifier) new StateNameReferenceVerifier(this.DesignerContext, ProjectNeutralTypes.DataStateBehavior, DataStateBehaviorNode.FalseStateProperty));
    }
  }
}
