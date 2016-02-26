// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontEmbedOption
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Expression.Project;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FontEmbedOption : INotifyPropertyChanged
  {
    private string characters = string.Empty;
    private bool isEmbedded;
    private SceneViewModel viewModel;
    private FontEmbeddingDialogModel embeddingModel;
    private SystemFontFamily systemFont;
    private IMSBuildItem representativeBuildItem;
    private ProjectFont projectFont;
    private FontFamilyItem fontFamilyItem;
    private bool uppercase;
    private bool lowercase;
    private bool numbers;
    private bool punctuation;
    private bool autoFill;
    private bool all;
    private bool changed;

    public SystemFontFamily SystemFont
    {
      get
      {
        return this.systemFont;
      }
      set
      {
        this.systemFont = value;
      }
    }

    public ProjectFont ProjectFont
    {
      get
      {
        return this.projectFont;
      }
    }

    public bool WasSystemFont
    {
      get
      {
        if (this.representativeBuildItem == null)
          return false;
        string metadata = this.representativeBuildItem.GetMetadata("IsSystemFont");
        if (metadata != null)
          return metadata == "True";
        return false;
      }
    }

    public string FontFamilyName
    {
      get
      {
        if (this.systemFont != null)
          return this.systemFont.FontFamilyName;
        return this.projectFont.FontFamilyName;
      }
    }

    public FontFamily FontFamily
    {
      get
      {
        if (this.systemFont != null)
          return this.systemFont.FontFamily;
        return this.projectFont.FontFamily;
      }
    }

    public FontFamilyItem FontFamilyItem
    {
      get
      {
        if (this.fontFamilyItem == null)
          this.fontFamilyItem = this.systemFont == null ? (FontFamilyItem) new ProjectFontFamilyItem(this.projectFont, this.viewModel.Document.DocumentContext) : (FontFamilyItem) new SystemFontFamilyItem(this.systemFont, this.viewModel.Document.DocumentContext);
        return this.fontFamilyItem;
      }
    }

    public bool IsEmbedded
    {
      get
      {
        return this.isEmbedded;
      }
      set
      {
        foreach (FontEmbedOption fontEmbedOption in this.embeddingModel.GetLinkedProjectFonts(this))
        {
          if (fontEmbedOption != this)
            fontEmbedOption.SetIsEmbedded(value);
        }
        this.SetIsEmbedded(value);
      }
    }

    private bool IsFontItemEmbedded
    {
      get
      {
        if (this.representativeBuildItem == null)
          return false;
        return this.representativeBuildItem.Name == "BlendEmbeddedFont";
      }
      set
      {
        if (value)
        {
          if (this.representativeBuildItem == null)
            this.projectFont = this.viewModel.FontEmbedder.EmbedSystemFont(this.systemFont);
          else
            this.viewModel.FontEmbedder.EmbedProjectFont(this.projectFont);
        }
        else
        {
          this.viewModel.FontEmbedder.UnembedProjectFont((IProjectFont) this.projectFont);
          if (this.systemFont == null)
            return;
          this.projectFont = (ProjectFont) null;
          this.representativeBuildItem = (IMSBuildItem) null;
          this.ClearEmbedOptions();
        }
      }
    }

    public bool IsFontSubsettingAllowed { get; private set; }

    public string Characters
    {
      get
      {
        return this.characters;
      }
      set
      {
        this.characters = value;
        this.changed = true;
        this.OnPropertyChanged("Characters");
      }
    }

    public bool Uppercase
    {
      get
      {
        return this.uppercase;
      }
      set
      {
        this.uppercase = value;
        this.changed = true;
        if (!value)
          this.ClearAll();
        this.OnPropertyChanged("Uppercase");
      }
    }

    public bool Lowercase
    {
      get
      {
        return this.lowercase;
      }
      set
      {
        this.lowercase = value;
        this.changed = true;
        if (!value)
          this.ClearAll();
        this.OnPropertyChanged("Lowercase");
      }
    }

    public bool Numbers
    {
      get
      {
        return this.numbers;
      }
      set
      {
        this.numbers = value;
        this.changed = true;
        if (!value)
          this.ClearAll();
        this.OnPropertyChanged("Numbers");
      }
    }

    public bool Punctuation
    {
      get
      {
        return this.punctuation;
      }
      set
      {
        this.punctuation = value;
        this.changed = true;
        if (!value)
          this.ClearAll();
        this.OnPropertyChanged("Punctuation");
      }
    }

    public bool AutoFill
    {
      get
      {
        return this.autoFill;
      }
      set
      {
        this.autoFill = value;
        this.changed = true;
        this.OnPropertyChanged("AutoFill");
      }
    }

    public bool All
    {
      get
      {
        return this.all;
      }
      set
      {
        this.all = value;
        this.changed = true;
        this.Uppercase = value;
        this.Lowercase = value;
        this.Numbers = value;
        this.Punctuation = value;
        this.OnPropertyChanged("All");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public FontEmbedOption(SystemFontFamily systemFont, SceneViewModel viewModel, FontEmbeddingDialogModel embeddingModel)
    {
      this.systemFont = systemFont;
      this.viewModel = viewModel;
      this.embeddingModel = embeddingModel;
      this.IsFontSubsettingAllowed = FontEmbedder.DoesFontFileSupportSubsetting(Enumerable.FirstOrDefault<string>((IEnumerable<string>) systemFont.FontSources) ?? string.Empty);
    }

    public FontEmbedOption(ProjectFont projectFont, IProjectItem projectItem, SceneViewModel viewModel, FontEmbeddingDialogModel embeddingModel)
    {
      this.projectFont = projectFont;
      this.viewModel = viewModel;
      this.InitializeEmbedOptions(projectItem);
      this.isEmbedded = this.IsFontItemEmbedded;
      this.embeddingModel = embeddingModel;
      this.IsFontSubsettingAllowed = FontEmbedder.DoesFontFileSupportSubsetting(projectFont.FontDocumentPath);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private void InitializeEmbedOptions(IProjectItem projectItem)
    {
      this.representativeBuildItem = (IMSBuildItem) projectItem;
      if (this.representativeBuildItem == null)
        return;
      this.All = this.representativeBuildItem.GetMetadata("All") == "True";
      this.Characters = this.representativeBuildItem.GetMetadata("Characters");
      this.Uppercase = this.representativeBuildItem.GetMetadata("Uppercase") == "True" || this.all;
      this.Lowercase = this.representativeBuildItem.GetMetadata("Lowercase") == "True" || this.all;
      this.Punctuation = this.representativeBuildItem.GetMetadata("Punctuation") == "True" || this.all;
      this.Numbers = this.representativeBuildItem.GetMetadata("Numbers") == "True" || this.all;
      this.AutoFill = this.representativeBuildItem.GetMetadata("AutoFill") == "True";
      if (this.Characters == null)
        this.Characters = string.Empty;
      this.changed = false;
    }

    private void ClearEmbedOptions()
    {
      this.All = false;
      this.Characters = string.Empty;
      this.Uppercase = false;
      this.Lowercase = false;
      this.Punctuation = false;
      this.Numbers = false;
      this.AutoFill = false;
    }

    private void SetIsEmbedded(bool value)
    {
      this.isEmbedded = value;
      this.OnPropertyChanged("IsEmbedded");
      this.embeddingModel.CurrentFont = this;
      if (this.representativeBuildItem == null)
      {
        if (value)
        {
          this.AutoFill = true;
          this.All = true;
        }
        else
          this.ClearEmbedOptions();
      }
      else
      {
        if (!value || !string.IsNullOrEmpty(this.representativeBuildItem.GetMetadata("All")))
          return;
        this.AutoFill = true;
        this.All = true;
      }
    }

    private void ClearAll()
    {
      this.all = false;
      this.changed = true;
      this.OnPropertyChanged("All");
    }

    public bool CommitChanges()
    {
      bool flag = false;
      if (this.IsEmbedded != this.IsFontItemEmbedded)
      {
        this.IsFontItemEmbedded = this.IsEmbedded;
        flag = true;
      }
      if (this.changed && this.projectFont != null)
      {
        foreach (DocumentReference documentReference in this.projectFont.FontDocuments)
        {
          IProjectItem projectItem = this.viewModel.DesignerContext.ActiveProject.FindItem(documentReference);
          if (projectItem != null)
          {
            ((IMSBuildItem) projectItem).SetMetadata("Characters", this.characters);
            ((IMSBuildItem) projectItem).SetMetadata("Uppercase", this.Uppercase ? "True" : "False");
            ((IMSBuildItem) projectItem).SetMetadata("Lowercase", this.Lowercase ? "True" : "False");
            ((IMSBuildItem) projectItem).SetMetadata("Numbers", this.Numbers ? "True" : "False");
            ((IMSBuildItem) projectItem).SetMetadata("Punctuation", this.Punctuation ? "True" : "False");
            ((IMSBuildItem) projectItem).SetMetadata("AutoFill", this.AutoFill ? "True" : "False");
            ((IMSBuildItem) projectItem).SetMetadata("All", this.All ? "True" : "False");
          }
        }
      }
      return flag;
    }
  }
}
