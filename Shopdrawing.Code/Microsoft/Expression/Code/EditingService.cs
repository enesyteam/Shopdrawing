// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.EditingService
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.AssetSystem.CompositionModel.Runtime;
using Microsoft.VisualStudio.AssetSystem.Loader;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.UI.Text.AdornmentLibrary.Squiggles;
using Microsoft.VisualStudio.UI.Undo;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.Code
{
  internal class EditingService : ITextEditorService, ITextBufferService
  {
    public static readonly IVariableDescription TextViewHostEnvironmentVariable = (IVariableDescription) new EditingService.VariableDescription((object) null, "TextViewHost", typeof (IWpfTextViewHost));
    private bool ready;
    private CodeOptionsModel codeOptionsModel;
    private IContentTypeRegistry contentTypeRegistry;
    private ITextBufferFactory textBufferFactoryProvider;
    private ITextEditorFactory textEditorFactoryProvider;
    private IEditorOperationsProvider editorCommandsProvider;
    private ITextBufferUndoManagerProvider undoManagerProvider;
    private ISquiggleProviderFactory squiggleProviderFactory;
    private ICompletionBrokerMap completionBrokerMap;
    private IUndoHistoryRegistry undoHistoryRegistry;
    private Loader loader;
    private IServiceProvider serviceProvider;

    internal CodeOptionsModel CodeOptionsModel
    {
      get
      {
        return this.codeOptionsModel;
      }
    }

    public EditingService(IServiceProvider serviceProvider, CodeOptionsModel codeOptionsModel)
    {
      this.serviceProvider = serviceProvider;
      this.codeOptionsModel = codeOptionsModel;
    }

    public ICodeAidProvider GetCodeAidProvider()
    {
      return (ICodeAidProvider) this.serviceProvider.GetService(typeof (ICodeAidProvider));
    }

    public void Dispose()
    {
      if (this.loader == null)
        return;
      this.loader.Dispose();
      this.loader = (Loader) null;
      this.textBufferFactoryProvider = (ITextBufferFactory) null;
      this.textEditorFactoryProvider = (ITextEditorFactory) null;
      this.editorCommandsProvider = (IEditorOperationsProvider) null;
      this.undoManagerProvider = (ITextBufferUndoManagerProvider) null;
      this.contentTypeRegistry = (IContentTypeRegistry) null;
      this.squiggleProviderFactory = (ISquiggleProviderFactory) null;
      this.completionBrokerMap = (ICompletionBrokerMap) null;
      this.undoHistoryRegistry = (IUndoHistoryRegistry) null;
    }

    private void Initialize()
    {
      if (this.ready)
        return;
      if (this.loader == null)
      {
        this.loader = Loader.CreateLoader(((EditingService.CompiledCompositionEntryPoint) Delegate.CreateDelegate(typeof (EditingService.CompiledCompositionEntryPoint), Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof (EditingService)).Location), "Microsoft.Nautilus.Composition.dll")).EntryPoint))());
        this.loader.Initialize();
      }
      this.textBufferFactoryProvider = this.loader.GetService<ITextBufferFactory>();
      this.textEditorFactoryProvider = this.loader.GetService<ITextEditorFactory>();
      this.editorCommandsProvider = this.loader.GetService<IEditorOperationsProvider>();
      this.undoManagerProvider = this.loader.GetService<ITextBufferUndoManagerProvider>();
      this.contentTypeRegistry = this.loader.GetService<IContentTypeRegistry>();
      this.squiggleProviderFactory = this.loader.GetService<ISquiggleProviderFactory>();
      this.completionBrokerMap = this.loader.GetService<ICompletionBrokerMap>();
      this.undoHistoryRegistry = this.loader.GetService<IUndoHistoryRegistry>();
      this.contentTypeRegistry.AddContentType("text.xml", "XML", (IEnumerable<string>) new string[1]
      {
        "text"
      });
      this.contentTypeRegistry.AddContentType("text.xaml", "XAML", (IEnumerable<string>) new string[1]
      {
        "text"
      });
      this.contentTypeRegistry.AddContentType("text.C#", "C#", (IEnumerable<string>) new string[1]
      {
        "text"
      });
      this.contentTypeRegistry.AddContentType("text.VB", "VB", (IEnumerable<string>) new string[1]
      {
        "text"
      });
      this.contentTypeRegistry.AddContentType("text.JS", "JS", (IEnumerable<string>) new string[1]
      {
        "text"
      });
      this.ready = true;
    }

    public ITextEditor CreateTextEditor(Microsoft.Expression.DesignModel.Text.ITextBuffer textBuffer)
    {
      EditableTextBuffer editableTextBuffer = textBuffer as EditableTextBuffer;
      if (editableTextBuffer != null)
        return (ITextEditor) this.CreateCodeEditor(editableTextBuffer.TextBuffer);
      return (ITextEditor) null;
    }

    public Microsoft.Expression.DesignModel.Text.ITextBuffer CreateTextBuffer()
    {
      return (Microsoft.Expression.DesignModel.Text.ITextBuffer) new EditableTextBuffer(this.CreateTextBuffer(string.Empty));
    }

    internal ITextBufferUndoManager CreateTextBuffer(string value)
    {
      this.Initialize();
      return this.undoManagerProvider.GetTextBufferUndoManager(this.textBufferFactoryProvider.CreateTextBuffer(value, this.contentTypeRegistry.GetContentType("text.xaml")));
    }

    internal ITextBufferUndoManager CreateTextBuffer(string contents, string documentType)
    {
      this.Initialize();
      return this.undoManagerProvider.GetTextBufferUndoManager(this.textBufferFactoryProvider.CreateTextBuffer(contents, this.contentTypeRegistry.GetContentType(documentType)));
    }

    internal ITextBufferUndoManager CreateTextBuffer(string contents, string documentType, out UndoTransactionMarker topUndoStackMarker)
    {
      ITextBufferUndoManager textBuffer = this.CreateTextBuffer(contents, documentType);
      topUndoStackMarker = this.undoHistoryRegistry.GetUndoTransactionMarker("TopMarker");
      return textBuffer;
    }

    internal CodeEditor CreateCodeEditor(Microsoft.VisualStudio.Text.ITextBuffer textBuffer)
    {
      EditingService.DefaultEnvironment defaultEnvironment = new EditingService.DefaultEnvironment();
      IWpfTextViewHost textViewHost = this.textEditorFactoryProvider.CreateTextViewHost(textBuffer, (IEnvironment) defaultEnvironment, false);
      defaultEnvironment.SetTextViewHost(textViewHost);
      IWpfTextView textView = textViewHost.TextView;
      IEditorOperations editorOperations = this.editorCommandsProvider.GetEditorOperations((ITextView) textView);
      ITextBufferUndoManager bufferUndoManager = this.undoManagerProvider.GetTextBufferUndoManager(textBuffer);
      IFindLogic service1 = this.loader.GetService<IFindLogic>();
      IClassificationTypeRegistry service2 = this.loader.GetService<IClassificationTypeRegistry>();
      IClassificationFormatMap service3 = this.loader.GetService<IClassificationFormatMap>();
      ISquiggleProvider squiggleProvider = this.squiggleProviderFactory.GetSquiggleProvider((ITextView) textView);
      ICompletionBroker brokerForTextView = this.completionBrokerMap.GetBrokerForTextView((ITextView) textView);
      IMessageDisplayService messageDisplayService = (IMessageDisplayService) this.serviceProvider.GetService(typeof (IMessageDisplayService));
      ICodeAidProvider codeAidProvider = this.GetCodeAidProvider();
      return new CodeEditor(textView, textViewHost, editorOperations, bufferUndoManager, service1, service2, service3, squiggleProvider, brokerForTextView, this.codeOptionsModel, messageDisplayService, codeAidProvider, (IEnvironment) defaultEnvironment);
    }

    private class DefaultEnvironment : IEnvironment
    {
      private IWpfTextViewHost textViewHost;

      public void SetTextViewHost(IWpfTextViewHost host)
      {
        this.textViewHost = host;
      }

      public object Get(IVariableDescription variableDescription)
      {
        if (variableDescription == EditingService.TextViewHostEnvironmentVariable)
          return (object) this.textViewHost;
        return (object) null;
      }

      public bool GetFromBindings(IVariableDescription variableDescription, out object result)
      {
        result = (object) null;
        return false;
      }
    }

    private class VariableDescription : IVariableDescription
    {
      public object DefaultValue { get; private set; }

      public string Name { get; private set; }

      public Type ValueType { get; private set; }

      public VariableDescription(object defaultValue, string name, Type valueType)
      {
        this.DefaultValue = defaultValue;
        this.Name = name;
        this.ValueType = valueType;
      }
    }

    private delegate CompiledCompositionRecord CompiledCompositionEntryPoint();
  }
}
