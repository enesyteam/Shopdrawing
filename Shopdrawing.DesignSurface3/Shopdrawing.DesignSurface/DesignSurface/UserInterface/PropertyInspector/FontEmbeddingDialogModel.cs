// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontEmbeddingDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FontEmbeddingDialogModel : INotifyPropertyChanged
  {
    private ObservableCollectionWorkaround<FontEmbedOption> fonts = new ObservableCollectionWorkaround<FontEmbedOption>();
    private Dictionary<DocumentReference, List<FontEmbedOption>> linkedProjectFonts = new Dictionary<DocumentReference, List<FontEmbedOption>>();
    private SceneViewModel viewModel;
    private ICollectionView fontsView;
    private string filterString;

    public string FilterString
    {
      get
      {
        return this.filterString;
      }
      set
      {
        this.filterString = value;
        this.fontsView.Filter = (Predicate<object>) (fontEntry => ((FontEmbedOption) fontEntry).FontFamilyName.IndexOf(this.filterString, StringComparison.CurrentCultureIgnoreCase) != -1);
        this.OnPropertyChanged("FilterString");
      }
    }

    public ICommand ClearFilterStringCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.FilterString = string.Empty));
      }
    }

    public ObservableCollection<FontEmbedOption> Fonts
    {
      get
      {
        return (ObservableCollection<FontEmbedOption>) this.fonts;
      }
    }

    public FontEmbedOption CurrentFont
    {
      get
      {
        return this.fontsView.CurrentItem as FontEmbedOption;
      }
      set
      {
        this.fontsView.MoveCurrentTo((object) value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public FontEmbeddingDialogModel(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      Dictionary<string, FontEmbedOption> dictionary = new Dictionary<string, FontEmbedOption>();
      foreach (ProjectFont projectFont in (Collection<IProjectFont>) viewModel.ProjectContext.ProjectFonts)
      {
        IProjectItem projectItem = viewModel.DesignerContext.ActiveProject.FindItem(projectFont.FontDocumentReference);
        if (projectItem != null)
        {
          FontEmbedOption fontEmbedOption = new FontEmbedOption(projectFont, projectItem, viewModel, this);
          this.fonts.Add(fontEmbedOption);
          dictionary[projectFont.FontFamilyName] = fontEmbedOption;
          List<FontEmbedOption> list;
          if (!this.linkedProjectFonts.TryGetValue(projectFont.FontDocumentReference, out list))
          {
            list = new List<FontEmbedOption>();
            this.linkedProjectFonts[projectFont.FontDocumentReference] = list;
          }
          list.Add(fontEmbedOption);
        }
      }
      foreach (SystemFontFamily systemFont in FontEmbedder.GetSystemFonts((ITypeResolver) viewModel.ProjectContext))
      {
        FontEmbedOption fontEmbedOption = (FontEmbedOption) null;
        if (dictionary.TryGetValue(systemFont.FontFamilyName, out fontEmbedOption))
        {
          if (fontEmbedOption.WasSystemFont)
            fontEmbedOption.SystemFont = systemFont;
        }
        else if (!systemFont.IsNativeSilverlightFont || !this.viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.DisallowEmbeddingSilverlightFonts))
          this.fonts.Add(new FontEmbedOption(systemFont, viewModel, this));
      }
      this.fonts.Sort((Comparison<FontEmbedOption>) ((lhs, rhs) =>
      {
        if (lhs.ProjectFont != null && rhs.ProjectFont == null)
          return -1;
        if (lhs.ProjectFont == null && rhs.ProjectFont != null)
          return 1;
        return lhs.FontFamilyName.CompareTo(rhs.FontFamilyName);
      }));
      this.fontsView = CollectionViewSource.GetDefaultView((object) this.fonts);
      this.fontsView.CurrentChanged += new EventHandler(this.fontsView_CurrentChanged);
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void fontsView_CurrentChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("CurrentFont");
    }

    public IEnumerable<FontEmbedOption> GetLinkedProjectFonts(FontEmbedOption font)
    {
      if (font.ProjectFont != null)
        return (IEnumerable<FontEmbedOption>) this.linkedProjectFonts[font.ProjectFont.FontDocumentReference];
      return Enumerable.Empty<FontEmbedOption>();
    }

    public void CommitChanges()
    {
      using (IEnumerator<FontEmbedOption> enumerator = this.fonts.GetEnumerator())
      {
        do
          ;
        while (enumerator.MoveNext() && (!enumerator.Current.CommitChanges() || this.viewModel.DesignerContext.ProjectManager.CurrentSolution.RefreshProject(this.viewModel.DesignerContext.ActiveProject)));
      }
    }
  }
}
