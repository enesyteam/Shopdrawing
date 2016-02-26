// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.XamlAnalyzer
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.Framework.Data;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using MS.Internal.Design.Markup;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public class XamlAnalyzer
  {
    public static readonly IClassificationType ClassStartTag = TokenClassificationStore.GetTokenType("xamlStartTag");
    public static readonly IClassificationType ClassStartClosingTag = TokenClassificationStore.GetTokenType("xamlStartClosingTag");
    public static readonly IClassificationType ClassTagNameIdentifier = TokenClassificationStore.GetTokenType("xamlTagNameIdentifier");
    public static readonly IClassificationType ClassAttrNameIdentifier = TokenClassificationStore.GetTokenType("xamlAttrNameIdentifier");
    public static readonly IClassificationType ClassNameColon = TokenClassificationStore.GetTokenType("xamlNameColon");
    public static readonly IClassificationType ClassAttrEquals = TokenClassificationStore.GetTokenType("xamlAttrEquals");
    public static readonly IClassificationType ClassAttrValue = TokenClassificationStore.GetTokenType("xamlAttrValue");
    public static readonly IClassificationType ClassEndTag = TokenClassificationStore.GetTokenType("xamlEndTag");
    public static readonly IClassificationType ClassEndEmptyTag = TokenClassificationStore.GetTokenType("xamlEndEmptyTag");
    public static readonly IClassificationType ClassNonTagContent = TokenClassificationStore.GetTokenType("xamlNonTagContent");
    public static readonly IClassificationType ClassComment = TokenClassificationStore.GetTokenType("xamlComment");
    public static readonly IClassificationType ClassUnknown = TokenClassificationStore.GetTokenType("xamlDontCare");
    private SnapshotSpan currentSpanCached = new SnapshotSpan();
    private XamlAnalyzer.Classifier statefulClassifier;
    private XamlAnalyzer.ClassificationScanner classificationScanner;
    private TextVersion cachedInfoVersion;
    private IList<ClassificationSpan> cachedClassificationSpans;

    public ITextBuffer TextBuffer { get; private set; }

    public XamlAnalyzer(ITextBuffer textBuffer, IEnvironment environment)
    {
      this.classificationScanner = new XamlAnalyzer.ClassificationScanner()
      {
        OwnerAnalyzer = this
      };
      this.statefulClassifier = new XamlAnalyzer.Classifier(textBuffer, (IClassificationScanner<XamlAnalyzer.LineState>) this.classificationScanner, environment);
      this.TextBuffer = textBuffer;
    }

    public static bool IsStartOfMarkup(IClassificationType type)
    {
      if (type != XamlAnalyzer.ClassStartTag)
        return type == XamlAnalyzer.ClassStartClosingTag;
      return true;
    }

    public static bool IsNameIdentifier(IClassificationType type)
    {
      if (type != XamlAnalyzer.ClassTagNameIdentifier)
        return type == XamlAnalyzer.ClassAttrNameIdentifier;
      return true;
    }

    public static bool IsEndOfMarkup(IClassificationType type)
    {
      if (type != XamlAnalyzer.ClassEndEmptyTag)
        return type == XamlAnalyzer.ClassEndTag;
      return true;
    }

    public static bool IsPartOfName(IClassificationType type)
    {
      if (type != XamlAnalyzer.ClassTagNameIdentifier && type != XamlAnalyzer.ClassAttrNameIdentifier)
        return type == XamlAnalyzer.ClassNameColon;
      return true;
    }

    internal static bool IsInsideTag(XamlAnalyzer.ParseState state)
    {
      if (state != XamlAnalyzer.ParseState.Content && state != XamlAnalyzer.ParseState.Error && state != XamlAnalyzer.ParseState.TagEnd)
        return state != XamlAnalyzer.ParseState.EmptyTagEnd;
      return false;
    }

    private static bool IsInsideClosingTag(IList<XamlAnalyzer.LineState> spans, int startSpanIndex)
    {
      for (int index = startSpanIndex; index >= 0; --index)
      {
        XamlAnalyzer.ParseState parseState1 = spans[index].ParseState;
        XamlAnalyzer.ParseState parseState2 = spans[Math.Max(0, index - 1)].ParseState;
        if (parseState1 == XamlAnalyzer.ParseState.ClosingTagStart)
          return true;
        if (parseState1 == XamlAnalyzer.ParseState.TagEnd || parseState1 == XamlAnalyzer.ParseState.EmptyTagEnd || parseState1 == XamlAnalyzer.ParseState.TagStart)
          return false;
        if (parseState2 == XamlAnalyzer.ParseState.ClosingTagStart && parseState1 == XamlAnalyzer.ParseState.ClosingTagNamePart)
          return true;
      }
      return false;
    }

    internal static bool IsAfterTagName(XamlAnalyzer.ParseState state)
    {
      if (state != XamlAnalyzer.ParseState.AfterTagName && state != XamlAnalyzer.ParseState.AttrEquals && state != XamlAnalyzer.ParseState.AttrNamePart)
        return state == XamlAnalyzer.ParseState.AttrPrefixColon;
      return true;
    }

    internal static bool Transition(Token token, bool wasWhitespaceSkipped, XamlAnalyzer.ParseState currentState, out XamlAnalyzer.ParseState newState, out IClassificationType classType)
    {
      if (wasWhitespaceSkipped && (currentState == XamlAnalyzer.ParseState.TagNamePart || currentState == XamlAnalyzer.ParseState.TagPrefixColon))
        currentState = XamlAnalyzer.ParseState.AfterTagName;
      switch (token)
      {
        case Token.Assign:
          newState = XamlAnalyzer.ParseState.AttrEquals;
          classType = XamlAnalyzer.ClassAttrEquals;
          return true;
        case Token.Colon:
          newState = currentState != XamlAnalyzer.ParseState.TagNamePart ? (currentState != XamlAnalyzer.ParseState.ClosingTagNamePart ? XamlAnalyzer.ParseState.AttrPrefixColon : XamlAnalyzer.ParseState.ClosingTagPrefixColon) : XamlAnalyzer.ParseState.TagPrefixColon;
          classType = XamlAnalyzer.ClassNameColon;
          return true;
        case Token.Comment:
          newState = currentState;
          classType = XamlAnalyzer.ClassComment;
          return true;
        case Token.EndOfLine:
          newState = currentState == XamlAnalyzer.ParseState.TagNamePart || currentState == XamlAnalyzer.ParseState.TagPrefixColon ? XamlAnalyzer.ParseState.AfterTagName : currentState;
          classType = XamlAnalyzer.ClassUnknown;
          return false;
        case Token.EndOfTag:
          newState = XamlAnalyzer.ParseState.TagEnd;
          classType = XamlAnalyzer.ClassEndTag;
          return true;
        case Token.EndOfSimpleTag:
          newState = XamlAnalyzer.ParseState.EmptyTagEnd;
          classType = XamlAnalyzer.ClassEndEmptyTag;
          return true;
        case Token.Identifier:
          if (XamlAnalyzer.IsAfterTagName(currentState) || currentState == XamlAnalyzer.ParseState.TagNamePart)
          {
            newState = XamlAnalyzer.ParseState.AttrNamePart;
            classType = XamlAnalyzer.ClassAttrNameIdentifier;
          }
          else if (XamlAnalyzer.IsInsideTag(currentState))
          {
            bool flag = false;
            if ((currentState & XamlAnalyzer.ParseState.IsClosingTag) == XamlAnalyzer.ParseState.IsClosingTag)
              flag = true;
            newState = flag ? XamlAnalyzer.ParseState.ClosingTagNamePart : XamlAnalyzer.ParseState.TagNamePart;
            classType = XamlAnalyzer.ClassTagNameIdentifier;
          }
          else
          {
            newState = currentState;
            classType = XamlAnalyzer.ClassUnknown;
          }
          return true;
        case Token.LiteralContentString:
          newState = XamlAnalyzer.ParseState.Content;
          classType = XamlAnalyzer.ClassNonTagContent;
          return true;
        case Token.StartOfClosingTag:
          newState = XamlAnalyzer.ParseState.ClosingTagStart;
          classType = XamlAnalyzer.ClassStartClosingTag;
          return true;
        case Token.StartOfTag:
          newState = XamlAnalyzer.ParseState.TagStart;
          classType = XamlAnalyzer.ClassStartTag;
          return true;
        case Token.StringLiteral:
          newState = XamlAnalyzer.ParseState.AfterTagName;
          classType = XamlAnalyzer.ClassAttrValue;
          return true;
        default:
          newState = currentState;
          classType = XamlAnalyzer.ClassUnknown;
          return false;
      }
    }

    internal static bool IsClosingTag(XamlAnalyzer.ParseState state)
    {
      return (state & XamlAnalyzer.ParseState.IsClosingTag) == XamlAnalyzer.ParseState.IsClosingTag;
    }

    internal static bool IsStartTag(XamlAnalyzer.ParseState state)
    {
      return (state & XamlAnalyzer.ParseState.IsClosingTag) != XamlAnalyzer.ParseState.IsClosingTag;
    }

    private void ValidateCachedLine(ITextSnapshot snapshot, int position)
    {
      ITextSnapshotLine lineFromPosition = snapshot.GetLineFromPosition(position);
      if (this.currentSpanCached.Contains(new Span(lineFromPosition.Start, lineFromPosition.Length)) && snapshot.Version == this.cachedInfoVersion)
        return;
      this.cachedClassificationSpans = this.statefulClassifier.GetClassificationSpans(new SnapshotSpan(snapshot, lineFromPosition.Start, lineFromPosition.Length));
    }

    private bool FindSpans(int position, out int previousSpanIndex, out int currentSpanIndex)
    {
      int num = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, int>(this.cachedClassificationSpans, position, (Func<int, ClassificationSpan, int>) ((pos, classSpan) => pos.CompareTo(classSpan.Span.Start)));
      if (num < 0)
      {
        int index = ~num - 1;
        if (index == -1)
        {
          currentSpanIndex = -1;
          previousSpanIndex = -1;
        }
        else if (this.cachedClassificationSpans[index].Span.Contains(position))
        {
          currentSpanIndex = index;
          previousSpanIndex = -1;
        }
        else
        {
          if (index == this.cachedClassificationSpans.Count - 1)
          {
            previousSpanIndex = index;
            currentSpanIndex = -1;
            return true;
          }
          currentSpanIndex = -1;
          previousSpanIndex = index;
        }
        return false;
      }
      currentSpanIndex = num;
      previousSpanIndex = num > 0 ? num - 1 : -1;
      if (previousSpanIndex >= 0 && this.cachedClassificationSpans[previousSpanIndex].Span.End < position)
        previousSpanIndex = -1;
      return true;
    }

    private ClassificationPosition CreateClassificationPosition(SnapshotPoint position)
    {
      SnapshotSpan spanToTokenize = this.statefulClassifier.GetSpanToTokenize(position);
      XamlAnalyzer.LineState state = this.statefulClassifier.GetState(spanToTokenize.Snapshot, spanToTokenize.Start);
      IList<ClassificationSpan> list = (IList<ClassificationSpan>) new List<ClassificationSpan>();
      IList<XamlAnalyzer.LineState> stateSpans = (IList<XamlAnalyzer.LineState>) new List<XamlAnalyzer.LineState>();
      XamlAnalyzer.LineState endState;
      this.classificationScanner.GetClassificationSpansWorker(spanToTokenize, list, stateSpans, state, out endState);
      int num = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, SnapshotPoint>(list, position, (Func<SnapshotPoint, ClassificationSpan, int>) ((pos, span) => pos.Position - span.Span.Start));
      if (num < 0)
        num = ~num - 1;
      return new ClassificationPosition()
      {
        CurrentLine = spanToTokenize,
        CurrentSpanList = list,
        CurrentSpanIndex = num
      };
    }

    public bool GetStateAt(SnapshotPoint position, out ClassificationSpan previousSpan, out ClassificationSpan currentSpan, out bool isEndOfLine)
    {
      this.ValidateCachedLine(position.Snapshot, position.Position);
      int previousSpanIndex;
      int currentSpanIndex;
      bool spans = this.FindSpans((int) position, out previousSpanIndex, out currentSpanIndex);
      previousSpan = previousSpanIndex >= 0 ? this.cachedClassificationSpans[previousSpanIndex] : (ClassificationSpan) null;
      currentSpan = currentSpanIndex >= 0 ? this.cachedClassificationSpans[currentSpanIndex] : (ClassificationSpan) null;
      isEndOfLine = currentSpanIndex == this.cachedClassificationSpans.Count - 1 || previousSpanIndex == this.cachedClassificationSpans.Count - 1;
      return spans;
    }

    public string GetDefaultNamespaceUri(ITextSnapshot snapshot)
    {
      return this.statefulClassifier.GetNamespaceUriForPrefix(new SnapshotSpan(snapshot, new Span()));
    }

    public string GetNamespaceUriForPrefix(SnapshotSpan prefixSpan)
    {
      return this.statefulClassifier.GetNamespaceUriForPrefix(prefixSpan);
    }

    public string GetPrefixForNamespaceUri(string namespaceUri, ITextSnapshot currentSnapshot)
    {
      return this.statefulClassifier.GetPrefixForNamespaceUri(namespaceUri, currentSnapshot);
    }

    public IEnumerable<KeyValuePair<string, string>> GetInScopePrefixes(SnapshotPoint position)
    {
      return this.statefulClassifier.GetInScopePrefixes(position);
    }

    public XamlNameDecomposition GetNameAtPosition(SnapshotPoint position)
    {
      IList<ClassificationSpan> classificationSpans = this.statefulClassifier.GetClassificationSpans(position.GetContainingLine().ExtentIncludingLineBreak);
      int index = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, int>(classificationSpans, position.Position, (Func<int, ClassificationSpan, int>) ((pos, span) => pos - span.Span.Start));
      if (index < 0)
        index = Math.Max(0, ~index - 1);
      if (this.CursorIsInSpanGap(classificationSpans, index, position.Position))
        return (XamlNameDecomposition) null;
      while (index > 0 && (classificationSpans[index].Span.Start == classificationSpans[index - 1].Span.End && XamlAnalyzer.IsPartOfName(classificationSpans[index - 1].ClassificationType)))
        --index;
      if (classificationSpans[index].ClassificationType == XamlAnalyzer.ClassTagNameIdentifier || classificationSpans[index].ClassificationType == XamlAnalyzer.ClassAttrNameIdentifier)
        return new XamlNameDecomposition(classificationSpans, index);
      return (XamlNameDecomposition) null;
    }

    private bool CursorIsInSpanGap(IList<ClassificationSpan> spans, int spanIndex, int cursorPosition)
    {
      return spans.Count == 0 || spans[spanIndex].Span.End < cursorPosition && (spanIndex == spans.Count - 1 || spans[spanIndex + 1].Span.Start >= cursorPosition) || spans[spanIndex].Span.Start >= cursorPosition && (spanIndex == 0 || spans[spanIndex - 1].Span.End < cursorPosition);
    }

    public static int TagDepthDelta(bool forward, IClassificationType classType)
    {
      int num = forward ? 1 : -1;
      if (classType == XamlAnalyzer.ClassStartTag)
        return num;
      if (classType == XamlAnalyzer.ClassStartClosingTag || classType == XamlAnalyzer.ClassEndEmptyTag)
        return -num;
      return 0;
    }

    public SnapshotSpan GetContainingTag(SnapshotPoint position)
    {
      bool isInCloseTag;
      return this.GetContainingTag(position, false, out isInCloseTag);
    }

    public SnapshotSpan GetContainingTag(SnapshotPoint position, bool shouldReturnCloseTagSpan, out bool isInCloseTag)
    {
      return this.GetContainingTag(position, shouldReturnCloseTagSpan, out isInCloseTag, false);
    }

    public bool IsInsideTag(SnapshotPoint position)
    {
      bool isInCloseTag;
      return !this.GetContainingTag(position, false, out isInCloseTag, true).IsEmpty;
    }

    private SnapshotSpan GetContainingTag(SnapshotPoint position, bool shouldReturnCloseTagSpan, out bool isInCloseTag, bool returnEmptySpanOnContent)
    {
      isInCloseTag = false;
      SnapshotSpan spanToTokenize = this.statefulClassifier.GetSpanToTokenize(position);
      XamlAnalyzer.LineState state1 = this.statefulClassifier.GetState(spanToTokenize.Snapshot, spanToTokenize.Start);
      IList<ClassificationSpan> list1 = (IList<ClassificationSpan>) new List<ClassificationSpan>();
      IList<XamlAnalyzer.LineState> list2 = (IList<XamlAnalyzer.LineState>) new List<XamlAnalyzer.LineState>();
      XamlAnalyzer.LineState endState;
      this.classificationScanner.GetClassificationSpansWorker(spanToTokenize, list1, list2, state1, out endState);
      int num1 = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, SnapshotPoint>(list1, position, (Func<SnapshotPoint, ClassificationSpan, int>) ((pos, span) => pos.Position - span.Span.Start));
      if (num1 < 0)
        num1 = ~num1;
      int startSpanIndex = num1 - 1;
      XamlAnalyzer.ParseState state2 = startSpanIndex >= 0 ? list2[startSpanIndex].ParseState : state1.ParseState;
      if (XamlAnalyzer.IsInsideClosingTag(list2, startSpanIndex))
      {
        isInCloseTag = true;
        ClassificationPosition startPosition = new ClassificationPosition()
        {
          CurrentLine = spanToTokenize,
          CurrentSpanList = list1,
          CurrentSpanIndex = startSpanIndex
        };
        int currentSpanIndex = startPosition.CurrentSpanIndex;
        if (startSpanIndex >= 0 && startPosition.CurrentSpan.ClassificationType != XamlAnalyzer.ClassStartClosingTag)
        {
          foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanBackward(startPosition))
          {
            if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartClosingTag)
            {
              startPosition = classificationPosition;
              currentSpanIndex = classificationPosition.CurrentSpanIndex;
              break;
            }
          }
        }
        if (shouldReturnCloseTagSpan)
        {
          int start = startPosition.CurrentSpanList[Math.Max(0, currentSpanIndex)].Span.Start;
          int end = startPosition.CurrentSpanList[Math.Max(0, currentSpanIndex)].Span.End;
          for (int index = currentSpanIndex + 1; index < startPosition.CurrentSpanList.Count && XamlAnalyzer.IsTokenValidInCloseTag(startPosition.CurrentSpanList[index].ClassificationType); ++index)
            end = startPosition.CurrentSpanList[index].Span.End;
          return new SnapshotSpan(startPosition.Snapshot, start, end - start);
        }
        int num2 = 1;
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanBackward(startPosition))
        {
          num2 += XamlAnalyzer.TagDepthDelta(false, classificationPosition.CurrentSpan.ClassificationType);
          if (num2 == 0)
          {
            startPosition = classificationPosition;
            break;
          }
        }
        if (num2 != 0)
          return new SnapshotSpan();
        int start1 = startPosition.CurrentSpan.Span.Start;
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanForward(startPosition))
        {
          if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndTag || classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndEmptyTag)
            return new SnapshotSpan(classificationPosition.Snapshot, new Span(start1, classificationPosition.CurrentSpan.Span.End - start1));
          if (classificationPosition.CurrentSpan.Span != startPosition.CurrentSpan.Span && (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartTag || classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartClosingTag))
            return new SnapshotSpan(classificationPosition.Snapshot, new Span(start1, classificationPosition.CurrentSpan.Span.Start - start1));
        }
      }
      else if (XamlAnalyzer.IsInsideTag(state2))
      {
        ClassificationPosition startPosition = new ClassificationPosition()
        {
          CurrentLine = spanToTokenize,
          CurrentSpanList = list1,
          CurrentSpanIndex = startSpanIndex
        };
        int start = -1;
        if (startPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartTag)
        {
          start = startPosition.CurrentSpan.Span.Start;
        }
        else
        {
          foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanBackward(startPosition))
          {
            if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartTag)
            {
              start = classificationPosition.CurrentSpan.Span.Start;
              break;
            }
          }
        }
        int num2 = startPosition.Snapshot.Length;
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanForward(startPosition))
        {
          if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndEmptyTag || classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndTag)
          {
            num2 = classificationPosition.CurrentSpan.Span.End;
            break;
          }
          if (classificationPosition.CurrentSpan.Span != startPosition.CurrentSpan.Span && (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartTag || classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartClosingTag))
          {
            num2 = classificationPosition.CurrentSpan.Span.Start;
            break;
          }
        }
        if (start >= 0 && num2 > start)
          return new SnapshotSpan(startPosition.Snapshot, new Span(start, num2 - start));
      }
      else
      {
        if (returnEmptySpanOnContent)
          return new SnapshotSpan();
        int num2 = state2 == XamlAnalyzer.ParseState.EmptyTagEnd ? 2 : 1;
        ClassificationPosition startPosition = new ClassificationPosition()
        {
          CurrentLine = spanToTokenize,
          CurrentSpanList = list1,
          CurrentSpanIndex = startSpanIndex
        };
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanBackward(startPosition))
        {
          num2 += XamlAnalyzer.TagDepthDelta(false, classificationPosition.CurrentSpan.ClassificationType);
          if (num2 == 0)
          {
            startPosition = classificationPosition;
            break;
          }
        }
        if (num2 != 0)
          return new SnapshotSpan();
        int start = startPosition.CurrentSpan.Span.Start;
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanForward(startPosition))
        {
          if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndTag)
            return new SnapshotSpan(classificationPosition.Snapshot, new Span(start, classificationPosition.CurrentSpan.Span.End - start));
        }
      }
      return new SnapshotSpan();
    }

    public static bool IsTokenValidInCloseTag(IClassificationType tokenType)
    {
      if (tokenType != XamlAnalyzer.ClassTagNameIdentifier && tokenType != XamlAnalyzer.ClassNameColon)
        return tokenType == XamlAnalyzer.ClassEndTag;
      return true;
    }

    public IEnumerable<XamlNameDecomposition> GetTagAncestry(SnapshotPoint point, bool includeSelf)
    {
      ITextSnapshot snapshot = point.Snapshot;
      SnapshotSpan containingTag = this.GetContainingTag(point);
      if (!containingTag.IsEmpty)
      {
        XamlNameDecomposition tagName = this.GetPreviousTagName(new SnapshotPoint(snapshot, containingTag.End));
        if (tagName != null)
        {
          if (includeSelf)
            yield return tagName;
          int lowWaterMark = containingTag.Start;
          while (!(containingTag = this.GetContainingTag(new SnapshotPoint(snapshot, containingTag.Start))).IsEmpty && containingTag.Start < lowWaterMark)
          {
            lowWaterMark = containingTag.Start;
            tagName = (XamlNameDecomposition) null;
            foreach (ClassificationPosition classificationPosition in this.Tokens(containingTag))
            {
              if ((tagName = this.GetNameAtPosition(new SnapshotPoint(snapshot, classificationPosition.CurrentSpan.Span.Start))) != null)
              {
                yield return tagName;
                break;
              }
            }
          }
        }
      }
    }

    internal XamlNameDecomposition GetPreviousTagName(SnapshotPoint position)
    {
      SnapshotSpan spanToTokenize = this.statefulClassifier.GetSpanToTokenize(position);
      XamlAnalyzer.LineState state1 = this.statefulClassifier.GetState(spanToTokenize.Snapshot, spanToTokenize.Start);
      IList<ClassificationSpan> list = (IList<ClassificationSpan>) new List<ClassificationSpan>();
      IList<XamlAnalyzer.LineState> stateSpans = (IList<XamlAnalyzer.LineState>) new List<XamlAnalyzer.LineState>();
      XamlAnalyzer.LineState endState;
      this.classificationScanner.GetClassificationSpansWorker(spanToTokenize, list, stateSpans, state1, out endState);
      int num1 = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, SnapshotPoint>(list, position, (Func<SnapshotPoint, ClassificationSpan, int>) ((pos, span) => pos.Position - span.Span.Start));
      if (num1 < 0)
        num1 = ~num1;
      int index = num1 - 1;
      XamlAnalyzer.ParseState state2;
      if (index < 0)
      {
        state2 = state1.ParseState;
      }
      else
      {
        state2 = stateSpans[index].ParseState;
        switch (state2)
        {
          case XamlAnalyzer.ParseState.EmptyTagEnd:
          case XamlAnalyzer.ParseState.TagEnd:
            if (list[index].Span.IntersectsWith(new Span(position.Position, 0)))
            {
              state2 = index != 0 ? stateSpans[index - 1].ParseState : stateSpans[index].ParseState;
              break;
            }
            break;
        }
      }
      int num2 = -1;
      if (XamlAnalyzer.IsInsideTag(state2))
      {
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanBackward(new ClassificationPosition()
        {
          CurrentLine = spanToTokenize,
          CurrentSpanList = list,
          CurrentSpanIndex = index
        }))
        {
          if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartTag)
          {
            num2 = classificationPosition.CurrentSpan.Span.Start;
            break;
          }
          if (classificationPosition.CurrentSpan.ClassificationType != XamlAnalyzer.ClassStartClosingTag)
          {
            if (classificationPosition.CurrentSpan.ClassificationType != XamlAnalyzer.ClassEndEmptyTag)
            {
              if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndTag)
                break;
            }
            else
              break;
          }
          else
            break;
        }
      }
      if (num2 == -1)
        return (XamlNameDecomposition) null;
      return this.GetNameAtPosition(new SnapshotPoint(position.Snapshot, num2 + 1));
    }

    public SnapshotSpan GetMatchingEndTag(SnapshotSpan startTagSpan)
    {
      SnapshotSpan spanToTokenize = this.statefulClassifier.GetSpanToTokenize(startTagSpan);
      IList<ClassificationSpan> classificationSpans = this.statefulClassifier.GetClassificationSpans(spanToTokenize);
      int index = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, int>(classificationSpans, startTagSpan.Start, (Func<int, ClassificationSpan, int>) ((pos, span) => pos - span.Span.Start));
      if (index < 0 || classificationSpans[index].ClassificationType != XamlAnalyzer.ClassStartTag)
        return new SnapshotSpan();
      int num1 = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, int>(classificationSpans, startTagSpan.End, (Func<int, ClassificationSpan, int>) ((pos, span) => pos - span.Span.Start));
      if (num1 < 0)
        num1 = ~num1;
      ClassificationPosition startPosition = new ClassificationPosition()
      {
        CurrentLine = spanToTokenize,
        CurrentSpanList = classificationSpans,
        CurrentSpanIndex = num1
      };
      int num2 = 1;
      int start = -1;
      foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanForward(startPosition))
      {
        num2 += XamlAnalyzer.TagDepthDelta(true, classificationPosition.CurrentSpan.ClassificationType);
        if (num2 == 0)
        {
          start = classificationPosition.CurrentSpan.Span.Start;
          startPosition = classificationPosition;
          break;
        }
      }
      if (start >= 0)
      {
        foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanForward(startPosition))
        {
          if (classificationPosition.CurrentSpan.ClassificationType == XamlAnalyzer.ClassEndTag)
            return new SnapshotSpan(classificationPosition.Snapshot, new Span(start, classificationPosition.CurrentSpan.Span.End - start));
        }
      }
      return new SnapshotSpan();
    }

    public IEnumerable<ClassificationPosition> BackwardClassificationPositions(SnapshotPoint position, bool includeStartPosition)
    {
      ClassificationPosition startPosition = this.CreateClassificationPosition(position);
      if (includeStartPosition)
        yield return startPosition;
      foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanBackward(startPosition))
        yield return classificationPosition;
    }

    public IEnumerable<ClassificationPosition> BackwardClassificationPositions(SnapshotPoint position)
    {
      return this.BackwardClassificationPositions(position, false);
    }

    public IEnumerable<ClassificationPosition> ForwardClassificationPositions(SnapshotPoint position)
    {
      return this.statefulClassifier.ScanForward(this.CreateClassificationPosition(position));
    }

    public static bool CrackXamlPrefixNamespaceBinding(string xmlns, out string clrNamespace, out string assemblyName)
    {
      return XamlParser.TryParseClrNamespaceUri(xmlns, out clrNamespace, out assemblyName);
    }

    public IEnumerable<ClassificationPosition> Tokens(SnapshotSpan spanToTokenize)
    {
      if (!spanToTokenize.IsEmpty)
      {
        SnapshotSpan startTagLine = this.statefulClassifier.GetSpanToTokenize(spanToTokenize);
        IList<ClassificationSpan> spans = this.statefulClassifier.GetClassificationSpans(startTagLine);
        int startTagStartSpanIndex = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, int>(spans, spanToTokenize.Start, (Func<int, ClassificationSpan, int>) ((pos, span) => pos - span.Span.Start));
        if (startTagStartSpanIndex >= 0 && spans[startTagStartSpanIndex].ClassificationType == XamlAnalyzer.ClassStartTag)
        {
          int startTagEndSpanIndex = OrderedListExtensions.GenericBinarySearch<ClassificationSpan, int>(spans, spanToTokenize.End, (Func<int, ClassificationSpan, int>) ((pos, span) => pos - span.Span.Start));
          if (startTagEndSpanIndex < 0)
            startTagEndSpanIndex = ~startTagEndSpanIndex;
          ClassificationPosition interestingPosition = new ClassificationPosition()
          {
            CurrentLine = startTagLine,
            CurrentSpanList = spans,
            CurrentSpanIndex = startTagStartSpanIndex
          };
          foreach (ClassificationPosition classificationPosition in this.statefulClassifier.ScanForward(interestingPosition))
          {
            if (classificationPosition.CurrentSpan.Span.Start >= spanToTokenize.Span.End)
              break;
            yield return classificationPosition;
          }
        }
      }
    }

    [Flags]
    internal enum ParseState
    {
      Content = 0,
      IsClosingTag = 1,
      TagStart = 2,
      TagNamePart = 4,
      TagPrefixColon = TagNamePart | TagStart,
      AfterTagName = 10,
      ClosingTagStart = TagStart | IsClosingTag,
      ClosingTagNamePart = TagNamePart | IsClosingTag,
      ClosingTagPrefixColon = ClosingTagNamePart | TagStart,
      AttrNamePart = 12,
      AttrPrefixColon = AttrNamePart | TagStart,
      AttrEquals = 18,
      TagEnd = 20,
      EmptyTagEnd = TagEnd | TagStart,
      Error = 24,
    }

    internal struct LineState : IComparable<XamlAnalyzer.LineState>
    {
      public XamlAnalyzer.ParseState ParseState { get; set; }

      public int ScannerState { get; set; }

      public int CompareTo(XamlAnalyzer.LineState other)
      {
        int num = this.ParseState.CompareTo((object) other.ParseState);
        if (num == 0)
          return this.ScannerState.CompareTo(other.ScannerState);
        return num;
      }
    }

    internal class ClassificationScanner : IClassificationScanner<XamlAnalyzer.LineState>
    {
      internal XamlAnalyzer OwnerAnalyzer;

      public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span, XamlAnalyzer.LineState startState, out XamlAnalyzer.LineState endState)
      {
        IList<ClassificationSpan> classSpans = (IList<ClassificationSpan>) new List<ClassificationSpan>();
        this.GetClassificationSpansWorker(span, classSpans, (IList<XamlAnalyzer.LineState>) null, startState, out endState);
        return classSpans;
      }

      private static bool GetNextToken(Scanner scanner, out Token token)
      {
        token = !scanner.StillInsideMultiLineToken ? scanner.ScanNextToken() : Token.EndOfFile;
        return token != Token.EndOfFile;
      }

      internal void GetClassificationSpansWorker(SnapshotSpan span, IList<ClassificationSpan> classSpans, IList<XamlAnalyzer.LineState> stateSpans, XamlAnalyzer.LineState startState, out XamlAnalyzer.LineState endState)
      {
        classSpans.Clear();
        if (stateSpans != null)
          stateSpans.Clear();
        Scanner scanner = new Scanner((IScannerSource) new TextSnapshotScannerSource(span), new ScannerErrorHandler(this.HandleScannerError));
        scanner.ScannerState = startState.ScannerState;
        ITextSnapshot snapshot = span.Snapshot;
        endState = startState;
        Token token;
        while (XamlAnalyzer.ClassificationScanner.GetNextToken(scanner, out token))
        {
          XamlAnalyzer.ParseState newState;
          IClassificationType classType;
          XamlAnalyzer.Transition(token, scanner.WasWhitespaceSkipped, endState.ParseState, out newState, out classType);
          endState.ParseState = newState;
          endState.ScannerState = scanner.ScannerState;
          if (classType != XamlAnalyzer.ClassUnknown && classType != XamlAnalyzer.ClassNonTagContent)
          {
            classSpans.Add(new ClassificationSpan(new SnapshotSpan(snapshot, scanner.StartPos + span.Start, scanner.EndPos - scanner.StartPos), classType));
            if (stateSpans != null)
              stateSpans.Add(endState);
          }
        }
      }

      private void HandleScannerError(int offset, int length, Error error, params object[] args)
      {
      }
    }

    internal class Classifier : StatefulClassifier<XamlAnalyzer.LineState>, IClassifier
    {
      private Dictionary<string, string> cachedPrefixBindings = new Dictionary<string, string>();
      private string cachedDefaultNamespaceUri;
      private TextVersion lastCachedVersion;

      internal Classifier(ITextBuffer textBuffer, IClassificationScanner<XamlAnalyzer.LineState> scanner, IEnvironment environment)
        : base(textBuffer, scanner, environment)
      {
      }

      private void ValidatePrefixBindingCache(SnapshotSpan prefixLocationSpan)
      {
        if (this.lastCachedVersion != null && this.lastCachedVersion.Equals((object) prefixLocationSpan.Snapshot.Version))
          return;
        this.cachedPrefixBindings.Clear();
        this.cachedDefaultNamespaceUri = (string) null;
        ITextSnapshot snapshot = prefixLocationSpan.Snapshot;
        SnapshotSpan spanToTokenize = this.GetSpanToTokenize(new SnapshotPoint(snapshot, 0));
        foreach (ClassificationPosition classificationPosition in this.ScanForward(new ClassificationPosition()
        {
          CurrentLine = spanToTokenize,
          CurrentSpanList = this.GetClassificationSpans(spanToTokenize),
          CurrentSpanIndex = 0
        }))
        {
          ClassificationSpan currentSpan = classificationPosition.CurrentSpan;
          IList<ClassificationSpan> currentSpanList = classificationPosition.CurrentSpanList;
          int currentSpanIndex = classificationPosition.CurrentSpanIndex;
          if (currentSpan.ClassificationType != XamlAnalyzer.ClassEndTag)
          {
            if (currentSpan.ClassificationType == XamlAnalyzer.ClassAttrNameIdentifier && currentSpan.Span.GetText() == "xmlns")
            {
              XamlNameDecomposition nameDecomposition = new XamlNameDecomposition(currentSpanList, currentSpanIndex);
              if (nameDecomposition.PrefixText == "xmlns" && (!nameDecomposition.NameSpan.IsEmpty && currentSpanList.Count > currentSpanIndex + 4 && currentSpanList[currentSpanIndex + 4].ClassificationType == XamlAnalyzer.ClassAttrValue))
              {
                string text = currentSpanList[currentSpanIndex + 4].Span.GetText();
                this.cachedPrefixBindings[nameDecomposition.NameText] = text.Substring(1, text.Length - 2);
              }
              else if (nameDecomposition.PrefixSpan.IsEmpty && nameDecomposition.NameText == "xmlns" && (currentSpanList.Count > currentSpanIndex + 2 && currentSpanList[currentSpanIndex + 2].ClassificationType == XamlAnalyzer.ClassAttrValue))
              {
                string text = currentSpanList[currentSpanIndex + 2].Span.GetText();
                this.cachedDefaultNamespaceUri = text.Substring(1, text.Length - 2);
              }
            }
          }
          else
            break;
        }
        this.lastCachedVersion = snapshot.Version;
      }

      internal string GetNamespaceUriForPrefix(SnapshotSpan prefixSpan)
      {
        this.ValidatePrefixBindingCache(prefixSpan);
        string str = (string) null;
        if (prefixSpan.IsEmpty)
          str = this.cachedDefaultNamespaceUri;
        else
          this.cachedPrefixBindings.TryGetValue(prefixSpan.GetText(), out str);
        return str;
      }

      internal string GetPrefixForNamespaceUri(string namespaceUri, ITextSnapshot currentSnapshot)
      {
        this.ValidatePrefixBindingCache(new SnapshotSpan(currentSnapshot, new Span()));
        foreach (KeyValuePair<string, string> keyValuePair in this.cachedPrefixBindings)
        {
          if (keyValuePair.Value == namespaceUri)
            return keyValuePair.Key;
        }
        return (string) null;
      }

      IList<ClassificationSpan> IClassifier.GetClassificationSpans(SnapshotSpan trackingSpan)
      {
        return this.GetClassificationSpans(trackingSpan);
      }

      internal IEnumerable<KeyValuePair<string, string>> GetInScopePrefixes(SnapshotPoint position)
      {
        this.ValidatePrefixBindingCache(new SnapshotSpan(position.Snapshot, position.Position, 0));
        return (IEnumerable<KeyValuePair<string, string>>) this.cachedPrefixBindings;
      }
    }
  }
}
