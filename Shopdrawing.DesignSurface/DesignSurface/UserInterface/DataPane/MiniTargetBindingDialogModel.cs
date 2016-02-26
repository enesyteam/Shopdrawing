// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.MiniTargetBindingDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class MiniTargetBindingDialogModel : MiniBindingDialogModel
  {
    private bool useDesignDataContext;
    private IList<ReferenceStep> targetBindableProperties;

    protected override bool CanUseTwoWayBinding
    {
      get
      {
        return !this.CurrentDataPath.Node.IsReadOnly;
      }
    }

    public MiniTargetBindingDialogModel(DataSchemaNodePath bindingPath, SceneNode targetElement, ReferenceStep targetProperty, bool useDesignDataContext)
      : base(bindingPath, targetElement, targetProperty)
    {
      this.useDesignDataContext = useDesignDataContext;
      this.targetBindableProperties = BindingPropertyHelper.GetBindableTargetProperties(this.TargetElement);
      IProperty dataContextProperty = DataContextHelper.GetDataContextProperty(this.TargetElement.Type);
      if (dataContextProperty != null && !this.TargetElement.ViewModel.BindingEditor.CanCreateAndSetBindingOrData(targetElement, (IPropertyId) dataContextProperty, bindingPath, true))
        this.targetBindableProperties.Remove((ReferenceStep) dataContextProperty);
      this.Initialize();
    }

    public SceneNode CreateAndSetBindingOrData(ref ReferenceStep targetProperty)
    {
      targetProperty = this.SelectedBindingField;
      if (this.useDesignDataContext && BaseFrameworkElement.DataContextProperty.Equals((object) targetProperty))
        targetProperty = (ReferenceStep) DesignTimeProperties.ResolveDesignTimePropertyKey(DesignTimeProperties.DesignDataContextProperty, (IPlatformMetadata) this.TargetElement.Platform.Metadata);
      SceneNode setBindingOrData = this.ViewModel.BindingEditor.CreateAndSetBindingOrData(this.TargetElement, (IPropertyId) targetProperty, this.BindingPath);
      BindingSceneNode binding = setBindingOrData as BindingSceneNode;
      if (binding != null)
        this.SetAdvancedPropertiesIfNeeded(binding);
      return setBindingOrData;
    }

    protected override IList<ReferenceStep> GetBindableProperties()
    {
      return this.targetBindableProperties;
    }

    protected override IPropertyId GetDefaultPropertySelection()
    {
      Type type = this.BindingPath.Node.Type;
      IType dataType = (IType) null;
      SceneViewModel viewModel = this.TargetElement.ViewModel;
      if (type != (Type) null)
        dataType = viewModel.ProjectContext.GetType(type);
      if (dataType == null)
        dataType = viewModel.ProjectContext.ResolveType(PlatformTypes.Object);
      IProperty property = BindingPropertyHelper.GetDefaultBindingPropertyInfo(this.TargetElement, dataType).Property;
      if (this.targetBindableProperties.IndexOf((ReferenceStep) property) < 0)
        property = (IProperty) null;
      if (property == null && dataType != null)
      {
        foreach (ReferenceStep referenceStep in (IEnumerable<ReferenceStep>) this.targetBindableProperties)
        {
          if (BindingPropertyHelper.GetPropertyCompatibility((IProperty) referenceStep, dataType, (ITypeResolver) this.ViewModel.ProjectContext) != BindingPropertyCompatibility.None)
          {
            property = (IProperty) referenceStep;
            break;
          }
        }
      }
      return (IPropertyId) property;
    }
  }
}
