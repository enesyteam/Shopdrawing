// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ViewOptionsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.View
{
  public class ViewOptionsModel : INotifyPropertyChanged
  {
    private const ViewMode DefaultViewMode = ViewMode.Design;
    private const bool DefaultIsVerticalSplit = true;
    private const bool DefaultIsDesignOnTop = true;
    private const double DefaultSplitRatio = 0.8;
    private IConfigurationObject configurationObject;
    private bool saveOnPropertyChanged;
    private bool isInitializing;
    private ViewMode viewMode;
    private bool isVerticalSplit;
    private bool isDesignOnTop;
    private double splitRatio;

    public ViewMode ViewMode
    {
      get
      {
        return this.viewMode;
      }
      set
      {
        this.SetProperty<ViewMode>("ViewMode", ref this.viewMode, value);
      }
    }

    public bool IsVerticalSplit
    {
      get
      {
        return this.isVerticalSplit;
      }
      set
      {
        this.SetProperty<bool>("IsVerticalSplit", ref this.isVerticalSplit, value);
      }
    }

    public bool IsDesignOnTop
    {
      get
      {
        return this.isDesignOnTop;
      }
      set
      {
        this.SetProperty<bool>("IsDesignOnTop", ref this.isDesignOnTop, value);
      }
    }

    public double SplitRatio
    {
      get
      {
        return this.splitRatio;
      }
      set
      {
        this.SetProperty<double>("SplitRatio", ref this.splitRatio, value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ViewOptionsModel(bool saveOnPropertyChanged)
    {
      this.saveOnPropertyChanged = saveOnPropertyChanged;
    }

    public void Load(IConfigurationObject value)
    {
      this.configurationObject = value;
      this.isInitializing = true;
      this.ViewMode = (ViewMode) value.GetProperty("ViewMode", (object) ViewMode.Design);
      this.IsVerticalSplit = (bool) value.GetProperty("IsVerticalSplit", (object) true);
      this.IsDesignOnTop = (bool) value.GetProperty("IsDesignOnTop", (object) true);
      this.SplitRatio = (double) value.GetProperty("SplitRatio", (object) 0.8);
      this.isInitializing = false;
    }

    public void Save(IConfigurationObject value)
    {
      value.SetProperty("ViewMode", (object) this.ViewMode, (object) ViewMode.Design);
      value.SetProperty("IsVerticalSplit", (object) (bool) (this.IsVerticalSplit ? true : false), (object) true);
      value.SetProperty("IsDesignOnTop", (object) (bool) (this.IsDesignOnTop ? true : false), (object) true);
      value.SetProperty("SplitRatio", (object) this.SplitRatio, (object) 0.8);
    }

    private void SetProperty<T>(string propertyName, ref T propertyStorage, T value)
    {
      if (object.Equals((object) propertyStorage, (object) value))
        return;
      propertyStorage = value;
      if (!this.isInitializing && this.saveOnPropertyChanged && this.configurationObject != null)
        this.Save(this.configurationObject);
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
