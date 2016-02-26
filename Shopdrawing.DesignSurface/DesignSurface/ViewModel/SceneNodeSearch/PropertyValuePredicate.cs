// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.PropertyValuePredicate
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class PropertyValuePredicate : ISearchPredicate
  {
    private IPropertyId propertyKey;
    private object testValue;

    public SearchScope AnalysisScope
    {
      get
      {
        return SearchScope.NodeTreeDescendant;
      }
    }

    public PropertyValuePredicate(IPropertyId propertyKey)
      : this(propertyKey, (object) null)
    {
    }

    public PropertyValuePredicate(IPropertyId propertyKey, object testValue)
    {
      this.propertyKey = propertyKey;
      this.testValue = testValue;
    }

    public bool Test(SceneNode subject)
    {
      if (subject.DocumentNode is DocumentPrimitiveNode || subject.IsSet(this.propertyKey) != PropertyState.Set)
        return false;
      if (this.testValue == null)
        return true;
      return this.testValue.Equals(subject.GetLocalValue(this.propertyKey));
    }
  }
}
