// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.EventHandlerProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class EventHandlerProvider : IEventHandlerProvider
  {
    private DesignerContext designerContext;
    private SceneDocument document;
    private ITypeDeclaration classModel;

    private bool ShouldSaveFiles
    {
      get
      {
        return this.designerContext.CodeModelService.ShouldSaveFiles;
      }
    }

    public EventHandlerProvider(DesignerContext designerContext, SceneDocument document, ITypeDeclaration classModel)
    {
      this.designerContext = designerContext;
      this.document = document;
      this.classModel = classModel;
    }

    public IDisposable SetHandlerScope()
    {
      return this.designerContext.ExternalChanges.DelayNotification();
    }

    public bool AddEventHandler(Type returnType, string methodName, ICollection<IParameterDeclaration> parameters)
    {
      if (this.classModel == null)
        return false;
      if (this.ShouldSaveFiles)
        this.SaveFiles();
      using (TemporaryCursor.SetWaitCursor())
        return this.classModel.AddMethod(returnType, methodName, (IEnumerable<IParameterDeclaration>) parameters);
    }

    public void SetHandler(DocumentCompositeNode compositeNode, IEvent eventKey, string methodName)
    {
      IDocumentContext context = compositeNode.Context;
      DocumentNode documentNode = (DocumentNode) null;
      if (!string.IsNullOrEmpty(methodName))
        documentNode = (DocumentNode) context.CreateNode((ITypeId) eventKey.PropertyType, (IDocumentNodeValue) new DocumentNodeStringValue(methodName));
      using (SceneEditTransaction editTransaction = this.document.CreateEditTransaction(StringTable.EventHandlerUndo))
      {
        compositeNode.Properties[(IPropertyId) eventKey] = documentNode;
        editTransaction.Commit();
      }
      if (!this.designerContext.CodeModelService.ShouldSaveFiles)
        return;
      this.SaveFiles();
    }

    public void ReportInvalidHandlerName(string methodName)
    {
      this.designerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.EventHandlerInvalidMethodNameDialogMessage, new object[1]
      {
        (object) methodName
      }));
    }

    public void ReportEventHandlerNotAdded()
    {
      this.designerContext.MessageDisplayService.ShowError(StringTable.EventHandlerNotAddedDialogMessage);
    }

    private void SaveFiles()
    {
      this.designerContext.CommandService.Execute("Application_SaveAll", CommandInvocationSource.Internally);
    }
  }
}
