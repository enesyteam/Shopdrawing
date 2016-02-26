// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ManagedResourceManager
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility;
using Microsoft.Expression.Utility.Globalization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Utility.IO
{
  public class ManagedResourceManager : IResourceManager
  {
    private ResourceSet resourceSet;

    public DocumentReference Source { get; private set; }

    private ManagedResourceManager(DocumentReference source, ResourceSet resourceSet)
    {
      this.Source = source;
      this.resourceSet = resourceSet;
    }

    public static IResourceManager Create(DocumentReference source, string baseName)
    {
      if (!PathHelper.FileExists(source.Path))
        return (IResourceManager) null;
      Assembly assembly = AssemblyHelper.LoadFrom(source.Path);
      if (assembly == (Assembly) null)
        return (IResourceManager) null;
      ResourceManager resourceManager;
      try
      {
        resourceManager = new ResourceManager(baseName, assembly);
      }
      catch (MissingManifestResourceException ex)
      {
        return (IResourceManager) null;
      }
      foreach (CultureInfo culture in CultureManager.PreferredCulturesExtended)
      {
        try
        {
          ResourceSet resourceSet = resourceManager.GetResourceSet(culture, true, true);
          if (resourceSet != null)
            return (IResourceManager) new ManagedResourceManager(source, resourceSet);
        }
        catch (MissingManifestResourceException ex)
        {
        }
      }
      return (IResourceManager) null;
    }

    public string GetStringResource(string identifier)
    {
      if (this.resourceSet == null)
        return (string) null;
      try
      {
        return this.resourceSet.GetString(identifier);
      }
      catch (MissingManifestResourceException ex)
      {
        return (string) null;
      }
    }

    public ImageSource GetImageResource(string identifier, int preferredWidth, int preferredHeight)
    {
      if (this.resourceSet == null)
        return (ImageSource) null;
      object @object;
      try
      {
        @object = this.resourceSet.GetObject(identifier);
      }
      catch (MissingManifestResourceException ex)
      {
        return (ImageSource) null;
      }
      Icon icon = @object as Icon;
      if (icon == null)
        return (ImageSource) null;
      if (preferredHeight != 0 && preferredWidth != 0)
        icon = new Icon(icon, preferredWidth, preferredHeight);
      return this.CreateImageSourceFromIcon(icon);
    }

    private ImageSource CreateImageSourceFromIcon(Icon icon)
    {
      return (ImageSource) Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
  }
}
