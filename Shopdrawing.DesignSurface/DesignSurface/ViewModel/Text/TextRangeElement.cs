// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Text.TextRangeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.Text
{
  internal abstract class TextRangeElement : SceneElement
  {
    private TextEditProxy textEditProxy;

    public TextEditProxy TextEditProxy
    {
      get
      {
        return this.textEditProxy;
      }
      protected set
      {
        this.textEditProxy = value;
      }
    }

    public override IPropertyId NameProperty
    {
      get
      {
        return (IPropertyId) this.textEditProxy.TextSource.Metadata.NameProperty;
      }
    }

    public abstract IEnumerable<IPropertyId> RangeProperties { get; }

    public bool IsTextRangeAttached
    {
      get
      {
        return this.textEditProxy.TextSource.IsAttached;
      }
    }

    public static TextRangeElement CreateTextRangeElementFromProxy(TextEditProxy proxy)
    {
      if (proxy.SupportsParagraphProperties)
      {
        ITypeId targetType = PlatformTypes.Inline;
        return (TextRangeElement) RichTextBoxParagraphsRangeElement.Factory.Instantiate(proxy, proxy.TextSource.ViewModel, targetType);
      }
      if (proxy.SupportsRangeProperties)
      {
        ITypeId targetType = PlatformTypes.Inline;
        return (TextRangeElement) RichTextBoxRangeElement.Factory.Instantiate(proxy, proxy.TextSource.ViewModel, targetType);
      }
      if (PlatformTypes.TextBox.IsAssignableFrom((ITypeId) proxy.TextSource.Type) || PlatformTypes.AccessText.IsAssignableFrom((ITypeId) proxy.TextSource.Type))
        return (TextRangeElement) TextBoxRangeElement.Factory.Instantiate(proxy, proxy.TextSource.ViewModel, (ITypeId) proxy.TextSource.Type);
      return (TextRangeElement) ControlTextRangeElement.Factory.Instantiate(proxy, proxy.TextSource.ViewModel, (ITypeId) proxy.TextSource.Type);
    }
  }
}
