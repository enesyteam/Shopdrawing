// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.GradientBrushEditorControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class GradientBrushEditorControl : Grid, IComponentConnector
  {
    private GradientBrushEditor cachedDataContext;
    internal TabbedColorEditorControl TabbedColorEditor;
    internal MultiSlider GradientEditorMultiSlider;
    internal Button ReverseGradientStopsButton;
    internal Button SelectPreviousGradientStopButton;
    internal Icon GradientStopIterator;
    internal Button SelectNextGradientStopButton;
    internal PropertyContainer GradientStopOffsetEditor;
    private bool _contentLoaded;

    public ICommand BeginGradientEyedropperCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (parameter =>
        {
          ValueEditorUtils.ExecuteCommand((ICommand) PropertyValueEditorCommands.get_BeginTransaction(), (IInputElement) this, (object) null);
          GradientBrushEditor gradientBrushEditor = (GradientBrushEditor) this.DataContext;
          if (gradientBrushEditor == null)
            return;
          this.cachedDataContext = gradientBrushEditor;
          gradientBrushEditor.HideSceneViewAdorners();
        }));
      }
    }

    public ICommand CancelGradientEyedropperCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (parameter =>
        {
          this.RestoreDataContext();
          ValueEditorUtils.ExecuteCommand((ICommand) PropertyValueEditorCommands.get_AbortTransaction(), (IInputElement) this, (object) null);
        }));
      }
    }

    public ICommand CommitGradientEyedropperCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand((ArgumentDelegateCommand.ArgumentEventHandler) (parameter =>
        {
          this.RestoreDataContext();
          ValueEditorUtils.ExecuteCommand((ICommand) PropertyValueEditorCommands.get_CommitTransaction(), (IInputElement) this, (object) null);
        }));
      }
    }

    public GradientBrushEditorControl()
    {
      this.InitializeComponent();
    }

    private void RestoreDataContext()
    {
      if (this.cachedDataContext == null)
        return;
      this.cachedDataContext.RestoreSceneViewAdorners();
      this.cachedDataContext = (GradientBrushEditor) null;
    }

    private void AddGradientStop(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      GradientBrushEditor gradientBrushEditor = (GradientBrushEditor) this.DataContext;
      if (gradientBrushEditor == null)
        return;
      gradientBrushEditor.AddGradientStop((IInputElement) this, RoundingHelper.RoundScale((double) eventArgs.Parameter));
    }

    private void RemoveGradientStop(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      GradientBrushEditor gradientBrushEditor = (GradientBrushEditor) this.DataContext;
      if (gradientBrushEditor == null)
        return;
      gradientBrushEditor.RemoveGradientStop((IInputElement) this);
    }

    private void RestoreGradientStop(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      GradientBrushEditor gradientBrushEditor = (GradientBrushEditor) this.DataContext;
      if (gradientBrushEditor == null)
        return;
      gradientBrushEditor.RestoreGradientStop((IInputElement) this);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/brusheditor/gradientbrusheditor.xaml", UriKind.Relative));
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
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.AddGradientStop);
          break;
        case 2:
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.RestoreGradientStop);
          break;
        case 3:
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.RemoveGradientStop);
          break;
        case 4:
          this.TabbedColorEditor = (TabbedColorEditorControl) target;
          break;
        case 5:
          this.GradientEditorMultiSlider = (MultiSlider) target;
          break;
        case 6:
          this.ReverseGradientStopsButton = (Button) target;
          break;
        case 7:
          this.SelectPreviousGradientStopButton = (Button) target;
          break;
        case 8:
          this.GradientStopIterator = (Icon) target;
          break;
        case 9:
          this.SelectNextGradientStopButton = (Button) target;
          break;
        case 10:
          this.GradientStopOffsetEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
