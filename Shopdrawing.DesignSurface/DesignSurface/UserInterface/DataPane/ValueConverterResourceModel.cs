// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ValueConverterResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ValueConverterResourceModel : ValueConverterModel
  {
    private DictionaryEntryNode valueConverter;

    public override string DisplayName
    {
      get
      {
        return this.valueConverter.Key as string;
      }
    }

    public ValueConverterResourceModel(DictionaryEntryNode valueConverter)
    {
      this.valueConverter = valueConverter;
    }

    public override SceneNode GenerateConverter()
    {
      IDocumentContext documentContext = this.valueConverter.DocumentContext;
      DocumentNode keyNode = (DocumentNode) documentContext.CreateNode(this.DisplayName);
      return this.valueConverter.ViewModel.GetSceneNode((DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(documentContext, keyNode));
    }
  }
}
