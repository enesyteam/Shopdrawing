// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.CharacterScanner
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text;

namespace Microsoft.Expression.Code.Classifiers
{
  public class CharacterScanner
  {
    private ITextSnapshot snapshot;
    private Span span;
    private char[] buffer;
    private int start;
    private int cursor;

    public ITextSnapshot Snapshot
    {
      get
      {
        return this.snapshot;
      }
    }

    public Span Span
    {
      get
      {
        return this.span;
      }
    }

    public int Cursor
    {
      get
      {
        return this.start + this.cursor;
      }
    }

    public bool IsEndOfScan
    {
      get
      {
        return this.cursor >= this.buffer.Length;
      }
    }

    public bool IsEndOfFile
    {
      get
      {
        return this.Cursor >= this.snapshot.Length;
      }
    }

    public char CurrentCharacter
    {
      get
      {
        return this.buffer[this.cursor];
      }
    }

    public CharacterScanner(SnapshotSpan span)
      : this(span.Snapshot, span.Span)
    {
    }

    public CharacterScanner(ITextSnapshot document, Span span)
    {
      this.snapshot = document;
      this.span = span;
      this.buffer = new char[this.span.Length];
      this.snapshot.CopyTo(this.span.Start, this.buffer, 0, this.span.Length);
      this.start = this.span.Start;
    }

    public char Peek(int offset)
    {
      if (this.cursor + offset < 0 || this.cursor + offset >= this.buffer.Length)
        return char.MinValue;
      return this.buffer[this.cursor + offset];
    }

    public bool MatchString(string target)
    {
      return this.MatchString(target, true);
    }

    public bool MatchString(string target, bool incrementAfterMatch)
    {
      if (this.cursor + target.Length > this.buffer.Length)
        return false;
      for (int index = 0; index < target.Length; ++index)
      {
        if ((int) target[index] != (int) this.buffer[this.cursor + index])
          return false;
      }
      if (incrementAfterMatch)
        this.cursor += target.Length;
      return true;
    }

    public void MoveNext()
    {
      ++this.cursor;
    }

    public void Seek(int offset)
    {
      this.cursor += offset;
    }
  }
}
