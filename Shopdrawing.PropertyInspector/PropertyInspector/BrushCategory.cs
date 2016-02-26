// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class BrushCategory : SceneNodeCategory
  {
    private DesignerContext designerContext;
    private ListCollectionView brushCollectionView;
    private bool forcedToSelectOpacityMask;

    public ICollectionView BrushCollectionView
    {
      get
      {
        return (ICollectionView) this.brushCollectionView;
      }
    }

    public BrushCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Brushes, localizedName, messageLogger)
    {
      ObservableCollection<PropertyEntry> basicProperties = this.BasicProperties;
      this.brushCollectionView = new ListCollectionView((IList) basicProperties);
      this.brushCollectionView.CustomSort = (IComparer) new BrushCategory.PropertyComparer();
      this.brushCollectionView.CurrentChanged += new EventHandler(this.OnBrushCurrentChanged);
      basicProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnBasicPropertiesChanged);
    }

    private void OnBrushCurrentChanged(object sender, EventArgs e)
    {
      this.UpdateSelectionSet();
    }

    private void UpdateSelectionSet()
    {
      if (this.designerContext == null)
        return;
      PropertyReferenceProperty referenceProperty = this.brushCollectionView.CurrentItem as PropertyReferenceProperty;
      if (referenceProperty == null || this.designerContext.ActiveDocument == null || (this.designerContext.ActiveSceneViewModel == null || this.designerContext.ActiveDocument.IsUndoingOrRedoing))
        return;
      this.designerContext.ActiveSceneViewModel.PropertySelectionSet.SetSelection(referenceProperty.Reference);
      this.forcedToSelectOpacityMask = this.brushCollectionView.Count == 1 && string.Compare(((PropertyEntry) referenceProperty).get_PropertyName(), UIElement.OpacityMaskProperty.Name, StringComparison.Ordinal) == 0;
    }

    private void OnBasicPropertiesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.brushCollectionView.MoveCurrentTo(null);
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      if (selectedObjects.Length > 0)
        this.designerContext = selectedObjects[0].DesignerContext;
      if (this.designerContext != null && this.designerContext.ActiveSceneViewModel != null)
      {
        PropertyReference primarySelection = this.designerContext.ActiveSceneViewModel.PropertySelectionSet.PrimarySelection;
        PropertyReferenceProperty referenceProperty1 = (PropertyReferenceProperty) null;
        if (!this.forcedToSelectOpacityMask && primarySelection != null)
        {
          using (IEnumerator<PropertyEntry> enumerator = this.BasicProperties.GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              PropertyReferenceProperty referenceProperty2 = (PropertyReferenceProperty) enumerator.Current;
              if (string.Compare(((PropertyEntry) referenceProperty2).get_PropertyName(), primarySelection.ShortPath, StringComparison.Ordinal) == 0)
              {
                referenceProperty1 = referenceProperty2;
                break;
              }
            }
          }
        }
        if (referenceProperty1 == null)
        {
          if (this.brushCollectionView.CurrentPosition == 0)
            this.UpdateSelectionSet();
          else
            this.brushCollectionView.MoveCurrentToFirst();
        }
        else if (referenceProperty1 != this.brushCollectionView.CurrentItem)
          this.brushCollectionView.MoveCurrentTo((object) referenceProperty1);
      }
      Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.BrushCategory.ResetLastUsedBrushes();
    }

    private class PropertyComparer : System.Collections.Generic.Comparer<PropertyEntry>
    {
      public override int Compare(PropertyEntry first, PropertyEntry second)
      {
        if (string.Compare(first.get_PropertyName(), UIElement.OpacityMaskProperty.Name, StringComparison.Ordinal) == 0)
          return 1;
        if (string.Compare(second.get_PropertyName(), UIElement.OpacityMaskProperty.Name, StringComparison.Ordinal) == 0)
          return -1;
        return string.Compare(first.get_PropertyName(), second.get_PropertyName(), StringComparison.Ordinal);
      }
    }
  }
}
