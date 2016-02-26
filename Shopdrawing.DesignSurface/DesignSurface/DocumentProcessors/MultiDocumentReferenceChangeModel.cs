// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.MultiDocumentReferenceChangeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal abstract class MultiDocumentReferenceChangeModel : ReferenceChangeModel
  {
    public ICollection<ChangedNodeInfo> NodesForLocalUpdate { get; private set; }

    public ICollection<ChangedNodeInfo> NodesForExternalUpdate { get; private set; }

    public IDocumentRoot DocumentRoot { get; set; }

    public IProjectContext ProjectContext { get; set; }

    public MultiDocumentReferenceChangeModel(string oldReferenceValue, string newReferenceValue, IDocumentRoot documentRoot, IProjectContext projectContext)
      : base(oldReferenceValue, newReferenceValue)
    {
      this.NodesForLocalUpdate = (ICollection<ChangedNodeInfo>) new List<ChangedNodeInfo>();
      this.NodesForExternalUpdate = (ICollection<ChangedNodeInfo>) new List<ChangedNodeInfo>();
      this.DocumentRoot = documentRoot;
      this.ProjectContext = projectContext;
    }
  }
}
