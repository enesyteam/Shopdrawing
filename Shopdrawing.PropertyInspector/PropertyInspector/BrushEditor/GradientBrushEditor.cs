// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.GradientBrushEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal class GradientBrushEditor : BrushSubtypeEditor
  {
    private DataTemplate editorTemplate;
    private BrushCategory brushCategory;
    private SceneNodeProperty gradientStopsProperty;
    private SceneNodeProperty mappingModeProperty;
    private SceneNodeProperty spreadMethodProperty;
    private SceneNodeProperty opacityProperty;
    private SceneNodeProperty relativeTransformProperty;
    private DocumentNode removedGradientStop;
    private int removedGradientStopIndex;
    private ObservableCollection<GradientStopEditor> gradientStops;
    private ListCollectionView gradientStopsView;
    private ListCollectionView sortedGradientStopsView;

    public override BrushCategory Category
    {
      get
      {
        return this.brushCategory;
      }
    }

    public override DataTemplate EditorTemplate
    {
      get
      {
        return this.editorTemplate;
      }
    }

    public ICommand SelectNextGradientStop
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectNextGradientStopHandler));
      }
    }

    public ICommand SelectPreviousGradientStop
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectPreviousGradientStopHandler));
      }
    }

    public bool IsGradientStopIteratorEnabled
    {
      get
      {
        return this.gradientStops.Count >= 2;
      }
    }

    public Brush Brush
    {
      get
      {
        LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
        foreach (GradientStopEditor gradientStopEditor in (Collection<GradientStopEditor>) this.gradientStops)
          linearGradientBrush.GradientStops.Add(new GradientStop(gradientStopEditor.Color, gradientStopEditor.Offset));
        linearGradientBrush.StartPoint = new Point(0.0, 0.5);
        linearGradientBrush.EndPoint = new Point(1.0, 0.5);
        return (Brush) linearGradientBrush;
      }
      set
      {
        GradientBrush gradientBrush = value as GradientBrush;
        if (gradientBrush == null)
          return;
        SceneView defaultView = this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView;
        PropertyValueEditorCommands.get_BeginTransaction().Execute((object) null, (IInputElement) this.BrushEditor);
        GradientStopCollection gradientStopCollection = new GradientStopCollection();
        this.gradientStopsProperty.SetValue(defaultView.ConvertFromWpfValue((object) gradientStopCollection));
        for (int index = 0; index < gradientBrush.GradientStops.Count; ++index)
          this.gradientStopsProperty.AddValue(defaultView.ConvertFromWpfValue((object) gradientBrush.GradientStops[index]));
        PropertyValueEditorCommands.get_CommitTransaction().Execute((object) null, (IInputElement) this.BrushEditor);
        this.RebuildModel(-1);
      }
    }

    public CollectionView GradientStops
    {
      get
      {
        return (CollectionView) this.gradientStopsView;
      }
    }

    public BrushMappingMode MappingMode
    {
      get
      {
        return (BrushMappingMode) this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.mappingModeProperty.GetValue());
      }
      set
      {
        this.mappingModeProperty.SetValue(this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue((object) value));
      }
    }

    public GradientSpreadMethod SpreadMethod
    {
      get
      {
        return (GradientSpreadMethod) this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.spreadMethodProperty.GetValue());
      }
      set
      {
        this.spreadMethodProperty.SetValue(this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue((object) value));
      }
    }

    public Transform RelativeTransform
    {
      get
      {
        return (Transform) this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.relativeTransformProperty.GetValue());
      }
      set
      {
        this.relativeTransformProperty.SetValue(this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue((object) value));
      }
    }

    public ICommand ReverseGradientStopsCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ReverseGradientStopsHandler));
      }
    }

    public bool IsLinearGradient
    {
      get
      {
        return this.Category.Type == PlatformTypes.LinearGradientBrush;
      }
      set
      {
        if (this.IsLinearGradient || !value)
          return;
        SceneView defaultView = this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView;
        object obj = this.BasisProperty.GetValue();
        LinearGradientBrush linearGradientBrush1 = new LinearGradientBrush(((GradientBrush) defaultView.ConvertToWpfValue(obj)).GradientStops);
        LinearGradientBrush linearGradientBrush2 = (LinearGradientBrush) BrushCategory.LinearGradient.DefaultBrush;
        linearGradientBrush1.StartPoint = linearGradientBrush2.StartPoint;
        linearGradientBrush1.EndPoint = linearGradientBrush2.EndPoint;
        linearGradientBrush1.MappingMode = linearGradientBrush2.MappingMode;
        this.BasisProperty.SetValue(defaultView.ConvertFromWpfValue((object) linearGradientBrush1));
        this.brushCategory = BrushCategory.LinearGradient;
        this.OnPropertyChanged("IsLinearGradient");
        this.OnPropertyChanged("IsRadialGradient");
        this.RebuildAdvancedProperties();
      }
    }

    public bool IsRadialGradient
    {
      get
      {
        return this.Category.Type == PlatformTypes.RadialGradientBrush;
      }
      set
      {
        if (this.IsRadialGradient || !value)
          return;
        SceneView defaultView = this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView;
        object obj = this.BasisProperty.GetValue();
        RadialGradientBrush radialGradientBrush = new RadialGradientBrush(((GradientBrush) defaultView.ConvertToWpfValue(obj)).GradientStops);
        this.BasisProperty.SetValue(defaultView.ConvertFromWpfValue((object) radialGradientBrush));
        this.brushCategory = BrushCategory.RadialGradient;
        this.OnPropertyChanged("IsLinearGradient");
        this.OnPropertyChanged("IsRadialGradient");
        this.RebuildAdvancedProperties();
      }
    }

    public GradientBrushEditor(BrushEditor brushEditor, ITypeId brushType, SceneNodeProperty basisProperty)
      : base(brushEditor, basisProperty)
    {
      IPlatformMetadata platformMetadata = basisProperty.SceneNodeObjectSet.DocumentContext.TypeResolver.PlatformMetadata;
      if (PlatformTypes.LinearGradientBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.LinearGradient;
      else if (PlatformTypes.RadialGradientBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.RadialGradient;
      this.editorTemplate = new DataTemplate();
      this.editorTemplate.VisualTree = new FrameworkElementFactory(typeof (GradientBrushEditorControl));
      this.gradientStopsProperty = this.RegisterProperty(GradientBrushNode.GradientStopsProperty, (PropertyChangedEventHandler) null);
      this.gradientStopsProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnGradientStopsChange);
      basisProperty.SceneNodeObjectSet.DesignerContext.GradientToolSelectionService.PropertyChanged += new PropertyChangedEventHandler(this.OnGradientStopSelectionChange);
      this.gradientStops = new ObservableCollection<GradientStopEditor>();
      this.gradientStopsView = (ListCollectionView) CollectionViewSource.GetDefaultView((object) this.gradientStops);
      this.sortedGradientStopsView = new ListCollectionView((IList) this.gradientStops);
      this.sortedGradientStopsView.SortDescriptions.Add(new SortDescription("Offset", ListSortDirection.Ascending));
      this.sortedGradientStopsView.SortDescriptions.Add(new SortDescription("PropertyIndex", ListSortDirection.Ascending));
      this.gradientStops.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGradientStopsChanged);
      this.gradientStopsView.CurrentChanged += new EventHandler(this.OnCurrentStopChanged);
      this.RebuildAdvancedProperties();
      this.RebuildModel(-1);
    }

    private void RebuildAdvancedProperties()
    {
      using (IEnumerator<PropertyEntry> enumerator = this.AdvancedProperties.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          SceneNodeProperty property = enumerator.Current as SceneNodeProperty;
          if (property != null)
            this.UnregisterProperty(property);
        }
      }
      this.AdvancedProperties.Clear();
      if (this.brushCategory == BrushCategory.RadialGradient)
      {
        this.AdvancedProperties.Add((PropertyEntry) this.RegisterProperty(RadialGradientBrushNode.GradientOriginProperty, new PropertyChangedEventHandler(this.OnRadialGradientOriginChanged)));
        this.AdvancedProperties.Add((PropertyEntry) this.RegisterProperty(RadialGradientBrushNode.CenterProperty, new PropertyChangedEventHandler(this.OnRadialCenterChanged)));
        this.AdvancedProperties.Add((PropertyEntry) this.RegisterProperty(RadialGradientBrushNode.RadiusXProperty, new PropertyChangedEventHandler(this.OnRadialRadiusXChanged)));
        this.AdvancedProperties.Add((PropertyEntry) this.RegisterProperty(RadialGradientBrushNode.RadiusYProperty, new PropertyChangedEventHandler(this.OnRadialRadiusYChanged)));
      }
      else if (this.brushCategory == BrushCategory.LinearGradient)
      {
        this.AdvancedProperties.Add((PropertyEntry) this.RegisterProperty(LinearGradientBrushNode.StartPointProperty, new PropertyChangedEventHandler(this.OnLinearStartPointChanged)));
        this.AdvancedProperties.Add((PropertyEntry) this.RegisterProperty(LinearGradientBrushNode.EndPointProperty, new PropertyChangedEventHandler(this.OnLinearEndPointChanged)));
      }
      this.mappingModeProperty = this.RegisterProperty(GradientBrushNode.MappingModeProperty, new PropertyChangedEventHandler(this.OnMappingModeChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.mappingModeProperty);
      this.spreadMethodProperty = this.RegisterProperty(GradientBrushNode.SpreadMethodProperty, new PropertyChangedEventHandler(this.OnSpreadMethodChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.spreadMethodProperty);
      this.opacityProperty = this.RegisterProperty(BrushNode.OpacityProperty, new PropertyChangedEventHandler(this.OnOpacityChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.opacityProperty);
      this.relativeTransformProperty = this.RegisterProperty(BrushNode.RelativeTransformProperty, new PropertyChangedEventHandler(this.OnRelativeTransformChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.relativeTransformProperty);
    }

    public override void Disassociate()
    {
      foreach (BrushPropertyEditor brushPropertyEditor in (Collection<GradientStopEditor>) this.gradientStops)
        brushPropertyEditor.Disassociate();
      this.gradientStops.Clear();
      this.gradientStopsProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnGradientStopsChange);
      this.BasisProperty.SceneNodeObjectSet.DesignerContext.GradientToolSelectionService.PropertyChanged -= new PropertyChangedEventHandler(this.OnGradientStopSelectionChange);
      this.gradientStopsView.CurrentChanged -= new EventHandler(this.OnCurrentStopChanged);
      base.Disassociate();
    }

    public bool IsSynchronizedWithArtboard()
    {
      bool isMixed;
      this.BasisProperty.GetLocalValueAsDocumentNode(true, out isMixed);
      if (isMixed)
        return false;
      SceneElement primarySelection = this.BasisProperty.SceneNodeObjectSet.DesignerContext.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null || this.BasisProperty.Reference.PlatformMetadata != primarySelection.ProjectContext.PlatformMetadata)
        return false;
      DocumentNodePath valueAsDocumentNode = primarySelection.GetLocalValueAsDocumentNode(this.BasisProperty.Reference);
      if (valueAsDocumentNode != null)
        return this.BasisProperty.SceneNodeObjectSet.DesignerContext.GradientToolSelectionService.AdornedBrush == valueAsDocumentNode.Node;
      return false;
    }

    public void SetSelectedStopIndex(int index)
    {
      if (!this.IsSynchronizedWithArtboard())
        return;
      this.BasisProperty.SceneNodeObjectSet.DesignerContext.GradientToolSelectionService.Index = index;
    }

    public void AddGradientStop(IInputElement sender, double offset)
    {
      SceneView defaultView = this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView;
      object obj = this.gradientStopsProperty.GetValue();
      GradientStopCollection gradientStops = (GradientStopCollection) defaultView.ConvertToWpfValue(obj);
      if (gradientStops == null)
      {
        gradientStops = new GradientStopCollection();
        this.gradientStopsProperty.SetValue(defaultView.ConvertFromWpfValue((object) gradientStops));
      }
      Color color = GradientBrushEditor.CalculateColor(gradientStops, offset);
      this.gradientStopsProperty.AddValue(defaultView.ConvertFromWpfValue((object) new GradientStop(color, offset)));
      this.RebuildModel(-1);
    }

    public void RemoveGradientStop(IInputElement sender)
    {
      if (this.GradientStops.Count <= 2)
        return;
      this.removedGradientStopIndex = this.GradientStops.IndexOf(this.GradientStops.CurrentItem);
      if (this.removedGradientStopIndex < 0)
        return;
      this.removedGradientStop = this.gradientStops[this.removedGradientStopIndex].CacheDocumentNodeForGradientStop();
      if (this.removedGradientStop == null)
        return;
      this.gradientStopsProperty.RemoveValueAt(this.removedGradientStopIndex);
      this.RebuildModel(-1);
    }

    public void RestoreGradientStop(IInputElement sender)
    {
      if (this.removedGradientStop == null)
        return;
      this.gradientStopsProperty.InsertValue(this.removedGradientStopIndex, (object) this.removedGradientStop);
      this.removedGradientStop = (DocumentNode) null;
      this.RebuildModel(this.removedGradientStopIndex);
    }

    public void HideSceneViewAdorners()
    {
      foreach (IView view in (IEnumerable<IView>) this.BasisProperty.SceneNodeObjectSet.ViewModel.DesignerContext.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null)
          sceneView.OverrideAdornerLayerVisibility = new Visibility?(Visibility.Collapsed);
      }
    }

    public void RestoreSceneViewAdorners()
    {
      foreach (IView view in (IEnumerable<IView>) this.BasisProperty.SceneNodeObjectSet.ViewModel.DesignerContext.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null)
          sceneView.OverrideAdornerLayerVisibility = new Visibility?();
      }
    }

    private void OnGradientStopsChange(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (e.PropertyReference.Count > this.gradientStopsProperty.Reference.Count + 1)
        return;
      this.RebuildModel(-1);
    }

    private void SetSelectionFromService()
    {
      if (!this.IsSynchronizedWithArtboard())
        return;
      int index = this.BasisProperty.SceneNodeObjectSet.DesignerContext.GradientToolSelectionService.Index;
      if (index == this.gradientStopsView.CurrentPosition)
        return;
      this.gradientStopsView.MoveCurrentToPosition(Math.Max(0, Math.Min(index, this.gradientStopsView.Count - 1)));
    }

    private void OnGradientStopSelectionChange(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Index")
        this.SetSelectionFromService();
      if (!(e.PropertyName == "AdornedBrush"))
        return;
      this.SetSelectedStopIndex(this.gradientStopsView.CurrentPosition);
    }

    private void ReverseGradientStopsHandler()
    {
      PropertyValueEditorCommands.get_BeginTransaction().Execute((object) null, (IInputElement) this.BrushEditor);
      if (!this.BasisProperty.IsValueLocal)
        this.BasisProperty.DoSetLocalValue();
      Dictionary<double, List<GradientStopEditor>> dictionary = new Dictionary<double, List<GradientStopEditor>>();
      foreach (GradientStopEditor gradientStopEditor in (IEnumerable) this.GradientStops)
      {
        if (!dictionary.ContainsKey(gradientStopEditor.Offset))
          dictionary.Add(gradientStopEditor.Offset, new List<GradientStopEditor>());
        dictionary[gradientStopEditor.Offset].Add(gradientStopEditor);
      }
      SceneNodeObjectSet sceneNodeObjectSet = this.BasisProperty.SceneNodeObjectSet;
      foreach (KeyValuePair<double, List<GradientStopEditor>> keyValuePair in dictionary)
      {
        double key = keyValuePair.Key;
        List<GradientStopEditor> list = keyValuePair.Value;
        bool flag = list.Count > 1;
        bool isMixed = false;
        for (int index = 0; index < list.Count; ++index)
        {
          list[index].Offset = RoundingHelper.RoundScale(1.0 - key);
          if (flag && index < list.Count / 2)
          {
            GradientStopEditor gradientStopEditor1 = list[index];
            GradientStopEditor gradientStopEditor2 = list[list.Count - 1 - index];
            DocumentNode valueAsDocumentNode1 = gradientStopEditor1.ColorProperty.GetLocalValueAsDocumentNode(false, out isMixed);
            DocumentNode valueAsDocumentNode2 = gradientStopEditor2.ColorProperty.GetLocalValueAsDocumentNode(false, out isMixed);
            gradientStopEditor1.ColorProperty.SetValue((object) valueAsDocumentNode2);
            gradientStopEditor2.ColorProperty.SetValue((object) valueAsDocumentNode1);
          }
        }
      }
      PropertyValueEditorCommands.get_CommitTransaction().Execute((object) null, (IInputElement) this.BrushEditor);
      this.RebuildModel(-1);
    }

    private void SelectNextGradientStopHandler()
    {
      if (!this.IsGradientStopIteratorEnabled)
        return;
      this.sortedGradientStopsView.Refresh();
      this.gradientStopsView.MoveCurrentTo(this.sortedGradientStopsView.GetItemAt((this.sortedGradientStopsView.IndexOf(this.gradientStopsView.CurrentItem) + this.sortedGradientStopsView.Count + 1) % this.sortedGradientStopsView.Count));
    }

    private void SelectPreviousGradientStopHandler()
    {
      if (!this.IsGradientStopIteratorEnabled)
        return;
      this.sortedGradientStopsView.Refresh();
      this.gradientStopsView.MoveCurrentTo(this.sortedGradientStopsView.GetItemAt((this.sortedGradientStopsView.IndexOf(this.gradientStopsView.CurrentItem) + this.sortedGradientStopsView.Count - 1) % this.sortedGradientStopsView.Count));
    }

    private void RebuildModel(int selectionIndex)
    {
      GradientStopCollection gradientStopCollection = (GradientStopCollection) this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.gradientStopsProperty.GetValue());
      int count1 = this.gradientStops.Count;
      int currentPosition = this.gradientStopsView.CurrentPosition;
      if (gradientStopCollection != null && this.gradientStops.Count == gradientStopCollection.Count)
        return;
      foreach (GradientStopEditor gradientStopEditor in (Collection<GradientStopEditor>) this.gradientStops)
      {
        gradientStopEditor.PropertyChanged -= new PropertyChangedEventHandler(this.OnStopEditorPropertyChanged);
        gradientStopEditor.Disassociate();
      }
      this.gradientStops.Clear();
      if (gradientStopCollection != null)
      {
        for (int propertyIndex = 0; propertyIndex < gradientStopCollection.Count; ++propertyIndex)
        {
          GradientStopEditor gradientStopEditor = new GradientStopEditor(this.BrushEditor, this.gradientStopsProperty, propertyIndex);
          gradientStopEditor.PropertyChanged += new PropertyChangedEventHandler(this.OnStopEditorPropertyChanged);
          this.gradientStops.Add(gradientStopEditor);
        }
      }
      int count2 = this.gradientStopsView.Count;
      if (count2 > count1)
      {
        if (this.IsSynchronizedWithArtboard() && currentPosition == -1)
          this.SetSelectionFromService();
        else if (selectionIndex >= 0)
          this.gradientStopsView.MoveCurrentToPosition(selectionIndex);
        else
          this.gradientStopsView.MoveCurrentToPosition(count2 - 1);
      }
      else
      {
        int val1 = currentPosition;
        if (this.IsSynchronizedWithArtboard())
          val1 = this.BasisProperty.SceneNodeObjectSet.DesignerContext.GradientToolSelectionService.Index;
        int num = Math.Max(0, Math.Min(val1, this.gradientStopsView.Count - 1));
        if (val1 != num)
          this.SetSelectedStopIndex(num);
        this.gradientStopsView.MoveCurrentToPosition(num);
      }
      this.OnPropertyChanged("GradientStops");
      this.OnPropertyChanged("Brush");
    }

    private void OnGradientStopsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.OnPropertyChanged("IsGradientStopIteratorEnabled");
    }

    private void OnStopEditorPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.OnPropertyChanged("Brush");
    }

    private void OnRadialGradientOriginChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("GradientOrigin");
      this.OnPropertyChanged("Brush");
    }

    private void OnRadialCenterChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Center");
      this.OnPropertyChanged("Brush");
    }

    private void OnRadialRadiusXChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("RadiusX");
      this.OnPropertyChanged("Brush");
    }

    private void OnRadialRadiusYChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("RadiusY");
      this.OnPropertyChanged("Brush");
    }

    private void OnLinearStartPointChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("StartPoint");
      this.OnPropertyChanged("Brush");
    }

    private void OnLinearEndPointChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("EndPoint");
      this.OnPropertyChanged("Brush");
    }

    private void OnMappingModeChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("MappingMode");
      this.OnPropertyChanged("Brush");
    }

    private void OnSpreadMethodChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("SpreadMethod");
      this.OnPropertyChanged("Brush");
    }

    private void OnOpacityChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Opacity");
      this.OnPropertyChanged("TileBrush");
    }

    private void OnRelativeTransformChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("RelativeTransform");
      this.OnPropertyChanged("Brush");
    }

    private void OnCurrentStopChanged(object sender, EventArgs e)
    {
      if (this.gradientStopsView.CurrentPosition == -1)
        return;
      this.SetSelectedStopIndex(this.gradientStopsView.CurrentPosition);
    }

    internal static Color CalculateColor(GradientStopCollection gradientStops, double offset)
    {
      GradientStop gradientStop1 = (GradientStop) null;
      GradientStop gradientStop2 = (GradientStop) null;
      for (int index = 0; index < gradientStops.Count; ++index)
      {
        GradientStop gradientStop3 = gradientStops[index];
        if (gradientStop3 != null)
        {
          if (gradientStop3.Offset <= offset && (gradientStop1 == null || gradientStop1.Offset <= gradientStop3.Offset))
            gradientStop1 = gradientStop3;
          else if (gradientStop3.Offset > offset && (gradientStop2 == null || gradientStop2.Offset > gradientStop3.Offset))
            gradientStop2 = gradientStop3;
        }
      }
      Color color1;
      if (gradientStop1 != null)
      {
        if (gradientStop2 != null)
        {
          double num = (offset - gradientStop1.Offset) / (gradientStop2.Offset - gradientStop1.Offset);
          Color color2 = gradientStop1.Color;
          Color color3 = gradientStop2.Color;
          color1 = Color.FromArgb((byte) ((1.0 - num) * (double) color2.A + num * (double) color3.A), (byte) ((1.0 - num) * (double) color2.R + num * (double) color3.R), (byte) ((1.0 - num) * (double) color2.G + num * (double) color3.G), (byte) ((1.0 - num) * (double) color2.B + num * (double) color3.B));
        }
        else
          color1 = gradientStop1.Color;
      }
      else
        color1 = gradientStop2 == null ? Colors.White : gradientStop2.Color;
      return color1;
    }
  }
}
