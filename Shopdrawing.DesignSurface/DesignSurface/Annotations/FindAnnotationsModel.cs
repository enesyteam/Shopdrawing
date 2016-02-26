// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.FindAnnotationsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public sealed class FindAnnotationsModel : INotifyPropertyChanged
  {
    private string findText = string.Empty;
    private DesignerContext designerContext;
    private bool matchCase;
    private bool searchReverse;
    private List<AnnotationSceneNode> matches;
    private Action<string> showMessageAction;

    private AnnotationService AnnotationService
    {
      get
      {
        return this.designerContext.AnnotationService;
      }
    }

    public string FindText
    {
      get
      {
        return this.findText;
      }
      set
      {
        this.findText = value;
        this.ClearFirstMatch();
        this.OnPropertyChanged("FindText");
      }
    }

    public bool MatchCase
    {
      get
      {
        return this.matchCase;
      }
      set
      {
        this.matchCase = value;
        this.ClearFirstMatch();
        this.OnPropertyChanged("MatchCase");
      }
    }

    public bool SearchReverse
    {
      get
      {
        return this.searchReverse;
      }
      set
      {
        this.searchReverse = value;
        this.ClearFirstMatch();
        this.OnPropertyChanged("SearchReverse");
      }
    }

    public AnnotationSceneNode MatchedAnnotation { get; private set; }

    public ICommand FindNextAnnotationCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.FindNextAnnotation()));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal FindAnnotationsModel(DesignerContext designerContext, Action<string> showMessageAction)
    {
      this.designerContext = designerContext;
      this.showMessageAction = showMessageAction;
      this.AnnotationService.AnnotationDeleted += new EventHandler<AnnotationEventArgs>(this.AnnotationService_AnnotationDeleted);
    }

    private void AnnotationService_AnnotationDeleted(object sender, AnnotationEventArgs e)
    {
      if (this.MatchedAnnotation != e.Annotation)
        return;
      this.MatchedAnnotation = Enumerable.ElementAtOrDefault<AnnotationSceneNode>((IEnumerable<AnnotationSceneNode>) this.matches, this.matches.IndexOf(this.MatchedAnnotation) - 1);
    }

    public void ClearFirstMatch()
    {
      this.MatchedAnnotation = (AnnotationSceneNode) null;
    }

    internal void FindNextAnnotation()
    {
      this.UpdateMatches();
      this.MatchedAnnotation = this.MatchedAnnotation != null ? Enumerable.ElementAtOrDefault<AnnotationSceneNode>((IEnumerable<AnnotationSceneNode>) this.matches, this.matches.IndexOf(this.MatchedAnnotation) + (this.SearchReverse ? -1 : 1)) : (this.SearchReverse ? Enumerable.LastOrDefault<AnnotationSceneNode>((IEnumerable<AnnotationSceneNode>) this.matches) : Enumerable.FirstOrDefault<AnnotationSceneNode>((IEnumerable<AnnotationSceneNode>) this.matches));
      if (this.MatchedAnnotation != null)
      {
        AnnotationVisual visual = this.MatchedAnnotation.Visual;
        using (visual.DisableFocusStealing())
        {
          visual.ViewModel.Selected = true;
          visual.ViewModel.ViewMode = ViewMode.Editing;
        }
        this.AnnotationService.MakeVisible(this.MatchedAnnotation);
      }
      if (this.MatchedAnnotation != null || this.showMessageAction == null)
        return;
      this.showMessageAction(StringTable.FindAnnotationCannotFind);
    }

    private void UpdateMatches()
    {
      IEnumerable<AnnotationSceneNode> source = this.AnnotationService.AnnotationsInActiveView;
      if (!string.IsNullOrEmpty(this.findText))
      {
        StringComparison comparison = this.MatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        source = Enumerable.Where<AnnotationSceneNode>(source, (Func<AnnotationSceneNode, bool>) (annotation => this.GetSearchText(annotation).IndexOf(this.findText, comparison) != -1));
      }
      this.matches = Enumerable.ToList<AnnotationSceneNode>((IEnumerable<AnnotationSceneNode>) Enumerable.OrderBy<AnnotationSceneNode, DateTime>(Enumerable.Where<AnnotationSceneNode>(source, (Func<AnnotationSceneNode, bool>) (annotation => annotation.Visual != null)), (Func<AnnotationSceneNode, DateTime>) (annotation => annotation.Timestamp)));
    }

    private string GetSearchText(AnnotationSceneNode annotation)
    {
      return annotation.Text + "\n" + annotation.AuthorInitials + "\n" + annotation.Author + "\n" + annotation.Timestamp.ToString();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
