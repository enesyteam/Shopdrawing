// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.RotationTab
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class RotationTab : Grid, ITransformEditorTab, IComponentConnector
  {
    public static readonly RoutedCommand RotationCommitCommand = new RoutedCommand("RotationCommit", typeof (RotationTab));
    public static readonly RoutedCommand RotationCancelCommand = new RoutedCommand("RotationCancel", typeof (RotationTab));
    public static readonly RoutedCommand EulerRotationBeginCommand = new RoutedCommand("EulerRotationBegin", typeof (RotationTab));
    public static readonly RoutedCommand EulerRotationUpdateCommand = new RoutedCommand("EulerRotationUpdate", typeof (RotationTab));
    public static readonly RoutedCommand EulerRotationCommitCommand = new RoutedCommand("EulerRotationCommit", typeof (RotationTab));
    public static readonly RoutedCommand EulerRotationCancelCommand = new RoutedCommand("EulerRotationCancel", typeof (RotationTab));
    private SceneNodeProperty transformProperty;
    private SceneEditTransaction editTransaction;
    internal RotationTab RotationTabGrid;
    internal PropertyContainer RotationSpinner;
    internal PropertyContainer ArcBall;
    internal PropertyContainer RotationPropertyContainer;
    internal NumberEditor ZInputTextBox;
    internal NumberEditor XInputTextBox;
    internal NumberEditor YInputTextBox;
    private bool _contentLoaded;

    public RotationTab()
    {
      this.InitializeComponent();
      this.CommandBindings.Add(new CommandBinding((ICommand) RotationTab.RotationCommitCommand, new ExecutedRoutedEventHandler(this.OnRotationCommit)));
      this.CommandBindings.Add(new CommandBinding((ICommand) RotationTab.RotationCancelCommand, new ExecutedRoutedEventHandler(this.OnRotationCancel)));
      this.CommandBindings.Add(new CommandBinding((ICommand) RotationTab.EulerRotationBeginCommand, new ExecutedRoutedEventHandler(this.OnEulerRotationBegin)));
      this.CommandBindings.Add(new CommandBinding((ICommand) RotationTab.EulerRotationUpdateCommand, new ExecutedRoutedEventHandler(this.OnEulerRotationUpdate)));
      this.CommandBindings.Add(new CommandBinding((ICommand) RotationTab.EulerRotationCommitCommand, new ExecutedRoutedEventHandler(this.OnEulerRotationCommit)));
      this.CommandBindings.Add(new CommandBinding((ICommand) RotationTab.EulerRotationCancelCommand, new ExecutedRoutedEventHandler(this.OnEulerRotationCancel)));
    }

    private void OnRotationCommit(object sender, ExecutedRoutedEventArgs e)
    {
      BindingOperations.GetBindingExpression((DependencyObject) e.OriginalSource, TextBox.TextProperty).UpdateSource();
    }

    private void OnRotationCancel(object sender, ExecutedRoutedEventArgs e)
    {
      BindingOperations.GetBindingExpression((DependencyObject) e.OriginalSource, TextBox.TextProperty).UpdateTarget();
    }

    private void OnEulerRotationBegin(object sender, ExecutedRoutedEventArgs e)
    {
      this.editTransaction = this.transformProperty.SceneNodeObjectSet.ViewModel.CreateEditTransaction("Euler angles");
    }

    private void OnEulerRotationUpdate(object sender, ExecutedRoutedEventArgs e)
    {
      this.editTransaction.Update();
    }

    private void OnEulerRotationCommit(object sender, ExecutedRoutedEventArgs e)
    {
      this.editTransaction.Commit();
      this.editTransaction = (SceneEditTransaction) null;
    }

    private void OnEulerRotationCancel(object sender, ExecutedRoutedEventArgs e)
    {
      this.editTransaction.Cancel();
      this.editTransaction = (SceneEditTransaction) null;
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      AttributeCollection attributes = new AttributeCollection(new Attribute[2]
      {
        (Attribute) new NumberRangesAttribute(new double?(double.NegativeInfinity), new double?(-180.0), new double?(180.0), new double?(double.PositiveInfinity), new bool?()),
        (Attribute) new NumberIncrementsAttribute(new double?(0.1), new double?(1.0), new double?(5.0))
      });
      this.transformProperty = propertyLookup.TransformProperty;
      if (propertyLookup.TransformType == TransformType.Transform2D)
      {
        if (propertyLookup.Relative)
        {
          this.RotationPropertyContainer.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("RotationAngle", attributes);
          this.RotationSpinner.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("RotationAngle", attributes);
        }
        else
        {
          this.RotationPropertyContainer.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("RotateTransform/Angle", attributes);
          this.RotationSpinner.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("RotateTransform/Angle", attributes);
        }
        this.ArcBall.PropertyEntry = (PropertyEntry) null;
      }
      else
      {
        this.RotationPropertyContainer.PropertyEntry = (PropertyEntry) null;
        this.RotationSpinner.PropertyEntry = (PropertyEntry) null;
        this.ArcBall.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("RotationAngles");
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/rotationtab.xaml", UriKind.Relative));
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
          this.RotationTabGrid = (RotationTab) target;
          break;
        case 2:
          this.RotationSpinner = (PropertyContainer) target;
          break;
        case 3:
          this.ArcBall = (PropertyContainer) target;
          break;
        case 4:
          this.RotationPropertyContainer = (PropertyContainer) target;
          break;
        case 5:
          this.ZInputTextBox = (NumberEditor) target;
          break;
        case 6:
          this.XInputTextBox = (NumberEditor) target;
          break;
        case 7:
          this.YInputTextBox = (NumberEditor) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
