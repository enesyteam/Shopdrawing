// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.FontGallery
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class FontGallery : Grid, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty SelectedFontFamilyItemProperty = DependencyProperty.Register("SelectedFontFamilyItem", typeof (FontFamilyItem), typeof (FontGallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty HighlightedFontFamilyItemProperty = DependencyProperty.Register("HighlightedFontFamilyItem", typeof (FontFamilyItem), typeof (FontGallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    private ObservableCollection<FontFamilyItem> fontFamilies = new ObservableCollection<FontFamilyItem>();
    private ObservableCollection<FontFamilyItem> fontFamiliesMRU = new ObservableCollection<FontFamilyItem>();
    private ObservableCollection<FontFamilyItem> fontFamiliesFavorites = new ObservableCollection<FontFamilyItem>();
    private ICollectionView fontFamiliesCollectionView;
    private ICollectionView fontFamiliesMRUCollectionView;
    private ICollectionView fontFamiliesFavoritesCollectionView;
    private static string defaultPreviewFontFamilyName;
    internal FontGallery UserControlSelf;
    internal Gallery InternalGalleryControl;
    private bool _contentLoaded;

    public FontFamilyItem SelectedFontFamilyItem
    {
      get
      {
        return (FontFamilyItem) this.GetValue(FontGallery.SelectedFontFamilyItemProperty);
      }
      set
      {
        this.SetValue(FontGallery.SelectedFontFamilyItemProperty, (object) value);
      }
    }

    public FontFamilyItem HighlightedFontFamilyItem
    {
      get
      {
        return (FontFamilyItem) this.GetValue(FontGallery.HighlightedFontFamilyItemProperty);
      }
      set
      {
        this.SetValue(FontGallery.HighlightedFontFamilyItemProperty, (object) value);
      }
    }

    public ICollectionView FontFamiliesCollectionView
    {
      get
      {
        return this.fontFamiliesCollectionView;
      }
    }

    public ICollectionView FontFamiliesMRUCollectionView
    {
      get
      {
        return this.fontFamiliesMRUCollectionView;
      }
    }

    public ICollectionView FontFamiliesFavoritesCollectionView
    {
      get
      {
        return this.fontFamiliesFavoritesCollectionView;
      }
    }

    public IList<FontFamilyItem> FontFamilies
    {
      get
      {
        return (IList<FontFamilyItem>) this.fontFamilies;
      }
    }

    public IList<FontFamilyItem> FontFamiliesMRU
    {
      get
      {
        return (IList<FontFamilyItem>) this.fontFamiliesMRU;
      }
    }

    public IList<FontFamilyItem> FontFamiliesFavorites
    {
      get
      {
        return (IList<FontFamilyItem>) this.fontFamiliesFavorites;
      }
    }

    public Gallery GalleryControl
    {
      get
      {
        return this.InternalGalleryControl;
      }
    }

    public IList FontFamiliesFavoritesList
    {
      get
      {
        return (IList) this.fontFamiliesFavorites;
      }
    }

    public static string DefaultPreviewFontFamilyName
    {
      get
      {
        return FontGallery.defaultPreviewFontFamilyName;
      }
    }

    public FontGallery()
    {
      this.fontFamiliesCollectionView = (ICollectionView) new ListCollectionView((IList) this.fontFamilies);
      this.fontFamiliesCollectionView.SortDescriptions.Add(new SortDescription("SortOverride", ListSortDirection.Ascending));
      this.fontFamiliesCollectionView.SortDescriptions.Add(new SortDescription("FamilyName", ListSortDirection.Ascending));
      this.fontFamiliesMRUCollectionView = CollectionViewSource.GetDefaultView((object) this.fontFamiliesMRU);
      this.fontFamiliesFavoritesCollectionView = CollectionViewSource.GetDefaultView((object) this.fontFamiliesFavorites);
      this.InitializeComponent();
      FontGallery.defaultPreviewFontFamilyName = ((FontFamily) this.FindResource((object) SystemFonts.MessageFontFamilyKey)).ToString();
    }

    private void GalleryItemDoubleClicked(object sender, MouseButtonEventArgs e)
    {
      this.RaiseEvent(new RoutedEventArgs(Gallery.DoubleClickOnItemEvent, sender));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/valueeditors/fontgallery.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControlSelf = (FontGallery) target;
          break;
        case 3:
          this.InternalGalleryControl = (Gallery) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((Style) target).Setters.Add((SetterBase) new EventSetter()
      {
        Event = Control.MouseDoubleClickEvent,
        Handler = (Delegate) new MouseButtonEventHandler(this.GalleryItemDoubleClicked)
      });
    }
  }
}
