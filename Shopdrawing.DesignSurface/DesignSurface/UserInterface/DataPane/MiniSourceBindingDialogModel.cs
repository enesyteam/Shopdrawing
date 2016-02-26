// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.MiniSourceBindingDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class MiniSourceBindingDialogModel : MiniBindingDialogModel
  {
    private IType targetPropertyType;
    private IList<ReferenceStep> sourceBindingProperties;

    protected override bool CanUseTwoWayBinding
    {
      get
      {
        if (this.SelectedBindingField != null)
          return (this.SelectedBindingField.WriteAccess & MemberAccessType.Public) != MemberAccessType.None;
        return false;
      }
    }

    public MiniSourceBindingDialogModel(DataSchemaNodePath bindingPath, SceneNode targetElement, IType targetPropertyType)
      : base(bindingPath, targetElement, (ReferenceStep) null)
    {
      this.targetPropertyType = targetPropertyType;
      this.sourceBindingProperties = MiniSourceBindingDialogModel.GetBindableProperties(targetElement, targetPropertyType);
      this.Initialize();
    }

    public static bool AnyBindableProperties(SceneNode sceneNode, IType targetPropertyType)
    {
      return Enumerable.Any<ReferenceStep>((IEnumerable<ReferenceStep>) BindingPropertyHelper.GetBindableSourceProperties(sceneNode), (Func<ReferenceStep, bool>) (prop => MiniSourceBindingDialogModel.IsValidProperty(sceneNode, prop, targetPropertyType)));
    }

    public static IList<ReferenceStep> GetBindableProperties(SceneNode sceneNode, IType targetPropertyType)
    {
      IList<ReferenceStep> sourceProperties = BindingPropertyHelper.GetBindableSourceProperties(sceneNode);
      for (int index = sourceProperties.Count - 1; index >= 0; --index)
      {
        if (!MiniSourceBindingDialogModel.IsValidProperty(sceneNode, sourceProperties[index], targetPropertyType))
          sourceProperties.RemoveAt(index);
      }
      return sourceProperties;
    }

    public BindingSceneNode CreateElementNameBinding(SceneNode targetNode)
    {
      BindingSceneNode binding = (BindingSceneNode) this.ViewModel.CreateSceneNode(PlatformTypes.Binding);
      ReferenceStep selectedBindingField = this.SelectedBindingField;
      targetNode.EnsureNamed();
      binding.ElementName = targetNode.Name;
      binding.SetPath(selectedBindingField.Name);
      this.SetAdvancedPropertiesIfNeeded(binding);
      return binding;
    }

    protected override IList<ReferenceStep> GetBindableProperties()
    {
      return this.sourceBindingProperties;
    }

    protected override IPropertyId GetDefaultPropertySelection()
    {
      BindingPropertyMatchInfo bindingPropertyInfo = BindingPropertyHelper.GetDefaultBindingPropertyInfo(this.TargetElement, this.targetPropertyType);
      if (bindingPropertyInfo.Compatibility != BindingPropertyCompatibility.None)
      {
        ReferenceStep referenceStep = (ReferenceStep) bindingPropertyInfo.Property;
        if (this.sourceBindingProperties.Contains(referenceStep))
          return (IPropertyId) referenceStep;
      }
      foreach (ReferenceStep referenceStep in (IEnumerable<ReferenceStep>) this.sourceBindingProperties)
      {
        if (this.targetPropertyType != null && this.targetPropertyType.IsAssignableFrom((ITypeId) referenceStep.PropertyType))
          return (IPropertyId) referenceStep;
      }
      return (IPropertyId) null;
    }

    private static bool IsValidProperty(SceneNode sceneNode, ReferenceStep referenceStep, IType targetPropertyType)
    {
      if (referenceStep.IsAttachable)
        return false;
      IType type1 = targetPropertyType.NullableType ?? targetPropertyType;
      if (PlatformTypes.String.Equals((object) type1) || PlatformTypes.Object.Equals((object) type1))
        return true;
      IType type2 = referenceStep.PropertyType.NullableType ?? referenceStep.PropertyType;
      if (type1.IsAssignableFrom((ITypeId) type2))
        return BindingPropertyHelper.IsPropertyValidBindingSource(sceneNode, referenceStep);
      return false;
    }
  }
}
