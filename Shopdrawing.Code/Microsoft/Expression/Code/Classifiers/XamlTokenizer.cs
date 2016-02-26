// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.XamlTokenizer
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using MS.Internal.Design.Markup;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Code.Classifiers
{
  public class XamlTokenizer : IClassificationScanner<XamlTokenizerContext>
  {
    private List<ClassificationSpan> tokens;
    private XamlTokenizerContext context;
    private Scanner scanner;
    private SnapshotSpan currentSpan;
    private int currentColorPosition;

    internal XamlTokenizer()
    {
    }

    private bool GetNextToken(out Token token)
    {
      token = !this.scanner.StillInsideMultiLineToken ? this.scanner.ScanNextToken() : Token.EndOfFile;
      return token != Token.EndOfFile;
    }

    private void ClassifyWorker()
    {
      Token token;
      while (this.GetNextToken(out token))
      {
        switch (token)
        {
          case Token.Assign:
            this.ApplyClassificationTo(XamlTokens.Text);
            break;
          case Token.Colon:
          case Token.Identifier:
            if ((this.context.State & XamlTokenizerState.InTag) != XamlTokenizerState.None)
            {
              if ((this.context.State & XamlTokenizerState.InTagPastName) != XamlTokenizerState.None || this.scanner.WasWhitespaceSkipped)
              {
                this.context.State &= ~XamlTokenizerState.InTagName;
                this.context.State |= XamlTokenizerState.InTagPastName;
                this.ApplyClassificationTo(XamlTokens.Attribute);
                break;
              }
              this.context.State |= XamlTokenizerState.InTagName;
              this.ApplyClassificationTo(XamlTokens.ElementName);
              break;
            }
            this.ApplyClassificationTo(XamlTokens.Text);
            break;
          case Token.Comment:
            this.ApplyClassificationTo(XamlTokens.Comment);
            break;
          case Token.CharacterData:
          case Token.LiteralContentString:
            this.ApplyClassificationTo(XamlTokens.Text);
            break;
          case Token.EndOfLine:
            if ((this.context.State & XamlTokenizerState.InTagName) != XamlTokenizerState.None)
            {
              this.context.State &= ~XamlTokenizerState.InTagName;
              this.context.State |= XamlTokenizerState.InTagPastName;
              break;
            }
            break;
          case Token.EndOfTag:
          case Token.EndOfSimpleTag:
            this.context.State &= ~(XamlTokenizerState.InTag | XamlTokenizerState.InTagName | XamlTokenizerState.InTagPastName);
            this.ApplyClassificationTo(XamlTokens.Tag);
            break;
          case Token.IllegalCharacter:
            this.ApplyClassificationTo(XamlTokens.Text);
            break;
          case Token.StartOfClosingTag:
          case Token.StartOfTag:
            this.context.State = XamlTokenizerState.InTag;
            this.ApplyClassificationTo(XamlTokens.Tag);
            break;
          case Token.StringLiteral:
            this.ApplyQuotesClassification();
            break;
        }
        this.context.ScannerState = this.scanner.ScannerState;
      }
    }

    private void ApplyQuotesClassification()
    {
      int num = Math.Min(this.scanner.EndPos, this.scanner.MaxPos) + this.currentSpan.Start;
      if (num < 0 || this.currentColorPosition > this.scanner.MaxPos + this.currentSpan.Start || this.currentColorPosition > num)
        return;
      string tokenSource = this.scanner.GetTokenSource();
      bool flag = false;
      if ((this.context.State & XamlTokenizerState.InQuotes) == XamlTokenizerState.None && tokenSource.Length > 0 && ((int) tokenSource[0] == 39 || (int) tokenSource[0] == 34))
      {
        flag = true;
        this.context.State |= XamlTokenizerState.InQuotes;
        this.tokens.Add(new ClassificationSpan(new SnapshotSpan(this.currentSpan.Snapshot, this.currentColorPosition, 1), XamlTokens.Quote));
        ++this.currentColorPosition;
      }
      if (this.currentColorPosition <= num)
      {
        if (flag && tokenSource.Length > 1 && (int) tokenSource[1] == 123)
          this.context.State |= XamlTokenizerState.InMarkupExtension;
        IClassificationType classification = (this.context.State & XamlTokenizerState.InMarkupExtension) != XamlTokenizerState.None ? XamlTokens.MarkupExtension : XamlTokens.QuotedString;
        int length = num - this.currentColorPosition - (this.scanner.StringLiteralTerminatedProperly ? 1 : 0);
        if (length > 0)
          this.tokens.Add(new ClassificationSpan(new SnapshotSpan(this.currentSpan.Snapshot, this.currentColorPosition, length), classification));
        if (this.scanner.StringLiteralTerminatedProperly)
        {
          this.context.State &= ~(XamlTokenizerState.InQuotes | XamlTokenizerState.InMarkupExtension);
          this.tokens.Add(new ClassificationSpan(new SnapshotSpan(this.currentSpan.Snapshot, num - 1, 1), XamlTokens.Quote));
        }
      }
      this.currentColorPosition = num;
    }

    private void ApplyClassificationTo(IClassificationType classification)
    {
      int num = Math.Min(this.scanner.EndPos, this.scanner.MaxPos) + this.currentSpan.Start;
      if (num < 0 || this.currentColorPosition > this.scanner.MaxPos + this.currentSpan.Start || this.currentColorPosition > num)
        return;
      if (this.tokens.Count > 0 && this.tokens[this.tokens.Count - 1].ClassificationType == classification)
      {
        int start = this.tokens[this.tokens.Count - 1].Span.Start;
        this.tokens[this.tokens.Count - 1] = new ClassificationSpan(new SnapshotSpan(this.currentSpan.Snapshot, start, num - start), classification);
      }
      else
        this.tokens.Add(new ClassificationSpan(new SnapshotSpan(this.currentSpan.Snapshot, this.currentColorPosition, num - this.currentColorPosition), classification));
      this.currentColorPosition = num;
    }

    private void HandleScannerError(int offset, int length, Error error, params object[] args)
    {
    }

    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span, XamlTokenizerContext startState, out XamlTokenizerContext endState)
    {
      this.tokens = new List<ClassificationSpan>();
      this.context = startState;
      if (!span.IsEmpty)
      {
        this.scanner = new Scanner((IScannerSource) new TextSnapshotScannerSource(span), new ScannerErrorHandler(this.HandleScannerError));
        this.scanner.ScannerState = startState.ScannerState;
        this.currentSpan = span;
        this.currentColorPosition = this.scanner.EndPos + span.Start;
        this.ClassifyWorker();
      }
      endState = this.context;
      this.currentSpan = new SnapshotSpan();
      return (IList<ClassificationSpan>) this.tokens;
    }
  }
}
