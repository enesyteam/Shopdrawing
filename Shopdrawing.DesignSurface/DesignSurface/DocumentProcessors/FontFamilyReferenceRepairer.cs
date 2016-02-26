// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.FontFamilyReferenceRepairer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class FontFamilyReferenceRepairer : ReferenceRepairer
  {
    protected FontFamilyChangeModel FontFamilyChangeModel { get; private set; }

    public override sealed Predicate<DocumentNode> AppliesTo
    {
      get
      {
        return new Predicate<DocumentNode>(this.AppliesToImpl);
      }
    }

    public FontFamilyReferenceRepairer(FontFamilyChangeModel fontFamilyChangeModel)
    {
      this.FontFamilyChangeModel = fontFamilyChangeModel;
    }

    public override sealed void Repair(DocumentNode node)
    {
      if (this.FontFamilyChangeModel.DocumentRoot != node.DocumentRoot)
        this.FontFamilyChangeModel.NodesForExternalUpdate.Add(new ChangedNodeInfo(node));
      else
        this.FontFamilyChangeModel.NodesForLocalUpdate.Add(new ChangedNodeInfo(node));
    }

    private bool AppliesToImpl(DocumentNode node)
    {
      if (PlatformTypes.FontFamily.IsAssignableFrom((ITypeId) node.Type))
      {
        DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
        {
          DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
          if (documentNodeStringValue != null)
          {
            FontFamily oldFontFamily;
            FontFamily newFontFamily;
            FontEmbedder.CreateFontFamilyChange(this.FontFamilyChangeModel.FontChangeType, this.FontFamilyChangeModel.ProjectFont, node.Context, out oldFontFamily, out newFontFamily);
            return FontEmbedder.AreFontsEqual(new FontFamily(FontFamilyHelper.EnsureFamilyName(documentNodeStringValue.Value)), oldFontFamily, node.Context);
          }
        }
      }
      return false;
    }
  }
}
