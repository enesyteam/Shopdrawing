// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.XamlCompletionProvider
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  internal class XamlCompletionProvider : ICompletionProvider
  {
    private XamlCodeAidEngine xamlCodeAidEngine;
    private ITextBuffer textBuffer;

    internal XamlCompletionProvider(ITextBuffer textBuffer)
    {
      this.textBuffer = textBuffer;
    }

    private void EnsureCodeAidEngine()
    {
      if (this.xamlCodeAidEngine != null)
        return;
      this.xamlCodeAidEngine = this.textBuffer.Properties.GetProperty<XamlCodeAidEngine>((object) "XamlCodeAidEngine");
    }

    public ReadOnlyCollection<ICompletion> GetCompletions(ICompletionSession session)
    {
      this.EnsureCodeAidEngine();
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.XamlIntellisenseGetCompletions);
      List<ICompletion> list1 = new List<ICompletion>();
      this.xamlCodeAidEngine.GetCompletions(this, session, (IList) list1);
      Func<ICompletion, string> func = (Func<ICompletion, string>) (completion => completion.DisplayText);
      IList<ICompletion> list2 = (IList<ICompletion>) Enumerable.ToList<ICompletion>(EnumerableExtensions.DistinctOnOrdered<ICompletion>((IEnumerable<ICompletion>) Enumerable.OrderBy<ICompletion, string>((IEnumerable<ICompletion>) list1, func), (IComparer<ICompletion>) new KeyComparer<ICompletion, string>(func)));
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.XamlIntellisenseGetCompletions);
      return new ReadOnlyCollection<ICompletion>(list2);
    }

    public bool TryGetBestMatch(ICompletionSession session, out ICompletion bestMatchingCompletion, out CompletionSelectionOptions selectionOptions)
    {
      throw new NotImplementedException();
    }
  }
}
