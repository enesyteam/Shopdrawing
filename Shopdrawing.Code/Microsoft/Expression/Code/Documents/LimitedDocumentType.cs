// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.LimitedDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.Code.Documents
{
  internal abstract class LimitedDocumentType : DocumentType
  {
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    private EditingService editingService;

    protected override string FileNameBase
    {
      get
      {
        return "Text";
      }
    }

    public abstract override bool IsDefaultTypeForExtension { get; }

    public abstract override string[] FileExtensions { get; }

    public abstract override string Description { get; }

    public abstract override string Name { get; }

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

    public LimitedDocumentType(EditingService editingService)
    {
      this.editingService = editingService;
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
        return (IDocument) new LimitedDocument(projectItem.DocumentReference, isReadOnly, encoding, contents, this.editingService);
      }
      throw new FileNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentFileNotFound, new object[1]
      {
        (object) documentReference.Path
      }));
    }

    public override IDocument CreateDocument(IProjectItem projectItem, IProject project, string initialContents)
    {
      return (IDocument) new LimitedDocument(projectItem.DocumentReference, false, LimitedDocumentType.DefaultEncoding, initialContents, this.editingService);
    }

    public override void CloseDocument(IProjectItem projectItem, IProject project)
    {
      base.CloseDocument(projectItem, project);
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.Code.FileTable.GetImageSource(path);
    }
  }
}
