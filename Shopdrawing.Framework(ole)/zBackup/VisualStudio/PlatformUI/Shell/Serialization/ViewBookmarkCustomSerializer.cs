// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.ViewBookmarkCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  internal class ViewBookmarkCustomSerializer : ViewElementCustomSerializer
  {
    protected override IEnumerable<DependencyProperty> SerializableProperties
    {
      get
      {
        yield return ViewBookmark.NameProperty;
        yield return ViewBookmark.AccessOrderProperty;
        yield return ViewBookmark.ViewBookmarkTypeProperty;
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

    public ViewBookmarkCustomSerializer(ViewBookmark mark)
      : base((ViewElement) mark)
    {
    }
  }
}
