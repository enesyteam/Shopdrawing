// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TriggerCondition
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class TriggerCondition
  {
    private SceneNode conditionNode;
    private IPropertyId propertyKey;
    private IPropertyId valueKey;
    private IPropertyId sourceNameKey;

    public string PresentationName
    {
      get
      {
        object obj = this.Value;
        bool flag = obj == null;
        if (!flag && Nullable.GetUnderlyingType(obj.GetType()) != (Type) null)
          flag = obj.Equals((object) null);
        string str = !flag ? obj.ToString() : "null";
        DependencyProperty propertyKey = this.PropertyKey;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} = {1}", new object[2]
        {
          (object) (propertyKey == null ? "(null)" : propertyKey.Name),
          (object) str
        });
      }
    }

    public SceneNode ConditionNode
    {
      get
      {
        return this.conditionNode;
      }
    }

    public DependencyProperty PropertyKey
    {
      get
      {
        return this.GetDependencyProperty();
      }
    }

    public object Value
    {
      get
      {
        return this.GetRequiredValue(this.valueKey);
      }
    }

    public string SourceName
    {
      get
      {
        return this.GetRequiredValue(this.sourceNameKey) as string;
      }
    }

    public TriggerCondition(SceneNode conditionNode, IPropertyId propertyKey, IPropertyId valueKey, IPropertyId sourceNameKey)
    {
      this.conditionNode = conditionNode;
      this.propertyKey = propertyKey;
      this.valueKey = valueKey;
      this.sourceNameKey = sourceNameKey;
    }

    private object GetRequiredValue(IPropertyId propertyKey)
    {
      return this.conditionNode.GetLocalValue(propertyKey);
    }

    private DependencyProperty GetDependencyProperty()
    {
      return this.GetRequiredValue(this.propertyKey) as DependencyProperty;
    }
  }
}
