// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ArtboardOptionsControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ArtboardOptionsControl : StackPanel, IComponentConnector
  {
    internal CheckBox showGridCheckBox;
    internal CheckBox snapToGridCheckBox;
    internal Label gridSpacingLabel;
    internal NumberEditor gridSpacingTextBox;
    internal CheckBox snapToSnapLinesCheckBox;
    internal Label snapLineMarginLabel;
    internal NumberEditor snapLineMarginTextBox;
    internal Label snapLinePaddingLabel;
    internal NumberEditor snapLinePaddingTextBox;
    internal CheckBox isInGridDesignModeCheckBox;
    internal ToggleButton ColorPopupButton;
    internal ChoiceEditor ZoomGestureChoiceEditor;
    internal CheckBox EffectsEnabled;
    internal NumberComboBox ZoomThresholdComboBox;
    private bool _contentLoaded;

    public ArtboardOptionsControl(ArtboardOptionsModel snappingOptions)
    {
      this.DataContext = (object) snappingOptions;
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/artboard/artboardoptionscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.showGridCheckBox = (CheckBox) target;
          break;
        case 2:
          this.snapToGridCheckBox = (CheckBox) target;
          break;
        case 3:
          this.gridSpacingLabel = (Label) target;
          break;
        case 4:
          this.gridSpacingTextBox = (NumberEditor) target;
          break;
        case 5:
          this.snapToSnapLinesCheckBox = (CheckBox) target;
          break;
        case 6:
          this.snapLineMarginLabel = (Label) target;
          break;
        case 7:
          this.snapLineMarginTextBox = (NumberEditor) target;
          break;
        case 8:
          this.snapLinePaddingLabel = (Label) target;
          break;
        case 9:
          this.snapLinePaddingTextBox = (NumberEditor) target;
          break;
        case 10:
          this.isInGridDesignModeCheckBox = (CheckBox) target;
          break;
        case 11:
          this.ColorPopupButton = (ToggleButton) target;
          break;
        case 12:
          this.ZoomGestureChoiceEditor = (ChoiceEditor) target;
          break;
        case 13:
          this.EffectsEnabled = (CheckBox) target;
          break;
        case 14:
          this.ZoomThresholdComboBox = (NumberComboBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
