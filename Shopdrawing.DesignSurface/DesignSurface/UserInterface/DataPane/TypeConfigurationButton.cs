// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.TypeConfigurationButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class TypeConfigurationButton : DataConfigurationButton
  {
    private static readonly string IconUriBooleanOn = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_bool_on_16x16.png";
    private static readonly string IconUriBooleanOff = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_bool_off_16x16.png";
    private static readonly string IconUriImageOn = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_image_on_16x16.png";
    private static readonly string IconUriImageOff = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_image_off_16x16.png";
    private static readonly string IconUriNumberOn = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_number_on_16x16.png";
    private static readonly string IconUriNumberOff = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_number_off_16x16.png";
    private static readonly string IconUriStringOn = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_string_on_16x16.png";
    private static readonly string IconUriStringOff = "/Microsoft.Expression.DesignSurface;Component/Resources/Icons/Data/data_string_off_16x16.png";
    private static Dictionary<SampleBasicType, ImageSource> iconResourcesMouseOn;
    private static Dictionary<SampleBasicType, ImageSource> iconResourcesMouseOff;

    public bool IsDataSetContextSampleData
    {
      get
      {
        return this.DataSchemaItem.IsDataSetContextSampleData;
      }
    }

    public bool HasNoErrors
    {
      get
      {
        return this.DataSchemaItem.HasNoErrors;
      }
    }

    public ImageSource ButtonTypeIcon
    {
      get
      {
        return this.GetIconResource(this.DataSchemaItem.SampleType as SampleBasicType, false);
      }
    }

    public ImageSource ButtonTypeIconMouseOver
    {
      get
      {
        return this.GetIconResource(this.DataSchemaItem.SampleType as SampleBasicType, true);
      }
    }

    protected override bool IsEnabledCore
    {
      get
      {
        if (this.DataSchemaItem.HasNoErrors)
          return base.IsEnabledCore;
        return false;
      }
    }

    public override DataConfigurationPopup CreatePopup(DataSchemaItem dataSchemaItem)
    {
      return (DataConfigurationPopup) new SampleDataConfigurationPopup(dataSchemaItem);
    }

    private ImageSource GetIconResource(SampleBasicType type, bool mouseOver)
    {
      if (type == null)
        return (ImageSource) null;
      if (TypeConfigurationButton.iconResourcesMouseOn == null)
      {
        TypeConfigurationButton.iconResourcesMouseOn = new Dictionary<SampleBasicType, ImageSource>();
        TypeConfigurationButton.iconResourcesMouseOn.Add(SampleBasicType.Boolean, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriBooleanOn));
        TypeConfigurationButton.iconResourcesMouseOn.Add(SampleBasicType.Image, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriImageOn));
        TypeConfigurationButton.iconResourcesMouseOn.Add(SampleBasicType.Number, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriNumberOn));
        TypeConfigurationButton.iconResourcesMouseOn.Add(SampleBasicType.String, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriStringOn));
      }
      if (TypeConfigurationButton.iconResourcesMouseOff == null)
      {
        TypeConfigurationButton.iconResourcesMouseOff = new Dictionary<SampleBasicType, ImageSource>();
        TypeConfigurationButton.iconResourcesMouseOff.Add(SampleBasicType.Boolean, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriBooleanOff));
        TypeConfigurationButton.iconResourcesMouseOff.Add(SampleBasicType.Image, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriImageOff));
        TypeConfigurationButton.iconResourcesMouseOff.Add(SampleBasicType.Number, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriNumberOff));
        TypeConfigurationButton.iconResourcesMouseOff.Add(SampleBasicType.String, this.GenerateImageSourceFromPath(TypeConfigurationButton.IconUriStringOff));
      }
      return (mouseOver ? TypeConfigurationButton.iconResourcesMouseOn : TypeConfigurationButton.iconResourcesMouseOff)[type];
    }

    private ImageSource GenerateImageSourceFromPath(string path)
    {
      return (ImageSource) new BitmapImage(new Uri(path, UriKind.Relative));
    }
  }
}
