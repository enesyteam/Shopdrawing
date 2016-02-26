// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.ConditionModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class ConditionModel : INotifyPropertyChanged
  {
    private ITriggerConditionNode condition;
    private PropertyTriggerModel propertyTrigger;
    private bool firstSourceNameInGroup;
    private DelegateCommand deleteCommand;

    public ITriggerConditionNode Condition
    {
      get
      {
        return this.condition;
      }
    }

    public PropertyTriggerModel PropertyTrigger
    {
      get
      {
        return this.propertyTrigger;
      }
    }

    public bool FirstSourceNameInGroup
    {
      get
      {
        return this.firstSourceNameInGroup;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) this.deleteCommand;
      }
    }

    private Type TargetElementType
    {
      get
      {
        ITriggerContainer triggerContainer = this.TriggerSource as ITriggerContainer;
        if (triggerContainer != null)
          return triggerContainer.TargetElementType;
        return typeof (object);
      }
    }

    public PropertyInformation Property
    {
      get
      {
        return PropertyInformation.FromDependencyProperty(this.condition.PropertyKey, this.TargetElementType, (ITypeResolver) this.TriggerSource.ProjectContext);
      }
      set
      {
        if (!((TriggerSourceInformation) value != (TriggerSourceInformation) null) || value.DependencyProperty == this.condition.PropertyKey)
          return;
        using (SceneEditTransaction editTransaction = this.condition.TriggerNode.DesignerContext.ActiveDocument.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          DependencyProperty propertyKey = this.condition.PropertyKey;
          this.condition.PropertyKey = value.DependencyProperty;
          if (propertyKey.PropertyType != value.DependencyProperty.PropertyType)
          {
            object obj = value.DependencyProperty.DefaultMetadata.DefaultValue;
            if (obj is bool)
              obj = (object) (bool) (!(bool) obj ? true : false);
            this.condition.Value = obj;
          }
          editTransaction.Commit();
        }
      }
    }

    public string TriggerValue
    {
      get
      {
        return this.GetTriggerValueCore();
      }
      set
      {
        this.SetTriggerValueCore(value);
      }
    }

    public SceneNode TriggerSource
    {
      get
      {
        return this.Condition.Source;
      }
      set
      {
        if (value == null || this.condition.Source == value)
          return;
        using (SceneEditTransaction editTransaction = this.condition.TriggerNode.DesignerContext.ActiveDocument.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
        {
          this.condition.Source = value;
          DependencyPropertyDescriptor propertyDescriptor = (DependencyPropertyDescriptor) null;
          if ((TriggerSourceInformation) this.Property != (TriggerSourceInformation) null)
            propertyDescriptor = DependencyPropertyDescriptor.FromName(this.Property.DependencyProperty.Name, value.TargetType, typeof (DependencyObject));
          if (propertyDescriptor != null)
          {
            this.condition.PropertyKey = propertyDescriptor.DependencyProperty;
          }
          else
          {
            PropertyInformation defaultProperty = TriggersHelper.GetDefaultProperty(value.TargetType, this.condition.TriggerNode.DocumentContext);
            if ((TriggerSourceInformation) defaultProperty != (TriggerSourceInformation) null)
              this.Property = defaultProperty;
          }
          editTransaction.Commit();
        }
      }
    }

    public string TriggerSourceDisplayName
    {
      get
      {
        return SceneNodeToStringConverter.ConvertToString(this.Condition.Source);
      }
    }

    public IEnumerable AvailableTriggerSources
    {
      get
      {
        yield return (object) this.TriggerSource;
        foreach (SceneNode sceneNode in this.condition.TriggerNode.ViewModel.ElementSelectionSet.Selection)
        {
          if (sceneNode != this.TriggerSource)
            yield return (object) sceneNode;
        }
      }
    }

    public IEnumerable AvailableProperties
    {
      get
      {
        ObservableCollectionWorkaround<PropertyInformation> collectionWorkaround = new ObservableCollectionWorkaround<PropertyInformation>();
        foreach (PropertyInformation propertyInformation in PropertyInformation.GetPropertiesForType(this.TargetElementType, (ITypeResolver) this.TriggerSource.ProjectContext))
          collectionWorkaround.Add(propertyInformation);
        PropertyInformation propertyInformation1 = PropertyInformation.FromDependencyProperty(this.condition.PropertyKey, this.TargetElementType, (ITypeResolver) this.TriggerSource.ProjectContext);
        if ((TriggerSourceInformation) propertyInformation1 != (TriggerSourceInformation) null && !collectionWorkaround.Contains(propertyInformation1))
          collectionWorkaround.Add(propertyInformation1);
        collectionWorkaround.Sort();
        ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) collectionWorkaround);
        defaultView.GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription()
        {
          PropertyName = "GroupBy"
        });
        ListCollectionView listCollectionView = defaultView as ListCollectionView;
        if (listCollectionView != null)
        {
          listCollectionView.CustomSort = (IComparer) new PropertyTriggerConditionPropertyCategoryComparer();
          listCollectionView.IsDataInGroupOrder = true;
        }
        return (IEnumerable) defaultView;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ConditionModel(ITriggerConditionNode condition, PropertyTriggerModel propertyTrigger)
    {
      this.condition = condition;
      this.propertyTrigger = propertyTrigger;
      this.deleteCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Delete));
      if (!(this.condition is TriggerNode))
        return;
      this.deleteCommand.IsEnabled = false;
    }

    public void Initialize()
    {
    }

    internal void SetFirstSourceNameInGroup(bool b)
    {
      this.firstSourceNameInGroup = b;
    }

    public void Update()
    {
      this.OnPropertyChanged("Property");
      this.OnPropertyChanged("AvailableTriggerSources");
      this.OnPropertyChanged("AvailableProperties");
      this.OnPropertyChanged("TriggerSource");
      this.OnPropertyChanged("TriggerValue");
    }

    public void Delete()
    {
      ConditionNode conditionNode = this.Condition as ConditionNode;
      if (conditionNode == null)
        return;
      using (SceneEditTransaction editTransaction = conditionNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        MultiTriggerNode nodeTypeAncestor = conditionNode.FindSceneNodeTypeAncestor<MultiTriggerNode>();
        conditionNode.Remove();
        if (nodeTypeAncestor != null && nodeTypeAncestor.Conditions.Count == 1)
          TriggersHelper.ConvertToTrigger(nodeTypeAncestor);
        editTransaction.Commit();
      }
    }

    private string GetTriggerValueCore()
    {
      object obj = this.condition.Value;
      bool flag = obj == null;
      if (!flag && Nullable.GetUnderlyingType(obj.GetType()) != (Type) null)
        flag = obj.Equals((object) null);
      string str;
      if (!flag)
      {
        TypeConverter typeConverter = MetadataStore.GetTypeConverter(obj);
        str = typeConverter == null || !typeConverter.CanConvertTo(typeof (string)) ? obj.ToString() : typeConverter.ConvertToString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, obj);
      }
      else
        str = "null";
      return str;
    }

    private void SetTriggerValueCore(string value)
    {
      object obj = (object) null;
      bool flag = false;
      if (value != null && !this.Property.PropertyReferenceStep.TargetType.IsValueType)
      {
        string[] strArray = new string[4]
        {
          "null",
          "x:null",
          "{x:null}",
          "nothing"
        };
        foreach (string strA in strArray)
        {
          if (string.Compare(strA, value, StringComparison.CurrentCultureIgnoreCase) == 0)
            flag = true;
        }
      }
      if (!flag)
      {
        TypeConverter converterForProperty = ConditionModel.GetTypeConverterForProperty(this.condition.PropertyKey);
        if (converterForProperty != null)
        {
          if (converterForProperty.CanConvertFrom(typeof (string)))
          {
            try
            {
              obj = converterForProperty.ConvertFromString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, value);
              flag = true;
            }
            catch (NotSupportedException ex)
            {
            }
          }
        }
      }
      if (!flag || object.Equals((object) value, this.condition.Value))
        return;
      using (SceneEditTransaction editTransaction = this.condition.TriggerNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        this.condition.Value = obj;
        editTransaction.Commit();
      }
    }

    private static TypeConverter GetTypeConverterForProperty(DependencyProperty property)
    {
      return !(property.Name == "Content") && !(property.Name == "Header") && (!(property.Name == "ToolTip") && !(property.Name == "Tag")) || !property.PropertyType.IsAssignableFrom(typeof (object)) ? MetadataStore.GetTypeConverter(property.PropertyType) : (TypeConverter) new StringConverter();
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
