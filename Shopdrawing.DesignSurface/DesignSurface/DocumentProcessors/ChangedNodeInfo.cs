// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ChangedNodeInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class ChangedNodeInfo
  {
    public DocumentNode Node { get; private set; }

    public string FilePath { get; private set; }

    public ChangedNodeInfo(DocumentNode node)
    {
      this.Node = node;
      this.FilePath = node.Context.DocumentUrl;
    }
  }
}
