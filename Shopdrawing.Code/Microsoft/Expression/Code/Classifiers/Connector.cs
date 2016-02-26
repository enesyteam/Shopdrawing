// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.Connector
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.CodeAid;
using Microsoft.Expression.Code.CodeAid.Xaml;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.AssetSystem;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.UI.Undo;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.Expression.Code.Classifiers
{
  internal sealed class Connector
  {
    [UndoTransactionMarker("TopMarker")]
    [ExtensionProduction(typeof (IUndoTransactionMarkerDataExtension))]
    private static DataExtension TopMarkerProperty = DataExtension.Default;

    [ExtensionProduction(typeof (IClassifierProviderExtension))]
    [ContentType("text.xml")]
    private IClassifier GetXmlClassifier(ITextBuffer bufferToClassify, IEnvironment context)
    {
      return (IClassifier) new StatefulClassifier<XamlTokenizerContext>(bufferToClassify, (IClassificationScanner<XamlTokenizerContext>) new XamlTokenizer(), context);
    }

    [ExtensionProduction(typeof (IClassifierProviderExtension))]
    [ContentType("text.xaml")]
    private IClassifier GetXamlClassifier(ITextBuffer bufferToClassify, IEnvironment context)
    {
      return (IClassifier) new StatefulClassifier<XamlTokenizerContext>(bufferToClassify, (IClassificationScanner<XamlTokenizerContext>) new XamlTokenizer(), context);
    }

    [ExtensionProduction(typeof (ICompletionProviderFactoryExtension))]
    [ContentType("text.xaml")]
    private ICompletionProvider GetXmlProvider(ITextBuffer buffer, IEnvironment context)
    {
      return (ICompletionProvider) new XamlCompletionProvider(buffer);
    }

    [ContentType("text.xml")]
    [ExtensionProduction(typeof (IIntellisensePresenterFactoryExtension))]
    private IIntellisensePresenter GetXmlIntellisensePresenter(IIntellisenseSession session)
    {
      ICompletionSession completionSession = session as ICompletionSession;
      if (completionSession != null && completionSession.Completions != null && completionSession.Completions.Count > 0)
        return (IIntellisensePresenter) new CodeAidIntellisensePresenter(session);
      return (IIntellisensePresenter) null;
    }
  }
}
