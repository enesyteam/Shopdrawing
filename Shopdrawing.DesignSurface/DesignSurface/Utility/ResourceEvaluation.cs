// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Utility.ResourceEvaluation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Utility
{
  internal class ResourceEvaluation
  {
    private DictionaryEntryNode originalResource;
    private DocumentNode evaluatedResource;
    private ResourceEvaluationResult conflictType;

    public DictionaryEntryNode OriginalResource
    {
      get
      {
        return this.originalResource;
      }
      set
      {
        this.originalResource = value;
      }
    }

    public DocumentNode EvaluatedResource
    {
      get
      {
        return this.evaluatedResource;
      }
      set
      {
        this.evaluatedResource = value;
      }
    }

    public ResourceEvaluationResult ConflictType
    {
      get
      {
        return this.conflictType;
      }
      set
      {
        this.conflictType = value;
      }
    }

    internal ResourceEvaluation(DictionaryEntryNode originalResource, DocumentNode evaluatedResource, ResourceEvaluationResult conflictType)
    {
      this.originalResource = originalResource;
      this.evaluatedResource = evaluatedResource;
      this.conflictType = conflictType;
    }
  }
}
