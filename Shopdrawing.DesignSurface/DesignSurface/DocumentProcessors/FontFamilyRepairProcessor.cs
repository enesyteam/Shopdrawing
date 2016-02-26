// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.FontFamilyRepairProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class FontFamilyRepairProcessor : MultiDocumentReferenceRepairProcessor
  {
    private FontFamilyChangeModel fontFamilyChangeModel;

    protected override string UndoDescription
    {
      get
      {
        return StringTable.EmbedFontUndoUnit;
      }
    }

    public FontFamilyRepairProcessor(DesignerContext designerContext, FontFamilyChangeModel fontFamilyChangeModel)
      : base(designerContext, (MultiDocumentReferenceChangeModel) fontFamilyChangeModel)
    {
      this.fontFamilyChangeModel = fontFamilyChangeModel;
    }

    protected override bool ShouldUpdateWithExternalChanges(List<ChangedNodeInfo> coalescedNodes)
    {
      return true;
    }

    protected override void ProcessDocument(SceneDocument document)
    {
      if (document.ProjectContext != this.fontFamilyChangeModel.ProjectContext)
        return;
      base.ProcessDocument(document);
    }

    protected override void ApplyChange(ChangedNodeInfo changedNodeInfo)
    {
      DocumentNode node = changedNodeInfo.Node;
      FontFamily oldFontFamily;
      FontFamily newFontFamily;
      FontEmbedder.CreateFontFamilyChange(this.fontFamilyChangeModel.FontChangeType, this.fontFamilyChangeModel.ProjectFont, changedNodeInfo.Node.Context, out oldFontFamily, out newFontFamily);
      DocumentNode documentNode = (DocumentNode) node.Context.CreateNode(PlatformTypes.FontFamily, (IDocumentNodeValue) new DocumentNodeStringValue(newFontFamily.Source));
      if (node.SiteChildIndex != -1)
      {
        node.Parent.Children[node.SiteChildIndex] = documentNode;
      }
      else
      {
        if (node.SitePropertyKey == null)
          return;
        node.Parent.Properties[(IPropertyId) node.SitePropertyKey] = documentNode;
      }
    }
  }
}
