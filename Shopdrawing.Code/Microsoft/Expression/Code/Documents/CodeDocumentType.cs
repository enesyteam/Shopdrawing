// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CodeDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.Code.Documents
{
  internal abstract class CodeDocumentType : DocumentType
  {
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    private const string BuildTask = "Compile";
    private ICodeProjectService codeProjectService;
    private IViewService viewService;
    private CodeOptionsModel codeOptionsModel;
    private IWindowService windowService;

    internal virtual bool SupportsGlobalImports
    {
      get
      {
        return false;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return "Code";
      }
    }

    public CodeOptionsModel CodeOptionsModel
    {
      get
      {
        return this.codeOptionsModel;
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Compile";
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

    public override PreferredExternalEditCommand PreferredExternalEditCommand
    {
      get
      {
        return PreferredExternalEditCommand.ShellOpen;
      }
    }

    public override bool AllowVisualStudioEdit
    {
      get
      {
        return true;
      }
    }

    public virtual string ClassEndToken
    {
      get
      {
        return string.Empty;
      }
    }

    public virtual CodeDomProvider CodeDomProvider
    {
      get
      {
        return (CodeDomProvider) null;
      }
    }

    protected CodeDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
    {
      this.codeProjectService = codeProjectService;
      this.viewService = viewService;
      this.codeOptionsModel = codeOptionsModel;
      this.windowService = windowService;
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.Code.FileTable.GetImageSource(path);
    }

    public override IDocument OpenDocument(IProjectItem projectItem, IProject project, bool isReadOnly)
    {
      MemoryStream memoryStream = new MemoryStream();
      Encoding encoding = Encoding.UTF8;
      using (Stream stream = projectItem.DocumentReference.GetStream(FileAccess.Read))
      {
        if (stream == null)
          throw new FileNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentFileNotFound, new object[1]
          {
            (object) projectItem.DocumentReference.Path
          }));
        byte[] numArray = new byte[(int) stream.Length];
        stream.Read(numArray, 0, numArray.Length);
        encoding = DocumentReference.DetermineDocumentEncoding(numArray);
        memoryStream.Write(numArray, 0, numArray.Length);
        memoryStream.Position = 0L;
      }
      return this.CreateDocument(projectItem, project, (Stream) memoryStream, encoding, isReadOnly);
    }

    public override IDocument CreateDocument(IProjectItem projectItem, IProject project, string initialContents)
    {
      MemoryStream memoryStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, CodeDocumentType.DefaultEncoding);
      streamWriter.Write(initialContents);
      streamWriter.Flush();
      memoryStream.Position = 0L;
      return this.CreateDocument(projectItem, project, (Stream) memoryStream, CodeDocumentType.DefaultEncoding, false);
    }

    private IDocument CreateDocument(IProjectItem projectItem, IProject project, Stream stream, Encoding encoding, bool isReadOnly)
    {
      return (IDocument) new CodeDocument(projectItem.DocumentReference, isReadOnly, encoding, stream, this.CodeDomProvider, this, this.codeProjectService, this.codeProjectService.GetCodeProject(project), project, this.viewService, this.windowService);
    }

    public override void CloseDocument(IProjectItem projectItem, IProject project)
    {
      this.codeProjectService.GetCodeProject(project).DeactivateEditing(projectItem.DocumentReference);
    }

    public virtual string CreateMethod(string indent, Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters, out int bodyInsertionOffset)
    {
      bodyInsertionOffset = 0;
      return (string) null;
    }

    public virtual string FormatUsingDirective(string namespaceName)
    {
      return string.Empty;
    }
  }
}
