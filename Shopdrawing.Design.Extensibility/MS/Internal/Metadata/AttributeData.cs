// Decompiled with JetBrains decompiler
// Type: MS.Internal.Metadata.AttributeData
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using System;

namespace MS.Internal.Metadata
{
  internal class AttributeData
  {
    private Type _attributeType;
    private bool? _isInheritable;
    private bool? _allowsMultiple;

    internal Type AttributeType
    {
      get
      {
        return this._attributeType;
      }
    }

    internal bool AllowsMultiple
    {
      get
      {
        if (!this._allowsMultiple.HasValue)
          this.ParseUsageAttributes();
        return this._allowsMultiple.Value;
      }
    }

    internal bool IsInheritable
    {
      get
      {
        if (!this._isInheritable.HasValue)
          this.ParseUsageAttributes();
        return this._isInheritable.Value;
      }
    }

    internal AttributeData(Type attributeType)
    {
      this._attributeType = attributeType;
    }

    private void ParseUsageAttributes()
    {
      this._isInheritable = new bool?(false);
      this._allowsMultiple = new bool?(false);
      object[] customAttributes = this._attributeType.GetCustomAttributes(typeof (AttributeUsageAttribute), true);
      if (customAttributes == null || customAttributes.Length <= 0)
        return;
      for (int index = 0; index < customAttributes.Length; ++index)
      {
        AttributeUsageAttribute attributeUsageAttribute = (AttributeUsageAttribute) customAttributes[index];
        this._isInheritable = new bool?(attributeUsageAttribute.Inherited);
        this._allowsMultiple = new bool?(attributeUsageAttribute.AllowMultiple);
      }
    }
  }
}
