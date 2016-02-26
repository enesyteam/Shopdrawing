// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.StateNameChangeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class StateNameChangeModel : MultiDocumentReferenceChangeModel
  {
    private SceneNode changedNode;
    private List<ReferenceRepairer> repairers;

    public SceneNode ChangedNode
    {
      get
      {
        return this.changedNode;
      }
    }

    public override DocumentSearchScope ChangeScope
    {
      get
      {
        return DocumentSearchScope.AllDocuments;
      }
    }

    public override IEnumerable<ReferenceRepairer> ReferenceRepairers
    {
      get
      {
        return (IEnumerable<ReferenceRepairer>) this.repairers;
      }
    }

    public StateNameChangeModel(string oldStateNameValue, string newStateNameValue, SceneNode changedNode)
      : base(oldStateNameValue, newStateNameValue, changedNode.DocumentNode.DocumentRoot, changedNode.ProjectContext)
    {
      this.changedNode = changedNode;
      this.repairers = new List<ReferenceRepairer>();
      this.repairers.Add((ReferenceRepairer) new StateNameReferenceRepairer(this, ProjectNeutralTypes.GoToStateAction, GoToStateActionNode.StateNameProperty));
      this.repairers.Add((ReferenceRepairer) new StateNameReferenceRepairer(this, ProjectNeutralTypes.NavigationMenuAction, NavigationMenuActionNode.ActiveStateProperty));
      this.repairers.Add((ReferenceRepairer) new StateNameReferenceRepairer(this, ProjectNeutralTypes.NavigationMenuAction, NavigationMenuActionNode.InactiveStateProperty));
      this.repairers.Add((ReferenceRepairer) new StateNameReferenceRepairer(this, ProjectNeutralTypes.DataStateBehavior, DataStateBehaviorNode.TrueStateProperty));
      this.repairers.Add((ReferenceRepairer) new StateNameReferenceRepairer(this, ProjectNeutralTypes.DataStateBehavior, DataStateBehaviorNode.FalseStateProperty));
    }
  }
}
