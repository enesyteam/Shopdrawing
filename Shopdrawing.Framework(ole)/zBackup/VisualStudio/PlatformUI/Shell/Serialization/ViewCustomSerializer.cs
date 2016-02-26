// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.ViewCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  public class ViewCustomSerializer : ViewElementCustomSerializer
  {
    protected override IEnumerable<DependencyProperty> SerializableProperties
    {
      get
      {
        yield return View.NameProperty;
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

    public ViewCustomSerializer(View view)
      : base((ViewElement) view)
    {
    }

    public override IEnumerable<KeyValuePair<string, object>> GetNonContentPropertyValues()
    {
      object title = (object) null;
      if (this.ShouldSerializeProperty(View.TitleProperty, out title))
        yield return new KeyValuePair<string, object>(View.TitleProperty.Name, title);
      foreach (KeyValuePair<string, object> keyValuePair in this.BaseGetNonContentPropertyValues())
        yield return keyValuePair;
    }

    private IEnumerable<KeyValuePair<string, object>> BaseGetNonContentPropertyValues()
    {
      return base.GetNonContentPropertyValues();
    }
  }
}
