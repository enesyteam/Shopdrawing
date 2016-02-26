// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.Point2DEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class Point2DEditor : Grid, IComponentConnector
  {
    private static IPropertyId PointXProperty = (IPropertyId) PlatformTypes.Point.GetMember(MemberType.LocalProperty, "X", MemberAccessTypes.Public);
    private static IPropertyId PointYProperty = (IPropertyId) PlatformTypes.Point.GetMember(MemberType.LocalProperty, "Y", MemberAccessTypes.Public);
    private static IPropertyId VectorXProperty = (IPropertyId) PlatformTypes.Vector.GetMember(MemberType.LocalProperty, "X", MemberAccessTypes.Public);
    private static IPropertyId VectorYProperty = (IPropertyId) PlatformTypes.Vector.GetMember(MemberType.LocalProperty, "Y", MemberAccessTypes.Public);
    private PropertyReferenceProperty editingProperty;
    private PropertyReferenceProperty xProperty;
    private PropertyReferenceProperty yProperty;
    internal Point2DEditor TreeRoot;
    internal PropertyContainer XPropertyContainer;
    internal PropertyContainer YPropertyContainer;
    private bool _contentLoaded;

    public Point2DEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChangedRebuild);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.Rebuild();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      this.Unhook();
    }

    private void OnDataContextChangedRebuild(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void Rebuild()
    {
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      PropertyReferenceProperty referenceProperty = (PropertyReferenceProperty) null;
      if (propertyValue != null)
        referenceProperty = (PropertyReferenceProperty) propertyValue.ParentProperty;
      if (referenceProperty == this.editingProperty)
        return;
      this.Unhook();
      this.editingProperty = referenceProperty;
      if (this.editingProperty == null)
        return;
      PropertyReference reference = this.editingProperty.Reference;
      SceneNodeObjectSet sceneNodeObjectSet = (SceneNodeObjectSet) this.editingProperty.ObjectSet;
      ITypeResolver typeResolver = (ITypeResolver) sceneNodeObjectSet.ProjectContext;
      Type propertyType = this.editingProperty.PropertyType;
      IPropertyId propertyId1;
      IPropertyId propertyId2;
      if (PlatformTypes.Vector.IsAssignableFrom((ITypeId) this.editingProperty.PropertyTypeId))
      {
        propertyId1 = Point2DEditor.VectorXProperty;
        propertyId2 = Point2DEditor.VectorYProperty;
      }
      else if (PlatformTypes.Point.IsAssignableFrom((ITypeId) this.editingProperty.PropertyTypeId))
      {
        propertyId1 = Point2DEditor.PointXProperty;
        propertyId2 = Point2DEditor.PointYProperty;
      }
      else
      {
        Type nullableType = PlatformTypeHelper.GetNullableType(this.editingProperty.PropertyType);
        if (nullableType == (Type) null)
          return;
        IType type = typeResolver.GetType(nullableType);
        if (PlatformTypes.Vector.IsAssignableFrom((ITypeId) type))
        {
          propertyId1 = Point2DEditor.VectorXProperty;
          propertyId2 = Point2DEditor.VectorYProperty;
        }
        else
        {
          if (!PlatformTypes.Point.IsAssignableFrom((ITypeId) type))
            return;
          propertyId1 = Point2DEditor.PointXProperty;
          propertyId2 = Point2DEditor.PointYProperty;
        }
      }
      ReferenceStep step1 = (ReferenceStep) typeResolver.ResolveProperty(propertyId1);
      ReferenceStep step2 = (ReferenceStep) typeResolver.ResolveProperty(propertyId2);
      this.xProperty = (PropertyReferenceProperty) sceneNodeObjectSet.CreateSceneNodeProperty(reference.Append(step1), step1.Attributes);
      this.yProperty = (PropertyReferenceProperty) sceneNodeObjectSet.CreateSceneNodeProperty(reference.Append(step2), step2.Attributes);
      this.XPropertyContainer.PropertyEntry = (PropertyEntry) this.xProperty;
      this.YPropertyContainer.PropertyEntry = (PropertyEntry) this.yProperty;
      ValueEditorParameters.OverrideValueEditorParameters(this.editingProperty, this.xProperty, "X");
      ValueEditorParameters.OverrideValueEditorParameters(this.editingProperty, this.yProperty, "Y");
    }

    private void Unhook()
    {
      if (this.editingProperty == null)
        return;
      this.editingProperty = (PropertyReferenceProperty) null;
      if (this.xProperty != null)
        this.xProperty.OnRemoveFromCategory();
      if (this.yProperty == null)
        return;
      this.yProperty.OnRemoveFromCategory();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/point2deditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.TreeRoot = (Point2DEditor) target;
          break;
        case 2:
          this.XPropertyContainer = (PropertyContainer) target;
          break;
        case 3:
          this.YPropertyContainer = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
