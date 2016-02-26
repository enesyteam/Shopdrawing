// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ElementPicker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class ElementPicker : ComplexValueEditorBase, IPickWhipHost, IComponentConnector
  {
    public static readonly DependencyProperty ElementSelectionStrategyProperty = DependencyProperty.Register("ElementSelectionStrategy", typeof (IElementSelectionStrategy), typeof (ElementPicker));
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof (bool), typeof (ElementPicker));
    internal ElementPicker ElementPickerControl;
    private bool _contentLoaded;

    public string BindingToolTip
    {
      get
      {
        return BindingPropertyHelper.GetElementNameFromBoundProperty(this.EditingProperty);
      }
    }

    public string OverlayText
    {
      get
      {
        if (this.EditingProperty != null)
        {
          SceneNode representativeSceneNode = this.EditingProperty.SceneNodeObjectSet.RepresentativeSceneNode;
          if (representativeSceneNode != null && (ProjectNeutralTypes.BehaviorEventTriggerBase.IsAssignableFrom((ITypeId) representativeSceneNode.Type) || ProjectNeutralTypes.BehaviorTargetedTriggerAction.IsAssignableFrom((ITypeId) representativeSceneNode.Type)))
          {
            SceneNode sceneNode;
            string str1;
            if (ProjectNeutralTypes.GoToStateAction.IsAssignableFrom((ITypeId) representativeSceneNode.Type) || ProjectNeutralTypes.NavigationMenuAction.IsAssignableFrom((ITypeId) representativeSceneNode.Type))
            {
              sceneNode = GoToStateActionNode.FindTargetElement((BehaviorTargetedTriggerActionNode) representativeSceneNode);
              str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.GoToStateUnsetTargetNameParentTypeWrapper, new object[1]
              {
                sceneNode != null ? (object) sceneNode.Type.Name : (object) StringTable.GoToStateUnsetTargetNameFallbackValue
              });
            }
            else
            {
              sceneNode = BehaviorHelper.FindNamedElement(representativeSceneNode, string.Empty);
              str1 = StringTable.ElementPickerDefaultUnnamedParentToken;
            }
            string str2 = sceneNode != null ? sceneNode.Name : string.Empty;
            if (string.IsNullOrEmpty(str2))
              return str1;
            return str2;
          }
        }
        return string.Empty;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return (bool) this.GetValue(ElementPicker.IsReadOnlyProperty);
      }
      set
      {
        this.SetValue(ElementPicker.IsReadOnlyProperty, (object) (bool) (value ? true : false));
      }
    }

    public FrameworkElement PropertyEditor
    {
      get
      {
        return (FrameworkElement) this;
      }
    }

    public IElementSelectionStrategy ElementSelectionStrategy
    {
      get
      {
        return (IElementSelectionStrategy) this.GetValue(ElementPicker.ElementSelectionStrategyProperty);
      }
      set
      {
        this.SetValue(ElementPicker.ElementSelectionStrategyProperty, (object) value);
      }
    }

    public new SceneNodeProperty EditingProperty
    {
      get
      {
        return base.EditingProperty;
      }
    }

    public Cursor PickWhipCursor
    {
      get
      {
        return ToolCursors.PickWhipCursor;
      }
    }

    public ElementPicker()
    {
      this.InitializeComponent();
    }

    internal bool IsElementConstraintType(SceneElement candidateElement)
    {
      if (this.ElementSelectionStrategy != null)
        return this.ElementSelectionStrategy.CanSelectElement(candidateElement);
      return true;
    }

    internal void ChangeElementName(SceneElement newElement)
    {
      if (this.ElementSelectionStrategy == null)
        return;
      this.ElementSelectionStrategy.SelectElement(newElement, this.EditingProperty);
    }

    protected override void Rebuild()
    {
      this.SuppressValueAreaWrapper();
      this.OnPropertyChanged("ElementName");
      this.OnPropertyChanged("OverlayText");
      this.OnPropertyChanged("BindingToolTip");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/elementpicker.xaml", UriKind.Relative));
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
      if (connectionId == 1)
        this.ElementPickerControl = (ElementPicker) target;
      else
        this._contentLoaded = true;
    }
  }
}
