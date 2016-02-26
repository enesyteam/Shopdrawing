// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.TransformEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TransformEditor : TransformEditorBase, IComponentConnector
  {
    private RegistrationPointModel registrationPointModel;
    private RotationModel rotationModel;
    internal TransformEditor TransformTabControl;
    internal FocusDenyingTabControl Tabs;
    internal TabItem PositionTabItem;
    internal TabItem RotationItem;
    internal RotationTab RotationTab;
    internal TabItem ScaleItem;
    internal TabItem SkewItem;
    internal TabItem CenterItem;
    internal TabItem ReflectItem;
    private bool _contentLoaded;

    public RegistrationPointModel RegistrationPointModel
    {
      get
      {
        return this.registrationPointModel;
      }
    }

    public RotationModel RotationModel
    {
      get
      {
        return this.rotationModel;
      }
    }

    public bool IsPaneEnabled
    {
      get
      {
        return this.ComponentType != (Type) null;
      }
    }

    public bool IsPane3DEnabled
    {
      get
      {
        if (this.IsPaneEnabled)
          return typeof (CanonicalTransform3D).Equals(this.ComponentType);
        return false;
      }
    }

    public bool IsPane2DEnabled
    {
      get
      {
        if (!this.IsPaneEnabled)
          return false;
        if (typeof (CanonicalTransform).Equals(this.ComponentType))
          return true;
        if (this.PropertyLookup != null)
          return this.PropertyLookup.IsCompositeSupported;
        return false;
      }
    }

    public bool IsRegistrationPointEnabled
    {
      get
      {
        if (this.IsPane2DEnabled && !this.IsRelativeTransform)
          return this.IsRenderTransform;
        return false;
      }
    }

    public Visibility IsRegistrationPointVisible
    {
      get
      {
        return !this.IsPane2DEnabled || !this.IsRenderTransform ? Visibility.Collapsed : Visibility.Visible;
      }
    }

    public bool IsRenderTransform
    {
      get
      {
        if (this.TransformProperty.Reference.Count == 1)
          return this.TransformProperty.Reference.FirstStep.Equals((object) Base2DElement.RenderTransformProperty);
        return false;
      }
    }

    public bool IsBrushTransform
    {
      get
      {
        if (this.TransformProperty.Reference.Count == 2)
          return this.TransformProperty.Reference.LastStep.Equals((object) BrushNode.RelativeTransformProperty);
        return false;
      }
    }

    public bool IsCenterTabEnabled
    {
      get
      {
        if (this.IsBrushTransform)
          return this.PropertyLookup.TransformProperty.SceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf);
        return true;
      }
    }

    public ICommand FlipCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (arg => this.FlipElements((BasisComponent) arg)));
      }
    }

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
        if (this.Tabs.SelectedItem != this.ReflectItem)
          return this.Tabs.SelectedItem != this.CenterItem;
        return false;
      }
    }

    public TransformEditor()
    {
      this.InitializeComponent();
      this.Tabs.SelectionChanged += new SelectionChangedEventHandler(((TransformEditorBase) this).TransformEditor_SelectionChanged);
    }

    protected override void Initialize()
    {
      this.registrationPointModel = new RegistrationPointModel(this.ObjectSet);
      this.rotationModel = new RotationModel();
    }

    private void FlipElements(BasisComponent basisComponent)
    {
      if (this.ObjectSet.RepresentativeSceneNode == null)
        return;
      using (SceneEditTransaction editTransaction = this.ObjectSet.RepresentativeSceneNode.ViewModel.CreateEditTransaction(StringTable.UndoUnitTransformPaneReflect))
      {
        this.ApplyRelativeTransformToElements((IEnumerable) this.ObjectSet.Objects, this.TransformProperty.Reference, (IApplyRelativeTransform) new ReflectTransform(basisComponent));
        editTransaction.Commit();
      }
    }

    private void ApplyRelativeTransformToElements(IEnumerable targetElements, PropertyReference reference, IApplyRelativeTransform transformModel)
    {
      foreach (SceneNode sceneNode in targetElements)
      {
        SceneElement sceneElement = sceneNode as SceneElement;
        if (sceneElement != null)
        {
          if (sceneElement is BaseFrameworkElement && sceneElement.IsSet(Base2DElement.RenderTransformOriginProperty) == PropertyState.Unset)
            sceneElement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) new Point(0.5, 0.5));
          object computedValueAsWpf = sceneElement.GetComputedValueAsWpf(reference);
          object obj = transformModel.ApplyRelativeTransform(computedValueAsWpf);
          CanonicalTransform canonicalTransform = obj as CanonicalTransform;
          if (canonicalTransform != (CanonicalTransform) null)
          {
            sceneElement.SetValue(reference, canonicalTransform.GetPlatformTransform(sceneElement.Platform.GeometryHelper));
          }
          else
          {
            CanonicalTransform3D canonicalTransform3D = obj as CanonicalTransform3D;
            if (canonicalTransform3D != (CanonicalTransform3D) null)
              sceneElement.SetValue(reference, (object) canonicalTransform3D.ToTransform());
          }
        }
      }
    }

    protected override void UpdateModel()
    {
      base.UpdateModel();
      this.OnPropertyChanged("IsPaneEnabled");
      this.OnPropertyChanged("IsPane3DEnabled");
      this.OnPropertyChanged("IsPane2DEnabled");
      this.OnPropertyChanged("IsRegistrationPointEnabled");
      this.OnPropertyChanged("IsRegistrationPointVisible");
      this.OnPropertyChanged("IsCenterTabEnabled");
    }

    protected override void Rehook()
    {
      base.Rehook();
      if (this.PropertyLookup == null)
        return;
      this.registrationPointModel.PropertyReference = this.TransformProperty.Reference;
      if (!typeof (CanonicalTransform3D).Equals(this.ComponentType))
        return;
      this.rotationModel.PropertyReference = this.TransformProperty.Reference;
      this.rotationModel.ObjectSet = this.PropertyLookup.ActiveObjectSet;
    }

    protected override void Unhook()
    {
      if (this.TransformProperty != null)
      {
        this.registrationPointModel.Unload();
        this.rotationModel.ObjectSet = (ObjectSetBase) null;
      }
      base.Unhook();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/transformeditor.xaml", UriKind.Relative));
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
          this.TransformTabControl = (TransformEditor) target;
          break;
        case 2:
          this.Tabs = (FocusDenyingTabControl) target;
          break;
        case 3:
          this.PositionTabItem = (TabItem) target;
          break;
        case 4:
          this.RotationItem = (TabItem) target;
          break;
        case 5:
          this.RotationTab = (RotationTab) target;
          break;
        case 6:
          this.ScaleItem = (TabItem) target;
          break;
        case 7:
          this.SkewItem = (TabItem) target;
          break;
        case 8:
          this.CenterItem = (TabItem) target;
          break;
        case 9:
          this.ReflectItem = (TabItem) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
