// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.BooleanAndPredicate
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class BooleanAndPredicate : ISearchPredicate
  {
    private ISearchPredicate lhs;
    private ISearchPredicate rhs;

    public SearchScope AnalysisScope
    {
      get
      {
        return this.lhs.AnalysisScope | this.rhs.AnalysisScope;
      }
    }

    public BooleanAndPredicate(ISearchPredicate lhs, ISearchPredicate rhs)
    {
      this.lhs = lhs;
      this.rhs = rhs;
    }

    public bool Test(SceneNode subject)
    {
      if (this.lhs.Test(subject))
        return this.rhs.Test(subject);
      return false;
    }
  }
}
