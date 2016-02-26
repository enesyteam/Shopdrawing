// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ConditionOperandEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ConditionOperandEditor : StackPanel, INotifyPropertyChanged, IComponentConnector
  {
    private DataStorePropertiesValueProvider dataStoreProvider;
    private OperandValueType currentValueType;
    private SceneNodeProperty editingProperty;
    private bool doNotInferEditorToUse;
    internal ConditionOperandEditor ConditionOperand;
    internal Control Control;
    private bool _contentLoaded;

    public OperandValueType CurrentOperandValueType
    {
      get
      {
        return this.currentValueType;
      }
      set
      {
        if (this.currentValueType == value)
          return;
        this.currentValueType = value;
        this.OnPropertyChanged("CurrentOperandValueType");
      }
    }

    public ICommand ChangeOperandValueTypeCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnChangeOperandValueType));
      }
    }

    public Array OperandValueTypes
    {
      get
      {
        return Enum.GetValues(typeof (OperandValueType));
      }
    }

    public string Tooltip
    {
      get
      {
        return BindingPropertyHelper.GetElementNameFromBoundProperty(this.editingProperty);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ConditionOperandEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChanged);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.InitializeComponent();
      this.dataStoreProvider = new DataStorePropertiesValueProvider((FrameworkElement) this);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnChangeOperandValueType()
    {
      if (this.editingProperty == null || this.editingProperty.GetValue() == null)
        return;
      this.editingProperty.PropertyValue.ClearValue();
      this.doNotInferEditorToUse = true;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.editingProperty != null)
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.PropertyReferenceChanged);
      this.Rebuild();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      this.Unhook();
    }

    private void Rebuild()
    {
      this.dataStoreProvider.Rebuild();
      SceneNodeProperty property = this.DataContext as SceneNodeProperty;
      if (property != null)
      {
        this.editingProperty = property;
        this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.PropertyReferenceChanged);
        this.CurrentOperandValueType = property.GetValue() != null || this.doNotInferEditorToUse ? (this.dataStoreProvider.FindMatchDataStorePropertyEntry(property, (string) null) == null ? OperandValueType.Literal : OperandValueType.DataStore) : (property.SceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.SupportPrototyping) ? OperandValueType.DataStore : OperandValueType.Literal);
      }
      else
        this.editingProperty = (SceneNodeProperty) null;
      this.doNotInferEditorToUse = false;
      this.OnPropertyChanged("Tooltip");
    }

    private void PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.OnPropertyChanged("Tooltip");
    }

    private void Unhook()
    {
      this.Unloaded -= new RoutedEventHandler(this.OnUnloaded);
      this.DataContextChanged -= new DependencyPropertyChangedEventHandler(this.OnDataContextChanged);
      if (this.editingProperty == null)
        return;
      this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.PropertyReferenceChanged);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/conditionoperandeditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ConditionOperand = (ConditionOperandEditor) target;
          break;
        case 2:
          this.Control = (Control) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
