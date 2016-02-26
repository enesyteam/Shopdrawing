// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.MiniBindingDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal abstract class MiniBindingDialogModel : BindingDialogModelBase
  {
    private DataSchemaNodePath bindingPath;
    private bool canSetTwoWayBinding;
    private ReadOnlyCollection<ReferenceStepItem> bindingFields;
    private ReferenceStepItem currentItem;
    private bool setAdvancedProperties;

    public override DataSchemaNodePath CurrentDataPath
    {
      get
      {
        return this.bindingPath;
      }
    }

    public ReadOnlyCollection<ReferenceStepItem> BindingFields
    {
      get
      {
        return this.bindingFields;
      }
    }

    public ReferenceStep SelectedBindingField
    {
      get
      {
        if (this.CurrentItem != null)
          return this.CurrentItem.ReferenceStep;
        return (ReferenceStep) null;
      }
      set
      {
        this.CurrentItem = Enumerable.FirstOrDefault<ReferenceStepItem>((IEnumerable<ReferenceStepItem>) this.bindingFields, (Func<ReferenceStepItem, bool>) (item => item.ReferenceStep == value));
      }
    }

    public ReferenceStepItem CurrentItem
    {
      get
      {
        return this.currentItem;
      }
      set
      {
        if (this.currentItem == value)
          return;
        this.currentItem = value;
        this.UpdateBindingModes();
        this.OnPropertyChanged("CurrentItem");
        this.OnPropertyChanged("SelectedBindingField");
      }
    }

    public string Label
    {
      get
      {
        string str = this.TargetElement.Name;
        if (string.IsNullOrEmpty(str))
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", new object[1]
          {
            (object) this.TargetElement.Type.Name
          });
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.MiniBindingDialogLabel, new object[1]
        {
          (object) str
        });
      }
    }

    protected DataSchemaNodePath BindingPath
    {
      get
      {
        return this.bindingPath;
      }
    }

    protected abstract bool CanUseTwoWayBinding { get; }

    protected MiniBindingDialogModel(DataSchemaNodePath bindingPath, SceneNode targetElement, ReferenceStep targetProperty)
      : base(targetElement, targetProperty)
    {
      this.bindingPath = bindingPath;
    }

    protected abstract IList<ReferenceStep> GetBindableProperties();

    protected abstract IPropertyId GetDefaultPropertySelection();

    protected void SetAdvancedPropertiesIfNeeded(BindingSceneNode binding)
    {
      if (!this.setAdvancedProperties)
        return;
      using (this.ViewModel.AnimationEditor.DeferKeyFraming())
        this.SetCommonBindingValues(binding);
    }

    protected override void InitializeExpandedArea()
    {
    }

    private void InitializeBindingFields()
    {
      List<ReferenceStepItem> list = new List<ReferenceStepItem>(Enumerable.Select<ReferenceStep, ReferenceStepItem>((IEnumerable<ReferenceStep>) this.GetBindableProperties(), (Func<ReferenceStep, ReferenceStepItem>) (referenceStep => new ReferenceStepItem(referenceStep))));
      list.Sort((IComparer<ReferenceStepItem>) new MiniBindingDialogModel.ReferenceStepComparer());
      this.bindingFields = new ReadOnlyCollection<ReferenceStepItem>((IList<ReferenceStepItem>) list);
      this.canSetTwoWayBinding = true;
    }

    protected void Initialize()
    {
      this.InitializeBindingFields();
      if (this.TargetProperty != null)
      {
        this.SelectedBindingField = this.TargetProperty;
      }
      else
      {
        IPropertyId propertyId = this.GetDefaultPropertySelection();
        if (propertyId == null && this.bindingFields.Count > 0)
          propertyId = (IPropertyId) this.bindingFields[0].ReferenceStep;
        this.SelectedBindingField = (ReferenceStep) propertyId;
      }
      this.UpdateBindingModes();
      this.PropertyChanged += new PropertyChangedEventHandler(this.Handle_PropertyChanged);
    }

    private void Handle_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "CurrentBindingMode") && !(e.PropertyName == "BindingFallbackValue") && !(e.PropertyName == "CurrentUpdateSourceTrigger"))
        return;
      this.setAdvancedProperties = true;
      this.PropertyChanged -= new PropertyChangedEventHandler(this.Handle_PropertyChanged);
    }

    private void UpdateBindingModes()
    {
      if (this.canSetTwoWayBinding == this.CanUseTwoWayBinding)
      {
        this.UpdateDefaultBindingOption();
      }
      else
      {
        this.canSetTwoWayBinding = this.CanUseTwoWayBinding;
        if (!this.canSetTwoWayBinding)
        {
          BindingMode currentBindingMode = this.CurrentBindingMode;
          this.BindingModesCollection.Remove(BindingMode.TwoWay);
          this.BindingModesCollection.Remove(BindingMode.OneWayToSource);
          this.UpdateDefaultBindingOption();
          if (this.BindingModesCollection.Contains(currentBindingMode))
            return;
          if (this.BindingModesCollection.Contains(BindingMode.Default))
            this.CurrentBindingMode = BindingMode.Default;
          else
            this.CurrentBindingMode = BindingMode.OneWay;
        }
        else
        {
          this.BindingModesCollection.Add(BindingMode.TwoWay);
          this.BindingModesCollection.Add(BindingMode.OneWayToSource);
          if (this.BindingModesCollection.Contains(BindingMode.Default) || !this.IsBindingModeSupported(BindingMode.Default))
            return;
          this.BindingModesCollection.Insert(0, BindingMode.Default);
        }
      }
    }

    private void UpdateDefaultBindingOption()
    {
      if (this.canSetTwoWayBinding || !this.IsBindingModeSupported(BindingMode.Default))
        return;
      bool flag = this.BindsTwoWayByDefault(this.TargetElement, this.SelectedBindingField);
      if (!flag && !this.BindingModesCollection.Contains(BindingMode.Default))
      {
        this.BindingModesCollection.Insert(0, BindingMode.Default);
      }
      else
      {
        if (!flag || !this.BindingModesCollection.Contains(BindingMode.Default))
          return;
        this.BindingModesCollection.Remove(BindingMode.Default);
      }
    }

    private class ReferenceStepComparer : System.Collections.Generic.Comparer<ReferenceStepItem>
    {
      public override int Compare(ReferenceStepItem x, ReferenceStepItem y)
      {
        return string.Compare(x.ReferenceStep.Name, y.ReferenceStep.Name, StringComparison.Ordinal);
      }
    }
  }
}
