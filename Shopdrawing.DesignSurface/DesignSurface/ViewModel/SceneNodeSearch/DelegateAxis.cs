// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.DelegateAxis
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class DelegateAxis : SearchAxis
  {
    private SearchScope scope;
    private DelegateAxis.EnumerationHandler enumerationHandler;

    public override SearchScope Scope
    {
      get
      {
        return this.scope;
      }
    }

    public DelegateAxis(DelegateAxis.EnumerationHandler enumerationHandler, SearchScope scope)
      : base(SearchAxis.AxisType.Custom)
    {
      this.enumerationHandler = enumerationHandler;
      this.scope = scope;
    }

    public override IEnumerable<SceneNode> Enumerate(SceneNode pivot, ISearchPredicate continueTester)
    {
      return this.enumerationHandler(pivot);
    }

    public delegate IEnumerable<SceneNode> EnumerationHandler(SceneNode pivot);
  }
}
