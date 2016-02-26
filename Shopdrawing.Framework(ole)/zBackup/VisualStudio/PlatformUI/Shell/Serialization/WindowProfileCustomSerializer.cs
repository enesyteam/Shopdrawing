// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.WindowProfileCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  internal class WindowProfileCustomSerializer : DependencyObjectCustomSerializer
  {
    protected override IEnumerable<DependencyProperty> SerializableProperties
    {
      get
      {
        yield return WindowProfile.NameProperty;
      }
    }

    public override object Content
    {
      get
      {
        return (object) ((WindowProfile) this.Owner).Children;
      }
    }

    public WindowProfileCustomSerializer(WindowProfile profile)
      : base((IDependencyObjectCustomSerializerAccess) profile)
    {
    }
  }
}
