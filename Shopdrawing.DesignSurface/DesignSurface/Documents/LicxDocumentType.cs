// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.LicxDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal sealed class LicxDocumentType : DocumentType
  {
    private const string BuildTask = "EmbeddedResource";

    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Licx.png";
      }
    }

    public override string Name
    {
      get
      {
        return "LicX";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.LicxDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return "License";
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".licx"
        };
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "EmbeddedResource";
      }
    }

    public override bool CanAddToProject(IProject project)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      if (projectContext == null || !projectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return false;
      return base.CanAddToProject(project);
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}
