// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Features.FeatureAttribute
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using MS.Internal.Properties;
using System;
using System.Globalization;

namespace Microsoft.Windows.Design.Features
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
  public sealed class FeatureAttribute : Attribute
  {
    private Type _featureProviderType;
    private EqualityArray _typeId;

    public Type FeatureProviderType
    {
      get
      {
        return this._featureProviderType;
      }
    }

    public override object TypeId
    {
      get
      {
        if (this._typeId == null)
          this._typeId = new EqualityArray(new object[2]
          {
            (object) typeof (FeatureProvider),
            (object) this._featureProviderType
          });
        return (object) this._typeId;
      }
    }

    public FeatureAttribute(Type featureProviderType)
    {
      if (featureProviderType == null)
        throw new ArgumentNullException("featureProviderType");
      if (!typeof (FeatureProvider).IsAssignableFrom(featureProviderType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
          (object) "featureProviderType",
          (object) typeof (FeatureProvider).Name
        }));
      this._featureProviderType = featureProviderType;
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      FeatureAttribute featureAttribute = obj as FeatureAttribute;
      if (featureAttribute != null)
        return featureAttribute._featureProviderType == this._featureProviderType;
      return false;
    }

    public override int GetHashCode()
    {
      return this._featureProviderType.GetHashCode();
    }
  }
}
