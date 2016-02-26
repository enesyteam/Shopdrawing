// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.NameChangeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class NameChangeModel : ReferenceChangeModel
  {
    private List<ReferenceRepairer> nameRepairers;

    public override DocumentSearchScope ChangeScope
    {
      get
      {
        return DocumentSearchScope.ActiveDocument;
      }
    }

    public DocumentNodeNameScope NameScope { get; private set; }

    public override IEnumerable<ReferenceRepairer> ReferenceRepairers
    {
      get
      {
        return (IEnumerable<ReferenceRepairer>) this.nameRepairers;
      }
    }

    public NameChangeModel(string oldName, string newName, SceneNode sceneNode)
      : base(oldName, newName)
    {
      this.NameScope = sceneNode.DocumentNode.NameScope != null ? sceneNode.DocumentNode.NameScope : sceneNode.DocumentNode.FindContainingNameScope();
      this.nameRepairers = new List<ReferenceRepairer>();
      this.nameRepairers.Add((ReferenceRepairer) new TargetNameReferenceRepairer(this));
      this.nameRepairers.Add((ReferenceRepairer) new SourceNameReferenceRepairer(this));
    }
  }
}
