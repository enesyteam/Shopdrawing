// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ConditionalExpressionBehaviorCategoryEditorControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ConditionalExpressionBehaviorCategoryEditorControl : Grid, IComponentConnector, IStyleConnector
  {
    internal ConditionalExpressionBehaviorCategoryEditorControl ConditionalExpressionBehaviorCategoryEditor;
    internal SingleSelectionListBox ConditionListBox;
    internal TextBlock BorderWrapper;
    private bool _contentLoaded;

    public ConditionalExpressionBehaviorCategoryEditorControl()
    {
      this.InitializeComponent();
    }

    private void AddCondition(object sender, RoutedEventArgs args)
    {
      ((ConditionalExpressionBehaviorCategory) this.DataContext).AddConditionNode();
    }

    private void DeleteCondition(object sender, RoutedEventArgs args)
    {
      ((ConditionalExpressionBehaviorCategory) this.DataContext).DeleteConditionNode(((FrameworkElement) sender).DataContext as SceneNodeProperty);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/triggers/conditionalexpressionbehaviorcategoryeditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ConditionalExpressionBehaviorCategoryEditor = (ConditionalExpressionBehaviorCategoryEditorControl) target;
          break;
        case 3:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.AddCondition);
          break;
        case 4:
          this.ConditionListBox = (SingleSelectionListBox) target;
          break;
        case 5:
          this.BorderWrapper = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((ButtonBase) target).Click += new RoutedEventHandler(this.DeleteCondition);
    }
  }
}
