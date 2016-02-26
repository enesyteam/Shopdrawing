// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.ExpressionViewCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  public class ExpressionViewCustomSerializer : ViewCustomSerializer
  {
    protected override IEnumerable<DependencyProperty> SerializableProperties
    {
      get
      {
        yield return ExpressionView.IsDesiredVisibleProperty;
        yield return ExpressionView.WasSelectedBeforeAutoHideProperty;
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

    public ExpressionViewCustomSerializer(ExpressionView view)
      : base((View) view)
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

    internal bool ShouldSerializeProperty(DependencyProperty property)
    {
      return property != View.TitleProperty;
    }
  }
}
