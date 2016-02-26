// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.IEditContextHistory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  internal interface IEditContextHistory
  {
    EditContext ActiveEditContext { get; }

    EditContext NextActiveParentContext { get; }

    EditContext GetNextActiveParentContext(EditContext editContext, bool crossView);

    EditContext GetParentContext(EditContext editContext, bool crossView);

    EditContext GetChildContext(EditContext editContext, bool crossView);

    EditContext GetContext(DocumentNodePath editingContainerPath);

    EditContext GetContextFromNodeViewPair(NodeViewPair nodeViewPair, bool crossView);

    NodeViewPair GetChildNodeViewPair(EditContext editContext);

    bool Walk(bool includeGhostedHistory, bool reverseWalk, SingleHistoryCallback callback);

    bool Walk(EditContext startAtContext, bool reverseWalk, SingleHistoryCallback callback);

    void MoveToParent();

    bool MoveToEditContext(EditContext editContext);

    bool MoveToEditContext(DocumentNodePath editingContainerPath);

    void ReplaceContext(EditContext editContext);

    void ReplaceActive(EditContext editContext);

    void UpdateDrillInHistory(DocumentNodePath selectedElementPath, NodeViewPair childEditingContainer);

    void Push(EditContext editContext);

    void Canonicalize(SceneUpdateTypeFlags flags, DocumentNodeChangeList damage);

    void RemoveContextsUnderNodePath(DocumentNodePath targetNodePath);
  }
}
