// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.TextSnapshotScannerSource
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text;
using MS.Internal.Design.Markup;

namespace Microsoft.Expression.Code.Classifiers
{
  internal class TextSnapshotScannerSource : IScannerSource
  {
    private SnapshotSpan span;

    public string Text
    {
      get
      {
        return this.span.GetText();
      }
    }

    public TextSnapshotScannerSource(SnapshotSpan snapshotSpan)
    {
      this.span = snapshotSpan;
    }
  }
}
