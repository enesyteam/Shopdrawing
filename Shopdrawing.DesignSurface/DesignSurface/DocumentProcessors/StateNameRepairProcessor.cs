// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.StateNameRepairProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class StateNameRepairProcessor : MultiDocumentReferenceRepairProcessor
  {
    protected override string UndoDescription
    {
      get
      {
        return StringTable.UpdateStateNameReferenceUndoDescription;
      }
    }

    public StateNameRepairProcessor(DesignerContext designerContext, StateNameChangeModel stateNameChangeModel)
      : base(designerContext, (MultiDocumentReferenceChangeModel) stateNameChangeModel)
    {
    }

    protected override bool ShouldUpdateWithExternalChanges(List<ChangedNodeInfo> coalescedNodes)
    {
      bool? nullable = new StateReferencesFoundDialog(new StateReferencesFoundModel((IEnumerable<ChangedNodeInfo>) coalescedNodes)).ShowDialog();
      if (nullable.HasValue)
        return nullable.Value;
      return false;
    }

    protected override void ApplyChange(ChangedNodeInfo changedNodeInfo)
    {
      ChangedStateNameNodeInfo stateNameNodeInfo = (ChangedStateNameNodeInfo) changedNodeInfo;
      stateNameNodeInfo.CompositeNode.SetValue<string>(stateNameNodeInfo.ChangedProperty, this.MultiDocumentReferenceChangeModel.NewReferenceValue);
    }
  }
}
