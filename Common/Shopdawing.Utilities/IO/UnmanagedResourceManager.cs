// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.UnmanagedResourceManager
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Interop;
using System.Windows.Media;

namespace Microsoft.Expression.Utility.IO
{
  public class UnmanagedResourceManager : IResourceManager
  {
    private SafeModuleHandle module;

    public DocumentReference Source { get; private set; }

    private UnmanagedResourceManager(DocumentReference source, SafeModuleHandle module)
    {
      this.Source = source;
      this.module = module;
    }

    public static UnmanagedResourceManager Create(DocumentReference source)
    {
      if (!PathHelper.FileExists(source.Path))
        return (UnmanagedResourceManager) null;
      SafeModuleHandle module = NativeMethods.LoadLibraryForResourceAccess(source.Path);
      if (module == null)
        return (UnmanagedResourceManager) null;
      return new UnmanagedResourceManager(source, module);
    }

    public string GetStringResource(string identifier)
    {
      uint result;
      if (!uint.TryParse(identifier, out result))
        return (string) null;
      return NativeMethods.LoadStringResource(this.module, result);
    }

    public ImageSource GetImageResource(string identifier, int preferredWidth, int preferredHeight)
    {
      return ResourceHelper.LoadIcon(this.module, "#" + identifier, preferredWidth, preferredHeight);
    }
  }
}
