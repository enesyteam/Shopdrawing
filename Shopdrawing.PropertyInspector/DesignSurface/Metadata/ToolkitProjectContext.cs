// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.ToolkitProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public class ToolkitProjectContext : CustomAssemblyReferencesProjectContext
  {
    public ToolkitProjectContext(IProjectContext sourceContext)
      : base(sourceContext, (ICollection<IAssembly>) null)
    {
      this.CustomAssemblyReferences = (ICollection<IAssembly>) new List<IAssembly>();
      if (!sourceContext.PlatformMetadata.IsNullType((ITypeId) sourceContext.ResolveType(ProjectNeutralTypes.VisualStateManager)) || !sourceContext.IsCapabilitySet(PlatformCapability.VsmInToolkit))
        return;
      if (!sourceContext.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager))
        return;
      try
      {
        Assembly assembly = ProjectAssemblyHelper.LoadFile(ToolkitHelper.GetToolkitPath());
        this.CustomAssemblyReferences = (ICollection<IAssembly>) new List<IAssembly>(1)
        {
          sourceContext.Platform.Metadata.CreateAssembly(assembly, AssemblySource.PlatformExtension)
        };
      }
      catch (Exception ex)
      {
      }
    }
  }
}
