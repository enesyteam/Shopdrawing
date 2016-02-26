// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ValueConverterModelFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public static class ValueConverterModelFactory
  {
    public static ValueConverterModel CreateValueConverterModel(SceneNode valueConverter)
    {
      DictionaryEntryNode dictionaryEntryNode = valueConverter as DictionaryEntryNode;
      if (dictionaryEntryNode != null && ValueConverterModelFactory.IsResourceValueConverter(dictionaryEntryNode))
        return (ValueConverterModel) new ValueConverterResourceModel(dictionaryEntryNode);
      if (PlatformTypes.IValueConverter.IsAssignableFrom((ITypeId) valueConverter.Type))
        return (ValueConverterModel) new ValueConverterLocalModel(valueConverter);
      return (ValueConverterModel) null;
    }

    public static bool IsResourceValueConverter(DictionaryEntryNode entry)
    {
      if (PlatformTypes.IValueConverter.IsAssignableFrom((ITypeId) entry.Value.Type))
        return entry.Key is string;
      return false;
    }
  }
}
