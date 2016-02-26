// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.TextBufferHelper
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Text;
using System;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class TextBufferHelper
  {
    public static ITextRange ExpandSpanLeftToFillWhitespace(IReadableTextBuffer textBuffer, ITextRange span)
    {
      return TextBufferHelper.ExpandSpanToFillWhitespace(textBuffer, span, true, false);
    }

    public static ITextRange ExpandSpanToFillWhitespace(IReadableTextBuffer textBuffer, ITextRange span, bool walkLeft, bool walkRight)
    {
      if (textBuffer.Length == 0)
        return span;
      int offset1 = Math.Max(0, span.Offset - 64);
      int num = Math.Min(textBuffer.Length - 1, span.Offset + span.Length + 64);
      int offset2 = span.Offset;
      int end = span.Offset + span.Length;
      for (int offset3 = span.Offset; walkLeft && offset1 < offset3 && offset2 == offset3; offset1 = Math.Max(0, offset3 - 64))
      {
        string text = textBuffer.GetText(offset1, offset3 - offset1);
        int index = text.Length - 1;
        while (index >= 0 && char.IsWhiteSpace(text[index]))
        {
          --index;
          --offset2;
        }
        offset3 -= text.Length;
      }
      for (int offset3 = span.Offset + span.Length; walkRight && num > offset3 && end == offset3; num = Math.Min(textBuffer.Length - 1, offset3 + 64))
      {
        string text = textBuffer.GetText(offset3, num - offset3 + 1);
        int index = 0;
        while (index < text.Length && char.IsWhiteSpace(text[index]))
        {
          ++index;
          ++end;
        }
        offset3 += text.Length;
      }
      return (ITextRange) new TextRange(offset2, end);
    }

    public static IReadableSelectableTextBuffer GetHostBuffer(IReadableSelectableTextBuffer textBuffer)
    {
      return textBuffer;
    }
  }
}
