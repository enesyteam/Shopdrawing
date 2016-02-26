// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.StyleResourceItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class StyleResourceItem : TypedResourceItem
  {
    public override IType EffectiveTypeId
    {
      get
      {
        return this.DocumentNode.TypeResolver.ResolveType(PlatformTypes.Style);
      }
    }

    protected override IPropertyId TargetTypeProperty
    {
      get
      {
        return StyleNode.TargetTypeProperty;
      }
    }

    public override string Key
    {
      get
      {
        return this.GetKeyOrFormattedTypeName(this.Type != null ? this.Type.Name : (string) null);
      }
      set
      {
        base.Key = value;
      }
    }

    public override object ToolTip
    {
      get
      {
        return (object) new DocumentationEntry((string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ResourceItemFormat, new object[1]
        {
          (object) this.Resource.TargetType.Name
        }), (Type) null, this.GetKeyOrFormattedTypeName(this.Type != null ? this.Type.FullName : (string) null));
      }
    }

    public StyleResourceItem(ResourceManager resourceManager, ResourceContainer resourceContainer, ResourceModel resource)
      : base(resourceManager, resourceContainer, resource)
    {
    }
  }
}
