// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.KeySplineEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class KeySplineEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private SceneNodeProperty keySplineProperty;
    private SceneNodeProperty x1Property;
    private SceneNodeProperty y1Property;
    private SceneNodeProperty x2Property;
    private SceneNodeProperty y2Property;
    private PropertyReference keyTimeProperty;
    private bool hasAllTime0KeyFrames;
    private bool hasTime0KeyFrame;
    internal PropertyContainer X1;
    internal PropertyContainer Y1;
    internal PropertyContainer X2;
    internal PropertyContainer Y2;
    private bool _contentLoaded;

    public SceneNodeProperty X1Property
    {
      get
      {
        return this.x1Property;
      }
      private set
      {
        this.x1Property = value;
        this.SendPropertyChanged("X1Property");
      }
    }

    public SceneNodeProperty Y1Property
    {
      get
      {
        return this.y1Property;
      }
      private set
      {
        this.y1Property = value;
        this.SendPropertyChanged("Y1Property");
      }
    }

    public SceneNodeProperty X2Property
    {
      get
      {
        return this.x2Property;
      }
      private set
      {
        this.x2Property = value;
        this.SendPropertyChanged("X2Property");
      }
    }

    public SceneNodeProperty Y2Property
    {
      get
      {
        return this.y2Property;
      }
      private set
      {
        this.y2Property = value;
        this.SendPropertyChanged("Y2Property");
      }
    }

    public bool HasTime0KeyFrame
    {
      get
      {
        return this.hasTime0KeyFrame;
      }
      set
      {
        if (this.hasTime0KeyFrame == value)
          return;
        this.hasTime0KeyFrame = value;
        this.SendPropertyChanged("HasTime0KeyFrame");
      }
    }

    public bool HasAllTime0KeyFrames
    {
      get
      {
        return this.hasAllTime0KeyFrames;
      }
      set
      {
        if (this.hasAllTime0KeyFrames == value)
          return;
        this.hasAllTime0KeyFrames = value;
        this.SendPropertyChanged("HasAllTime0KeyFrames");
      }
    }

    public ICommand DeselectTime0KeyFramesCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeselectTime0KeyFrames));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public KeySplineEditor()
    {
      this.InitializeComponent();
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.KeySplineEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.KeySplineEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.KeySplineEditor_Unloaded);
    }

    private void KeySplineEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.Unhook();
    }

    private void KeySplineEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.Unhook();
      this.Rebuild();
    }

    private void KeySplineEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.Unhook();
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = e.NewValue as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue == null)
        return;
      this.keySplineProperty = propertyValue.ParentProperty as SceneNodeProperty;
      this.Rebuild();
    }

    private void Rebuild()
    {
      this.X1Property = this.CreateProperty("KeySpline.ControlPoint1/X");
      this.Y1Property = this.CreateProperty("KeySpline.ControlPoint1/Y");
      this.X2Property = this.CreateProperty("KeySpline.ControlPoint2/X");
      this.Y2Property = this.CreateProperty("KeySpline.ControlPoint2/Y");
      if (this.keySplineProperty == null || this.keySplineProperty.IsEmpty)
        return;
      KeyFrameSceneNode keyFrameSceneNode = this.keySplineProperty.SceneNodeObjectSet.RepresentativeSceneNode as KeyFrameSceneNode;
      if (keyFrameSceneNode == null)
        return;
      ReferenceStep singleStep = (IProperty) keyFrameSceneNode.Type.GetMember(MemberType.LocalProperty, "KeyTime", MemberAccessTypes.Public) as ReferenceStep;
      if (singleStep != null)
      {
        this.keyTimeProperty = new PropertyReference(singleStep);
        this.keySplineProperty.SceneNodeObjectSet.RegisterPropertyChangedHandler(this.keyTimeProperty, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.KeyTimePropertyChanged));
      }
      this.UpdateTime0KeyFrameStatus();
    }

    private void KeyTimePropertyChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.UpdateTime0KeyFrameStatus();
    }

    private void UpdateTime0KeyFrameStatus()
    {
      if (this.keySplineProperty.IsEmpty)
        return;
      SceneNodeObjectSet sceneNodeObjectSet = this.keySplineProperty.SceneNodeObjectSet;
      bool flag1 = false;
      bool flag2 = true;
      if (sceneNodeObjectSet.RepresentativeSceneNode is KeyFrameSceneNode)
      {
        foreach (KeyFrameSceneNode keyFrameSceneNode in sceneNodeObjectSet.Objects)
        {
          if (keyFrameSceneNode.Time == 0.0)
            flag1 = true;
          else
            flag2 = false;
        }
      }
      this.HasTime0KeyFrame = flag1;
      this.HasAllTime0KeyFrames = flag2;
    }

    private void DeselectTime0KeyFrames()
    {
      if (this.keySplineProperty == null || this.keySplineProperty.IsEmpty)
        return;
      List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
      KeyFrameSelectionSet frameSelectionSet = this.keySplineProperty.SceneNodeObjectSet.ViewModel.KeyFrameSelectionSet;
      KeyFrameSceneNode primarySelection = frameSelectionSet.PrimarySelection;
      foreach (KeyFrameSceneNode keyFrameSceneNode in frameSelectionSet.Selection)
      {
        if (keyFrameSceneNode.Time != 0.0)
          list.Add(keyFrameSceneNode);
      }
      frameSelectionSet.SetSelection((ICollection<KeyFrameSceneNode>) list, primarySelection);
    }

    private SceneNodeProperty CreateProperty(string propertyPath)
    {
      if (this.keySplineProperty.IsEmpty)
        return (SceneNodeProperty) null;
      PropertyReference propertyReference = (PropertyReference) null;
      try
      {
        if (this.keySplineProperty != null)
          propertyReference = new PropertyReference((ITypeResolver) this.keySplineProperty.SceneNodeObjectSet.ProjectContext, propertyPath);
      }
      catch (Exception ex)
      {
      }
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
      if (propertyReference != null)
        sceneNodeProperty = this.keySplineProperty.SceneNodeObjectSet.CreateSceneNodeProperty(this.keySplineProperty.Reference.Append(propertyReference), (AttributeCollection) null);
      sceneNodeProperty.OverrideValueEditorParameters(new ValueEditorParameters(this.keySplineProperty.Attributes));
      return sceneNodeProperty;
    }

    private void Unhook()
    {
      if (this.x1Property != null)
      {
        this.x1Property.OnRemoveFromCategory();
        this.x1Property = (SceneNodeProperty) null;
      }
      if (this.y1Property != null)
      {
        this.y1Property.OnRemoveFromCategory();
        this.y1Property = (SceneNodeProperty) null;
      }
      if (this.x2Property != null)
      {
        this.x2Property.OnRemoveFromCategory();
        this.x2Property = (SceneNodeProperty) null;
      }
      if (this.y2Property != null)
      {
        this.y2Property.OnRemoveFromCategory();
        this.y2Property = (SceneNodeProperty) null;
      }
      if (this.keySplineProperty == null || this.keyTimeProperty == null)
        return;
      this.keySplineProperty.SceneNodeObjectSet.UnregisterPropertyChangedHandler(this.keyTimeProperty, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.KeyTimePropertyChanged));
      this.keyTimeProperty = (PropertyReference) null;
    }

    private void SendPropertyChanged(string propertyName)
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/easing/keysplineeditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.X1 = (PropertyContainer) target;
          break;
        case 2:
          this.Y1 = (PropertyContainer) target;
          break;
        case 3:
          this.X2 = (PropertyContainer) target;
          break;
        case 4:
          this.Y2 = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
