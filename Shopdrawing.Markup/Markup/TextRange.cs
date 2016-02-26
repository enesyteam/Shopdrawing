// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.TextRange
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Text;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class TextRange : ITextRange
  {
    public static readonly ITextRange Null = (ITextRange) new TextRange(-1, -1);
    private readonly int start;
    private readonly int end;

    public int Offset
    {
      get
      {
        return this.start;
      }
    }

    public int Length
    {
      get
      {
        return this.end - this.start;
      }
    }

    public TextRange(int start, int end)
    {
      this.start = start;
      this.end = end;
    }

    public static bool IsNull(ITextRange range)
    {
      if (range == null)
        return true;
      int offset = range.Offset;
      int num = offset + range.Length;
      if (offset == -1)
        return num == -1;
      return false;
    }

    public static ITextRange Union(ITextRange range, ITextRange other)
    {
      if (TextRange.IsNull(range))
        return other;
      if (TextRange.IsNull(other))
        return range;
      return (ITextRange) new TextRange(Math.Min(range.Offset, other.Offset), Math.Max(range.Offset + range.Length, other.Offset + other.Length));
    }

    public static bool Contains(ITextRange range, ITextRange other)
    {
      if (other.Offset >= range.Offset && other.Offset < range.Offset + range.Length)
        return other.Offset + other.Length <= range.Offset + range.Length;
      return false;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0},{1})", new object[2]
      {
        (object) this.start,
        (object) this.end
      });
    }
  }
}
