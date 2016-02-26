// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.StoryboardPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class StoryboardPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private IList<TimelineHeader> storyboards = (IList<TimelineHeader>) new List<TimelineHeader>();
    private Microsoft.Windows.Design.PropertyEditing.PropertyValue editingValue;
    private PropertyReferenceProperty editingProperty;
    private ICollectionView storyboardsView;
    internal StoryboardPickerEditor StoryboardPickerEditorControl;
    private bool _contentLoaded;

    public ICollectionView Storyboards
    {
      get
      {
        return this.storyboardsView;
      }
    }

    public TimelineHeader CurrentStoryboard
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
        if (sceneNodeProperty != null && sceneNodeProperty.IsResource)
        {
          bool isMixed;
          DocumentCompositeNode node = sceneNodeProperty.GetLocalValueAsDocumentNode(false, out isMixed) as DocumentCompositeNode;
          if (node != null)
          {
            string resourceKey = DocumentPrimitiveNode.GetValueAsString(ResourceNodeHelper.GetResourceKey(node));
            return Enumerable.FirstOrDefault<TimelineHeader>((IEnumerable<TimelineHeader>) this.storyboards, (Func<TimelineHeader, bool>) (item => item.Name == resourceKey));
          }
        }
        return (TimelineHeader) null;
      }
      set
      {
        StoryboardTimelineSceneNode timeline = value.Timeline;
        if (!timeline.IsInResourceDictionary)
          return;
        DocumentNode keyNode = (timeline.Parent as DictionaryEntryNode).KeyNode.DocumentNode.Clone(timeline.DocumentContext);
        this.editingProperty.SetValue((object) DocumentNodeUtilities.NewStaticResourceNode(timeline.DocumentContext, keyNode));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public StoryboardPickerEditor()
    {
      this.InitializeComponent();
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.StoryboardPickerEditor_DataContextChanged);
    }

    private void StoryboardPickerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.editingValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
        this.editingProperty = (PropertyReferenceProperty) null;
      }
      if (this.editingValue != null)
        this.editingProperty = (PropertyReferenceProperty) this.editingValue.ParentProperty;
      if (this.editingProperty == null)
        return;
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      this.Rebuild();
    }

    private void OnEditingPropertyChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void Rebuild()
    {
      SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
      if (sceneNodeProperty != null)
      {
        PropertyContainer propertyContainer = (PropertyContainer) this.VisualParent.GetValue(PropertyContainer.OwningPropertyContainerProperty);
        if (propertyContainer != null)
        {
          Rectangle rectangle = propertyContainer.InlineRowTemplate.FindName("ValueAreaWrapperRectangle", (FrameworkElement) propertyContainer) as Rectangle;
          if (rectangle != null)
          {
            if (sceneNodeProperty.IsResource || !sceneNodeProperty.IsExpression)
              rectangle.Visibility = Visibility.Hidden;
            else
              rectangle.Visibility = Visibility.Visible;
          }
        }
        SceneViewModel viewModel = sceneNodeProperty.SceneNodeObjectSet.ViewModel;
        this.storyboards.Clear();
        foreach (StoryboardTimelineSceneNode timeline in viewModel.AnimationEditor.EnumerateStoryboardsForContainer(viewModel.AnimationEditor.ActiveStoryboardContainer))
        {
          if (timeline.IsInResourceDictionary)
            this.storyboards.Add((TimelineHeader) new StoryboardTimelineHeader(timeline));
        }
        this.storyboardsView = CollectionViewSource.GetDefaultView((object) this.storyboards);
      }
      this.OnPropertyChanged("CurrentStoryboard");
      this.OnPropertyChanged("Storyboards");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/storyboardpickereditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.StoryboardPickerEditorControl = (StoryboardPickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
