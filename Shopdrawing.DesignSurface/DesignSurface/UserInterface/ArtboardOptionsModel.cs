// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ArtboardOptionsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class ArtboardOptionsModel : INotifyPropertyChanged
  {
    private const double gridSpacingMin = 0.01;
    private const double gridSpacingMax = 100.0;
    private const double gridSpacingDefault = 8.0;
    private const double snapLineMarginMin = 0.0;
    private const double snapLineMarginMax = 100.0;
    private const double snapLineMarginDefault = 4.0;
    private const double snapLinePaddingMin = 0.0;
    private const double snapLinePaddingMax = 100.0;
    private const double snapLinePaddingDefault = 8.0;
    private const double effectsEnabledZoomThresholdDefault = 4.0;
    private IConfigurationObject configurationObject;
    private bool saveOnPropertyChanged;
    private ArtboardZoomGesture artboardZoomGesture;
    private double effectsEnabledZoomThreshold;
    private bool effectsEnabled;
    private bool isInitializing;
    private bool snapToGrid;
    private double gridSpacing;
    private bool showGrid;
    private bool snapToSnapLines;
    private double snapLineMargin;
    private double snapLinePadding;
    private bool isInGridDesignMode;
    private bool isUsingCustomBackgroundColor;
    private Color customBackgroundColor;

    public PresetCollection ZoomComboCollection
    {
      get
      {
        return SceneScrollViewer.zoomComboCollection;
      }
    }

    public bool EffectsEnabled
    {
      get
      {
        return this.effectsEnabled;
      }
      set
      {
        this.SetProperty<bool>("EffectsEnabled", ref this.effectsEnabled, value);
      }
    }

    public double EffectsEnabledZoomThreshold
    {
      get
      {
        return this.effectsEnabledZoomThreshold;
      }
      set
      {
        this.SetProperty<double>("EffectsEnabledZoomThreshold", ref this.effectsEnabledZoomThreshold, value);
      }
    }

    public ArtboardZoomGesture ArtboardZoomGesture
    {
      get
      {
        return this.artboardZoomGesture;
      }
      set
      {
        this.SetProperty<ArtboardZoomGesture>("ArtboardZoomGesture", ref this.artboardZoomGesture, value);
      }
    }

    public Array ArtboardZoomGestureValues
    {
      get
      {
        return Enum.GetValues(typeof (ArtboardZoomGesture));
      }
    }

    public bool SnapToGrid
    {
      get
      {
        return this.snapToGrid;
      }
      set
      {
        this.SetProperty<bool>("SnapToGrid", ref this.snapToGrid, value);
      }
    }

    public double GridSpacing
    {
      get
      {
        return this.gridSpacing;
      }
      set
      {
        value = Math.Min(Math.Max(0.01, value), 100.0);
        this.SetProperty<double>("GridSpacing", ref this.gridSpacing, value);
      }
    }

    public bool ShowGrid
    {
      get
      {
        return this.showGrid;
      }
      set
      {
        this.SetProperty<bool>("ShowGrid", ref this.showGrid, value);
      }
    }

    public bool SnapToSnapLines
    {
      get
      {
        return this.snapToSnapLines;
      }
      set
      {
        this.SetProperty<bool>("SnapToSnapLines", ref this.snapToSnapLines, value);
      }
    }

    public double SnapLineMargin
    {
      get
      {
        return this.snapLineMargin;
      }
      set
      {
        value = Math.Min(Math.Max(0.0, value), 100.0);
        this.SetProperty<double>("SnapLineMargin", ref this.snapLineMargin, value);
      }
    }

    public double SnapLinePadding
    {
      get
      {
        return this.snapLinePadding;
      }
      set
      {
        value = Math.Min(Math.Max(0.0, value), 100.0);
        this.SetProperty<double>("SnapLinePadding", ref this.snapLinePadding, value);
      }
    }

    public bool IsInGridDesignMode
    {
      get
      {
        return this.isInGridDesignMode;
      }
      set
      {
        this.SetProperty<bool>("IsInGridDesignMode", ref this.isInGridDesignMode, value);
      }
    }

    public bool IsUsingCustomBackgroundColor
    {
      get
      {
        return this.isUsingCustomBackgroundColor;
      }
      set
      {
        this.SetProperty<bool>("IsUsingCustomBackgroundColor", ref this.isUsingCustomBackgroundColor, value);
      }
    }

    public Color CustomBackgroundColor
    {
      get
      {
        return this.customBackgroundColor;
      }
      set
      {
        if (value != this.customBackgroundColor && !this.isInitializing)
          this.IsUsingCustomBackgroundColor = true;
        this.SetProperty<Color>("CustomBackgroundColor", ref this.customBackgroundColor, value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ArtboardOptionsModel(IConfigurationObject configurationObject, bool saveOnPropertyChanged)
    {
      this.configurationObject = configurationObject;
      this.saveOnPropertyChanged = saveOnPropertyChanged;
      this.isInitializing = true;
      this.SnapToGrid = (bool) this.GetProperty("SnapToGrid", (object) false);
      this.GridSpacing = (double) this.GetProperty("GridSpacing", (object) 8.0);
      this.ShowGrid = (bool) this.GetProperty("ShowGrid", (object) false);
      this.SnapToSnapLines = (bool) this.GetProperty("SnapToSnapLines", (object) true);
      this.SnapLineMargin = (double) this.GetProperty("SnapLineMargin", (object) 4.0);
      this.SnapLinePadding = (double) this.GetProperty("SnapLinePadding", (object) 8.0);
      this.IsInGridDesignMode = (bool) this.GetProperty("IsInGridDesignMode", (object) true);
      this.CustomBackgroundColor = (Color) this.GetProperty("CustomBackgroundColor", (object) Colors.White);
      this.IsUsingCustomBackgroundColor = (bool) this.GetProperty("IsUsingCustomBackgroundColor", (object) false);
      string str = this.GetProperty("ArtboardZoomGesture", (object) ArtboardZoomGesture.MouseWheel.ToString()) as string;
      if (string.IsNullOrEmpty(str) || !Enum.TryParse<ArtboardZoomGesture>(str, out this.artboardZoomGesture))
        this.ArtboardZoomGesture = ArtboardZoomGesture.MouseWheel;
      this.effectsEnabledZoomThreshold = (double) this.GetProperty("EffectsEnabledZoomThreshold", (object) 4.0);
      this.effectsEnabled = (bool) this.GetProperty("EffectsEnabled", (object) true);
      this.isInitializing = false;
    }

    public void Save()
    {
      this.configurationObject.SetProperty("SnapToGrid", (object) (bool) (this.SnapToGrid ? true : false));
      this.configurationObject.SetProperty("GridSpacing", (object) this.GridSpacing);
      this.configurationObject.SetProperty("ShowGrid", (object) (bool) (this.ShowGrid ? true : false));
      this.configurationObject.SetProperty("SnapToSnapLines", (object) (bool) (this.SnapToSnapLines ? true : false));
      this.configurationObject.SetProperty("SnapLineMargin", (object) this.SnapLineMargin);
      this.configurationObject.SetProperty("SnapLinePadding", (object) this.SnapLinePadding);
      this.configurationObject.SetProperty("IsInGridDesignMode", (object) (bool) (this.IsInGridDesignMode ? true : false));
      this.configurationObject.SetProperty("CustomBackgroundColor", (object) this.CustomBackgroundColor);
      this.configurationObject.SetProperty("IsUsingCustomBackgroundColor", (object) (bool) (this.IsUsingCustomBackgroundColor ? true : false));
      this.configurationObject.SetProperty("ArtboardZoomGesture", (object) this.ArtboardZoomGesture.ToString());
      this.configurationObject.SetProperty("EffectsEnabled", (object) (bool) (this.EffectsEnabled ? true : false));
      this.configurationObject.SetProperty("EffectsEnabledZoomThreshold", (object) this.EffectsEnabledZoomThreshold);
    }

    private void SetProperty<T>(string propertyName, ref T propertyStorage, T value)
    {
      if (object.Equals((object) propertyStorage, (object) value))
        return;
      propertyStorage = value;
      if (!this.isInitializing && this.saveOnPropertyChanged)
        this.Save();
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private object GetProperty(string propertyName, object defaultValue)
    {
      if (this.configurationObject == null)
        return defaultValue;
      return this.configurationObject.GetProperty(propertyName, defaultValue);
    }
  }
}
