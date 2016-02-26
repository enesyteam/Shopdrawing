// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.SelectableCompletion
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Language.Intellisense;
using System.ComponentModel;

namespace Microsoft.Expression.Code.UserInterface
{
  public class SelectableCompletion : INotifyPropertyChanged
  {
    private ICompletionSession session;
    private ICompletion completion;

    public string Text
    {
      get
      {
        return this.completion.DisplayText;
      }
    }

    public bool IsSelected
    {
      get
      {
        if (this.session.SelectionStatus.SelectedCompletion == this.completion)
          return (this.session.SelectionStatus.SelectionOptions & CompletionSelectionOptions.Selected) == CompletionSelectionOptions.Selected;
        return false;
      }
    }

    public ICompletion Completion
    {
      get
      {
        return this.completion;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SelectableCompletion(ICompletionSession session, ICompletion completion)
    {
      this.session = session;
      this.completion = completion;
    }

    public void OnIsSelectedChanged()
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs("IsSelected"));
    }
  }
}
