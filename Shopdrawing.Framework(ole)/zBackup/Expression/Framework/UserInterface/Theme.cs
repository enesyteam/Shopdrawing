// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.Theme
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Windows;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class Theme : ITheme
  {
    private string name;
    private string resourceName;
    private bool isSystemTheme;
    private Uri resourceDictionaryUriToMerge;
    private ResourceDictionary resourceDictionary;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public bool IsSystemTheme
    {
      get
      {
        return this.isSystemTheme;
      }
    }

    public ResourceDictionary ResourceDictionary
    {
      get
      {
        if (this.resourceDictionary == null)
        {
          this.resourceDictionary = FileTable.GetResourceDictionary(this.resourceName);
          if (this.resourceDictionaryUriToMerge != (Uri) null)
            this.resourceDictionary.MergedDictionaries.Add(new ResourceDictionary()
            {
              Source = this.resourceDictionaryUriToMerge
            });
        }
        return this.resourceDictionary;
      }
    }

    public Theme(string name, string resourceName, bool isSystemTheme)
    {
      this.name = name;
      this.resourceName = resourceName;
      this.isSystemTheme = isSystemTheme;
    }

    public Theme(string name, string resourceName, Uri resourceDictionaryUriToMerge)
    {
      this.name = name;
      this.resourceName = resourceName;
      this.resourceDictionaryUriToMerge = resourceDictionaryUriToMerge;
    }

    public void Reset()
    {
      this.resourceDictionary = (ResourceDictionary) null;
    }
  }
}
