// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontFamilyValueEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FontFamilyValueEditor : ContentControl
  {
    public static readonly DependencyProperty CurrentFontItemProperty = DependencyProperty.Register("CurrentFontItem", typeof (SourcedFontFamilyItem), typeof (FontFamilyValueEditor));
    private IProjectContext projectContext;
    private SceneNodeObjectSet sceneNodeObjectSet;
    private ObservableCollection<IProjectFont> projectFontFamilies;
    private FontFamilyEditor fontFamilyEditor;

    public SourcedFontFamilyItem CurrentFontItem
    {
      get
      {
        return (SourcedFontFamilyItem) this.GetValue(FontFamilyValueEditor.CurrentFontItemProperty);
      }
      set
      {
        this.SetValue(FontFamilyValueEditor.CurrentFontItemProperty, (object) value);
      }
    }

    public FontFamilyValueEditor()
    {
      this.fontFamilyEditor = new FontFamilyEditor();
      this.Content = (object) this.fontFamilyEditor;
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.FontFamilyValueEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.FontFamilyValueEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.FontFamilyValueEditor_Unloaded);
      this.SetBinding(FontFamilyValueEditor.CurrentFontItemProperty, (BindingBase) new Binding()
      {
        Source = (object) this.fontFamilyEditor,
        Path = new PropertyPath((object) FontFamilyEditor.CurrentFontItemProperty)
      });
    }

    private void Unload()
    {
      if (this.projectFontFamilies != null)
      {
        this.projectFontFamilies.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.projectFontFamilies_CollectionChanged);
        this.projectFontFamilies = (ObservableCollection<IProjectFont>) null;
      }
      SelectedElementsObjectSet elementsObjectSet = this.sceneNodeObjectSet as SelectedElementsObjectSet;
      if (elementsObjectSet != null)
        elementsObjectSet.ViewModelChanged -= new EventHandler(this.objectSet_ViewModelChanged);
      this.sceneNodeObjectSet = (SceneNodeObjectSet) null;
      this.projectContext = (IProjectContext) null;
    }

    private void Rebuild()
    {
      this.Unload();
      SceneNodePropertyValue nodePropertyValue = this.DataContext as SceneNodePropertyValue;
      if (nodePropertyValue == null)
        return;
      this.sceneNodeObjectSet = ((SceneNodeProperty) nodePropertyValue.ParentProperty).SceneNodeObjectSet;
      this.projectContext = this.sceneNodeObjectSet.ProjectContext;
      this.projectFontFamilies = this.projectContext.ProjectFonts;
      this.projectFontFamilies.CollectionChanged += new NotifyCollectionChangedEventHandler(this.projectFontFamilies_CollectionChanged);
      this.fontFamilyEditor.FontFamilies = FontFamilyValueEditor.GetFontFamilies(this.sceneNodeObjectSet);
      SelectedElementsObjectSet elementsObjectSet = this.sceneNodeObjectSet as SelectedElementsObjectSet;
      if (elementsObjectSet == null)
        return;
      elementsObjectSet.ViewModelChanged += new EventHandler(this.objectSet_ViewModelChanged);
    }

    private void objectSet_ViewModelChanged(object sender, EventArgs e)
    {
      if (this.projectContext == this.sceneNodeObjectSet.ProjectContext)
        return;
      this.Rebuild();
    }

    public static List<SourcedFontFamilyItem> GetFontFamilies(SceneNodeObjectSet sceneNodeObjectSet)
    {
      return FontFamilyValueEditor.GetFontFamilies(FontEmbedder.GetSystemFonts(sceneNodeObjectSet.DocumentContext.TypeResolver), (ICollection<IProjectFont>) sceneNodeObjectSet.ProjectContext.ProjectFonts, sceneNodeObjectSet);
    }

    private void FontFamilyValueEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.Unload();
    }

    private void FontFamilyValueEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.Rebuild();
    }

    private void FontFamilyValueEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.Unload();
      if (!this.IsLoaded)
        return;
      this.Rebuild();
    }

    private void InsertFontItem(SourcedFontFamilyItem item)
    {
      int index = this.fontFamilyEditor.FontFamilies.BinarySearch(item, (IComparer<SourcedFontFamilyItem>) new FontFamilyValueEditor.FontFamilyComparer());
      if (index < 0)
        index = ~index;
      this.fontFamilyEditor.FontFamilies.Insert(index, item);
    }

    private void projectFontFamilies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (this.sceneNodeObjectSet == null)
        return;
      if (e.OldItems != null)
      {
        foreach (ProjectFont projectFont in (IEnumerable) e.OldItems)
        {
          SystemFontFamilyItem systemFontFamilyItem = (SystemFontFamilyItem) null;
          bool flag = false;
          for (int index = this.fontFamilyEditor.FontFamilies.Count - 1; index >= 0; --index)
          {
            ProjectFontFamilyItem projectFontFamilyItem = this.fontFamilyEditor.FontFamilies[index] as ProjectFontFamilyItem;
            if (projectFontFamilyItem != null && projectFontFamilyItem.ProjectFont == projectFont)
            {
              if (projectFontFamilyItem.SystemFontFamilyItem != null)
                systemFontFamilyItem = projectFontFamilyItem.SystemFontFamilyItem;
              flag = this.fontFamilyEditor.CurrentFontItem == projectFontFamilyItem;
              this.fontFamilyEditor.FontFamilies.RemoveAt(index);
            }
          }
          if (systemFontFamilyItem != null)
          {
            this.InsertFontItem((SourcedFontFamilyItem) systemFontFamilyItem);
            if (flag)
              this.fontFamilyEditor.InvalidateProperty(FontFamilyEditor.CurrentFontItemProperty);
          }
        }
      }
      if (e.NewItems == null)
        return;
      foreach (ProjectFont projectFont in (IEnumerable) e.NewItems)
      {
        bool flag = false;
        ProjectFontFamilyItem projectFontFamilyItem = new ProjectFontFamilyItem(projectFont, this.sceneNodeObjectSet);
        SystemFontFamilyItem systemFontFamilyItem1 = (SystemFontFamilyItem) null;
        for (int index = 0; index < this.fontFamilyEditor.FontFamilies.Count; ++index)
        {
          SystemFontFamilyItem systemFontFamilyItem2 = this.fontFamilyEditor.FontFamilies[index] as SystemFontFamilyItem;
          if (systemFontFamilyItem2 != null && systemFontFamilyItem2.FamilyName == projectFontFamilyItem.FamilyName)
          {
            systemFontFamilyItem1 = systemFontFamilyItem2;
            this.fontFamilyEditor.FontFamilies.RemoveAt(index);
            flag = this.fontFamilyEditor.CurrentFontItem == systemFontFamilyItem2;
            break;
          }
        }
        projectFontFamilyItem.IsInEmbeddedFontSection = true;
        projectFontFamilyItem.SystemFontFamilyItem = systemFontFamilyItem1;
        this.InsertFontItem((SourcedFontFamilyItem) projectFontFamilyItem);
        if (flag)
          this.fontFamilyEditor.InvalidateProperty(FontFamilyEditor.CurrentFontItemProperty);
        this.InsertFontItem((SourcedFontFamilyItem) new ProjectFontFamilyItem(projectFont, this.sceneNodeObjectSet)
        {
          SystemFontFamilyItem = systemFontFamilyItem1
        });
      }
    }

    private static List<SourcedFontFamilyItem> GetFontFamilies(IEnumerable<SystemFontFamily> systemFonts, ICollection<IProjectFont> projectFonts, SceneNodeObjectSet sceneNodeObjectSet)
    {
      Dictionary<string, ProjectFontFamilyItem> dictionary = new Dictionary<string, ProjectFontFamilyItem>();
      List<SourcedFontFamilyItem> list1 = new List<SourcedFontFamilyItem>();
      foreach (ProjectFont projectFont in (IEnumerable<IProjectFont>) projectFonts)
      {
        ProjectFontFamilyItem projectFontFamilyItem = new ProjectFontFamilyItem(projectFont, sceneNodeObjectSet);
        projectFontFamilyItem.IsInEmbeddedFontSection = true;
        list1.Add((SourcedFontFamilyItem) projectFontFamilyItem);
        dictionary[projectFontFamilyItem.FamilyName] = projectFontFamilyItem;
      }
      List<SourcedFontFamilyItem> list2 = new List<SourcedFontFamilyItem>();
      foreach (SystemFontFamily systemFontFamily in systemFonts)
      {
        SystemFontFamilyItem systemFontFamilyItem = new SystemFontFamilyItem(systemFontFamily, sceneNodeObjectSet);
        ProjectFontFamilyItem projectFontFamilyItem;
        if (dictionary.TryGetValue(systemFontFamily.FontFamilyName, out projectFontFamilyItem))
        {
          projectFontFamilyItem = new ProjectFontFamilyItem(projectFontFamilyItem.ProjectFont, sceneNodeObjectSet);
          projectFontFamilyItem.SystemFontFamilyItem = systemFontFamilyItem;
          list2.Add((SourcedFontFamilyItem) projectFontFamilyItem);
        }
        else
          list2.Add((SourcedFontFamilyItem) systemFontFamilyItem);
      }
      list1.AddRange((IEnumerable<SourcedFontFamilyItem>) list2);
      list1.Sort((IComparer<SourcedFontFamilyItem>) new FontFamilyValueEditor.FontFamilyComparer());
      return list1;
    }

    private class FontFamilyComparer : IComparer<SourcedFontFamilyItem>
    {
      public int Compare(SourcedFontFamilyItem x, SourcedFontFamilyItem y)
      {
        ProjectFontFamilyItem projectFontFamilyItem1 = x as ProjectFontFamilyItem;
        ProjectFontFamilyItem projectFontFamilyItem2 = y as ProjectFontFamilyItem;
        if (projectFontFamilyItem1 != null && projectFontFamilyItem2 != null)
        {
          if (projectFontFamilyItem1.IsInEmbeddedFontSection != projectFontFamilyItem2.IsInEmbeddedFontSection)
            return !projectFontFamilyItem1.IsInEmbeddedFontSection ? 1 : -1;
        }
        else
        {
          if (projectFontFamilyItem1 != null && projectFontFamilyItem1.IsInEmbeddedFontSection)
            return -1;
          if (projectFontFamilyItem2 != null && projectFontFamilyItem2.IsInEmbeddedFontSection)
            return 1;
        }
        SystemFontFamilyItem systemFontFamilyItem1 = x as SystemFontFamilyItem;
        SystemFontFamilyItem systemFontFamilyItem2 = y as SystemFontFamilyItem;
        if (systemFontFamilyItem1 != null && systemFontFamilyItem2 != null)
        {
          if (systemFontFamilyItem1.DisplayAsNativeSilverlightFont != systemFontFamilyItem2.DisplayAsNativeSilverlightFont)
            return !systemFontFamilyItem1.DisplayAsNativeSilverlightFont ? 1 : -1;
        }
        else
        {
          if (systemFontFamilyItem1 != null && systemFontFamilyItem1.DisplayAsNativeSilverlightFont)
            return -1;
          if (systemFontFamilyItem2 != null && systemFontFamilyItem2.DisplayAsNativeSilverlightFont)
            return 1;
        }
        return string.Compare(x.FamilyName, y.FamilyName, StringComparison.CurrentCulture);
      }
    }
  }
}
