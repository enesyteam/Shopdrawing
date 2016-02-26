// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.FallbackResourceManager
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Windows.Media;

namespace Microsoft.Expression.Utility.IO
{
  public class FallbackResourceManager : IResourceManager
  {
    private IResourceManager primaryResourceManager;
    private IResourceManager fallbackResourceManager;

    public DocumentReference Source
    {
      get
      {
        return this.primaryResourceManager.Source;
      }
    }

    private FallbackResourceManager(IResourceManager primaryResourceManager, IResourceManager fallbackResourceManager)
    {
      this.primaryResourceManager = primaryResourceManager;
      this.fallbackResourceManager = fallbackResourceManager;
    }

    public static FallbackResourceManager Create(IResourceManager primaryResourceManager, IResourceManager fallbackResourceManager)
    {
      if (primaryResourceManager == null)
        throw new ArgumentNullException("primaryResourceManager");
      if (fallbackResourceManager == null)
        throw new ArgumentNullException("fallbackResourceManager");
      return new FallbackResourceManager(primaryResourceManager, fallbackResourceManager);
    }

    public string GetStringResource(string identifier)
    {
      return this.primaryResourceManager.GetStringResource(identifier) ?? this.fallbackResourceManager.GetStringResource(identifier);
    }

    public ImageSource GetImageResource(string identifier, int preferredWidth, int preferredHeight)
    {
      return this.primaryResourceManager.GetImageResource(identifier, preferredWidth, preferredHeight) ?? this.fallbackResourceManager.GetImageResource(identifier, preferredWidth, preferredHeight);
    }
  }
}
