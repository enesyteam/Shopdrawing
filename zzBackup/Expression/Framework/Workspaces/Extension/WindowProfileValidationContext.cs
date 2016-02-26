// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.WindowProfileValidationContext
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class WindowProfileValidationContext
  {
    private List<MainSite> mainSites;

    public List<MainSite> MainSites
    {
      get
      {
        if (this.mainSites == null)
          this.mainSites = new List<MainSite>();
        return this.mainSites;
      }
    }
  }
}
