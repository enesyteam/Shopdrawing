// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.XamlDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal abstract class XamlDocumentType : DocumentType
  {
    private DesignerContext designerContext;

    protected DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\file_xaml_on_16x16.png";
      }
    }

    public override bool IsDefaultTypeForExtension
    {
      get
      {
        return false;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".xaml"
        };
      }
    }

    public override bool CanCreate
    {
      get
      {
        return true;
      }
    }

    public override bool CanView
    {
      get
      {
        return true;
      }
    }

    public override bool AllowVisualStudioEdit
    {
      get
      {
        return true;
      }
    }

    protected override IDictionary<string, string> MetadataInformation
    {
      get
      {
        return (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "Generator",
            "MSBuild:Compile"
          },
          {
            "SubType",
            "Designer"
          }
        };
      }
    }

    protected XamlDocumentType()
    {
      if (this.GetType().FullName.IndexOf("UnitTest", StringComparison.OrdinalIgnoreCase) == -1)
        throw new InvalidOperationException("The default constructor should only be used from unit tests.");
    }

    protected XamlDocumentType(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public override IDocument OpenDocument(IProjectItem projectItem, IProject project, bool isReadOnly)
    {
      DocumentReference documentReference = projectItem.DocumentReference;
      Stream stream = documentReference.GetStream(FileAccess.Read);
      if (stream != null)
      {
        Encoding encoding;
        string contents;
        using (stream)
          contents = DocumentReference.ReadDocumentContents(stream, out encoding);
        return (IDocument) this.CreateDocument(project, projectItem, contents, isReadOnly, encoding);
      }
      throw new FileNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentFileNotFound, new object[1]
      {
        (object) documentReference.Path
      }));
    }

    public override IDocument CreateDocument(IProjectItem projectItem, IProject project, string initialContents)
    {
      return (IDocument) this.CreateDocument(project, projectItem, initialContents, false, DocumentEncodingHelper.DefaultEncoding);
    }

    private SceneDocument CreateDocument(IProject project, IProjectItem projectItem, string contents, bool isReadOnly, Encoding encoding)
    {
      ITextBuffer textBuffer = this.designerContext.TextBufferService.CreateTextBuffer();
      textBuffer.SetText(0, textBuffer.Length, contents);
      return this.CreateDocument(project, projectItem, textBuffer, isReadOnly, encoding);
    }

    internal SceneDocument CreateDocument(IProject project, IProjectItem projectItem, ITextBuffer textBuffer, bool isReadOnly, Encoding encoding)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      DocumentReference documentReference = projectItem.DocumentReference;
      DocumentContext documentContext = DocumentContextHelper.CreateDocumentContext(project, projectContext, DocumentReferenceLocator.GetDocumentLocator(documentReference), projectItem.Properties["BuildAction"] == "Content");
      SceneXamlDocument xamlDocument = new SceneXamlDocument((IDocumentContext) documentContext, (IUndoService) new UndoService(), PlatformTypes.Object, textBuffer, encoding);
      if (xamlDocument.ParseErrorsCount > 0)
      {
        IErrorService errorManager = this.designerContext.ErrorManager;
        if (errorManager != null)
          errorManager.DisplayErrors();
      }
      SceneDocument sceneDocument = this.CreateSceneDocument(documentReference, xamlDocument, isReadOnly, this.designerContext);
      documentContext.DocumentLocator = (IDocumentLocator) sceneDocument;
      return sceneDocument;
    }

    internal virtual SceneDocument CreateSceneDocument(DocumentReference documentReference, SceneXamlDocument xamlDocument, bool isReadOnly, DesignerContext designerContext)
    {
      return new SceneDocument(documentReference, xamlDocument, isReadOnly, this.designerContext);
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}
