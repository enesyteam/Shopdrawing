// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationsOptionsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Security;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public sealed class AnnotationsOptionsModel : INotifyPropertyChanging, INotifyPropertyChanged
  {
    private string authorName = string.Empty;
    private string authorInitials = string.Empty;
    private IConfigurationObject configurationObject;
    private bool saveOnPropertyChanged;
    private bool isInitializing;
    private bool showAnnotations;

    public bool ShowAnnotations
    {
      get
      {
        return this.showAnnotations;
      }
      set
      {
        this.Set<bool>(ref this.showAnnotations, "ShowAnnotations", value);
      }
    }

    public string AuthorName
    {
      get
      {
        return this.authorName;
      }
      set
      {
        this.Set<string>(ref this.authorName, "AuthorName", value);
      }
    }

    public string AuthorInitials
    {
      get
      {
        return this.authorInitials;
      }
      set
      {
        this.Set<string>(ref this.authorInitials, "AuthorInitials", value);
      }
    }

    public static string DefaultAuthorName
    {
      get
      {
        string str = AnnotationsOptionsModel.GetOfficeUserInfoValue("UserName");
        if (string.IsNullOrEmpty(str))
          str = AnnotationsOptionsModel.GetWindowsUserName();
        return str;
      }
    }

    public static string DefaultAuthorInitials
    {
      get
      {
        string str = AnnotationsOptionsModel.GetOfficeUserInfoValue("UserInitials");
        if (string.IsNullOrEmpty(str))
          str = AnnotationsOptionsModel.GetWindowsUserName();
        return str;
      }
    }

    public event PropertyChangingEventHandler PropertyChanging;

    public event PropertyChangedEventHandler PropertyChanged;

    public AnnotationsOptionsModel(bool saveOnPropertyChanged)
    {
      this.saveOnPropertyChanged = saveOnPropertyChanged;
    }

    public void InitAuthorInfoIfNeeded()
    {
      if (this.AuthorName != null)
        return;
      this.AuthorName = AnnotationsOptionsModel.DefaultAuthorName;
      this.AuthorInitials = AnnotationsOptionsModel.DefaultAuthorInitials;
    }

    public void Load(IConfigurationObject config)
    {
      this.configurationObject = config;
      this.isInitializing = true;
      this.ShowAnnotations = (bool) config.GetProperty("ShowAnnotations", (object) false);
      this.AuthorName = (string) config.GetProperty("AuthorName", (object) null);
      this.AuthorInitials = (string) config.GetProperty("AuthorInitials", (object) null);
      this.isInitializing = false;
    }

    public void Save(IConfigurationObject config)
    {
      config.SetProperty("ShowAnnotations", (object) (bool) (this.ShowAnnotations ? true : false));
      config.SetProperty("AuthorName", (object) this.AuthorName);
      config.SetProperty("AuthorInitials", (object) this.AuthorInitials);
    }

    private void OnPropertyChanging(string propertyName)
    {
      PropertyChangingEventHandler changingEventHandler = this.PropertyChanging;
      if (changingEventHandler == null)
        return;
      changingEventHandler((object) this, new PropertyChangingEventArgs(propertyName));
    }

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void Set<T>(ref T field, string name, T value)
    {
      if (object.Equals((object) field, (object) value))
        return;
      this.OnPropertyChanging(name);
      field = value;
      if (!this.isInitializing && this.saveOnPropertyChanged && this.configurationObject != null)
        this.Save(this.configurationObject);
      this.OnPropertyChanged(name);
    }

    private static string GetOfficeUserInfoValue(string valueName)
    {
      try
      {
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\Common\\UserInfo");
        if (registryKey != null)
        {
          string str = registryKey.GetValue(valueName) as string;
          return str != null ? str.Trim() : string.Empty;
        }
      }
      catch (SecurityException ex)
      {
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      return string.Empty;
    }

    private static string GetWindowsUserName()
    {
      try
      {
        return Environment.UserName;
      }
      catch (SecurityException ex)
      {
      }
      return string.Empty;
    }
  }
}
