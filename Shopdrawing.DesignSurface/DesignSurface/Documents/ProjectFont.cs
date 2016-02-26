// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.ProjectFont
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public class ProjectFont : IProjectFont
  {
    private List<IProjectItem> fontItems = new List<IProjectItem>();
    private List<FontFamily> fontFamilies = new List<FontFamily>();
    private FontFamily fontFamily;
    private string fontFamilyName;
    private IProjectItem fontItem;

    public FontFamily FontFamily
    {
      get
      {
        return this.fontFamily;
      }
    }

    public string FontDocumentPath
    {
      get
      {
        return this.fontItem.DocumentReference.Path;
      }
    }

    public DocumentReference FontDocumentReference
    {
      get
      {
        return this.fontItem.DocumentReference;
      }
    }

    public IEnumerable<DocumentReference> FontDocuments
    {
      get
      {
        foreach (IProjectItem projectItem in this.fontItems)
          yield return projectItem.DocumentReference;
      }
    }

    public IList<IProjectItem> FontItems
    {
      get
      {
        return (IList<IProjectItem>) this.fontItems;
      }
    }

    public IList<FontFamily> FontFamilies
    {
      get
      {
        return (IList<FontFamily>) this.fontFamilies;
      }
    }

    public string FontFamilyName
    {
      get
      {
        return this.fontFamilyName;
      }
    }

    public bool IsEmbedded
    {
      get
      {
        return this.fontItem.Properties["BuildAction"] == "BlendEmbeddedFont";
      }
    }

    public ProjectFont(FontFamily fontFamily, IProjectItem fontItem)
    {
      this.Initialize(fontFamily, fontItem);
    }

    public void Initialize(FontFamily fontFamily, IProjectItem fontItem)
    {
      this.fontFamily = fontFamily;
      this.fontItem = fontItem;
      this.fontFamilyName = FontEmbedder.GetFontNameFromSource(fontFamily);
    }
  }
}
