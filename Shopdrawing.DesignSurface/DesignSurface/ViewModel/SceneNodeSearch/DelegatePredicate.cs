// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.DelegatePredicate
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class DelegatePredicate : ISearchPredicate
  {
    private Predicate<SceneNode> tester;
    private SearchScope testerScope;

    public SearchScope AnalysisScope
    {
      get
      {
        return this.testerScope;
      }
    }

    public DelegatePredicate(Predicate<SceneNode> tester, SearchScope scope)
    {
      this.tester = tester;
      this.testerScope = scope;
    }

    public bool Test(SceneNode subject)
    {
      return this.tester(subject);
    }
  }
}
