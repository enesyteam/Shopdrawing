// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SimpleTextBuffer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Text;
using System;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public sealed class SimpleTextBuffer : ITextBuffer, IReadableSelectableTextBuffer, IReadableTextBuffer
  {
    private string textBuffer = string.Empty;

    public int Length
    {
      get
      {
        return this.textBuffer.Length;
      }
    }

    event EventHandler<TextChangedEventArgs> ITextBuffer.TextChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public void SetText(int offset, int length, string text)
    {
      this.textBuffer = text.Substring(offset, length);
    }

    public ITextChangesTracker RecordChanges(ShouldTrackChange shouldTrackChange)
    {
      return (ITextChangesTracker) null;
    }

    public void Undo()
    {
    }

    public void Redo()
    {
    }

    public string GetText(int offset, int length)
    {
      return this.textBuffer.Substring(offset, length);
    }

    public IReadableSelectableTextBuffer Freeze()
    {
      return (IReadableSelectableTextBuffer) this;
    }

    public ITextRange FreezeRange(ITextRange textRange)
    {
      return this.CreateRange(textRange.Offset, textRange.Length);
    }

    public ITextRange CreateRange(int offset, int length)
    {
      return (ITextRange) new SimpleTextBuffer.TextRange(offset, length);
    }

    public ITextLocation GetLocation(int offset)
    {
      return (ITextLocation) new SimpleTextBuffer.TextLocation(offset);
    }

    private class TextRange : ITextRange
    {
      private int offset;
      private int length;

      public int Offset
      {
        get
        {
          return this.offset;
        }
      }

      public int Length
      {
        get
        {
          return this.length;
        }
      }

      public TextRange(int offset, int length)
      {
        this.offset = offset;
        this.length = length;
      }
    }

    private class TextLocation : ITextLocation
    {
      private int offset;

      public int Line
      {
        get
        {
          return 1;
        }
      }

      public int Column
      {
        get
        {
          return this.offset;
        }
      }

      public TextLocation(int offset)
      {
        this.offset = offset;
      }
    }
  }
}
