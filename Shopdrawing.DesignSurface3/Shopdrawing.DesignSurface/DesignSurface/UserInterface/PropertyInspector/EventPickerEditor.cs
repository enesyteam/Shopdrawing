// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EventPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
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

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class EventPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private Microsoft.Windows.Design.PropertyEditing.PropertyValue editingValue;
    private SceneNodeProperty sourceNameProperty;
    private SceneNodeProperty sourceObjectProperty;
    private SceneNodeProperty editingProperty;
    private IEnumerable<EventInformation> events;
    private ICollectionView eventsView;
    internal EventPickerEditor EventPickerEditorControl;
    private bool _contentLoaded;

    public ICollectionView Events
    {
      get
      {
        return this.eventsView;
      }
    }

    public EventInformation CurrentEvent
    {
      get
      {
        if (this.editingProperty != null)
        {
          string eventName = this.editingProperty.GetValue() as string;
          if (eventName != null)
            return Enumerable.FirstOrDefault<EventInformation>(this.events, (Func<EventInformation, bool>) (item => item.EventName == eventName));
        }
        return (EventInformation) null;
      }
      set
      {
        this.editingProperty.SetValue((object) value.EventName);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public EventPickerEditor()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.OnEventPickerEditorLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnEventPickerEditorUnloaded);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnEventPickerEditorDataContextChanged);
    }

    private void OnEventPickerEditorLoaded(object sender, RoutedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void OnEventPickerEditorUnloaded(object sender, RoutedEventArgs e)
    {
      this.UnhookEditingProperty();
      this.Rebuild();
    }

    private void OnEventPickerEditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void UnhookEditingProperty()
    {
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
        this.editingProperty = (SceneNodeProperty) null;
      }
      if (this.sourceNameProperty != null)
      {
        this.sourceNameProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnSourceNameOrSourceObjectPropertyPropertyReferenceChanged);
        this.sourceNameProperty.OnRemoveFromCategory();
        this.sourceNameProperty = (SceneNodeProperty) null;
      }
      if (this.sourceObjectProperty == null)
        return;
      this.sourceObjectProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnSourceNameOrSourceObjectPropertyPropertyReferenceChanged);
      this.sourceObjectProperty.OnRemoveFromCategory();
      this.sourceObjectProperty = (SceneNodeProperty) null;
    }

    private void UpdateFromDataContext()
    {
      this.editingValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      this.UnhookEditingProperty();
      if (this.editingValue != null)
        this.editingProperty = (SceneNodeProperty) this.editingValue.ParentProperty;
      if (this.editingProperty == null)
        return;
      SceneNodeObjectSet sceneNodeObjectSet = this.editingProperty.SceneNodeObjectSet;
      ReferenceStep singleStep1 = (ReferenceStep) sceneNodeObjectSet.ProjectContext.ResolveProperty(BehaviorEventTriggerBaseNode.BehaviorSourceNameProperty);
      if (singleStep1 != null)
      {
        this.sourceNameProperty = sceneNodeObjectSet.CreateSceneNodeProperty(new PropertyReference(singleStep1), (AttributeCollection) null);
        this.sourceNameProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnSourceNameOrSourceObjectPropertyPropertyReferenceChanged);
      }
      ReferenceStep singleStep2 = (ReferenceStep) sceneNodeObjectSet.ProjectContext.ResolveProperty(BehaviorEventTriggerBaseNode.BehaviorSourceObjectProperty);
      if (singleStep2 != null)
      {
        this.sourceObjectProperty = sceneNodeObjectSet.CreateSceneNodeProperty(new PropertyReference(singleStep2), (AttributeCollection) null);
        this.sourceObjectProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnSourceNameOrSourceObjectPropertyPropertyReferenceChanged);
      }
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      this.Rebuild();
    }

    private void OnSourceNameOrSourceObjectPropertyPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void OnEditingPropertyChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void Rebuild()
    {
      SceneNodeProperty sceneNodeProperty = this.editingProperty;
      BehaviorEventTriggerBaseNode eventTriggerBaseNode = sceneNodeProperty != null ? sceneNodeProperty.SceneNodeObjectSet.RepresentativeSceneNode as BehaviorEventTriggerBaseNode : (BehaviorEventTriggerBaseNode) null;
      this.events = eventTriggerBaseNode == null || eventTriggerBaseNode.SourceNode == null ? (IEnumerable<EventInformation>) new List<EventInformation>() : EventInformation.GetEventsForType((ITypeResolver) sceneNodeProperty.SceneNodeObjectSet.ProjectContext, eventTriggerBaseNode.SourceNode.Type, MemberType.LocalEvent);
      this.eventsView = CollectionViewSource.GetDefaultView((object) this.events);
      this.eventsView.GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription()
      {
        PropertyName = "GroupBy"
      });
      this.eventsView.SortDescriptions.Add(new SortDescription());
      this.OnPropertyChanged("CurrentEvent");
      this.OnPropertyChanged("Events");
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/eventpickereditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.EventPickerEditorControl = (EventPickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
