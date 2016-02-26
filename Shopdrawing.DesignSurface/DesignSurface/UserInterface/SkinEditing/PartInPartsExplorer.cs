// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.PartInPartsExplorer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class PartInPartsExplorer : NotifyingObject
  {
    private string name;
    private ITypeId targetType;
    private PartStatus status;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string TypeName
    {
      get
      {
        return this.targetType.Name;
      }
    }

    public string Label
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.PartsPanePartLabelFormat, new object[2]
        {
          (object) this.Name,
          (object) this.TypeName
        });
      }
    }

    public ITypeId TargetType
    {
      get
      {
        return this.targetType;
      }
    }

    public PartStatus Status
    {
      get
      {
        return this.status;
      }
      set
      {
        if (value == this.status)
          return;
        this.status = value;
        this.OnPropertyChanged("Status");
      }
    }

    public PartInPartsExplorer(string name, ITypeId type)
    {
      this.name = name;
      this.status = PartStatus.Unused;
      this.targetType = type;
    }
  }
}
