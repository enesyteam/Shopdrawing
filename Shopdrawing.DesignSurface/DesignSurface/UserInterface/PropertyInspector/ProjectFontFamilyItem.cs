// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ProjectFontFamilyItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ProjectFontFamilyItem : SourcedFontFamilyItem
  {
    public const string ProjectFontCategory = "Project Font";
    private ProjectFont projectFont;

    public ProjectFont ProjectFont
    {
      get
      {
        return this.projectFont;
      }
    }

    public override FontFamily DisplayFontFamily
    {
      get
      {
        return this.FontFamily;
      }
    }

    protected override FontFamily FontFamilySource
    {
      get
      {
        return this.SerializeFontFamily;
      }
    }

    public override FontFamily SerializeFontFamily
    {
      get
      {
        return FontEmbedder.MakeProjectFontReference((IProjectFont) this.projectFont, this.DocumentContext);
      }
    }

    public bool IsInEmbeddedFontSection { get; set; }

    public SystemFontFamilyItem SystemFontFamilyItem { get; set; }

    public ProjectFontFamilyItem(ProjectFont projectFont, IDocumentContext documentContext)
      : base(projectFont.FontFamily, "Project Font", documentContext)
    {
      this.projectFont = projectFont;
    }

    public ProjectFontFamilyItem(ProjectFont projectFont, SceneNodeObjectSet sceneNodeObjectSet)
      : base(projectFont.FontFamily, "Project Font", sceneNodeObjectSet)
    {
      this.projectFont = projectFont;
    }
  }
}
