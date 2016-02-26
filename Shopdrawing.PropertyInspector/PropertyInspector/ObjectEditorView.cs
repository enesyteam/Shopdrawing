// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ObjectEditorView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ObjectEditorView : Grid, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof (PropertyValue), typeof (ObjectEditorView), new PropertyMetadata(new PropertyChangedCallback(ObjectEditorView.OnPropertyValueChanged)));
    private ICollectionView quickTypeView;
    private ObservableCollection<object> quickTypeList;
    internal ObjectEditorView ObjectEditorViewControl;
    private bool _contentLoaded;

    public PropertyValue PropertyValue
    {
      get
      {
        return (PropertyValue) this.GetValue(ObjectEditorView.PropertyValueProperty);
      }
      set
      {
        this.SetValue(ObjectEditorView.PropertyValueProperty, value);
      }
    }

    public string PropertyValueTypeName
    {
      get
      {
        if (this.PropertyValue != null)
        {
          object obj = this.PropertyValue.get_Value();
          if (obj != null)
          {
            Type type = obj.GetType();
            if (PlatformTypes.DesignToolAssemblies.Contains(type.Assembly))
            {
              type = this.PropertyValue.get_ParentProperty().get_PropertyType();
              SceneNodeProperty sceneNodeProperty = this.PropertyValue.get_ParentProperty() as SceneNodeProperty;
              if (sceneNodeProperty != null)
              {
                DocumentNodePath valueAsDocumentNode = sceneNodeProperty.SceneNodeObjectSet.RepresentativeSceneNode.GetLocalValueAsDocumentNode(sceneNodeProperty.Reference);
                if (valueAsDocumentNode != null)
                  type = valueAsDocumentNode.Node.TargetType;
              }
            }
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.SubPropertyEditorTypeName, new object[1]
            {
              type.Name
            });
          }
        }
        return string.Empty;
      }
    }

    public Collection<object> QuickTypeList
    {
      get
      {
        return (Collection<object>) this.quickTypeList;
      }
    }

    public ICommand AddItemCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddItem));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObjectEditorView()
    {
      this.quickTypeList = new ObservableCollection<object>();
      this.quickTypeView = CollectionViewSource.GetDefaultView(this.quickTypeList);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.ObjectEditorView_DataContextChanged);
      this.InitializeComponent();
    }

    private void ObjectEditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      PropertyValue propertyValue1 = (PropertyValue) e.OldValue;
      PropertyValue propertyValue2 = (PropertyValue) e.NewValue;
      if (propertyValue1 != null)
        propertyValue1.remove_RootValueChanged(new EventHandler(this.PropertyRootValueChanged));
      if (propertyValue2 == null)
        return;
      propertyValue2.add_RootValueChanged(new EventHandler(this.PropertyRootValueChanged));
    }

    private void PropertyRootValueChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("PropertyValueTypeName");
    }

    private static void OnPropertyValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ObjectEditorView objectEditorView = o as ObjectEditorView;
      if (objectEditorView == null)
        return;
      objectEditorView.Rebuild();
    }

    private void Rebuild()
    {
      this.quickTypeList.Clear();
      if (this.quickTypeView != null)
        this.quickTypeView.CurrentChanged -= new EventHandler(this.OnQuickTypeViewCurrentChanged);
      if (this.PropertyValue != null)
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) this.PropertyValue.get_ParentProperty();
        using (IEnumerator<KeyValuePair<Type, IList<NewItemFactory>>> enumerator1 = ((IEnumerable<KeyValuePair<Type, IList<NewItemFactory>>>) ExtensibilityMetadataHelper.GetNewItemFactoriesFromAttributes(sceneNodeProperty.GetAttributes<NewItemTypesAttribute>(true), sceneNodeProperty.SceneNodeObjectSet.DesignerContext.MessageLoggingService)).GetEnumerator())
        {
          while (((IEnumerator) enumerator1).MoveNext())
          {
            KeyValuePair<Type, IList<NewItemFactory>> current1 = enumerator1.Current;
            using (IEnumerator<NewItemFactory> enumerator2 = ((IEnumerable<NewItemFactory>) current1.Value).GetEnumerator())
            {
              while (((IEnumerator) enumerator2).MoveNext())
              {
                NewItemFactory current2 = enumerator2.Current;
                this.quickTypeList.Add((object) new NewItemFactoryTypeModel(current1.Key, current2, sceneNodeProperty.SceneNodeObjectSet.DesignerContext.MessageLoggingService));
              }
            }
          }
        }
        this.quickTypeView.CurrentChanged += new EventHandler(this.OnQuickTypeViewCurrentChanged);
      }
      this.OnPropertyChanged("PropertyValueTypeName");
      this.OnPropertyChanged("QuickTypeList");
    }

    private void OnQuickTypeViewCurrentChanged(object sender, EventArgs e)
    {
      if (this.quickTypeView.CurrentPosition < 0)
        return;
      this.AddItem(this.quickTypeView.CurrentItem as NewItemFactoryTypeModel);
    }

    private void AddItem()
    {
      if (!((SceneNodeProperty) this.PropertyValue.get_ParentProperty()).SceneNodeObjectSet.ViewModel.IsEditable)
        return;
      this.AddItem((NewItemFactoryTypeModel) null);
    }

    private void AddItem(NewItemFactoryTypeModel itemFactoryTypeModel)
    {
      if (itemFactoryTypeModel != null)
      {
        try
        {
          object instance = itemFactoryTypeModel.CreateInstance();
          if (instance == null)
            return;
          this.SetPropertyAsObject(instance);
        }
        catch (Exception ex)
        {
          ((SceneNodeProperty) this.PropertyValue.get_ParentProperty()).SceneNodeObjectSet.DesignerContext.MessageLoggingService.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.ObjectEditorViewCollectionItemFactoryInstantiateFailed, new object[2]
          {
            (object) ((object) itemFactoryTypeModel.ItemFactory).GetType().Name,
            (object) ExtensibilityMetadataHelper.GetExceptionMessage(ex)
          }));
        }
      }
      else
      {
        Type type = this.PromptForClrType();
        if (!(type != (Type) null))
          return;
        this.SetPropertyAsType(type);
      }
    }

    private void SetPropertyValueCore(SceneNodeProperty parentProperty, SceneNode valueAsSceneNode)
    {
      parentProperty.SetValue(valueAsSceneNode.DocumentNode);
      if (!valueAsSceneNode.TrueTargetType.IsClass)
        return;
      SceneNodePropertyCollection propertyCollection = (SceneNodePropertyCollection) parentProperty.get_PropertyValue().get_SubProperties();
      propertyCollection.Rebuild();
      if (((PropertyEntryCollection) propertyCollection).get_Count() <= 0 || !((SceneNodeProperty) this.PropertyValue.get_ParentProperty()).SceneNodeObjectSet.IsViewRepresentationValid)
        return;
      PropertyValueEditorCommands.get_ShowExtendedPinnedEditor().Execute(null, (IInputElement) this);
    }

    private void SetPropertyAsObject(object instance)
    {
      SceneNodeProperty parentProperty = (SceneNodeProperty) this.PropertyValue.get_ParentProperty();
      SceneViewModel viewModel = parentProperty.SceneNodeObjectSet.ViewModel;
      SceneNode valueAsSceneNode = instance == null || !(instance is UserControl) ? viewModel.CreateSceneNode(instance) : viewModel.CreateSceneNode(instance.GetType());
      this.SetPropertyValueCore(parentProperty, valueAsSceneNode);
    }

    private void SetPropertyAsType(Type type)
    {
      SceneNodeProperty parentProperty = (SceneNodeProperty) this.PropertyValue.get_ParentProperty();
      SceneNode sceneNode = parentProperty.SceneNodeObjectSet.ViewModel.CreateSceneNode(type);
      this.SetPropertyValueCore(parentProperty, sceneNode);
    }

    public Type PromptForClrType()
    {
      if (this.PropertyValue == null)
        return (Type) null;
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) this.PropertyValue.get_ParentProperty();
      return ClrNewObjectDialog.CreateNewClrObject(sceneNodeProperty.SceneNodeObjectSet.ViewModel, ((PropertyEntry) sceneNodeProperty).get_PropertyType());
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/objecteditorview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ObjectEditorViewControl = (ObjectEditorView) target;
      else
        this._contentLoaded = true;
    }
  }
}
