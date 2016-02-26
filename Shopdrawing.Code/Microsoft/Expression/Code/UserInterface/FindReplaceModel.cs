// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.FindReplaceModel
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

namespace Microsoft.Expression.Code.UserInterface
{
  internal sealed class FindReplaceModel
  {
    private string findText = string.Empty;
    private string replaceText = string.Empty;
    private bool matchCase;
    private bool searchReverse;
    private int matchOffset;

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
      }
    }

    public string ReplaceText
    {
      get
      {
        return this.replaceText;
      }
      set
      {
        this.replaceText = value;
        this.ClearFirstMatch();
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
      }
    }

    public int MatchOffset
    {
      get
      {
        return this.matchOffset;
      }
    }

    public void ClearFirstMatch()
    {
      this.matchOffset = -1;
    }

    public void SetMatchIfFirst(int matchOffset)
    {
      if (this.matchOffset != -1)
        return;
      this.matchOffset = matchOffset;
    }

    public void UpdateMatchOffset(int offset)
    {
      if (this.matchOffset == -1)
        return;
      this.matchOffset += offset;
    }
  }
}
