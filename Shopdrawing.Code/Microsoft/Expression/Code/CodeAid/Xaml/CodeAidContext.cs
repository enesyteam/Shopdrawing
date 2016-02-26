// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.CodeAidContext
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  public struct CodeAidContext
  {
    public SnapshotPoint CurrentPosition { get; set; }

    public SnapshotSpan SelectionSpan { get; set; }

    public ITrackingPoint SessionStartingPosition { get; set; }

    public bool IsSessionActive
    {
      get
      {
        return this.SessionStartingPosition != null;
      }
    }

    public ICompletion SessionSelectedCompletion { get; set; }

    public ICodeAidCompletion SessionSelectedCompletionCodeAid
    {
      get
      {
        return this.SessionSelectedCompletion as ICodeAidCompletion;
      }
    }

    public ICompletion SessionHollowSelectedCompletion { get; set; }

    public ICodeAidCompletion SessionHollowSelectedCompletionCodeAid
    {
      get
      {
        return this.SessionHollowSelectedCompletion as ICodeAidCompletion;
      }
    }

    public ITextSnapshot CurrentSnapshot
    {
      get
      {
        SnapshotPoint currentPosition = this.CurrentPosition;
        return this.CurrentPosition.Snapshot;
      }
    }

    public string SessionSelectedCompletionText
    {
      get
      {
        if (this.SessionSelectedCompletion == null)
          return (string) null;
        return this.SessionSelectedCompletion.DisplayText;
      }
    }

    public string SessionSelectedCompletionBufferText
    {
      get
      {
        ITextSnapshot snapshot = this.CurrentPosition.Snapshot;
        if (this.SessionSelectedCompletion == null)
          return (string) null;
        return snapshot.GetText((Span) this.SessionSelectedCompletion.ApplicableTo.GetSpan(snapshot));
      }
    }
  }
}
