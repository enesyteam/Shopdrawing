//// Decompiled with JetBrains decompiler
//// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ElementPickerEditor
//// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
//// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
//// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

//using Microsoft.Expression.DesignModel.Metadata;
//using Microsoft.Expression.DesignSurface.UserInterface;
//using Microsoft.Expression.DesignSurface.ViewModel;
//using System;
//using System.CodeDom.Compiler;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Markup;

//namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
//{
//  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
//  public class ElementPickerEditor : Grid, IComponentConnector
//  {
//    public static readonly DependencyProperty TypeConstraintProperty = DependencyProperty.Register("TypeConstraint", typeof (ITypeId), typeof (ElementPickerEditor));
//    internal ElementPickerEditor ElementPickerEditorControl;
//    internal ElementPicker ElementPicker;
//    private bool _contentLoaded;

//    public ITypeId TypeConstraint
//    {
//      get
//      {
//        return (ITypeId) this.GetValue(ElementPickerEditor.TypeConstraintProperty);
//      }
//      set
//      {
//        this.SetValue(ElementPickerEditor.TypeConstraintProperty, value);
//      }
//    }

//    public ElementPickerEditor()
//    {
//      this.InitializeComponent();
//    }

//    private void OnSelectElement(object o, EventArgs e)
//    {
//      SceneNodeProperty editingProperty = this.ElementPicker.EditingProperty;
//      if (editingProperty == null)
//        return;
//      SceneElement root = editingProperty.SceneNodeObjectSet.ViewModel.ActiveEditingContainer as SceneElement;
//      if (root == null)
//        return;
//      this.ElementPicker.ChangeElementName(PathTargetDialog.ChooseElementForBehavior(root, this.TypeConstraint));
//    }

//    [DebuggerNonUserCode]
//    public void InitializeComponent()
//    {
//      if (this._contentLoaded)
//        return;
//      this._contentLoaded = true;
//      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/elementpickereditor.xaml", UriKind.Relative));
//    }

//    [DebuggerNonUserCode]
//    internal Delegate _CreateDelegate(Type delegateType, string handler)
//    {
//      return Delegate.CreateDelegate(delegateType, this, handler);
//    }

//    [DebuggerNonUserCode]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    void IComponentConnector.Connect(int connectionId, object target)
//    {
//      switch (connectionId)
//      {
//        case 1:
//          this.ElementPickerEditorControl = (ElementPickerEditor) target;
//          break;
//        case 2:
//          this.ElementPicker = (ElementPicker) target;
//          break;
//        case 3:
//          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnSelectElement);
//          break;
//        default:
//          this._contentLoaded = true;
//          break;
//      }
//    }
//  }
//}
