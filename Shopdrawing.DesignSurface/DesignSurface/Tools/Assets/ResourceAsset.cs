// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ResourceAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public abstract class ResourceAsset : Asset
  {
    private ResourceModel resourceModel;
    private string name;
    private string location;

    public ResourceModel ResourceModel
    {
      get
      {
        return this.resourceModel;
      }
    }

    public override string Name
    {
      get
      {
        this.UpdateNameAndLocation();
        return this.name;
      }
    }

    public override string Location
    {
      get
      {
        return this.location;
      }
    }

    protected ResourceAsset(ResourceModel resourceModel)
    {
      this.resourceModel = resourceModel;
      this.UpdateNameAndLocation();
    }

    public override int CompareTo(Asset other)
    {
      ResourceAsset resourceAsset = other as ResourceAsset;
      if (resourceAsset == null)
        return base.CompareTo(other);
      int num1 = string.CompareOrdinal(this.name, resourceAsset.name);
      if (num1 != 0)
        return num1;
      int num2 = string.CompareOrdinal(this.location, resourceAsset.location);
      if (num2 != 0)
        return num2;
      return 0;
    }

    private void UpdateNameAndLocation()
    {
      if (this.resourceModel.ResourceNode.DocumentRoot == null || this.resourceModel.ResourceNode.DocumentRoot.DocumentContext == null)
        return;
      this.name = this.resourceModel.Name;
      this.location = this.resourceModel.Location;
    }
  }
}
