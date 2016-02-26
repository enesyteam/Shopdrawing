// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.UnderlineStickyButtonConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.NodeBuilders;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class UnderlineStickyButtonConverter : NinchedStickyButtonConverter
  {
    public object Underline { get; set; }

    public override object TrueValue
    {
      get
      {
        return this.Underline;
      }
    }

    public override object FalseValue
    {
      get
      {
        return (object) null;
      }
    }

    protected override bool AreEqual(object value, object referenceValue)
    {
      if (object.Equals(value, referenceValue))
        return true;
      TextDecorationCollection lhsCollection = value as TextDecorationCollection;
      TextDecorationCollection rhsCollection = referenceValue as TextDecorationCollection;
      if (lhsCollection != null || rhsCollection != null)
        return TextDecorationCollectionHelper.AreEqual(lhsCollection, rhsCollection);
      return false;
    }
  }
}
