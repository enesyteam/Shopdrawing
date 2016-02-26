// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.SplitComboBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.ValueEditors
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class SplitComboBox : ComboBox, IComponentConnector
  {
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof (PlacementMode), typeof (SplitComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) PlacementMode.Bottom, FrameworkPropertyMetadataOptions.AffectsArrange));
    public static readonly DependencyProperty ActionNameProperty = DependencyProperty.Register("ActionName", typeof (string), typeof (SplitComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) "SplitComboBox", FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof (DataTemplate), typeof (SplitComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty LastActionCommandProperty = DependencyProperty.Register("LastActionCommand", typeof (ICommand), typeof (SplitComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    private int savedIndex = -1;
    private bool rememberLastItem = true;
    private bool suspendSelectionChangeNotification;
    private ICommand internalLastActionCommand;
    private bool _contentLoaded;

    public string ActionName
    {
      get
      {
        return (string) this.GetValue(SplitComboBox.ActionNameProperty);
      }
      set
      {
        this.SetValue(SplitComboBox.ActionNameProperty, (object) value);
      }
    }

    public PlacementMode Placement
    {
      get
      {
        return (PlacementMode) this.GetValue(SplitComboBox.PlacementProperty);
      }
      set
      {
        this.SetValue(SplitComboBox.PlacementProperty, (object) value);
      }
    }

    public DataTemplate SelectedItemTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(SplitComboBox.SelectedItemTemplateProperty);
      }
      set
      {
        this.SetValue(SplitComboBox.SelectedItemTemplateProperty, (object) value);
      }
    }

    public ICommand LastActionCommand
    {
      get
      {
        return (ICommand) this.GetValue(SplitComboBox.LastActionCommandProperty);
      }
      set
      {
        this.SetValue(SplitComboBox.LastActionCommandProperty, (object) value);
      }
    }

    public bool RememberLastItem
    {
      get
      {
        return this.rememberLastItem;
      }
      set
      {
        this.rememberLastItem = value;
      }
    }

    public ICommand InternalLastActionCommand
    {
      get
      {
        return this.internalLastActionCommand;
      }
    }

    public SplitComboBox()
    {
      this.InitializeComponent();
      this.internalLastActionCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnLastAction));
    }

    private void OnLastAction()
    {
      if (this.LastActionCommand != null)
      {
        ValueEditorUtils.ExecuteCommand(this.LastActionCommand, (IInputElement) this, (object) null);
      }
      else
      {
        int num = this.SelectedIndex < 0 ? this.savedIndex : this.SelectedIndex;
        this.SelectedIndex = -1;
        this.SelectedIndex = num;
      }
    }

    protected override void OnDropDownOpened(EventArgs e)
    {
      if (this.RememberLastItem)
        this.savedIndex = this.SelectedIndex;
      this.SelectedIndex = -1;
      base.OnDropDownOpened(e);
    }

    protected override void OnDropDownClosed(EventArgs e)
    {
      base.OnDropDownClosed(e);
      if (this.SelectedIndex >= 0)
        return;
      this.suspendSelectionChangeNotification = true;
      this.SelectedIndex = this.savedIndex;
      this.suspendSelectionChangeNotification = false;
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      if (this.suspendSelectionChangeNotification)
        return;
      base.OnSelectionChanged(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/valueeditors/splitcombobox.xaml", UriKind.Relative));
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
      this._contentLoaded = true;
    }
  }
}
