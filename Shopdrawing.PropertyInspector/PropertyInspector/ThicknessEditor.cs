// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ThicknessEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
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
  public sealed class ThicknessEditor : Grid, IComponentConnector
  {
    private PropertyReferenceProperty editingProperty;
    private PropertyReferenceProperty top;
    private PropertyReferenceProperty left;
    private PropertyReferenceProperty right;
    private PropertyReferenceProperty bottom;
    internal PropertyContainer LeftPropertyContainer;
    internal PropertyContainer RightPropertyContainer;
    internal PropertyContainer TopPropertyContainer;
    internal PropertyContainer BottomPropertyContainer;
    private bool _contentLoaded;

    public ThicknessEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChangedRebuild);
      this.Loaded += new RoutedEventHandler(this.ThicknessEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.InitializeComponent();
    }

    private void ThicknessEditor_Loaded(object sender, RoutedEventArgs e)
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
      PropertyValue propertyValue = this.DataContext as PropertyValue;
      PropertyReferenceProperty referenceProperty = (PropertyReferenceProperty) null;
      if (propertyValue != null)
        referenceProperty = (PropertyReferenceProperty) propertyValue.get_ParentProperty();
      if (referenceProperty == this.editingProperty)
        return;
      this.Unhook();
      this.editingProperty = referenceProperty;
      if (this.editingProperty == null)
        return;
      PropertyReference reference = this.editingProperty.Reference;
      SceneNodeObjectSet sceneNodeObjectSet = (SceneNodeObjectSet) this.editingProperty.ObjectSet;
      IPlatformMetadata platformMetadata = reference.PlatformMetadata;
      ReferenceStep step1 = (ReferenceStep) platformMetadata.ResolveProperty(ThicknessNode.TopProperty);
      ReferenceStep step2 = (ReferenceStep) platformMetadata.ResolveProperty(ThicknessNode.LeftProperty);
      ReferenceStep step3 = (ReferenceStep) platformMetadata.ResolveProperty(ThicknessNode.RightProperty);
      ReferenceStep step4 = (ReferenceStep) platformMetadata.ResolveProperty(ThicknessNode.BottomProperty);
      Type runtimeType = platformMetadata.ResolveType(PlatformTypes.Thickness).RuntimeType;
      this.top = (PropertyReferenceProperty) sceneNodeObjectSet.CreateSceneNodeProperty(reference.Append(step1), step1.Attributes);
      this.left = (PropertyReferenceProperty) sceneNodeObjectSet.CreateSceneNodeProperty(reference.Append(step2), step2.Attributes);
      this.right = (PropertyReferenceProperty) sceneNodeObjectSet.CreateSceneNodeProperty(reference.Append(step3), step3.Attributes);
      this.bottom = (PropertyReferenceProperty) sceneNodeObjectSet.CreateSceneNodeProperty(reference.Append(step4), step4.Attributes);
      ValueEditorParameters.OverrideValueEditorParameters(this.editingProperty, this.left, "Left");
      ValueEditorParameters.OverrideValueEditorParameters(this.editingProperty, this.top, "Top");
      ValueEditorParameters.OverrideValueEditorParameters(this.editingProperty, this.right, "Right");
      ValueEditorParameters.OverrideValueEditorParameters(this.editingProperty, this.bottom, "Bottom");
      this.TopPropertyContainer.set_PropertyEntry((PropertyEntry) this.top);
      this.LeftPropertyContainer.set_PropertyEntry((PropertyEntry) this.left);
      this.RightPropertyContainer.set_PropertyEntry((PropertyEntry) this.right);
      this.BottomPropertyContainer.set_PropertyEntry((PropertyEntry) this.bottom);
    }

    private void Unhook()
    {
      if (this.editingProperty == null)
        return;
      this.editingProperty = (PropertyReferenceProperty) null;
      this.top.OnRemoveFromCategory();
      this.left.OnRemoveFromCategory();
      this.right.OnRemoveFromCategory();
      this.bottom.OnRemoveFromCategory();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/thicknesseditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.LeftPropertyContainer = (PropertyContainer) target;
          break;
        case 2:
          this.RightPropertyContainer = (PropertyContainer) target;
          break;
        case 3:
          this.TopPropertyContainer = (PropertyContainer) target;
          break;
        case 4:
          this.BottomPropertyContainer = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
