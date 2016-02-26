// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontFamilyEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class FontFamilyEditor : Decorator, IComponentConnector
  {
    public static readonly DependencyProperty FontFamiliesProperty = DependencyProperty.Register("FontFamilies", typeof (List<SourcedFontFamilyItem>), typeof (FontFamilyEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new List<SourcedFontFamilyItem>()));
    public static readonly DependencyProperty CurrentFontItemProperty = DependencyProperty.Register("CurrentFontItem", typeof (SourcedFontFamilyItem), typeof (FontFamilyEditor));
    public static readonly DependencyProperty FontFamilyEditorProperty = DependencyProperty.RegisterAttached("FontFamilyEditor", typeof (FontFamilyEditor), typeof (FontFamilyEditor));
    public static readonly DependencyProperty OwningPropertyContainerProperty = DependencyProperty.Register("OwningPropertyContainer", typeof (PropertyContainer), typeof (FontFamilyEditor), new PropertyMetadata(new PropertyChangedCallback(FontFamilyEditor.owningPropertyContainerChanged)));
    internal FontFamilyEditor Root;
    internal ChoiceEditor FontChoiceEditor;
    private bool _contentLoaded;

    public List<SourcedFontFamilyItem> FontFamilies
    {
      get
      {
        return (List<SourcedFontFamilyItem>) this.GetValue(FontFamilyEditor.FontFamiliesProperty);
      }
      set
      {
        this.SetValue(FontFamilyEditor.FontFamiliesProperty, value);
      }
    }

    public SourcedFontFamilyItem CurrentFontItem
    {
      get
      {
        return (SourcedFontFamilyItem) this.GetValue(FontFamilyEditor.CurrentFontItemProperty);
      }
      set
      {
        this.SetValue(FontFamilyEditor.CurrentFontItemProperty, value);
      }
    }

    public PropertyContainer OwningPropertyContainer
    {
      get
      {
        return (PropertyContainer) this.GetValue(FontFamilyEditor.OwningPropertyContainerProperty);
      }
      set
      {
        this.SetValue(FontFamilyEditor.OwningPropertyContainerProperty, value);
      }
    }

    public FontFamilyEditor()
    {
      SourcedFontFamilyItem.DefaultPreviewFontFamilyName = ((FontFamily) this.FindResource((object) SystemFonts.MessageFontFamilyKey)).ToString();
      this.InitializeComponent();
      BindingOperations.SetBinding((DependencyObject) this.Resources[(object) "FontConverter"], FontConverter.PropertyValueProperty, (BindingBase) new Binding("DataContext")
      {
        Source = this
      });
      this.SetBinding(FontFamilyEditor.CurrentFontItemProperty, (BindingBase) new MultiBinding()
      {
        Converter = (IMultiValueConverter) new FontFamilyEditor.FontItemConverter(),
        Bindings = {
          (BindingBase) new Binding()
          {
            Source = this.FontChoiceEditor,
            Path = new PropertyPath((object) ChoiceEditor.ValueProperty)
          },
          (BindingBase) new Binding()
          {
            Source = this,
            Path = new PropertyPath((object) FontFamilyEditor.FontFamiliesProperty)
          }
        }
      });
      this.SetBinding(FontFamilyEditor.OwningPropertyContainerProperty, (BindingBase) new Binding()
      {
        RelativeSource = RelativeSource.Self,
        Path = new PropertyPath("(0)", new object[1]
        {
          (object) PropertyContainer.OwningPropertyContainerProperty
        })
      });
    }

    private static void owningPropertyContainerChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      PropertyContainer propertyContainer = (PropertyContainer) e.NewValue;
      if (propertyContainer == null)
        return;
      ((DependencyObject) propertyContainer).SetValue(FontFamilyEditor.FontFamilyEditorProperty, sender);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/fontfamilyeditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (FontFamilyEditor) target;
          break;
        case 2:
          this.FontChoiceEditor = (ChoiceEditor) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class FontItemConverter : IMultiValueConverter
    {
      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
      {
        SourcedFontFamilyItem sourcedFontFamilyItem = values[0] as SourcedFontFamilyItem;
        IList list = values[1] as IList;
        if (sourcedFontFamilyItem == null || list == null)
          return values[0];
        int index = list.IndexOf((object) sourcedFontFamilyItem);
        if (index == -1)
          return (object) sourcedFontFamilyItem;
        return (object) (SourcedFontFamilyItem) list[index];
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
      {
        throw new NotImplementedException();
      }
    }
  }
}
