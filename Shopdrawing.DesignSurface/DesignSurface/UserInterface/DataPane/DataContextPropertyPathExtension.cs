// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextPropertyPathExtension
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataContextPropertyPathExtension
  {
    public static DataContextPropertyPathExtension Invalid
    {
      get
      {
        return new DataContextPropertyPathExtension();
      }
    }

    public IProperty Property { get; private set; }

    public bool IsCollectionItem { get; private set; }

    public bool IsValid { get; private set; }

    public DataContextPropertyPathExtension(IProperty property, bool isCollectionItem)
    {
      this.Property = property;
      this.IsCollectionItem = isCollectionItem;
      this.IsValid = true;
    }

    private DataContextPropertyPathExtension()
    {
      this.IsValid = false;
    }
  }
}
