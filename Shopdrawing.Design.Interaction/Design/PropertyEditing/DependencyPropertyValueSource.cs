// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.DependencyPropertyValueSource
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.ComponentModel;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class DependencyPropertyValueSource : PropertyValueSource
  {
    private static DependencyPropertyValueSource _binding;
    private static DependencyPropertyValueSource _templateBinding;
    private static DependencyPropertyValueSource _static;
    private static DependencyPropertyValueSource _staticResource;
    private static DependencyPropertyValueSource _dynamicResource;
    private static DependencyPropertyValueSource _customMarkupExtension;
    private static DependencyPropertyValueSource _defaultValue;
    private static DependencyPropertyValueSource _localValue;
    private static DependencyPropertyValueSource _inheritedValue;
    private static DependencyPropertyValueSource _null;
    private static DependencyPropertyValueSource _ambient;
    private readonly DependencyPropertyValueSource.ValueSource _source;

    public static DependencyPropertyValueSource Binding
    {
      get
      {
        if (DependencyPropertyValueSource._binding == null)
          DependencyPropertyValueSource._binding = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.Binding);
        return DependencyPropertyValueSource._binding;
      }
    }

    public static DependencyPropertyValueSource TemplateBinding
    {
      get
      {
        if (DependencyPropertyValueSource._templateBinding == null)
          DependencyPropertyValueSource._templateBinding = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.TemplateBinding);
        return DependencyPropertyValueSource._templateBinding;
      }
    }

    public static DependencyPropertyValueSource Static
    {
      get
      {
        if (DependencyPropertyValueSource._static == null)
          DependencyPropertyValueSource._static = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.Static);
        return DependencyPropertyValueSource._static;
      }
    }

    public static DependencyPropertyValueSource StaticResource
    {
      get
      {
        if (DependencyPropertyValueSource._staticResource == null)
          DependencyPropertyValueSource._staticResource = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.StaticResource);
        return DependencyPropertyValueSource._staticResource;
      }
    }

    public static DependencyPropertyValueSource DynamicResource
    {
      get
      {
        if (DependencyPropertyValueSource._dynamicResource == null)
          DependencyPropertyValueSource._dynamicResource = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.DynamicResource);
        return DependencyPropertyValueSource._dynamicResource;
      }
    }

    public static DependencyPropertyValueSource CustomMarkupExtension
    {
      get
      {
        if (DependencyPropertyValueSource._customMarkupExtension == null)
          DependencyPropertyValueSource._customMarkupExtension = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.CustomMarkupExtension);
        return DependencyPropertyValueSource._customMarkupExtension;
      }
    }

    public static DependencyPropertyValueSource DefaultValue
    {
      get
      {
        if (DependencyPropertyValueSource._defaultValue == null)
          DependencyPropertyValueSource._defaultValue = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.DefaultValue);
        return DependencyPropertyValueSource._defaultValue;
      }
    }

    public static DependencyPropertyValueSource LocalValue
    {
      get
      {
        if (DependencyPropertyValueSource._localValue == null)
          DependencyPropertyValueSource._localValue = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.LocalValue);
        return DependencyPropertyValueSource._localValue;
      }
    }

    public static DependencyPropertyValueSource InheritedValue
    {
      get
      {
        if (DependencyPropertyValueSource._inheritedValue == null)
          DependencyPropertyValueSource._inheritedValue = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.InheritedValue);
        return DependencyPropertyValueSource._inheritedValue;
      }
    }

    public static DependencyPropertyValueSource Null
    {
      get
      {
        if (DependencyPropertyValueSource._null == null)
          DependencyPropertyValueSource._null = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.Null);
        return DependencyPropertyValueSource._null;
      }
    }

    public static DependencyPropertyValueSource Ambient
    {
      get
      {
        if (DependencyPropertyValueSource._ambient == null)
          DependencyPropertyValueSource._ambient = new DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource.Ambient);
        return DependencyPropertyValueSource._ambient;
      }
    }

    public bool IsBinding
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.Binding;
      }
    }

    public bool IsTemplateBinding
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.TemplateBinding;
      }
    }

    public bool IsStatic
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.Static;
      }
    }

    public bool IsStaticResource
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.StaticResource;
      }
    }

    public bool IsDynamicResource
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.DynamicResource;
      }
    }

    public bool IsCustomMarkupExtension
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.CustomMarkupExtension;
      }
    }

    public bool IsDefaultValue
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.DefaultValue;
      }
    }

    public bool IsLocalValue
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.LocalValue;
      }
    }

    public bool IsInheritedValue
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.InheritedValue;
      }
    }

    public bool IsResource
    {
      get
      {
        if (this._source != DependencyPropertyValueSource.ValueSource.DynamicResource)
          return this._source == DependencyPropertyValueSource.ValueSource.StaticResource;
        return true;
      }
    }

    public bool IsMarkupExtension
    {
      get
      {
        if (this._source != DependencyPropertyValueSource.ValueSource.Binding && this._source != DependencyPropertyValueSource.ValueSource.TemplateBinding && (this._source != DependencyPropertyValueSource.ValueSource.Static && this._source != DependencyPropertyValueSource.ValueSource.StaticResource) && (this._source != DependencyPropertyValueSource.ValueSource.DynamicResource && this._source != DependencyPropertyValueSource.ValueSource.CustomMarkupExtension))
          return this._source == DependencyPropertyValueSource.ValueSource.Null;
        return true;
      }
    }

    public bool IsNull
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.Null;
      }
    }

    public bool IsAmbient
    {
      get
      {
        return this._source == DependencyPropertyValueSource.ValueSource.Ambient;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use DependencyPropertyValueSource.Binding instead")]
    public static DependencyPropertyValueSource DataBound
    {
      get
      {
        return DependencyPropertyValueSource.Binding;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.Static instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DependencyPropertyValueSource SystemResource
    {
      get
      {
        return DependencyPropertyValueSource.Static;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use DependencyPropertyValueSource.StaticResource instead")]
    public static DependencyPropertyValueSource LocalStaticResource
    {
      get
      {
        return DependencyPropertyValueSource.StaticResource;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.DynamicResource instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DependencyPropertyValueSource LocalDynamicResource
    {
      get
      {
        return DependencyPropertyValueSource.DynamicResource;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.LocalValue instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DependencyPropertyValueSource Local
    {
      get
      {
        return DependencyPropertyValueSource.LocalValue;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use DependencyPropertyValueSource.InheritedValue instead")]
    public static DependencyPropertyValueSource Inherited
    {
      get
      {
        return DependencyPropertyValueSource.InheritedValue;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.IsMarkupExtension instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsExpression
    {
      get
      {
        return this.IsMarkupExtension;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use DependencyPropertyValueSource.IsBinding instead")]
    public bool IsDataBound
    {
      get
      {
        return this.IsBinding;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.IsStatic instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSystemResource
    {
      get
      {
        return this.IsStatic;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.IsResource instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsLocalResource
    {
      get
      {
        return this.IsResource;
      }
    }

    [Obsolete("Use DependencyPropertyValueSource.IsLocalValue instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsLocal
    {
      get
      {
        return this.IsLocalValue;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use DependencyPropertyValueSource.IsInheritedValue instead")]
    public bool IsInherited
    {
      get
      {
        return this.IsInheritedValue;
      }
    }

    private DependencyPropertyValueSource(DependencyPropertyValueSource.ValueSource source)
    {
      this._source = source;
    }

    private enum ValueSource
    {
      Binding,
      TemplateBinding,
      Static,
      StaticResource,
      DynamicResource,
      CustomMarkupExtension,
      LocalValue,
      DefaultValue,
      InheritedValue,
      Null,
      Ambient,
    }
  }
}
