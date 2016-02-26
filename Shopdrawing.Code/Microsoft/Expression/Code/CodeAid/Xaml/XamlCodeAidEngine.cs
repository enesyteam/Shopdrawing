// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.XamlCodeAidEngine
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Classifiers;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  public class XamlCodeAidEngine : IDisposable
  {
    private List<char> currentExtendedXamlCharacters = new List<char>();
    private List<XamlCodeAidEngine.XamlNamespaceMemberInfo> xamlNamespaceMembers = new List<XamlCodeAidEngine.XamlNamespaceMemberInfo>()
    {
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "Class",
        Description = StringTable.XamlNsAttr_Class_Description,
        InRootTag = new bool?(true),
        InResourceDictionary = new bool?(false)
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "ClassModifier",
        Description = StringTable.XamlNsAttr_ClassModifier_Description,
        InRootTag = new bool?(true),
        InResourceDictionary = new bool?(false)
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "FieldModifier",
        Description = StringTable.XamlNsAttr_FieldModifier_Description,
        InRootTag = new bool?(),
        InResourceDictionary = new bool?()
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "Key",
        Description = StringTable.XamlNsAttr_Key_Description,
        InRootTag = new bool?(false),
        InResourceDictionary = new bool?(true)
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "Name",
        Description = StringTable.XamlNsAttr_Name_Description,
        InRootTag = new bool?(),
        InResourceDictionary = new bool?(false)
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "Shared",
        Description = StringTable.XamlNsAttr_Shared_Description,
        InRootTag = new bool?(false),
        InResourceDictionary = new bool?(true)
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "Subclass",
        Description = StringTable.XamlNsAttr_Subclass_Description,
        InRootTag = new bool?(true),
        InResourceDictionary = new bool?(false)
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "TypeArguments",
        Description = StringTable.XamlNsAttr_TypeArguments_Description,
        InRootTag = new bool?(true),
        InResourceDictionary = new bool?()
      },
      new XamlCodeAidEngine.XamlNamespaceMemberInfo()
      {
        Name = "Uid",
        Description = StringTable.XamlNsAttr_Uid_Description,
        InRootTag = new bool?(),
        InResourceDictionary = new bool?()
      }
    };
    private const string XamlNamespaceUri = "http://schemas.microsoft.com/winfx/2006/xaml";

    private XamlAnalyzer Analyzer { get; set; }

    public ICodeAidProvider CodeAidProvider { get; private set; }

    public XamlCodeAidEngine(ITextBuffer textBuffer, ICodeAidProvider codeAidProvider, IEnvironment environment)
    {
      this.CodeAidProvider = codeAidProvider;
      this.Analyzer = new XamlAnalyzer(textBuffer, environment);
      XamlCodeAidEngine property;
      if (textBuffer.Properties.TryGetProperty<XamlCodeAidEngine>((object) "XamlCodeAidEngine", out property))
        textBuffer.Properties.RemoveProperty((object) "XamlCodeAidEngine");
      textBuffer.Properties.AddProperty((object) "XamlCodeAidEngine", (object) this);
    }

    ~XamlCodeAidEngine()
    {
      this.Dispose(false);
    }

    internal string GetBeginningWhitespaceForContainingTag(ITextSnapshot textSnapshot, int caretPosition)
    {
      SnapshotSpan containingTag = this.Analyzer.GetContainingTag(new SnapshotPoint(textSnapshot, caretPosition));
      if (!containingTag.IsEmpty)
      {
        ITextSnapshotLine lineFromPosition = textSnapshot.GetLineFromPosition(containingTag.Start);
        if (lineFromPosition != null)
        {
          int whiteSpaceCharacter = lineFromPosition.GetPositionOfNextNonWhiteSpaceCharacter(0);
          if (whiteSpaceCharacter != -1)
            return textSnapshot.GetText(lineFromPosition.Start, whiteSpaceCharacter);
        }
      }
      return string.Empty;
    }

    internal CodeAidAction ProcessTextInput(string text, CodeAidContext context)
    {
      ITextSnapshot currentSnapshot = context.CurrentSnapshot;
      CodeAidActionType resultAction = CodeAidActionType.None;
      SnapshotSpan containingTag = this.Analyzer.GetContainingTag(context.CurrentPosition);
      XamlNameDecomposition nameAtPosition = this.Analyzer.GetNameAtPosition(context.CurrentPosition);
      bool flag1 = nameAtPosition != null && !nameAtPosition.IsTagName;
      bool flag2 = XamlCodeAidEngine.IsPositionInsideTagSpan(context.CurrentPosition, containingTag);
      ClassificationSpan previousSpan;
      ClassificationSpan currentSpan;
      bool isEndOfLine;
      bool stateAt = this.Analyzer.GetStateAt(context.CurrentPosition, out previousSpan, out currentSpan, out isEndOfLine);
      IClassificationType classificationType1 = previousSpan != null ? previousSpan.ClassificationType : (IClassificationType) null;
      IClassificationType currentType = currentSpan != null ? currentSpan.ClassificationType : (IClassificationType) null;
      IClassificationType classificationType2 = !stateAt ? currentType : classificationType1;
      SnapshotSpan containingTagSpan = new SnapshotSpan();
      if (text == "<")
      {
        if (!flag2 && !this.IsInsideCommentTag(context.CurrentPosition.Position, previousSpan, currentSpan))
          resultAction = CodeAidActionType.StartSession | CodeAidActionType.CommitSession;
      }
      else if (text == " ")
      {
        if (flag2)
        {
          bool flag3 = stateAt && currentType == XamlAnalyzer.ClassAttrEquals;
          if (currentType != XamlAnalyzer.ClassAttrValue && (currentType != XamlAnalyzer.ClassAttrEquals || flag3) && classificationType1 != XamlAnalyzer.ClassAttrEquals)
          {
            resultAction = context.IsSessionActive ? (XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.All) ? CodeAidActionType.CommitSession : CodeAidActionType.DismissSession) : CodeAidActionType.None;
            if (!flag3)
              resultAction |= CodeAidActionType.StartSession;
          }
        }
        else
        {
          bool isInCloseTag;
          containingTagSpan = this.Analyzer.GetContainingTag(context.CurrentPosition, true, out isInCloseTag);
          if (XamlCodeAidEngine.IsPositionInsideTagSpan(context.CurrentPosition, containingTagSpan) && isInCloseTag)
            resultAction = !context.IsSessionActive ? CodeAidActionType.None : (XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.ClosingTag) ? CodeAidActionType.CommitSession | CodeAidActionType.EatInput : CodeAidActionType.DismissSession);
        }
      }
      else if (!context.IsSessionActive && text.Length == 1 && (char.IsLetter(text[0]) || (int) text[0] == 95))
      {
        if (flag2 && currentType != XamlAnalyzer.ClassAttrValue)
          resultAction = CodeAidActionType.StartSession;
      }
      else if (text == ">")
      {
        resultAction = CodeAidActionType.DismissSession;
        if (XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.ClosingTag))
          resultAction = CodeAidActionType.CommitSession;
        else if (this.ShouldAutocompleteCloseTag(context, currentSnapshot, currentType, nameAtPosition, containingTag) && this.Analyzer.GetPreviousTagName(context.CurrentPosition) != null)
          resultAction = (CodeAidActionType) ((XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.All) ? 4 : 8) | 4096);
      }
      else if (text == "." || text == ":")
      {
        if (flag2 && currentType != XamlAnalyzer.ClassAttrValue && classificationType2 != XamlAnalyzer.ClassAttrValue)
        {
          bool flag3 = text == ":";
          CompletionType completionTypeFilter = flag3 ? CompletionType.Prefixes | CompletionType.XmlnsMarkup : CompletionType.All;
          CodeAidActionType codeAidActionType = XamlCodeAidEngine.IsCurrentCompletionValid(context, completionTypeFilter) ? CodeAidActionType.CommitSession : CodeAidActionType.DismissSession;
          CompletionType completionType = context.SessionSelectedCompletionCodeAid != null ? context.SessionSelectedCompletionCodeAid.CompletionType : CompletionType.None;
          if (flag3 && (completionType & CompletionType.Prefixes) == CompletionType.Prefixes)
            codeAidActionType |= CodeAidActionType.EatInput;
          if ((completionType & CompletionType.XmlnsMarkup) != CompletionType.XmlnsMarkup)
            codeAidActionType |= CodeAidActionType.StartSession;
          resultAction = codeAidActionType;
        }
      }
      else if (text == "=" || text == "\"")
      {
        if (flag1)
        {
          if (XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.All))
            resultAction = !XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.Properties | CompletionType.AttachedProperties | CompletionType.XmlnsMarkup) ? CodeAidActionType.DismissSession : CodeAidActionType.CommitSession | CodeAidActionType.EatInput;
          else if (flag2)
            resultAction = CodeAidActionType.StartSession | CodeAidActionType.DismissSession | CodeAidActionType.EatInput | CodeAidActionType.AutoGenAttrQuotes;
        }
      }
      else if (text == "/")
      {
        if (flag2)
        {
          resultAction = CodeAidActionType.DismissSession;
          if (classificationType1 == XamlAnalyzer.ClassStartTag)
            resultAction |= CodeAidActionType.StartSession;
        }
      }
      else if (!this.IsAllXamlIdentifierChars(text))
        resultAction = XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.All) ? CodeAidActionType.CommitSession : CodeAidActionType.DismissSession;
      else if (context.IsSessionActive)
        resultAction = CodeAidActionType.MatchTracking;
      XamlCodeAidEngine.CheckIfShouldAutoGenSomething(context, text, nameAtPosition, containingTagSpan, ref resultAction);
      return new CodeAidAction(resultAction);
    }

    private bool IsInsideCommentTag(int position, ClassificationSpan previousSpan, ClassificationSpan currentSpan)
    {
      if (previousSpan != null && previousSpan.ClassificationType == XamlAnalyzer.ClassComment && currentSpan == null)
      {
        if (position == previousSpan.Span.End && !previousSpan.Span.GetText().EndsWith(">", StringComparison.OrdinalIgnoreCase))
          return true;
      }
      else if (currentSpan != null && currentSpan.ClassificationType == XamlAnalyzer.ClassComment && previousSpan == null && position > currentSpan.Span.Start)
        return true;
      return false;
    }

    private bool ShouldAutocompleteCloseTag(CodeAidContext context, ITextSnapshot snapshot, IClassificationType currentType, XamlNameDecomposition nameAtPosition, SnapshotSpan containingTagSpan)
    {
      if ((int) context.CurrentPosition <= 0 || !(snapshot.GetText((int) context.CurrentPosition - 1, 1) != "/") || currentType == XamlAnalyzer.ClassAttrValue || currentType == XamlAnalyzer.ClassAttrNameIdentifier && (nameAtPosition == null || (int) context.CurrentPosition >= nameAtPosition.NameSpan.Start) || containingTagSpan.Length <= 0)
        return false;
      if (!(snapshot.GetText(containingTagSpan.End - 1, 1) == ">"))
        return true;
      SnapshotSpan selectionSpan = context.SelectionSpan;
      if (!context.SelectionSpan.IsEmpty)
        return context.SelectionSpan.Contains((int) context.CurrentPosition);
      return false;
    }

    private bool IsAllXamlIdentifierChars(string text)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        char c = text[index];
        if (!char.IsLetterOrDigit(c) && (int) c != 95 && !this.currentExtendedXamlCharacters.Contains(c))
          return false;
      }
      return true;
    }

    private static void CheckIfShouldAutoGenSomething(CodeAidContext context, string text, XamlNameDecomposition nameAtPosition, SnapshotSpan containingTagSpan, ref CodeAidActionType resultAction)
    {
      if ((resultAction & CodeAidActionType.CommitSession) != CodeAidActionType.CommitSession)
        return;
      if (string.IsNullOrEmpty(text) && context.SessionSelectedCompletionCodeAid != null && (context.SessionSelectedCompletionCodeAid.CompletionType == CompletionType.AttachedPropertyTypesOnly || context.SessionSelectedCompletionCodeAid.CompletionType == CompletionType.Prefixes))
      {
        resultAction |= CodeAidActionType.StartSession;
        if (context.SessionSelectedCompletionCodeAid.CompletionType != CompletionType.AttachedPropertyTypesOnly)
          return;
        resultAction |= CodeAidActionType.AutoGenDot;
      }
      else if (nameAtPosition != null && !nameAtPosition.IsTagName && (text != "." && text != ":") && (context.SessionSelectedCompletionCodeAid != null && (context.SessionSelectedCompletionCodeAid.CompletionType & (CompletionType.Properties | CompletionType.AttachedProperties | CompletionType.XmlnsMarkup)) != CompletionType.None))
      {
        int end = nameAtPosition.FullSpan.End;
        if (context.CurrentSnapshot.Length > end && !(context.CurrentSnapshot.GetText(end, 1) != "="))
          return;
        resultAction |= CodeAidActionType.AutoGenAttrQuotes;
        if (!(text != ">"))
          return;
        resultAction |= CodeAidActionType.StartSession;
      }
      else if (XamlCodeAidEngine.IsSelectionOfCompletionType(context.SessionSelectedCompletionCodeAid, CompletionType.StartCommentMarkup) || XamlCodeAidEngine.IsSelectionOfCompletionType(context.SessionHollowSelectedCompletionCodeAid, CompletionType.StartCommentMarkup))
      {
        resultAction |= CodeAidActionType.AutoGenCloseComment;
      }
      else
      {
        if (containingTagSpan.IsEmpty || containingTagSpan.GetText().EndsWith(">", StringComparison.OrdinalIgnoreCase) || !XamlCodeAidEngine.IsSelectionOfCompletionType(context.SessionSelectedCompletionCodeAid, CompletionType.ClosingTag) && !XamlCodeAidEngine.IsSelectionOfCompletionType(context.SessionHollowSelectedCompletionCodeAid, CompletionType.ClosingTag))
          return;
        resultAction |= CodeAidActionType.AutoGenClosingBracket;
      }
    }

    private static bool IsSelectionOfCompletionType(ICodeAidCompletion completionCodeAid, CompletionType targetCompletionType)
    {
      if (completionCodeAid != null)
        return (completionCodeAid.CompletionType & targetCompletionType) != CompletionType.None;
      return false;
    }

    private static bool IsPositionInsideTagSpan(SnapshotPoint position, SnapshotSpan containingTagSpan)
    {
      ITextSnapshot snapshot = position.Snapshot;
      if (containingTagSpan.Contains((int) position))
        return true;
      if (containingTagSpan.Length <= 0 || !(snapshot.GetText(position.Position - 1, 1) != ">"))
        return false;
      if (containingTagSpan.Span.End != snapshot.Length)
        return containingTagSpan.Span.End == (int) position;
      return true;
    }

    internal CodeAidAction ProcessKeyDown(Key key, CodeAidContext context)
    {
      XamlNameDecomposition nameAtPosition = this.Analyzer.GetNameAtPosition(context.CurrentPosition);
      CodeAidActionType resultAction = CodeAidActionType.None;
      if (context.IsSessionActive)
      {
        switch (key)
        {
          case Key.Back:
          case Key.Left:
            resultAction = context.CurrentPosition.Position > context.SessionStartingPosition.GetPosition(context.CurrentSnapshot) ? CodeAidActionType.MatchTracking : CodeAidActionType.DismissSession;
            break;
          case Key.Tab:
            resultAction = CodeAidActionType.CommitSession | CodeAidActionType.EatInput | CodeAidActionType.CommitOnHollowSelection;
            break;
          case Key.Return:
            resultAction = !XamlCodeAidEngine.IsCurrentCompletionValid(context, CompletionType.All) ? CodeAidActionType.DismissSession : CodeAidActionType.CommitSession | CodeAidActionType.EatInput;
            break;
          case Key.Escape:
            resultAction = CodeAidActionType.DismissSession | CodeAidActionType.EatInput;
            break;
          case Key.Prior:
            resultAction = CodeAidActionType.EatInput | CodeAidActionType.MoveSelPageUp;
            break;
          case Key.Next:
            resultAction = CodeAidActionType.EatInput | CodeAidActionType.MoveSelPageDown;
            break;
          case Key.End:
          case Key.Home:
            resultAction = CodeAidActionType.DismissSession;
            break;
          case Key.Up:
            resultAction = CodeAidActionType.EatInput | CodeAidActionType.MoveSelUp;
            break;
          case Key.Right:
          case Key.Delete:
            resultAction = CodeAidActionType.DismissSession;
            break;
          case Key.Down:
            resultAction = CodeAidActionType.EatInput | CodeAidActionType.MoveSelDown;
            break;
        }
      }
      bool isInCloseTag;
      SnapshotSpan containingTagSpan = this.Analyzer.GetContainingTag(context.CurrentPosition, true, out isInCloseTag);
      if (containingTagSpan.Snapshot != null)
        resultAction = this.AddFormattingOptions(key, resultAction, containingTagSpan, context.CurrentPosition);
      if (!XamlCodeAidEngine.IsPositionInsideTagSpan(context.CurrentPosition, containingTagSpan))
        containingTagSpan = new SnapshotSpan();
      XamlCodeAidEngine.CheckIfShouldAutoGenSomething(context, string.Empty, nameAtPosition, containingTagSpan, ref resultAction);
      return new CodeAidAction(resultAction);
    }

    private CodeAidActionType AddFormattingOptions(Key key, CodeAidActionType actionType, SnapshotSpan containingTagSpan, SnapshotPoint currentPosition)
    {
      if (key == Key.Return && !actionType.HasFlag((Enum) CodeAidActionType.EatInput))
      {
        SnapshotSpan matchingEndTag = this.Analyzer.GetMatchingEndTag(containingTagSpan);
        int position = currentPosition.Position;
        if (this.IsBetweenStartEndTags(containingTagSpan, matchingEndTag, position, true) && this.IsBlankContentBetweenTags(containingTagSpan, matchingEndTag))
          actionType = actionType | CodeAidActionType.AutoGenIndent | CodeAidActionType.AutoGenNewLineBelow;
        else if (this.IsBetweenStartEndTags(containingTagSpan, matchingEndTag, position, false) && this.IsCursorOnSameLineAsTag(containingTagSpan, position))
          actionType |= CodeAidActionType.AutoGenIndent;
      }
      return actionType;
    }

    private bool IsBetweenStartEndTags(SnapshotSpan startTag, SnapshotSpan endTag, int cursorPosition, bool includeBoundaryForEndTag)
    {
      bool flag1 = this.IsTagClosed(startTag);
      bool flag2 = includeBoundaryForEndTag ? cursorPosition <= endTag.Start : cursorPosition < endTag.Start;
      bool flag3 = cursorPosition >= startTag.End;
      if (flag1 && flag3)
        return flag2;
      return false;
    }

    private bool IsTagClosed(SnapshotSpan tag)
    {
      if (tag.End == 0)
        return false;
      return tag.Snapshot.GetText(tag.End - 1, 1) == ">";
    }

    private bool IsBlankContentBetweenTags(SnapshotSpan startTag, SnapshotSpan endTag)
    {
      return endTag.Start == startTag.End;
    }

    private bool IsCursorOnSameLineAsTag(SnapshotSpan tag, int cursorPosition)
    {
      ITextSnapshot snapshot = tag.Snapshot;
      return snapshot.GetLineNumberFromPosition(tag.End) == snapshot.GetLineNumberFromPosition(cursorPosition);
    }

    private static bool IsCurrentCompletionValid(CodeAidContext context, CompletionType completionTypeFilter)
    {
      return context.IsSessionActive && context.SessionSelectedCompletionText != null && (context.SessionSelectedCompletionCodeAid.CompletionType & completionTypeFilter) != CompletionType.None;
    }

    internal static int NumCharsCompletionMatched(bool caseSensitive, string matchText, string bufferText)
    {
      int num = 0;
      for (int index = 0; index < bufferText.Length && index < matchText.Length; ++index)
      {
        char c1 = bufferText[index];
        char c2 = matchText[index];
        if (!caseSensitive)
        {
          c1 = char.ToLowerInvariant(c1);
          c2 = char.ToLowerInvariant(c2);
        }
        if ((int) c1 == (int) c2)
          ++num;
        else
          break;
      }
      return num;
    }

    private static bool TestCompletionClass(CompletionType flags, CompletionType flag)
    {
      return (flags & flag) == flag;
    }

    private bool IsInsideComment(bool isAtValidTokenBoundary, IClassificationType previousSpan, IClassificationType currentSpan)
    {
      if (!isAtValidTokenBoundary && currentSpan == XamlAnalyzer.ClassComment)
        return true;
      if (isAtValidTokenBoundary)
        return previousSpan == XamlAnalyzer.ClassComment;
      return false;
    }

    internal void GetCompletions(XamlCompletionProvider owner, ICompletionSession session, IList addTo)
    {
        ClassificationSpan classificationSpan;
        ClassificationSpan classificationSpan1;
        bool flag;
        ITrackingSpan trackingSpan;
        ICodeAidTypeInfo codeAidTypeInfo;
        List<ICodeAidTypeInfo> codeAidTypeInfos;
        bool flag1;
        ICodeAidTypeInfo codeAidTypeInfo1;
        List<ICodeAidTypeInfo> codeAidTypeInfos1;
        Span fullSpan;
        ClassificationPosition current;
        SnapshotSpan span;
        XamlNameDecomposition xamlNameDecomposition;
        IClassificationType classificationType;
        IClassificationType classificationType1;
        ICodeAidTypeInfo collectionItemType;
        ICodeAidTypeInfo defaultContentPropertyType;
        ICodeAidTypeInfo propertyType;
        this.currentExtendedXamlCharacters.Clear();
        ITextSnapshot currentSnapshot = this.Analyzer.TextBuffer.CurrentSnapshot;
        SnapshotPoint point = session.TriggerPoint.GetPoint(currentSnapshot);
        bool stateAt = this.Analyzer.GetStateAt(point, out classificationSpan, out classificationSpan1, out flag);
        if (classificationSpan != null)
        {
            classificationType = classificationSpan.ClassificationType;
        }
        else
        {
            classificationType = null;
        }
        IClassificationType classificationType2 = classificationType;
        if (classificationSpan1 != null)
        {
            classificationType1 = classificationSpan1.ClassificationType;
        }
        else
        {
            classificationType1 = null;
        }
        IClassificationType classificationType3 = classificationType1;
        bool flag2 = this.IsInsideComment(stateAt, classificationType2, classificationType3);
        if (!this.ShouldCompletionsBeAvailable(classificationType2, classificationType3, point) && !flag2)
        {
            return;
        }
        XamlNameDecomposition previousTagName = this.Analyzer.GetPreviousTagName(point);
        XamlCodeAidEngine.PositionContext nameAtPosition = new XamlCodeAidEngine.PositionContext();
        if (!flag2)
        {
            if (previousTagName != null)
            {
                fullSpan = previousTagName.FullSpan;
                if (fullSpan.IntersectsWith(new Span(point, 0)))
                {
                    goto Label1;
                }
                if (classificationType3 == XamlAnalyzer.ClassAttrValue)
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.AttributeValue;
                    SnapshotSpan snapshotSpan = classificationSpan1.Span;
                    span = classificationSpan1.Span;
                    int num = Math.Min(snapshotSpan.Start + 1, span.End);
                    span = classificationSpan1.Span;
                    int num1 = Math.Max(span.Length - 2, 0);
                    nameAtPosition.ApplicableToSpan = new Span(num, num1);
                    IEnumerator<ClassificationPosition> enumerator = this.Analyzer.BackwardClassificationPositions(point).GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        if (current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassAttrEquals && enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            if (current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassAttrNameIdentifier)
                            {
                                XamlAnalyzer analyzer = this.Analyzer;
                                current = enumerator.Current;
                                span = current.CurrentSpan.Span;
                                nameAtPosition.Name = analyzer.GetNameAtPosition(new SnapshotPoint(currentSnapshot, span.Start + 1));
                                if (nameAtPosition.Name != null)
                                {
                                    nameAtPosition.NameFlags = nameAtPosition.Name.PositionFor(point);
                                    goto Label0;
                                }
                                else
                                {
                                    goto Label0;
                                }
                            }
                            else
                            {
                                goto Label0;
                            }
                        }
                        else
                        {
                            goto Label0;
                        }
                    }
                    else
                    {
                        goto Label0;
                    }
                }
                else if (classificationType3 != XamlAnalyzer.ClassAttrEquals)
                {
                    XamlNameDecomposition nameAtPosition1 = this.Analyzer.GetNameAtPosition(point);
                    xamlNameDecomposition = nameAtPosition1;
                    nameAtPosition.Name = nameAtPosition1;
                    if (xamlNameDecomposition != null)
                    {
                        nameAtPosition.MarkupPosition = (nameAtPosition.Name.IsTagName ? XamlCodeAidEngine.MarkupPosition.ElementName : XamlCodeAidEngine.MarkupPosition.AttributeName);
                        nameAtPosition.NameFlags = nameAtPosition.Name.PositionFor(point);
                        goto Label0;
                    }
                    else
                    {
                        nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.AttributeName;
                        nameAtPosition.NameFlags = XamlNamePositionFlags.BeforeStart | XamlNamePositionFlags.Name;
                        goto Label0;
                    }
                }
                else
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.AttributeName;
                    IEnumerator<ClassificationPosition> enumerator1 = this.Analyzer.BackwardClassificationPositions(point).GetEnumerator();
                    if (enumerator1.MoveNext())
                    {
                        current = enumerator1.Current;
                        if (current.CurrentSpan.ClassificationType != XamlAnalyzer.ClassAttrNameIdentifier)
                        {
                            current = enumerator1.Current;
                            if (current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassNameColon && enumerator1.MoveNext())
                            {
                                current = enumerator1.Current;
                                if (current.CurrentSpan.ClassificationType != XamlAnalyzer.ClassAttrNameIdentifier)
                                {
                                    goto Label0;
                                }
                            }
                            else
                            {
                                goto Label0;
                            }
                        }
                        XamlAnalyzer xamlAnalyzer = this.Analyzer;
                        current = enumerator1.Current;
                        span = current.CurrentSpan.Span;
                        nameAtPosition.Name = xamlAnalyzer.GetNameAtPosition(new SnapshotPoint(currentSnapshot, span.Start + 1));
                        if (nameAtPosition.Name != null)
                        {
                            nameAtPosition.NameFlags = nameAtPosition.Name.PositionFor(point);
                            goto Label0;
                        }
                        else
                        {
                            goto Label0;
                        }
                    }
                    else
                    {
                        goto Label0;
                    }
                }
            }
        Label1:
            bool flag3 = false;
            if (classificationType3 == null)
            {
                if (classificationType2 == XamlAnalyzer.ClassStartClosingTag || classificationType2 == XamlAnalyzer.ClassTagNameIdentifier || classificationType2 == XamlAnalyzer.ClassStartTag)
                {
                    flag3 = true;
                }
            }
            else if (classificationType3 == XamlAnalyzer.ClassTagNameIdentifier)
            {
                flag3 = true;
            }
            IEnumerator<ClassificationPosition> enumerator2 = this.Analyzer.BackwardClassificationPositions(point, flag3).GetEnumerator();
            if (enumerator2.MoveNext())
            {
                current = enumerator2.Current;
                IClassificationType classificationType4 = current.CurrentSpan.ClassificationType;
                if (this.IsInCloseTag(enumerator2, classificationType4))
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.StartCloseTag;
                }
                else if (classificationType2 == XamlAnalyzer.ClassTagNameIdentifier || classificationType3 == XamlAnalyzer.ClassTagNameIdentifier)
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.ElementName;
                }
                else if (classificationType4 == XamlAnalyzer.ClassStartClosingTag)
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.StartCloseTag;
                }
                else if (classificationType4 == XamlAnalyzer.ClassEndEmptyTag || classificationType4 == XamlAnalyzer.ClassEndTag)
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.Other;
                }
                else
                {
                    nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.ElementName;
                }
                if (nameAtPosition.MarkupPosition == XamlCodeAidEngine.MarkupPosition.StartCloseTag)
                {
                    span = enumerator2.Current.CurrentSpan.Span;
                    int start = span.Start + 1;
                    int currentSpanIndex = enumerator2.Current.CurrentSpanIndex + 1;
                    ClassificationSpan currentSpan = enumerator2.Current.CurrentSpan;
                    while (true)
                    {
                        if (currentSpanIndex >= enumerator2.Current.CurrentSpanList.Count)
                        {
                            break;
                        }
                        ClassificationPosition classificationPosition = enumerator2.Current;
                        if (!XamlAnalyzer.IsTokenValidInCloseTag(classificationPosition.CurrentSpanList[currentSpanIndex].ClassificationType))
                        {
                            break;
                        }
                        if (enumerator2.Current.CurrentSpanList[currentSpanIndex].ClassificationType == XamlAnalyzer.ClassEndTag)
                        {
                            break;
                        }
                        currentSpan = enumerator2.Current.CurrentSpanList[currentSpanIndex];
                        currentSpanIndex++;
                    }
                    int end = currentSpan.Span.End;
                    nameAtPosition.ApplicableToSpan = new Span(start, end - start);
                }
            }
            XamlNameDecomposition xamlNameDecomposition1 = previousTagName;
            xamlNameDecomposition = xamlNameDecomposition1;
            nameAtPosition.Name = xamlNameDecomposition1;
            if (xamlNameDecomposition != null)
            {
                nameAtPosition.NameFlags = nameAtPosition.Name.PositionFor(point);
            }
            else
            {
                nameAtPosition.NameFlags = XamlNamePositionFlags.BeforeStart | XamlNamePositionFlags.Name;
            }
        }
        else
        {
            nameAtPosition.MarkupPosition = XamlCodeAidEngine.MarkupPosition.Comment;
        }
    Label0:
        CompletionType completionType = CompletionType.None;
        switch (nameAtPosition.MarkupPosition)
        {
            case XamlCodeAidEngine.MarkupPosition.ElementName:
                {
                    if (nameAtPosition.NamePart == XamlNamePositionFlags.Name && nameAtPosition.Name != null)
                    {
                        fullSpan = nameAtPosition.Name.TypeSpecifierSpan;
                        if (fullSpan.Length <= 0)
                        {
                            goto Label3;
                        }
                        completionType = completionType | CompletionType.Properties;
                        completionType = completionType | CompletionType.AttachedProperties;
                        goto Label2;
                    }
                Label3:
                    if (nameAtPosition.NamePart != XamlNamePositionFlags.Prefix)
                    {
                        completionType = completionType | CompletionType.Types;
                    }
                Label2:
                    if (nameAtPosition.Name == null || nameAtPosition.NamePart == XamlNamePositionFlags.Prefix)
                    {
                        completionType = completionType | CompletionType.Prefixes;
                    }
                    if (nameAtPosition.Name != null)
                    {
                        break;
                    }
                    completionType = completionType | CompletionType.StartCommentMarkup;
                    completionType = completionType | CompletionType.ClosingTag;
                    break;
                }
            case XamlCodeAidEngine.MarkupPosition.StartCloseTag:
                {
                    completionType = completionType | CompletionType.ClosingTag;
                    break;
                }
            case XamlCodeAidEngine.MarkupPosition.AttributeName:
                {
                    if (nameAtPosition.NamePart == XamlNamePositionFlags.Name && nameAtPosition.Name != null)
                    {
                        fullSpan = nameAtPosition.Name.TypeSpecifierSpan;
                        if (fullSpan.Length <= 0)
                        {
                            goto Label5;
                        }
                        completionType = completionType | CompletionType.AttachedProperties;
                        goto Label4;
                    }
                Label5:
                    if (nameAtPosition.NamePart != XamlNamePositionFlags.Prefix)
                    {
                        completionType = completionType | CompletionType.Properties;
                        completionType = completionType | CompletionType.AttachedPropertyTypesOnly;
                        if (nameAtPosition.Name != null && "http://schemas.microsoft.com/winfx/2006/xaml".Equals(this.Analyzer.GetNamespaceUriForPrefix(new SnapshotSpan(currentSnapshot, nameAtPosition.Name.PrefixSpan)), StringComparison.OrdinalIgnoreCase))
                        {
                            completionType = completionType | CompletionType.XamlNamespaceMembers;
                        }
                    }
                Label4:
                    if (nameAtPosition.Name != null && nameAtPosition.NamePart != XamlNamePositionFlags.Prefix)
                    {
                        if (nameAtPosition.NamePart == XamlNamePositionFlags.Name)
                        {
                            fullSpan = nameAtPosition.Name.PrefixSpan;
                            if (fullSpan.IsEmpty)
                            {
                                fullSpan = nameAtPosition.Name.TypeSpecifierSpan;
                                if (!fullSpan.IsEmpty)
                                {
                                    goto Label6;
                                }
                            }
                            else
                            {
                                goto Label6;
                            }
                        }
                        else
                        {
                            goto Label6;
                        }
                    }
                    completionType = completionType | CompletionType.Prefixes;
                Label6:
                    if (nameAtPosition.Name != null)
                    {
                        fullSpan = nameAtPosition.Name.PrefixSpan;
                        if (!fullSpan.IsEmpty)
                        {
                            break;
                        }
                        fullSpan = nameAtPosition.Name.TypeSpecifierSpan;
                        if (!fullSpan.IsEmpty)
                        {
                            break;
                        }
                    }
                    completionType = completionType | CompletionType.XmlnsMarkup;
                    break;
                }
            case XamlCodeAidEngine.MarkupPosition.AttributeValue:
                {
                    completionType = completionType | CompletionType.EnumerationValues;
                    break;
                }
            case XamlCodeAidEngine.MarkupPosition.Comment:
                {
                    completionType = completionType | CompletionType.EndCommentMarkup;
                    break;
                }
        }
        if (!nameAtPosition.ApplicableToSpan.IsEmpty)
        {
            trackingSpan = currentSnapshot.CreateTrackingSpan(nameAtPosition.ApplicableToSpan, SpanTrackingMode.EdgeInclusive);
        }
        else if (nameAtPosition.Name == null || this.IsDirectlyBeforeAttribute(nameAtPosition, point))
        {
            trackingSpan = currentSnapshot.CreateTrackingSpan(new Span(session.TriggerPoint.GetPosition(currentSnapshot), 0), SpanTrackingMode.EdgeInclusive);
        }
        else
        {
            Span span1 = nameAtPosition.Name.NamePartForPosition(point);
            fullSpan = new Span();
            if (span1 == fullSpan)
            {
                if (nameAtPosition.MarkupPosition != XamlCodeAidEngine.MarkupPosition.AttributeName)
                {
                    span1 = new Span(point.Position, 0);
                }
                else
                {
                    fullSpan = nameAtPosition.Name.NameSpan;
                    int start1 = fullSpan.Start;
                    int position = point.Position;
                    fullSpan = nameAtPosition.Name.NameSpan;
                    span1 = new Span(start1, Math.Max(0, position - fullSpan.Start));
                }
            }
            trackingSpan = currentSnapshot.CreateTrackingSpan(span1, SpanTrackingMode.EdgeInclusive);
        }
        string defaultNamespaceUri = this.Analyzer.GetDefaultNamespaceUri(currentSnapshot);
        bool nameFlags = nameAtPosition.NameFlags == XamlNamePositionFlags.Prefix;
        string xamlNameUri = this.GetXamlNameUri(currentSnapshot, previousTagName, nameAtPosition.Name, nameFlags);
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.Types))
        {
            ICodeAidTypeInfo codeAidTypeInfo2 = null;
            ICodeAidTypeInfo codeAidTypeInfo3 = null;
            SnapshotSpan containingTag = this.Analyzer.GetContainingTag(point);
            XamlNameDecomposition xamlNameDecomposition2 = null;
            if (!containingTag.IsEmpty)
            {
                xamlNameDecomposition2 = this.Analyzer.GetTagAncestry(new SnapshotPoint(containingTag.Snapshot, Math.Max(containingTag.Start - 1, 0)), true).FirstOrDefault<XamlNameDecomposition>();
            }
            ICodeAidTypeInfo codeAidTypeInfo4 = null;
            if (!this.IsPropertyElementTag(xamlNameDecomposition2))
            {
                completionType = completionType | CompletionType.AttachedPropertyTypesOnly;
                if (this.IsElementTag(xamlNameDecomposition2))
                {
                    ICodeAidTypeInfo codeAidTypeForTagName = this.GetCodeAidTypeForTagName(currentSnapshot, xamlNameDecomposition2);
                    if (codeAidTypeForTagName != null)
                    {
                        defaultContentPropertyType = codeAidTypeForTagName.DefaultContentPropertyType;
                    }
                    else
                    {
                        defaultContentPropertyType = null;
                    }
                    codeAidTypeInfo4 = defaultContentPropertyType;
                    if ((codeAidTypeForTagName == null ? true : !codeAidTypeForTagName.IsDictionaryType))
                    {
                        codeAidTypeInfo3 = codeAidTypeForTagName;
                    }
                }
            }
            else
            {
                ICodeAidPropertyInfo codeAidPropertyForPropertyElementTag = this.GetCodeAidPropertyForPropertyElementTag(xamlNameDecomposition2);
                if (codeAidPropertyForPropertyElementTag != null)
                {
                    propertyType = codeAidPropertyForPropertyElementTag.PropertyType;
                }
                else
                {
                    propertyType = null;
                }
                codeAidTypeInfo4 = propertyType;
            }
            if (codeAidTypeInfo4 != null)
            {
                if (codeAidTypeInfo4 != null)
                {
                    collectionItemType = codeAidTypeInfo4.CollectionItemType;
                }
                else
                {
                    collectionItemType = null;
                }
                ICodeAidTypeInfo codeAidTypeInfo5 = collectionItemType;
                if (codeAidTypeInfo4 == null || !codeAidTypeInfo4.IsDictionaryType)
                {
                    codeAidTypeInfo2 = (codeAidTypeInfo5 != null ? codeAidTypeInfo5 : codeAidTypeInfo4);
                }
                else
                {
                    codeAidTypeInfo2 = null;
                }
            }
            this.GetUriNamespaceTypeCompletions(owner, session, trackingSpan, addTo, xamlNameUri, codeAidTypeInfo2, codeAidTypeInfo3);
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.Properties) && this.Analyzer.GetContainingTag(point).Length > 0)
        {
            ICodeAidTypeInfo codeAidTypeForTag = this.GetCodeAidTypeForTag(xamlNameUri, previousTagName.TypeSpecifierText, previousTagName.NameText);
            if (codeAidTypeForTag != null)
            {
                this.GetPropertyCompletionsForType(owner, session, trackingSpan, addTo, codeAidTypeForTag);
            }
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.Prefixes))
        {
            foreach (KeyValuePair<string, string> inScopePrefix in this.Analyzer.GetInScopePrefixes(point))
            {
                addTo.Add(new XamlCompletion(session, trackingSpan, XamlTokens.ElementName, CompletionType.Prefixes, owner, CodeAidMemberInfoTypes.CreatePrefixInfo(string.Concat(inScopePrefix.Key, ":"), inScopePrefix.Value)));
            }
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.AttachedPropertyTypesOnly) && this.GetAttachedPropertyAncestryInformation(point, previousTagName, previousTagName != nameAtPosition.Name, out codeAidTypeInfo, out codeAidTypeInfos))
        {
            this.GetAttachedPropertyTypeCompletions(owner, session, trackingSpan, codeAidTypeInfo, codeAidTypeInfos, addTo, xamlNameUri);
            if (!string.IsNullOrEmpty(xamlNameUri) && xamlNameUri != defaultNamespaceUri)
            {
                if (nameAtPosition.Name != null)
                {
                    fullSpan = nameAtPosition.Name.PrefixSpan;
                    if (!fullSpan.IsEmpty)
                    {
                        goto Label7;
                    }
                }
                this.GetAttachedPropertyTypeCompletions(owner, session, trackingSpan, codeAidTypeInfo, codeAidTypeInfos, addTo, defaultNamespaceUri);
            }
        }
    Label7:
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.XamlNamespaceMembers))
        {
            fullSpan = nameAtPosition.Name.FullSpan;
            bool flag4 = this.IsRangeInsideRootTag(currentSnapshot, fullSpan.End, out flag1);
            this.GetXamlNamespaceMembers(owner, session, trackingSpan, flag1, flag4, addTo);
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.AttachedProperties))
        {
            ICodeAidTypeInfo codeAidTypeForTag1 = this.GetCodeAidTypeForTag(xamlNameUri, nameAtPosition.Name.TypeSpecifierText, nameAtPosition.Name.NameText);
            if (codeAidTypeForTag1 == null && defaultNamespaceUri != xamlNameUri)
            {
                codeAidTypeForTag1 = this.GetCodeAidTypeForTag(defaultNamespaceUri, nameAtPosition.Name.TypeSpecifierText, nameAtPosition.Name.NameText);
            }
            if (codeAidTypeForTag1 != null && this.GetAttachedPropertyAncestryInformation(point, previousTagName, previousTagName != nameAtPosition.Name, out codeAidTypeInfo1, out codeAidTypeInfos1))
            {
                this.GetAttachedPropertyCompletionsForType(owner, session, trackingSpan, addTo, codeAidTypeForTag1, codeAidTypeInfo1, codeAidTypeInfos1);
            }
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.EnumerationValues) && nameAtPosition.Name != null && !nameAtPosition.Name.IsTagName)
        {
            this.GetValueCompletionsForProperty(owner, session, trackingSpan, addTo, previousTagName, nameAtPosition.Name);
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.ClosingTag))
        {
            this.GetCloseTagCompletions(point, nameAtPosition.MarkupPosition, owner, session, trackingSpan, addTo);
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.StartCommentMarkup))
        {
            this.currentExtendedXamlCharacters.Add('-');
            this.currentExtendedXamlCharacters.Add('!');
            addTo.Add(new XamlCompletion(session, trackingSpan, XamlTokens.Comment, CompletionType.StartCommentMarkup, owner, CodeAidMemberInfoTypes.CreateStartCommentInfo()));
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.EndCommentMarkup))
        {
            this.currentExtendedXamlCharacters.Add('-');
            addTo.Add(new XamlCompletion(session, trackingSpan, XamlTokens.Comment, CompletionType.EndCommentMarkup, owner, CodeAidMemberInfoTypes.CreateCloseCommentInfo()));
        }
        if (XamlCodeAidEngine.TestCompletionClass(completionType, CompletionType.XmlnsMarkup))
        {
            addTo.Add(new XamlCompletion(session, trackingSpan, XamlTokens.Attribute, CompletionType.XmlnsMarkup, owner, CodeAidMemberInfoTypes.CreateXmlnsInfo()));
        }
    }

    private bool IsDirectlyBeforeAttribute(XamlCodeAidEngine.PositionContext positionContext, int cursorPosition)
    {
      if (positionContext.MarkupPosition == XamlCodeAidEngine.MarkupPosition.AttributeName)
        return cursorPosition < positionContext.Name.FullSpan.Start;
      return false;
    }

    private bool ShouldCompletionsBeAvailable(IClassificationType previousSpan, IClassificationType currentSpan, SnapshotPoint position)
    {
      if (currentSpan == XamlAnalyzer.ClassEndEmptyTag && previousSpan == null && position.Snapshot.GetText(Math.Max(0, position.Position - 1), 1) == "/")
        return false;
      return this.Analyzer.IsInsideTag(position);
    }

    private bool GetAttachedPropertyAncestryInformation(SnapshotPoint position, XamlNameDecomposition tagName, bool isInsideAttribute, out ICodeAidTypeInfo targetType, out List<ICodeAidTypeInfo> ancestorChain)
    {
      SnapshotSpan containingTag = this.Analyzer.GetContainingTag(position);
      XamlNameDecomposition tagName1 = (XamlNameDecomposition) null;
      if (!containingTag.IsEmpty)
        tagName1 = this.Analyzer.GetNameAtPosition(new SnapshotPoint(position.Snapshot, Math.Min(containingTag.Start + 1, position.Snapshot.Length - 1))) ?? Enumerable.FirstOrDefault<XamlNameDecomposition>(this.Analyzer.GetTagAncestry(new SnapshotPoint(position.Snapshot, Math.Max(containingTag.Start - 1, 0)), true));
      targetType = (ICodeAidTypeInfo) null;
      ancestorChain = new List<ICodeAidTypeInfo>();
      if (tagName1 != null)
      {
        if (isInsideAttribute)
          targetType = this.GetCodeAidTypeForTagName(position.Snapshot, tagName1);
        foreach (XamlNameDecomposition tagName2 in this.Analyzer.GetTagAncestry(new SnapshotPoint(position.Snapshot, tagName1.NameSpan.End), true))
        {
          if (!tagName2.Equals(tagName) && tagName2.IsTagName && tagName2.TypeSpecifierSpan.IsEmpty)
          {
            ICodeAidTypeInfo aidTypeForTagName = this.GetCodeAidTypeForTagName(position.Snapshot, tagName2);
            if (targetType == null)
              targetType = aidTypeForTagName;
            else if (aidTypeForTagName != null)
              ancestorChain.Add(aidTypeForTagName);
          }
        }
      }
      return tagName1 != null;
    }

    private bool IsInCloseTag(IEnumerator<ClassificationPosition> previousPositions, IClassificationType classificationType)
    {
      if (classificationType == XamlAnalyzer.ClassTagNameIdentifier)
      {
        if (previousPositions.MoveNext() && (previousPositions.Current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartClosingTag || previousPositions.Current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassNameColon && previousPositions.MoveNext() && (previousPositions.Current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassTagNameIdentifier && previousPositions.MoveNext()) && previousPositions.Current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartClosingTag))
          return true;
      }
      else if (classificationType == XamlAnalyzer.ClassNameColon && previousPositions.MoveNext() && (previousPositions.Current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassTagNameIdentifier && previousPositions.MoveNext()) && previousPositions.Current.CurrentSpan.ClassificationType == XamlAnalyzer.ClassStartClosingTag)
        return true;
      return false;
    }

    private void GetCloseTagCompletions(SnapshotPoint position, XamlCodeAidEngine.MarkupPosition markupPosition, XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, IList addTo)
    {
      SnapshotSpan containingTag = this.Analyzer.GetContainingTag(position);
      if (containingTag.IsEmpty)
        return;
      int position1 = markupPosition == XamlCodeAidEngine.MarkupPosition.StartCloseTag ? containingTag.End : Math.Max(containingTag.Start, 0);
      XamlNameDecomposition name = Enumerable.FirstOrDefault<XamlNameDecomposition>(this.Analyzer.GetTagAncestry(new SnapshotPoint(containingTag.Snapshot, position1), true));
      if (name == null)
        return;
      addTo.Add((object) new XamlCompletion(session, editRegion, XamlTokens.ElementName, CompletionType.ClosingTag, owner, CodeAidMemberInfoTypes.CreateCloseTagInfo(name)));
    }

    private bool IsRangeInsideRootTag(ITextSnapshot snapshot, int end, out bool insideResourceDictionary)
    {
      insideResourceDictionary = false;
      bool flag = false;
      IEnumerable<XamlNameDecomposition> tagAncestry = this.Analyzer.GetTagAncestry(new SnapshotPoint(snapshot, end), true);
      if (tagAncestry != null)
      {
        IEnumerator<XamlNameDecomposition> enumerator = tagAncestry.GetEnumerator();
        if (!enumerator.MoveNext())
          return false;
        flag = !enumerator.MoveNext();
        ICodeAidTypeInfo codeAidTypeInfo = (ICodeAidTypeInfo) null;
        if (this.IsPropertyElementTag(enumerator.Current))
        {
          ICodeAidPropertyInfo propertyElementTag = this.GetCodeAidPropertyForPropertyElementTag(enumerator.Current);
          if (propertyElementTag != null)
            codeAidTypeInfo = propertyElementTag.PropertyType;
        }
        else
          codeAidTypeInfo = this.GetCodeAidTypeForTagName(snapshot, enumerator.Current);
        insideResourceDictionary = codeAidTypeInfo != null && codeAidTypeInfo.IsDictionaryType;
      }
      return flag;
    }

    private bool IsElementTag(XamlNameDecomposition potentialElementTag)
    {
      if (potentialElementTag != null && string.IsNullOrEmpty(potentialElementTag.TypeSpecifierText))
        return !string.IsNullOrEmpty(potentialElementTag.NameText);
      return false;
    }

    private bool IsPropertyElementTag(XamlNameDecomposition potentialPropertyElementTag)
    {
      if (potentialPropertyElementTag != null && !string.IsNullOrEmpty(potentialPropertyElementTag.TypeSpecifierText))
        return !string.IsNullOrEmpty(potentialPropertyElementTag.NameText);
      return false;
    }

    private ICodeAidPropertyInfo GetCodeAidPropertyForPropertyElementTag(XamlNameDecomposition propertyElementTagName)
    {
      if (propertyElementTagName == null)
        return (ICodeAidPropertyInfo) null;
      if (string.IsNullOrEmpty(propertyElementTagName.NameText) || string.IsNullOrEmpty(propertyElementTagName.TypeSpecifierText))
        return (ICodeAidPropertyInfo) null;
      string xamlNameUri = this.GetXamlNameUri(this.Analyzer.TextBuffer.CurrentSnapshot, propertyElementTagName, (XamlNameDecomposition) null, false);
      ICodeAidTypeInfo codeAidTypeInfo = (ICodeAidTypeInfo) null;
      if (!string.IsNullOrEmpty(xamlNameUri))
      {
        string clrNamespace;
        string assemblyName;
        codeAidTypeInfo = !XamlAnalyzer.CrackXamlPrefixNamespaceBinding(xamlNameUri, out clrNamespace, out assemblyName) ? this.CodeAidProvider.GetTypeByName(xamlNameUri, propertyElementTagName.TypeSpecifierText) : this.CodeAidProvider.GetTypeByName(assemblyName, clrNamespace, propertyElementTagName.TypeSpecifierText);
      }
      if (codeAidTypeInfo == null)
        return (ICodeAidPropertyInfo) null;
      return Enumerable.FirstOrDefault<ICodeAidMemberInfo>(codeAidTypeInfo.Properties, (Func<ICodeAidMemberInfo, bool>) (p => p.Name == propertyElementTagName.NameText)) as ICodeAidPropertyInfo;
    }

    private void GetValueCompletionsForProperty(XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, IList addTo, XamlNameDecomposition tagName, XamlNameDecomposition attrName)
    {
      ICodeAidPropertyInfo codeAidProperty = this.GetCodeAidProperty(tagName, attrName);
      if (codeAidProperty == null)
        return;
      ICodeAidTypeInfo propertyType = codeAidProperty.PropertyType;
      if (propertyType == null)
        return;
      bool flag = false;
      foreach (ICodeAidMemberInfo codeAidMemberInfo in propertyType.EnumerationValues)
      {
        ICodeAidMemberInfo memberInfo = codeAidMemberInfo;
        if (codeAidMemberInfo.Name == null)
        {
          flag = true;
          memberInfo = CodeAidMemberInfoTypes.CreateNullEnumInfo(this.Analyzer.GetPrefixForNamespaceUri("http://schemas.microsoft.com/winfx/2006/xaml", this.Analyzer.TextBuffer.CurrentSnapshot));
        }
        addTo.Add((object) new XamlCompletion(session, editRegion, XamlTokens.ElementName, CompletionType.EnumerationValues, owner, memberInfo));
      }
      if (!flag)
        return;
      this.currentExtendedXamlCharacters.Add('{');
      this.currentExtendedXamlCharacters.Add('}');
    }

    private ICodeAidPropertyInfo GetCodeAidProperty(XamlNameDecomposition tagName, XamlNameDecomposition attrName)
    {
      if (tagName == null || attrName == null)
        return (ICodeAidPropertyInfo) null;
      if (!string.IsNullOrEmpty(tagName.TypeSpecifierText))
        return (ICodeAidPropertyInfo) null;
      ITextSnapshot currentSnapshot = this.Analyzer.TextBuffer.CurrentSnapshot;
      string typeName = string.IsNullOrEmpty(attrName.TypeSpecifierText) ? tagName.NameText : attrName.TypeSpecifierText;
      string propertyName = attrName.NameText;
      if (string.IsNullOrEmpty(propertyName))
        return (ICodeAidPropertyInfo) null;
      string xamlNameUri = this.GetXamlNameUri(currentSnapshot, tagName, attrName, false);
      ICodeAidTypeInfo codeAidTypeInfo = (ICodeAidTypeInfo) null;
      if (!string.IsNullOrEmpty(xamlNameUri))
      {
        string clrNamespace;
        string assemblyName;
        codeAidTypeInfo = !XamlAnalyzer.CrackXamlPrefixNamespaceBinding(xamlNameUri, out clrNamespace, out assemblyName) ? this.CodeAidProvider.GetTypeByName(xamlNameUri, typeName) : this.CodeAidProvider.GetTypeByName(assemblyName, clrNamespace, typeName);
      }
      if (codeAidTypeInfo == null)
        return (ICodeAidPropertyInfo) null;
      return Enumerable.FirstOrDefault<ICodeAidMemberInfo>(codeAidTypeInfo.Properties, (Func<ICodeAidMemberInfo, bool>) (p => p.Name == propertyName)) as ICodeAidPropertyInfo ?? Enumerable.FirstOrDefault<ICodeAidMemberInfo>(codeAidTypeInfo.AllAttachedProperties, (Func<ICodeAidMemberInfo, bool>) (p => p.Name == propertyName)) as ICodeAidPropertyInfo;
    }

    private void GetXamlNamespaceMembers(XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, bool isInsideResourceDictionary, bool isInFirstTag, IList addTo)
    {
      foreach (XamlCodeAidEngine.XamlNamespaceMemberInfo namespaceMemberInfo in Enumerable.Where<XamlCodeAidEngine.XamlNamespaceMemberInfo>((IEnumerable<XamlCodeAidEngine.XamlNamespaceMemberInfo>) this.xamlNamespaceMembers, (Func<XamlCodeAidEngine.XamlNamespaceMemberInfo, bool>) (xnm =>
      {
        if (xnm.InRootTag.HasValue && xnm.InRootTag.Value != isInFirstTag)
          return false;
        if (xnm.InResourceDictionary.HasValue)
          return xnm.InResourceDictionary.Value == isInsideResourceDictionary;
        return true;
      })))
        addTo.Add((object) new XamlCompletion(session, editRegion, XamlTokens.Attribute, CompletionType.XamlNamespaceMembers, owner, CodeAidMemberInfoTypes.CreateXamlNamespaceAttributeInfo(namespaceMemberInfo.Name, namespaceMemberInfo.Description)));
    }

    private ICodeAidTypeInfo GetCodeAidTypeForTagName(ITextSnapshot snapshot, XamlNameDecomposition tagName)
    {
      return this.GetCodeAidTypeForTag(this.GetXamlNameUri(snapshot, tagName, tagName, false), tagName.TypeSpecifierText, tagName.NameText);
    }

    private string GetXamlNameUri(ITextSnapshot snapshot, XamlNameDecomposition tagName, XamlNameDecomposition contextName, bool isPrefix)
    {
      string defaultNamespaceUri = this.Analyzer.GetDefaultNamespaceUri(snapshot);
      return contextName == null || isPrefix || string.IsNullOrEmpty(contextName.PrefixText) ? (tagName == null || tagName.PrefixSpan.IsEmpty ? defaultNamespaceUri : this.Analyzer.GetNamespaceUriForPrefix(new SnapshotSpan(snapshot, tagName.PrefixSpan))) : this.Analyzer.GetNamespaceUriForPrefix(new SnapshotSpan(snapshot, contextName.PrefixSpan));
    }

    private void GetAttachedPropertyCompletionsForType(XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, IList results, ICodeAidTypeInfo attachedPropertyType, ICodeAidTypeInfo targetType, IEnumerable<ICodeAidTypeInfo> ancestorTypeChain)
    {
      foreach (ICodeAidMemberInfo memberInfo in attachedPropertyType.FilteredAttachedProperties(targetType, ancestorTypeChain))
        results.Add((object) new XamlCompletion(session, editRegion, XamlTokens.ElementName, CompletionType.AttachedProperties, owner, memberInfo));
    }

    private ICodeAidTypeInfo GetCodeAidTypeForTag(string uriNamespace, string typeSpec, string name)
    {
      if (string.IsNullOrEmpty(uriNamespace))
        return (ICodeAidTypeInfo) null;
      string typeName = name;
      if (!string.IsNullOrEmpty(typeSpec))
        typeName = typeSpec;
      if (string.IsNullOrEmpty(typeName))
        return (ICodeAidTypeInfo) null;
      string clrNamespace;
      string assemblyName;
      if (XamlAnalyzer.CrackXamlPrefixNamespaceBinding(uriNamespace, out clrNamespace, out assemblyName))
        return this.CodeAidProvider.GetTypeByName(assemblyName, clrNamespace, typeName);
      return this.CodeAidProvider.GetTypeByName(uriNamespace, typeName);
    }

    private void GetAttachedPropertyTypeCompletions(XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, ICodeAidTypeInfo targetType, List<ICodeAidTypeInfo> ancestorChain, IList results, string uriNamespace)
    {
      if (uriNamespace == null)
        return;
      string clrNamespace;
      string assemblyName;
      foreach (ICodeAidTypeInfo codeAidTypeInfo in !XamlAnalyzer.CrackXamlPrefixNamespaceBinding(uriNamespace, out clrNamespace, out assemblyName) ? this.CodeAidProvider.GetAttachedPropertyTypesInXmlNamespace(uriNamespace) : this.CodeAidProvider.GetAttachedPropertyTypesInClrNamespace(assemblyName, clrNamespace))
      {
        if (Enumerable.FirstOrDefault<ICodeAidMemberInfo>(codeAidTypeInfo.FilteredAttachedProperties(targetType, (IEnumerable<ICodeAidTypeInfo>) ancestorChain)) != null)
          results.Add((object) new XamlCompletion(session, editRegion, XamlTokens.ElementName, CompletionType.AttachedPropertyTypesOnly, owner, (ICodeAidMemberInfo) codeAidTypeInfo));
      }
    }

    private void GetUriNamespaceTypeCompletions(XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, IList results, string uriNamespace, ICodeAidTypeInfo baseType, ICodeAidTypeInfo derivedType)
    {
      if (string.IsNullOrEmpty(uriNamespace))
        return;
      string clrNamespace;
      string assemblyName;
      foreach (ICodeAidTypeInfo type in !XamlAnalyzer.CrackXamlPrefixNamespaceBinding(uriNamespace, out clrNamespace, out assemblyName) ? this.CodeAidProvider.GetTypesInXmlNamespace(uriNamespace) : this.CodeAidProvider.GetTypesInClrNamespace(assemblyName, clrNamespace))
      {
        if (baseType == null && derivedType == null || baseType != null && baseType.IsAssignableFrom(type) || derivedType != null && type.IsAssignableFrom(derivedType))
          results.Add((object) new XamlCompletion(session, editRegion, XamlTokens.ElementName, CompletionType.Types, owner, (ICodeAidMemberInfo) type));
      }
    }

    private void GetPropertyCompletionsForType(XamlCompletionProvider owner, ICompletionSession session, ITrackingSpan editRegion, IList results, ICodeAidTypeInfo typeInfo)
    {
      foreach (ICodeAidMemberInfo memberInfo in typeInfo.Properties)
        results.Add((object) new XamlCompletion(session, editRegion, XamlTokens.ElementName, CompletionType.Properties, owner, memberInfo));
    }

    internal string GetAutoCloseTag(CodeAidContext context)
    {
      SnapshotSpan containingTag = this.Analyzer.GetContainingTag(context.CurrentPosition);
      ITextSnapshot snapshot = containingTag.Snapshot;
      if (containingTag.IsEmpty || containingTag.End < context.CurrentPosition.Position)
        return (string) null;
      XamlNameDecomposition nameAtPosition1 = this.Analyzer.GetNameAtPosition(new SnapshotPoint(snapshot, containingTag.Start + 1));
      if (nameAtPosition1 == null)
        return (string) null;
      SnapshotSpan matchingEndTag1 = this.Analyzer.GetMatchingEndTag(containingTag);
      XamlNameDecomposition nameAtPosition2;
      if (!matchingEndTag1.IsEmpty && (nameAtPosition2 = this.Analyzer.GetNameAtPosition(new SnapshotPoint(snapshot, matchingEndTag1.Start + 2))) != null && !(nameAtPosition2.ToString() != nameAtPosition1.ToString()))
      {
        SnapshotSpan selectionSpan = context.SelectionSpan;
        if (!context.SelectionSpan.Contains(matchingEndTag1.Span))
        {
          XamlNameDecomposition nameDecomposition = Enumerable.FirstOrDefault<XamlNameDecomposition>(this.Analyzer.GetTagAncestry(new SnapshotPoint(snapshot, containingTag.Start + 1), false));
          if (nameDecomposition != null && nameDecomposition.ToString() == nameAtPosition1.ToString())
          {
            SnapshotSpan matchingEndTag2 = this.Analyzer.GetMatchingEndTag(this.Analyzer.GetContainingTag(new SnapshotPoint(snapshot, nameDecomposition.FullSpan.Start + 1)));
            XamlNameDecomposition nameAtPosition3;
            if ((nameAtPosition3 = this.Analyzer.GetNameAtPosition(new SnapshotPoint(snapshot, matchingEndTag2.Start + 2))) == null || nameAtPosition3.ToString() != nameAtPosition1.ToString())
              return nameAtPosition1.ToString();
          }
          return (string) null;
        }
      }
      return nameAtPosition1.ToString();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
      if (!isDisposing || this.Analyzer == null || (this.Analyzer.TextBuffer == null || this.Analyzer.TextBuffer.Properties == null))
        return;
      this.Analyzer.TextBuffer.Properties.RemoveProperty((object) "XamlCodeAidEngine");
      this.Analyzer = (XamlAnalyzer) null;
    }

    private enum MarkupPosition
    {
      None,
      ElementName,
      StartCloseTag,
      AttributeName,
      AttributeValue,
      Comment,
      Content,
      Other,
    }

    private struct PositionContext
    {
      public XamlNamePositionFlags NameFlags;
      public XamlCodeAidEngine.MarkupPosition MarkupPosition;
      public XamlNameDecomposition Name;
      public Span ApplicableToSpan;

      public XamlNamePositionFlags NamePart
      {
        get
        {
          return this.NameFlags & XamlNamePositionFlags.PartMask;
        }
      }
    }

    private struct XamlNamespaceMemberInfo
    {
      public string Name { get; set; }

      public string Description { get; set; }

      public bool? InRootTag { get; set; }

      public bool? InResourceDictionary { get; set; }
    }
  }
}
