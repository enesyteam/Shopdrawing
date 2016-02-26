// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.CodeAidIntellisensePresenter
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.UserInterface;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Windows;

namespace Microsoft.Expression.Code.CodeAid
{
  public class CodeAidIntellisensePresenter : IPopupIntellisensePresenter, IIntellisensePresenter
  {
    private ITrackingSpan presentationSpan;

    public IntellisenseMenu Menu { get; private set; }

    public ICompletionSession Session { get; private set; }

    public ITrackingSpan PresentationSpan
    {
      get
      {
        if (this.presentationSpan == null)
          this.ComputePresentationSpan();
        return this.presentationSpan;
      }
    }

    public UIElement SurfaceElement
    {
      get
      {
        return (UIElement) this.Menu;
      }
    }

    public event EventHandler SurfaceElementChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public CodeAidIntellisensePresenter(IIntellisenseSession session)
    {
      this.Session = session as ICompletionSession;
      this.Session.Committed += new EventHandler(this.Session_Committed);
      this.Session.Dismissed += new EventHandler(this.Session_Dismissed);
      this.Session.CompletionsChanged += new EventHandler(this.Session_CompletionsChanged);
      this.Session.SelectionStatusChanged += new EventHandler<SelectionStatusEventArgs>(this.Session_SelectionStatusChanged);
      this.Menu = new IntellisenseMenu();
      this.Menu.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.SurfaceElement_IsVisibleChanged);
      this.Menu.Session = this.Session;
    }

    private void SurfaceElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.SurfaceElement.IsVisible)
        return;
      this.Session.Dismiss();
    }

    private void Session_SelectionStatusChanged(object sender, SelectionStatusEventArgs e)
    {
    }

    private void Session_CompletionsChanged(object sender, EventArgs e)
    {
      this.ComputePresentationSpan();
    }

    private void Session_Dismissed(object sender, EventArgs e)
    {
      this.Menu.Session = (ICompletionSession) null;
      this.Session.Committed -= new EventHandler(this.Session_Committed);
      this.Session.Dismissed -= new EventHandler(this.Session_Dismissed);
      this.Session.CompletionsChanged -= new EventHandler(this.Session_CompletionsChanged);
      this.Session.SelectionStatusChanged -= new EventHandler<SelectionStatusEventArgs>(this.Session_SelectionStatusChanged);
      this.SurfaceElement.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(this.SurfaceElement_IsVisibleChanged);
    }

    private void Session_Committed(object sender, EventArgs e)
    {
    }

    private void ComputePresentationSpan()
    {
      if (this.Session.Completions.Count > 0)
        this.presentationSpan = this.Session.Completions[0].ApplicableTo;
      else
        this.presentationSpan = this.Session.TextView.TextSnapshot.CreateTrackingSpan(this.Session.TextView.Selection.SelectionSpan.Span, SpanTrackingMode.EdgeInclusive);
    }

    public void CaptureKeyboard()
    {
    }

    public void ReleaseKeyboard()
    {
    }
  }
}
