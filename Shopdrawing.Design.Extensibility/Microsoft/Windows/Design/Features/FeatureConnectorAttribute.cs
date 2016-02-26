// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Features.FeatureConnectorAttribute
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using MS.Internal.Properties;
using System;
using System.Globalization;

namespace Microsoft.Windows.Design.Features
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class FeatureConnectorAttribute : Attribute
  {
    private Type _featureConnectorType;
    private EqualityArray _typeId;

    public Type FeatureConnectorType
    {
      get
      {
        return this._featureConnectorType;
      }
    }

    public override object TypeId
    {
      get
      {
        if (this._typeId == null)
          this._typeId = new EqualityArray(new object[2]
          {
            (object) typeof (FeatureConnectorAttribute),
            (object) this._featureConnectorType
          });
        return (object) this._typeId;
      }
    }

    public FeatureConnectorAttribute(Type featureConnectorType)
    {
      if (featureConnectorType == null)
        throw new ArgumentNullException("featureConnectorType");
      if (!typeof (IFeatureConnectorMarker).IsAssignableFrom(featureConnectorType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
          (object) "featureConnectorType",
          (object) typeof (FeatureConnector<FeatureProvider>).GetGenericTypeDefinition().Name
        }));
      this._featureConnectorType = featureConnectorType;
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      FeatureConnectorAttribute connectorAttribute = obj as FeatureConnectorAttribute;
      if (connectorAttribute != null)
        return connectorAttribute._featureConnectorType == this._featureConnectorType;
      return false;
    }

    public override int GetHashCode()
    {
      return this._featureConnectorType.GetHashCode();
    }
  }
}
