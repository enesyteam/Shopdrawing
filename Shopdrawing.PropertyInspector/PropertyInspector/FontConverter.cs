// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FontConverter : DependencyObject, IValueConverter
  {
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof (PropertyValue), typeof (FontConverter));

    public PropertyValue PropertyValue
    {
      get
      {
        return (PropertyValue) this.GetValue(FontConverter.PropertyValueProperty);
      }
      set
      {
        this.SetValue(FontConverter.PropertyValueProperty, value);
      }
    }

    private SceneViewModel ViewModel
    {
      get
      {
        return ((SceneNodeProperty) this.PropertyValue.get_ParentProperty()).SceneNodeObjectSet.ViewModel;
      }
    }

    private SceneNodeObjectSet SceneNodeObjectSet
    {
      get
      {
        return ((SceneNodeProperty) this.PropertyValue.get_ParentProperty()).SceneNodeObjectSet;
      }
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      IDocumentContext documentContext = this.SceneNodeObjectSet.DocumentContext;
      FontFamily fontFamily1 = this.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(documentContext, o) as FontFamily;
      if (fontFamily1 == null)
        return o;
      string source = fontFamily1.Source;
      Uri result;
      bool flag = !Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out result) ? !Path.IsPathRooted(source) : !result.IsAbsoluteUri;
      if (string.IsNullOrEmpty(source) || !flag || source.IndexOf('#') == -1)
        return (object) new UnknownSourceFontFamilyItem(fontFamily1, this.SceneNodeObjectSet);
      FontFamily fontFamily2 = fontFamily1;
      return (object) new UnknownSourceFontFamilyItem(FontEmbedder.MakeDesignTimeFontFamily(fontFamily2, documentContext), fontFamily2, this.SceneNodeObjectSet);
    }

    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      SourcedFontFamilyItem sourcedFontFamilyItem = o as SourcedFontFamilyItem;
      if (sourcedFontFamilyItem == null)
        return o;
      o = this.ViewModel.DefaultView.ConvertFromWpfValue((object) sourcedFontFamilyItem.SerializeFontFamily);
      return o;
    }
  }
}
