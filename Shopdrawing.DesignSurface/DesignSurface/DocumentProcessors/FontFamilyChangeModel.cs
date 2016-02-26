// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.FontFamilyChangeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class FontFamilyChangeModel : MultiDocumentReferenceChangeModel
  {
    private List<ReferenceRepairer> fontFamilyRepairers;

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
        return (IEnumerable<ReferenceRepairer>) this.fontFamilyRepairers;
      }
    }

    public FontChangeType FontChangeType { get; private set; }

    public IProjectFont ProjectFont { get; private set; }

    public FontFamilyChangeModel(string oldFontFamilyName, string newFontFamilyName, FontChangeType fontChangeType, IProjectFont projectFont, IDocumentRoot documentRoot, IProjectContext projectContext)
      : base(oldFontFamilyName, newFontFamilyName, documentRoot, projectContext)
    {
      this.fontFamilyRepairers = new List<ReferenceRepairer>();
      this.fontFamilyRepairers.Add((ReferenceRepairer) new FontFamilyReferenceRepairer(this));
      this.FontChangeType = fontChangeType;
      this.ProjectFont = projectFont;
    }
  }
}
