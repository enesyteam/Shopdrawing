// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SystemResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class SystemResourceModel : BaseResourceModel
  {
    private string resourceName;
    private string collectionName;

    public string CollectionName
    {
      get
      {
        return this.collectionName;
      }
      set
      {
        this.collectionName = value;
      }
    }

    public override string ResourceName
    {
      get
      {
        return this.resourceName;
      }
    }

    public string MenuHeader
    {
      get
      {
        return this.ToString();
      }
    }

    public SystemResourceModel(string collectionName, string name, Type type, object value)
      : base(type, value)
    {
      this.collectionName = collectionName;
      this.resourceName = name;
    }

    public override string ToString()
    {
      return this.collectionName + "." + this.resourceName;
    }
  }
}
