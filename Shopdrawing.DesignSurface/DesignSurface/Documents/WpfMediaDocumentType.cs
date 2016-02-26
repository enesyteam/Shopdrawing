// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.WpfMediaDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class WpfMediaDocumentType : MediaDocumentType
  {
    public override string[] FileExtensions
    {
      get
      {
        return new string[21]
        {
          ".dvr-ms",
          ".mid",
          ".rmi",
          ".midi",
          ".mpeg",
          ".mpg",
          ".m1v",
          ".mp2",
          ".mpa",
          ".mpe",
          ".ifo",
          ".vob",
          ".wav",
          ".snd",
          ".au",
          ".aif",
          ".aifc",
          ".aiff",
          ".wm",
          ".wmd",
          ".avi"
        };
      }
    }

    public override string Name
    {
      get
      {
        return "WpfMedia";
      }
    }

    public WpfMediaDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    public override bool CanAddToProject(IProject project)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      if (projectContext == null || !projectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return false;
      return base.CanAddToProject(project);
    }
  }
}
