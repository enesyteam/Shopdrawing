// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ToolGroupItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.Framework.Globalization;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class ToolGroupItem : INotifyPropertyChanged
  {
    private Tool tool;

    public Tool Tool
    {
      get
      {
        return this.tool;
      }
    }

    public bool IsVisible
    {
      get
      {
        return this.tool.IsVisible;
      }
    }

    public DrawingBrush NormalIconBrush
    {
      get
      {
        return this.tool.NormalIconBrush;
      }
    }

    public DrawingBrush HoverIconBrush
    {
      get
      {
        return this.tool.HoverIconBrush;
      }
    }

    public string Description
    {
      get
      {
        return this.tool.Caption;
      }
    }

    public string AutomationId
    {
      get
      {
        return this.tool.Identifier;
      }
    }

    public string ToolTipCaption
    {
      get
      {
        Key key = this.tool.Key;
        if (key == Key.None)
          key = this.tool.ToolContext.ToolManager.GetToolCategoryKey(this.tool.Category);
        if (key == Key.None)
          return this.tool.Caption;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ToolTooltipFormatString, new object[2]
        {
          (object) this.tool.Caption,
          (object) CultureManager.GetKeyText(key)
        });
      }
    }

    public string ToolTipDescription
    {
      get
      {
        return this.tool.Description;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return this.tool.IsEnabled;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ToolGroupItem(Tool tool)
    {
      this.tool = tool;
      AssetTool assetTool = tool as AssetTool;
      if (assetTool == null)
        return;
      assetTool.Asset.PropertyChanged += new PropertyChangedEventHandler(this.Asset_PropertyChanged);
    }

    public void DoubleClick()
    {
      this.tool.DoubleClick();
    }

    public ContextMenu CreateContextMenu()
    {
      return this.tool.CreateContextMenu();
    }

    public void FireIsEnabledChanged()
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs("IsEnabled"));
    }

    private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsValid"))
        return;
      this.FireIsEnabledChanged();
    }
  }
}
