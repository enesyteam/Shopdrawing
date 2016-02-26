// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.ColorEditor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ColorEditor : ContentControl, INotifyEdit, IComponentConnector
  {
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof (Color), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ColorEditor.ColorChanged)));
    public static readonly DependencyProperty ColorSpaceProperty = DependencyProperty.Register("ColorSpace", typeof (ColorSpace), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) ColorSpace.Rgb, new PropertyChangedCallback(ColorEditor.ColorSpaceChanged)));
    public static readonly DependencyProperty BeginEditCommandProperty = DependencyProperty.Register("BeginEditCommand", typeof (ICommand), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ContinueEditCommandProperty = DependencyProperty.Register("ContinueEditCommand", typeof (ICommand), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty EndEditCommandProperty = DependencyProperty.Register("EndEditCommand", typeof (ICommand), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty CancelEditCommandProperty = DependencyProperty.Register("CancelEditCommand", typeof (ICommand), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty FinishEditingCommandProperty = DependencyProperty.Register("FinishEditingCommand", typeof (ICommand), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsAlphaEnabledProperty = DependencyProperty.Register("IsAlphaEnabled", typeof (bool), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(ColorEditor.IsAlphaEnabledChanged)));
    public static readonly DependencyProperty IsLastSwatchEnabledProperty = DependencyProperty.Register("IsLastSwatchEnabled", typeof (bool), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty AdditionalContentTemplateProperty = DependencyProperty.Register("AdditionalContentTemplate", typeof (DataTemplate), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty InitialColorProperty = DependencyProperty.Register("InitialColor", typeof (Color), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) Colors.Black, new PropertyChangedCallback(ColorEditor.InitialColorChanged)));
    public static readonly DependencyProperty IsInitialColorIndependentProperty = DependencyProperty.Register("IsInitialColorIndependent", typeof (bool), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty EyedropperTemplateProperty = DependencyProperty.Register("EyedropperTemplate", typeof (DataTemplate), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty ShowTrackProperty = DependencyProperty.Register("ShowTrack", typeof (bool), typeof (ColorEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.None));
    private ColorEditorModel colorEditorModel;
    internal ColorEditor ColorEditorRoot;
    private bool _contentLoaded;

    public Color Color
    {
      get
      {
        return (Color) this.GetValue(ColorEditor.ColorProperty);
      }
      set
      {
        this.SetValue(ColorEditor.ColorProperty, (object) value);
      }
    }

    public ColorSpace ColorSpace
    {
      get
      {
        return (ColorSpace) this.GetValue(ColorEditor.ColorSpaceProperty);
      }
      set
      {
        this.SetValue(ColorEditor.ColorSpaceProperty, (object) value);
      }
    }

    public ICommand BeginEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(ColorEditor.BeginEditCommandProperty);
      }
      set
      {
        this.SetValue(ColorEditor.BeginEditCommandProperty, (object) value);
      }
    }

    public ICommand ContinueEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(ColorEditor.ContinueEditCommandProperty);
      }
      set
      {
        this.SetValue(ColorEditor.ContinueEditCommandProperty, (object) value);
      }
    }

    public ICommand EndEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(ColorEditor.EndEditCommandProperty);
      }
      set
      {
        this.SetValue(ColorEditor.EndEditCommandProperty, (object) value);
      }
    }

    public ICommand CancelEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(ColorEditor.CancelEditCommandProperty);
      }
      set
      {
        this.SetValue(ColorEditor.CancelEditCommandProperty, (object) value);
      }
    }

    public ICommand FinishEditingCommand
    {
      get
      {
        return (ICommand) this.GetValue(ColorEditor.FinishEditingCommandProperty);
      }
      set
      {
        this.SetValue(ColorEditor.FinishEditingCommandProperty, (object) value);
      }
    }

    public ICommand ShowColorSpaceMenu
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand(new ArgumentDelegateCommand.ArgumentEventHandler(this.OpenColorSpaceMenu));
      }
    }

    public bool IsAlphaEnabled
    {
        get
        {
            return (bool)base.GetValue(ColorEditor.IsAlphaEnabledProperty);
        }
        set
        {
            base.SetValue(ColorEditor.IsAlphaEnabledProperty, value);
        }
    }

    public DataTemplate AdditionalContentTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(ColorEditor.AdditionalContentTemplateProperty);
      }
      set
      {
        this.SetValue(ColorEditor.AdditionalContentTemplateProperty, (object) value);
      }
    }

    public DataTemplate EyedropperTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(ColorEditor.EyedropperTemplateProperty);
      }
      set
      {
        this.SetValue(ColorEditor.EyedropperTemplateProperty, (object) value);
      }
    }

    public bool IsLastSwatchEnabled
    {
        get
        {
            return (bool)base.GetValue(ColorEditor.IsLastSwatchEnabledProperty);
        }
        set
        {
            base.SetValue(ColorEditor.IsLastSwatchEnabledProperty, value);
        }
    }

    public bool IsAdditionalContentTemplateSet
    {
      get
      {
        return this.GetValue(ColorEditor.AdditionalContentTemplateProperty) != null;
      }
    }

    public Color InitialColor
    {
      get
      {
        return (Color) this.GetValue(ColorEditor.InitialColorProperty);
      }
      set
      {
        this.SetValue(ColorEditor.InitialColorProperty, (object) value);
      }
    }

    public bool IsInitialColorIndependent
    {
        get
        {
            return (bool)base.GetValue(ColorEditor.IsInitialColorIndependentProperty);
        }
        set
        {
            base.SetValue(ColorEditor.IsInitialColorIndependentProperty, value);
        }
    }

    public object InternalColorEditorModel
    {
      get
      {
        return (object) this.colorEditorModel;
      }
    }

    public bool ShowTrack
    {
        get
        {
            return (bool)base.GetValue(ColorEditor.ShowTrackProperty);
        }
        set
        {
            base.SetValue(ColorEditor.ShowTrackProperty, value);
        }
    }

    public ColorEditor()
    {
      this.colorEditorModel = new ColorEditorModel(this.Color);
      this.ColorSpace = ColorEditorModel.SharedColorSpace;
      this.colorEditorModel.BeginEditCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ColorModel_BeginEdit));
      this.colorEditorModel.ContinueEditCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ColorModel_ContinueEdit));
      this.colorEditorModel.EndEditCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ColorModel_EndEdit));
      this.colorEditorModel.CancelEditCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ColorModel_CancelEdit));
      this.Loaded += new RoutedEventHandler(this.OnColorEditorLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnColorEditorUnloaded);
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;v" + (object) this.GetType().Assembly.GetName().Version + ";component/valueeditors/coloreditor/coloreditor.xaml", UriKind.Relative));
    }

    private void OnColorEditorLoaded(object sender, RoutedEventArgs e)
    {
      this.colorEditorModel.PropertyChanged += new PropertyChangedEventHandler(this.colorModel_PropertyChanged);
      this.colorEditorModel.Associate();
    }

    private void OnColorEditorUnloaded(object sender, RoutedEventArgs e)
    {
      this.colorEditorModel.PropertyChanged -= new PropertyChangedEventHandler(this.colorModel_PropertyChanged);
      this.colorEditorModel.Disassociate();
    }

    private static void ColorSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is FrameworkElement))
        return;
      ColorEditorModel.SharedColorSpace = (ColorSpace) d.GetValue(ColorEditor.ColorSpaceProperty);
    }

    private static void InitialColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorEditor colorEditor = d as ColorEditor;
      if (colorEditor == null)
        return;
      Color color = (Color) d.GetValue(ColorEditor.ColorProperty);
      if (!(colorEditor.colorEditorModel.InitialColor != color))
        return;
      colorEditor.colorEditorModel.InitialColor = color;
    }

    private void colorModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e != null && e.PropertyName == "ColorModel")
      {
        if (this.Color != this.colorEditorModel.ColorModel.Color)
          this.Color = this.colorEditorModel.ColorModel.Color;
        this.ColorSpace = ColorEditorModel.SharedColorSpace;
        this.InvalidateProperty(ColorEditor.ColorProperty);
      }
      if (e == null || !(e.PropertyName == "SharedColorSpace"))
        return;
      this.ColorSpace = ColorEditorModel.SharedColorSpace;
    }

    private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorEditor colorEditor = d as ColorEditor;
      if (colorEditor == null)
        return;
      Color color = (Color) d.GetValue(ColorEditor.ColorProperty);
      if (colorEditor.colorEditorModel.ColorModel.IsEqualToColor(color))
        return;
      colorEditor.colorEditorModel.ColorModel.Color = color;
      if (colorEditor.IsInitialColorIndependent)
        return;
      colorEditor.colorEditorModel.InitialColor = color;
    }

    private static void IsAlphaEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorEditor colorEditor = d as ColorEditor;
      if (colorEditor == null)
        return;
      bool flag = (bool) d.GetValue(ColorEditor.IsAlphaEnabledProperty);
      colorEditor.colorEditorModel.ColorModel.IsAlphaEnabled = flag;
    }

    private void ColorModel_BeginEdit()
    {
      ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
    }

    private void ColorModel_ContinueEdit()
    {
      ValueEditorUtils.ExecuteCommand(this.ContinueEditCommand, (IInputElement) this, (object) null);
    }

    private void ColorModel_EndEdit()
    {
      ValueEditorUtils.ExecuteCommand(this.EndEditCommand, (IInputElement) this, (object) null);
      this.colorEditorModel.SetLastColorModel();
    }

    private void ColorModel_CancelEdit()
    {
      ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
    }

    private void OpenColorSpaceMenu(object target)
    {
      ContextMenu contextMenu = (ContextMenu) this.FindResource((object) "ColorSpaceMenu");
      contextMenu.PlacementTarget = (UIElement) target;
      contextMenu.Placement = PlacementMode.Bottom;
      contextMenu.IsOpen = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/valueeditors/coloreditor/coloreditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ColorEditorRoot = (ColorEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
