// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.OptionsDialogService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Configuration
{
  public class OptionsDialogService : IOptionsDialogService, INotifyPropertyChanged
  {
    private IOptionsPageCollection optionsPages;
    private IOptionsPage activePage;
    private IConfigurationService configurationService;

    public IOptionsPageCollection OptionsPages
    {
      get
      {
        return this.optionsPages;
      }
    }

    public IOptionsPage ActivePage
    {
      get
      {
        if (this.activePage != null)
          return this.activePage;
        if (this.OptionsPages.Count > 0)
          return this.OptionsPages[0];
        return (IOptionsPage) null;
      }
      set
      {
        this.activePage = value;
        this.OnPropertyChanged("ActivePage");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public OptionsDialogService(IConfigurationService configurationService)
    {
      this.configurationService = configurationService;
      this.optionsPages = (IOptionsPageCollection) new OptionsDialogService.OptionsPageCollection(this, configurationService);
    }

    public void Cancel()
    {
      foreach (IOptionsPage optionsPage in (IEnumerable) this.OptionsPages)
        optionsPage.Cancel();
    }

    public void Commit()
    {
      foreach (IOptionsPage optionsPage in (IEnumerable) this.OptionsPages)
        optionsPage.Commit();
      this.configurationService.Save();
    }

    private void NotifyRemove(IOptionsPage value)
    {
      if (this.activePage != value)
        return;
      this.activePage = (IOptionsPage) null;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private class OptionsPageCollection : IOptionsPageCollection, ICollection, IEnumerable
    {
      private List<IOptionsPage> pages = new List<IOptionsPage>();
      private OptionsDialogService owner;
      private IConfigurationService configurationService;

      public IOptionsPage this[int index]
      {
        get
        {
          return this.pages[index];
        }
        set
        {
          this.pages[index] = value;
        }
      }

      public int Count
      {
        get
        {
          return this.pages.Count;
        }
      }

      public object SyncRoot
      {
        get
        {
          return (object) null;
        }
      }

      public bool IsSynchronized
      {
        get
        {
          return false;
        }
      }

      public OptionsPageCollection(OptionsDialogService owner, IConfigurationService configurationService)
      {
        this.owner = owner;
        this.configurationService = configurationService;
      }

      public void Clear()
      {
        this.pages.Clear();
      }

      public void Add(IOptionsPage value)
      {
        if (value == null)
          throw new ArgumentNullException("value");
        IConfigurationObject configurationObject = this.configurationService["Options." + value.Name];
        value.Load(configurationObject);
        this.pages.Add(value);
      }

      public void Insert(int index, IOptionsPage value)
      {
        if (value == null)
          throw new ArgumentNullException("value");
        IConfigurationObject configurationObject = this.configurationService["Options." + value.Name];
        value.Load(configurationObject);
        this.pages.Insert(index, value);
      }

      public void Remove(IOptionsPage value)
      {
        this.owner.NotifyRemove(value);
        this.pages.Remove(value);
      }

      public void RemoveAt(int index)
      {
        if (index >= this.pages.Count)
          throw new ArgumentOutOfRangeException("index");
        this.owner.NotifyRemove(this.pages[index]);
        this.pages.RemoveAt(index);
      }

      public void CopyTo(Array array, int index)
      {
        IOptionsPage[] optionsPageArray = this.pages.ToArray();
        Array.Copy((Array) optionsPageArray, 0, array, index, optionsPageArray.Length);
      }

      public IEnumerator GetEnumerator()
      {
        return (IEnumerator) this.pages.GetEnumerator();
      }
    }
  }
}
