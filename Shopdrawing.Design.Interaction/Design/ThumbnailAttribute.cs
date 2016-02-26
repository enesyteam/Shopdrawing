// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.ThumbnailAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Reflection;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class ThumbnailAttribute : Attribute
  {
    private string _resourceName;
    private Assembly _resourceAssembly;

    public Assembly ResourceAssembly
    {
      get
      {
        return this._resourceAssembly;
      }
    }

    public string ResourceName
    {
      get
      {
        return this._resourceName;
      }
    }

    public ThumbnailAttribute(Type resourceAssemblyType, string resourceName)
    {
      if (resourceAssemblyType == null)
        throw new ArgumentNullException("resourceAssemblyType");
      if (string.IsNullOrEmpty(resourceName))
        throw new ArgumentNullException("resourceName");
      this._resourceAssembly = resourceAssemblyType.Assembly;
      this._resourceName = resourceName;
    }
  }
}
