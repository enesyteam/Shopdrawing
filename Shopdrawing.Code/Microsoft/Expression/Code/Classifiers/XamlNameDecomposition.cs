// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.XamlNameDecomposition
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public class XamlNameDecomposition : IEquatable<XamlNameDecomposition>
  {
    private SnapshotSpan prefix;
    private SnapshotSpan typeSpecifier;
    private SnapshotSpan name;

    public bool IsTagName { get; private set; }

    public Span FullSpan
    {
      get
      {
        int start = 0;
        int num = 0;
        if (this.prefix.Length > 0)
        {
          start = this.prefix.Start;
          num = this.prefix.End + 1;
        }
        if (this.typeSpecifier.Length > 0)
        {
          if (start == 0)
            start = this.typeSpecifier.Start;
          num = this.typeSpecifier.End + 1;
        }
        if (this.name.Length > 0)
        {
          if (start == 0)
            start = this.name.Start;
          num = this.name.End;
        }
        if (start == 0 && num == 0)
          return new Span();
        return new Span(start, num - start);
      }
    }

    public Span PrefixSpan
    {
      get
      {
        return this.prefix.Span;
      }
    }

    public Span TypeSpecifierSpan
    {
      get
      {
        return this.typeSpecifier.Span;
      }
    }

    public Span NameSpan
    {
      get
      {
        return this.name.Span;
      }
    }

    public string PrefixText
    {
      get
      {
        if (!this.prefix.IsEmpty)
          return this.prefix.GetText();
        return string.Empty;
      }
    }

    public string TypeSpecifierText
    {
      get
      {
        if (!this.typeSpecifier.IsEmpty)
          return this.typeSpecifier.GetText();
        return string.Empty;
      }
    }

    public string NameText
    {
      get
      {
        if (!this.name.IsEmpty)
          return this.name.GetText();
        return string.Empty;
      }
    }

    public XamlNameDecomposition(IList<ClassificationSpan> classificationSpans, int firstIndex)
    {
      int index1 = firstIndex;
      if (classificationSpans[index1].ClassificationType != XamlAnalyzer.ClassTagNameIdentifier && classificationSpans[index1].ClassificationType != XamlAnalyzer.ClassAttrNameIdentifier)
        return;
      IClassificationType classificationType = classificationSpans[index1].ClassificationType;
      this.IsTagName = classificationType == XamlAnalyzer.ClassTagNameIdentifier;
      this.ParseTypeSpecifierAndName(classificationSpans[index1].Span);
      int end = classificationSpans[index1].Span.End;
      int index2 = index1 + 1;
      if (index2 == classificationSpans.Count || classificationSpans[index2].ClassificationType != XamlAnalyzer.ClassNameColon || classificationSpans[index2].Span.Start != end)
        return;
      int index3 = index2 + 1;
      this.prefix = this.name;
      if (index3 == classificationSpans.Count || classificationSpans[index3].ClassificationType != classificationType || index3 > 0 && classificationSpans[index3 - 1].Span.End != classificationSpans[index3].Span.Start)
        this.name = new SnapshotSpan(classificationSpans[index3 - 1].Span.Snapshot, classificationSpans[index3 - 1].Span.End, 0);
      else
        this.ParseTypeSpecifierAndName(classificationSpans[index3].Span);
    }

    private void ParseTypeSpecifierAndName(SnapshotSpan currentSpan)
    {
      int length = currentSpan.GetText().IndexOf('.');
      if (length < 0)
      {
        this.name = currentSpan;
      }
      else
      {
        this.typeSpecifier = new SnapshotSpan(currentSpan.Snapshot, new Span(currentSpan.Start, length));
        this.name = new SnapshotSpan(currentSpan.Snapshot, new Span(currentSpan.Start + length + 1, currentSpan.Length - length - 1));
      }
    }

    public Span NamePartForPosition(SnapshotPoint point)
    {
      XamlNamePositionFlags namePositionFlags = this.PositionFor(point);
      if ((namePositionFlags & XamlNamePositionFlags.PartMask) == XamlNamePositionFlags.Prefix)
        return new Span(this.PrefixSpan.Start, this.PrefixSpan.Length + 1);
      if ((namePositionFlags & XamlNamePositionFlags.PartMask) == XamlNamePositionFlags.TypeSpecifier)
        return this.TypeSpecifierSpan;
      if ((namePositionFlags & XamlNamePositionFlags.PartMask) == XamlNamePositionFlags.Name)
        return this.NameSpan;
      return new Span();
    }

    public XamlNamePositionFlags PositionFor(SnapshotPoint point)
    {
      if (this.name.Snapshot != null && this.name.Snapshot.Version != point.Snapshot.Version)
        throw new ArgumentException("wrong version", "point");
      if (this.prefix != new SnapshotSpan())
      {
        if (this.prefix.Start == point.Position)
          return XamlNamePositionFlags.BeforeStart | XamlNamePositionFlags.Prefix;
        if (this.prefix.End == point.Position)
          return XamlNamePositionFlags.AfterEnd | XamlNamePositionFlags.Prefix;
        if (this.prefix.Contains(point.Position))
          return XamlNamePositionFlags.InBetween | XamlNamePositionFlags.Prefix;
      }
      if (this.name != new SnapshotSpan())
      {
        if (this.name.Start == point.Position)
          return XamlNamePositionFlags.BeforeStart | XamlNamePositionFlags.Name;
        if (this.name.End == point.Position)
          return XamlNamePositionFlags.AfterEnd | XamlNamePositionFlags.Name;
        if (this.name.Contains(point.Position))
          return XamlNamePositionFlags.InBetween | XamlNamePositionFlags.Name;
      }
      if (this.typeSpecifier != new SnapshotSpan())
      {
        if (this.typeSpecifier.Start == point.Position)
          return XamlNamePositionFlags.BeforeStart | XamlNamePositionFlags.TypeSpecifier;
        if (this.typeSpecifier.End == point.Position)
          return XamlNamePositionFlags.AfterEnd | XamlNamePositionFlags.TypeSpecifier;
        if (this.typeSpecifier.Contains(point.Position))
          return XamlNamePositionFlags.InBetween | XamlNamePositionFlags.TypeSpecifier;
      }
      return XamlNamePositionFlags.None;
    }

    public override string ToString()
    {
      string prefixText = this.PrefixText;
      string typeSpecifierText = this.TypeSpecifierText;
      string nameText = this.NameText;
      string str = string.Empty;
      if (prefixText.Length > 0)
        str = str + prefixText + ":";
      if (typeSpecifierText.Length > 0)
        str = str + typeSpecifierText + ".";
      return str + nameText;
    }

    public bool Equals(XamlNameDecomposition other)
    {
      if (other != null)
        return this.FullSpan == other.FullSpan;
      return false;
    }
  }
}
