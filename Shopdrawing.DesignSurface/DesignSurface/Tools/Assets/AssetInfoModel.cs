// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetInfoModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Project;
using System;
using System.IO;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public abstract class AssetInfoModel
  {
    public string Location { get; private set; }

    public string FullPath { get; private set; }

    public static AssetInfoModel Empty
    {
      get
      {
        return (AssetInfoModel) new AssetInfoModel.EmptyAssetInfoModel();
      }
    }

    protected void SetFullPath(string fullPath)
    {
      this.FullPath = fullPath;
      this.Location = Path.GetFileName(fullPath);
    }

    public static string AssemblyLocation(IType type, IProject project)
    {
      ProjectAssembly projectAssembly = project.ReferencedAssemblies.Find(type.RuntimeAssembly.Name);
      if (projectAssembly != null && projectAssembly.ProjectItem != null)
        return projectAssembly.ProjectItem.DocumentReference.Path;
      if (projectAssembly == project.TargetAssembly)
        return project.FullTargetPath;
      IUnreferencedType unreferencedType;
      if ((unreferencedType = type as IUnreferencedType) != null)
        return unreferencedType.AssemblyLocation;
      return type.RuntimeType.Assembly.Location;
    }

    public static string GetDescription(IType type)
    {
      IUnreferencedType unreferencedType = type as IUnreferencedType;
      if (unreferencedType == null)
        return AssetInfoModel.GetDescription(type.RuntimeType);
      return unreferencedType.Description;
    }

    public static string GetDescription(Type type)
    {
      string result = (string) null;
      if (type != (Type) null)
        PlatformNeutralAttributeHelper.TryGetAttributeValue<string>(type, PlatformTypes.DescriptionAttribute, "Description", out result);
      return result;
    }

    private class EmptyAssetInfoModel : AssetInfoModel
    {
    }
  }
}
