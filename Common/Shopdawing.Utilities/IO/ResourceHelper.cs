// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ResourceHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Interop;
using System.Windows.Media;

namespace Microsoft.Expression.Utility.IO
{
  public static class ResourceHelper
  {
    public static ImageSource LoadIcon(SystemIcon systemIcon)
    {
      return NativeMethods.LoadIcon(systemIcon);
    }

    public static ImageSource LoadIcon(SafeModuleHandle module, string resourceIdentifier, int preferredWidth = 0, int preferredHeight = 0)
    {
      return NativeMethods.LoadIcon(module, resourceIdentifier, preferredWidth, preferredHeight);
    }
  }
}
