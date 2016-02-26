// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.SpecialFolderManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Configuration
{
  public sealed class SpecialFolderManager
  {
    private string folderConfigurationName;
    private string guardConfigurationName;
    private string defaultFolder;
    private IConfigurationObject configuration;

    public string Path
    {
      get
      {
        string str = (string) this.configuration.GetProperty(this.folderConfigurationName, (object) null);
        if (!(bool) this.configuration.GetProperty(this.guardConfigurationName, (object) true) || str == null)
          str = this.defaultFolder;
        return str;
      }
      set
      {
        this.configuration.SetProperty(this.folderConfigurationName, (object) value);
      }
    }

    public SpecialFolderManager(string folderConfigurationName, IConfigurationObject configuration)
      : this(folderConfigurationName, configuration, (string) null)
    {
    }

    public SpecialFolderManager(string folderConfigurationName, IConfigurationObject configuration, string defaultFolder)
    {
      this.folderConfigurationName = folderConfigurationName;
      this.guardConfigurationName = "Use" + folderConfigurationName;
      this.configuration = configuration;
      if (defaultFolder == null)
        this.defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      else
        this.defaultFolder = defaultFolder;
    }
  }
}
