// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BindingSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BindingSceneNode : SceneNode
  {
    public static readonly IPropertyId PathProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "Path", MemberAccessTypes.Public);
    public static readonly IPropertyId ConverterProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "Converter", MemberAccessTypes.Public);
    public static readonly IPropertyId ConverterParameterProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "ConverterParameter", MemberAccessTypes.Public);
    public static readonly IPropertyId ElementNameProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "ElementName", MemberAccessTypes.Public);
    public static readonly IPropertyId ModeProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "Mode", MemberAccessTypes.Public);
    public static readonly IPropertyId RelativeSourceProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "RelativeSource", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly IPropertyId UpdateSourceTriggerProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "UpdateSourceTrigger", MemberAccessTypes.Public);
    public static readonly IPropertyId XPathProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "XPath", MemberAccessTypes.Public);
    public static readonly IPropertyId FallbackValueProperty = (IPropertyId) PlatformTypes.Binding.GetMember(MemberType.LocalProperty, "FallbackValue", MemberAccessTypes.Public);
    private static CategoryAttribute contentCategory = new CategoryAttribute("Content");
    public static readonly BindingSceneNode.ConcreteBindingSceneNodeFactory Factory = new BindingSceneNode.ConcreteBindingSceneNodeFactory();
    private SceneNode elementBindingTarget;

    public string Path
    {
      get
      {
        IViewObject viewObject = this.Platform.ViewObjectFactory.Instantiate(this.GetLocalOrDefaultValue(BindingSceneNode.PathProperty));
        if (viewObject != null)
          return (string) viewObject.GetCurrentValue(this.ProjectContext.ResolveProperty(this.Platform.Metadata.KnownProperties.PropertyPathPath));
        return (string) null;
      }
    }

    public string PathOrXPath
    {
      get
      {
        string str = this.Path;
        if (string.IsNullOrEmpty(str) && this.SupportsXPath)
          str = this.XPath;
        if (str == null)
          str = string.Empty;
        return str;
      }
    }

    public string ElementName
    {
      get
      {
        return this.GetLocalValue(BindingSceneNode.ElementNameProperty) as string;
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.ElementNameProperty, (object) value);
      }
    }

    public bool SupportsElementName
    {
      get
      {
        return BindingSceneNode.IsElementNameSupported(this.ViewModel);
      }
    }

    public string XPath
    {
      get
      {
        return this.GetLocalValue(BindingSceneNode.XPathProperty) as string;
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.XPathProperty, (object) value);
      }
    }

    public bool SupportsXPath
    {
      get
      {
        return BindingSceneNode.IsXPathSupported(this.ViewModel);
      }
    }

    public DocumentNode Source
    {
      get
      {
        using (this.ViewModel.ForceBaseValue())
        {
          SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(BindingSceneNode.SourceProperty, false);
          return valueAsSceneNode != null ? valueAsSceneNode.DocumentNode : (DocumentNode) null;
        }
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.SourceProperty, value);
      }
    }

    public bool SupportsSource
    {
      get
      {
        return BindingSceneNode.IsSourceSupported(this.ViewModel);
      }
    }

    public DocumentNode RelativeSource
    {
      get
      {
        using (this.ViewModel.ForceBaseValue())
        {
          SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(BindingSceneNode.RelativeSourceProperty, false);
          return valueAsSceneNode != null ? valueAsSceneNode.DocumentNode : (DocumentNode) null;
        }
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.RelativeSourceProperty, value);
      }
    }

    public bool SupportsRelativeSource
    {
      get
      {
        return BindingSceneNode.IsRelativeSourceSupported(this.ViewModel);
      }
    }

    public object UpdateSourceTrigger
    {
      get
      {
        return this.GetLocalOrDefaultValue(BindingSceneNode.UpdateSourceTriggerProperty);
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.UpdateSourceTriggerProperty, value);
      }
    }

    public bool SupportsUpdateSourceTrigger
    {
      get
      {
        return BindingSceneNode.IsUpdateSourceTriggerSupported(this.ViewModel);
      }
    }

    public BindingMode Mode
    {
      get
      {
        object defaultValueAsWpf = this.GetLocalOrDefaultValueAsWpf(BindingSceneNode.ModeProperty);
        if (defaultValueAsWpf is BindingMode)
          return (BindingMode) defaultValueAsWpf;
        return BindingMode.TwoWay;
      }
      set
      {
        this.SetLocalValueAsWpf(BindingSceneNode.ModeProperty, (object) value);
      }
    }

    public bool IsModeSet
    {
      get
      {
        return ((DocumentCompositeNode) this.DocumentNode).Properties[BindingSceneNode.ModeProperty] != null;
      }
    }

    public DocumentNode FallbackValue
    {
      get
      {
        using (this.ViewModel.ForceBaseValue())
        {
          SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(BindingSceneNode.FallbackValueProperty, false);
          return valueAsSceneNode != null ? valueAsSceneNode.DocumentNode : (DocumentNode) null;
        }
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.FallbackValueProperty, value);
      }
    }

    public bool SupportsFallbackValue
    {
      get
      {
        return BindingSceneNode.IsFallbackValueSupported(this.ViewModel);
      }
    }

    public DocumentNode Converter
    {
      get
      {
        using (this.ViewModel.ForceBaseValue())
        {
          SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(BindingSceneNode.ConverterProperty, false);
          return valueAsSceneNode != null ? valueAsSceneNode.DocumentNode : (DocumentNode) null;
        }
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.ConverterProperty, value);
      }
    }

    public bool SupportsConverter
    {
      get
      {
        return BindingSceneNode.IsConverterSupported(this.ViewModel);
      }
    }

    public object ConverterParameter
    {
      get
      {
        return this.GetLocalValue(BindingSceneNode.ConverterParameterProperty);
      }
      set
      {
        this.SetLocalValue(BindingSceneNode.ConverterParameterProperty, value);
      }
    }

    public bool IsDataContextBinding
    {
      get
      {
        if (this.SupportsSource && this.IsSet(BindingSceneNode.SourceProperty) != PropertyState.Unset || this.SupportsRelativeSource && this.IsSet(BindingSceneNode.RelativeSourceProperty) != PropertyState.Unset)
          return false;
        if (this.SupportsElementName)
          return this.IsSet(BindingSceneNode.ElementNameProperty) == PropertyState.Unset;
        return true;
      }
    }

    public SceneNode ElementBindingTarget
    {
      get
      {
        return this.elementBindingTarget;
      }
      set
      {
        this.elementBindingTarget = value;
      }
    }

    public BindingSceneNode SetPath(string path)
    {
      this.SetLocalValue(BindingSceneNode.PathProperty, this.Platform.Metadata.MakePropertyPath(path));
      return this;
    }

    public static bool IsElementNameSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.ElementNameProperty) != null;
    }

    public static bool IsXPathSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.XPathProperty) != null;
    }

    public static bool IsSourceSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.SourceProperty) != null;
    }

    public static bool IsRelativeSourceSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.RelativeSourceProperty) != null;
    }

    public static bool IsUpdateSourceTriggerSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.UpdateSourceTriggerProperty) != null;
    }

    public static bool IsFallbackValueSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.FallbackValueProperty) != null;
    }

    public static bool IsConverterSupported(SceneViewModel viewModel)
    {
      return viewModel.ProjectContext.ResolveProperty(BindingSceneNode.ConverterProperty) != null;
    }

    public bool IsBindingLegal(SceneNode targetElement, ReferenceStep targetProperty, out string errorMessage)
    {
      errorMessage = string.Empty;
      string pathOrXpath = this.PathOrXPath;
      if (this.SupportsElementName && this.ElementName != null && targetElement.Name == this.ElementName || this.ElementBindingTarget == targetElement)
      {
        if (pathOrXpath.Length == 0)
        {
          errorMessage = StringTable.BindingPropertyToDefiningElementError;
          return false;
        }
        if (pathOrXpath == targetProperty.Name)
        {
          errorMessage = StringTable.BindingPropertyToSelfError;
          return false;
        }
      }
      bool flag = false;
      if (targetProperty.Attributes.Matches((Attribute) BindingSceneNode.contentCategory))
        flag = true;
      if (ContentPresenterElement.ContentProperty.Equals((object) targetProperty))
        flag = true;
      if (flag && (this.SupportsElementName && this.ElementName != null || this.ElementBindingTarget != null))
      {
        string visualTarget = (string) null;
        if (BindingSceneNode.DoesVisualPropertyEvaluateToVisual(pathOrXpath, out visualTarget))
        {
          errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.BindingVisualToVisualError, (object) targetProperty.Name, (object) targetElement.TargetType.Name, (object) visualTarget);
          return false;
        }
      }
      return true;
    }

    private static bool DoesVisualPropertyEvaluateToVisual(string propertyName, out string visualTarget)
    {
      visualTarget = string.Empty;
      switch (propertyName)
      {
        case "":
          visualTarget = StringTable.BindingToVisualError;
          break;
        case "Content":
          visualTarget = StringTable.BindingToContentError;
          break;
        case "Items":
        case "ItemsSource":
          visualTarget = StringTable.BindingToItemsError;
          break;
        case "Header":
          visualTarget = StringTable.BindingToHeaderError;
          break;
        case "Children":
          visualTarget = StringTable.BindingToChildrenError;
          break;
      }
      return visualTarget != string.Empty;
    }

    public class ConcreteBindingSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BindingSceneNode();
      }

      public BindingSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (BindingSceneNode) this.Instantiate(viewModel, PlatformTypes.Binding);
      }
    }
  }
}
