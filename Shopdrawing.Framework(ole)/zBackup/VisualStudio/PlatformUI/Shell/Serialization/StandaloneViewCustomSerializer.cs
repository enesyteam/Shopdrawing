// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.StandaloneViewCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Workspaces.Extension;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  public class StandaloneViewCustomSerializer : ExpressionViewCustomSerializer
  {
    protected override IEnumerable<DependencyProperty> SerializableProperties
    {
      get
      {
        foreach (DependencyProperty dependencyProperty in this.BaseSerializableProperties)
          yield return dependencyProperty;
      }
    }

    private IEnumerable<DependencyProperty> BaseSerializableProperties
    {
      get
      {
        return base.SerializableProperties;
      }
    }

    public StandaloneViewCustomSerializer(StandaloneView view)
      : base((ExpressionView) view)
    {
    }

    public override IEnumerable<KeyValuePair<string, object>> GetNonContentPropertyValues()
    {
      foreach (KeyValuePair<string, object> keyValuePair in this.BaseGetNonContentPropertyValues())
        yield return keyValuePair;
    }

    private IEnumerable<KeyValuePair<string, object>> BaseGetNonContentPropertyValues()
    {
      return base.GetNonContentPropertyValues();
    }
  }
}
