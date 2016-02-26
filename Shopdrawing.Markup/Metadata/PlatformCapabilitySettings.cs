// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.PlatformCapabilitySettings
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public class PlatformCapabilitySettings
  {
    public FrameworkName RequiredTargetFramework { get; private set; }

    public Version MaxFrameworkVersion { get; private set; }

    public IDictionary<PlatformCapability, object> Capabilities { get; private set; }

    public PlatformCapabilitySettings(FrameworkName requiredTargetFramework, Version maxFrameworkVersion)
    {
      this.RequiredTargetFramework = requiredTargetFramework;
      this.MaxFrameworkVersion = maxFrameworkVersion;
      this.Capabilities = (IDictionary<PlatformCapability, object>) new Dictionary<PlatformCapability, object>();
    }

    public void AddCapability(PlatformCapability capability, object value)
    {
      this.Capabilities.Add(capability, value);
    }
  }
}
