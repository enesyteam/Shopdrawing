// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ProjectionEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ProjectionEditor : TransformEditorBase, IComponentConnector
  {
    private PropertyReferenceProperty rotationXAngleProperty;
    private PropertyReferenceProperty rotationYAngleProperty;
    private PropertyReferenceProperty rotationZAngleProperty;
    internal ProjectionEditor ProjectionTabControl;
    internal FocusDenyingTabControl Tabs;
    internal TabItem ProjectionRotationItem;
    internal ProjectionRotationTab ProjectionRotationTab;
    internal TabItem ProjectionCenterItem;
    internal ProjectionCenterTab ProjectionCenterTab;
    internal TabItem ProjectionGlobalOffsetItem;
    internal ProjectionGlobalOffsetTab ProjectionGlobalOffsetTab;
    internal TabItem ProjectionLocalOffsetItem;
    internal ProjectionLocalOffsetTab ProjectionLocalOffsetTab;
    private bool _contentLoaded;

    protected override ItemCollection TabCollection
    {
      get
      {
        return this.Tabs.Items;
      }
    }

    protected override bool CurrentTabSupportsRelativeMode
    {
      get
      {
        return this.Tabs.SelectedItem != this.ProjectionCenterItem;
      }
    }

    public Rotation3D OrientationFromAngles
    {
      get
      {
        if (this.TransformProperty != null)
        {
          object obj1 = this.rotationXAngleProperty.GetValue();
          object obj2 = this.rotationYAngleProperty.GetValue();
          object obj3 = this.rotationZAngleProperty.GetValue();
          if (obj1 != null && obj1 != MixedProperty.Mixed && (obj2 != null && obj2 != MixedProperty.Mixed) && (obj3 != null && obj3 != MixedProperty.Mixed))
            return (Rotation3D) new QuaternionRotation3D(Helper3D.QuaternionFromEulerAngles(new Vector3D((double) obj1, (double) obj2, (double) obj3)));
        }
        return (Rotation3D) null;
      }
      set
      {
        if (value == null)
          return;
        Vector3D vector3D = Helper3D.EulerAnglesFromQuaternion(Helper3D.QuaternionFromRotation3D(value));
        this.rotationXAngleProperty.SetValue((object) RoundingHelper.RoundAngle(vector3D.X));
        this.rotationYAngleProperty.SetValue((object) RoundingHelper.RoundAngle(vector3D.Y));
        this.rotationZAngleProperty.SetValue((object) RoundingHelper.RoundAngle(vector3D.Z));
      }
    }

    public bool IsValuePlaneProjection
    {
      get
      {
        if (this.TransformProperty == null)
          return false;
        object obj = this.TransformProperty.GetValue();
        if (obj != null)
          return PlatformTypes.PlaneProjection.IsAssignableFrom((ITypeId) this.TransformProperty.SceneNodeObjectSet.ProjectContext.GetType(obj.GetType()));
        return true;
      }
    }

    public ProjectionEditor()
    {
      this.InitializeComponent();
      this.Tabs.SelectionChanged += new SelectionChangedEventHandler(((TransformEditorBase) this).TransformEditor_SelectionChanged);
    }

    protected override void Initialize()
    {
      this.rotationXAngleProperty = this.TransformProperty.SceneNodeObjectSet.CreateProperty(this.TransformProperty.Reference.Append(new PropertyReference((ITypeResolver) this.TransformProperty.SceneNodeObjectSet.ProjectContext, "PlaneProjection.RotationX")), (AttributeCollection) null);
      this.rotationYAngleProperty = this.TransformProperty.SceneNodeObjectSet.CreateProperty(this.TransformProperty.Reference.Append(new PropertyReference((ITypeResolver) this.TransformProperty.SceneNodeObjectSet.ProjectContext, "PlaneProjection.RotationY")), (AttributeCollection) null);
      this.rotationZAngleProperty = this.TransformProperty.SceneNodeObjectSet.CreateProperty(this.TransformProperty.Reference.Append(new PropertyReference((ITypeResolver) this.TransformProperty.SceneNodeObjectSet.ProjectContext, "PlaneProjection.RotationZ")), (AttributeCollection) null);
    }

    protected override void Unhook()
    {
      if (this.TransformProperty != null)
      {
        this.TransformProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.transformProperty_PropertyReferenceChanged);
        this.rotationXAngleProperty.OnRemoveFromCategory();
        this.rotationYAngleProperty.OnRemoveFromCategory();
        this.rotationZAngleProperty.OnRemoveFromCategory();
      }
      base.Unhook();
    }

    protected override void Rehook()
    {
      base.Rehook();
      if (this.TransformProperty == null)
        return;
      this.TransformProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.transformProperty_PropertyReferenceChanged);
    }

    private void transformProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.OnPropertyChanged("OrientationFromAngles");
      this.OnPropertyChanged("IsValuePlaneProjection");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/projectioneditor.xaml", UriKind.Relative));
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
          this.ProjectionTabControl = (ProjectionEditor) target;
          break;
        case 2:
          this.Tabs = (FocusDenyingTabControl) target;
          break;
        case 3:
          this.ProjectionRotationItem = (TabItem) target;
          break;
        case 4:
          this.ProjectionRotationTab = (ProjectionRotationTab) target;
          break;
        case 5:
          this.ProjectionCenterItem = (TabItem) target;
          break;
        case 6:
          this.ProjectionCenterTab = (ProjectionCenterTab) target;
          break;
        case 7:
          this.ProjectionGlobalOffsetItem = (TabItem) target;
          break;
        case 8:
          this.ProjectionGlobalOffsetTab = (ProjectionGlobalOffsetTab) target;
          break;
        case 9:
          this.ProjectionLocalOffsetItem = (TabItem) target;
          break;
        case 10:
          this.ProjectionLocalOffsetTab = (ProjectionLocalOffsetTab) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
