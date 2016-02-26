// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.CopyAnnotationTextTemplate
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  internal class CopyAnnotationTextTemplate : Section
  {
    public CopyAnnotationTextTemplate(RawAnnotation annotation)
    {
      this.TextAlignment = TextAlignment.Left;
      BlockCollection blocks = this.Blocks;
      Inline[] inlineArray1 = new Inline[1];
      Inline[] inlineArray2 = inlineArray1;
      int index = 0;
      Run run1 = new Run();
      run1.FontWeight = FontWeights.Bold;
      run1.Text = CopyAnnotationTextTemplate.GenerateHeaderText(annotation);
      Run run2 = run1;
      inlineArray2[index] = (Inline) run2;
      Paragraph paragraph = this.CreateParagraph((IEnumerable<Inline>) inlineArray1);
      blocks.Add((Block) paragraph);
      this.Blocks.AddRange((IEnumerable) this.BreakTextIntoParagraphs(annotation.Text));
    }

    private Paragraph CreateParagraph(IEnumerable<Inline> inlines)
    {
      Paragraph paragraph = new Paragraph();
      paragraph.Inlines.AddRange((IEnumerable) inlines);
      return paragraph;
    }

    private IEnumerable<Paragraph> BreakTextIntoParagraphs(string text)
    {
      if (string.IsNullOrEmpty(text))
        return Enumerable.Empty<Paragraph>();
      text = text.Replace("\r\n", "\n").Replace("\r", "\n");
      return Enumerable.Select<string, Paragraph>((IEnumerable<string>) Regex.Split(text, "\n"), (Func<string, Paragraph>) (line => this.CreateParagraph((IEnumerable<Inline>) new Inline[1]
      {
        (Inline) new Run()
        {
          Text = line
        }
      })));
    }

    private static string GenerateHeaderText(RawAnnotation annotation)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1}) {2}:", (object) annotation.Author, (object) annotation.SerialNumber, (object) annotation.Timestamp.ToLocalTime().ToString("g", (IFormatProvider) CultureInfo.CurrentCulture));
    }
  }
}
