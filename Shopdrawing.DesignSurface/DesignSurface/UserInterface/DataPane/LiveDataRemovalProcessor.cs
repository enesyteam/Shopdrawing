// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.LiveDataRemovalProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class LiveDataRemovalProcessor : DataRemovalProcessor
  {
    private DocumentCompositeNode resourceNode;

    internal override string TransactionDescription
    {
      get
      {
        return StringTable.UndoUnitRemoveDataSource;
      }
    }

    public LiveDataRemovalProcessor(IAsyncMechanism asyncMechanism, DocumentCompositeNode resourceNode, ChangeProcessingModes processingMode)
      : base(asyncMechanism, (IProjectContext) resourceNode.TypeResolver, processingMode)
    {
      this.resourceNode = resourceNode;
    }

    internal override bool ShouldRemoveNode(DocumentCompositeNode documentNode)
    {
      return this.ResolveResourceReferenceIfNeeded(documentNode) == this.resourceNode;
    }
  }
}
