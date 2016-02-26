// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.IMSBuildProject
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Project;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  [Prototype]
  public interface IMSBuildProject
  {
    string CodeLanguage { get; }

    IProject Project { get; }

    bool IsSafeIdentifier(string identifier);

    bool UpdateProject(IEnumerable<ProjectItemAction> actions);
  }
}
