// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleImageConfiguration
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleImageConfiguration : ISampleTypeConfiguration
  {
    private IProjectContext projectContext;
    private string imageDirectory;
    private string fallbackImageDirectory;
    private List<string> imageFiles;
    private int index;

    public SampleBasicType SampleType
    {
      get
      {
        return SampleBasicType.Image;
      }
    }

    public object Value
    {
      get
      {
        if (this.ImageFiles.Count == 0)
          return (object) null;
        ++this.index;
        if (this.index >= this.ImageFiles.Count || this.index < 0)
          this.index = 0;
        return (object) this.ImageFiles[this.index];
      }
    }

    public string Format
    {
      get
      {
        return (string) null;
      }
    }

    public string FormatParameters
    {
      get
      {
        return this.GetRelativePath(this.imageDirectory);
      }
    }

    private ICollection<string> AllowedImageExtensions
    {
      get
      {
        return this.projectContext.Platform.Metadata.ImageFileExtensions;
      }
    }

    private List<string> ImageFiles
    {
      get
      {
        if (this.imageFiles == null)
        {
          this.imageFiles = this.GetImagesFromFolder(this.imageDirectory);
          if (this.imageFiles == null || this.imageFiles.Count == 0)
          {
            this.imageFiles = this.GetImagesFromFolder(this.fallbackImageDirectory);
            if (this.imageFiles == null)
              this.imageFiles = new List<string>();
          }
        }
        return this.imageFiles;
      }
    }

    public SampleImageConfiguration(SampleDataSet sampleData, string formatParameters)
    {
      this.projectContext = (IProjectContext) sampleData.ProjectContext;
      this.fallbackImageDirectory = sampleData.FallbackImageFolder;
      this.SetConfigurationValue(ConfigurationPlaceholder.ImageFolderBrowser, (object) formatParameters);
      this.index = -1;
    }

    public void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value)
    {
      string absolutePath = this.GetAbsolutePath(value == null ? string.Empty : value.ToString());
      if (string.IsNullOrEmpty(absolutePath) || !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(absolutePath) || !(this.imageDirectory != absolutePath))
        return;
      this.imageDirectory = absolutePath;
      this.imageFiles = (List<string>) null;
      this.index = -1;
    }

    public object GetConfigurationValue(ConfigurationPlaceholder placeholder)
    {
      if (placeholder == ConfigurationPlaceholder.ImageFolderBrowser)
        return (object) this.imageDirectory;
      return (object) null;
    }

    private string GetRelativePath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      string path1 = path;
      try
      {
        if (!Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(path))
        {
          path1 = DocumentReference.Create(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(this.projectContext.ProjectPath)).GetRelativePath(DocumentReference.Create(path));
          path1 = Microsoft.Expression.Framework.Documents.PathHelper.TrimTrailingDirectorySeparators(path1);
        }
      }
      catch (ArgumentException ex)
      {
      }
      catch (UriFormatException ex)
      {
      }
      return path1;
    }

    private string GetAbsolutePath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      string str = path;
      try
      {
        Uri relativeUri = new Uri(path, UriKind.RelativeOrAbsolute);
        if (!relativeUri.IsAbsoluteUri)
          str = new Uri(new Uri(this.projectContext.ProjectPath, UriKind.Absolute), relativeUri).OriginalString;
      }
      catch (ArgumentException ex)
      {
      }
      catch (UriFormatException ex)
      {
      }
      return str;
    }

    private List<string> GetImagesFromFolder(string folder)
    {
      if (string.IsNullOrEmpty(folder))
        return (List<string>) null;
      List<string> list = new List<string>();
      DirectoryInfo directoryInfo = new DirectoryInfo(folder);
      foreach (string str in (IEnumerable<string>) this.AllowedImageExtensions)
      {
        foreach (FileInfo fileInfo in directoryInfo.GetFiles("*" + str))
          list.Add(fileInfo.FullName);
      }
      return list;
    }
  }
}
