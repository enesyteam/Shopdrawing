// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.IDocumentContext
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public interface IDocumentContext : IDocumentRootResolver
  {
    ITypeResolver TypeResolver { get; }

    string DocumentUrl { get; }

    bool IsLooseXaml { get; }

    bool IsUsingDesignTimeTypes { get; }

    DocumentPrimitiveNode CreateNode(ITypeId typeId, IDocumentNodeValue value);

    DocumentPrimitiveNode CreateNode(string value);

    DocumentCompositeNode CreateNode(ITypeId typeId);

    DocumentCompositeNode CreateNode(Type type);

    DocumentNode CreateNode(Type type, object value);

    Type GetChildNodeType(Type type);

    Uri MakeDesignTimeUri(Uri uri);

    string MakeResourceReference(string resourceUrl);

    string MakeResourceReference(string resourceUrl, bool useProjectRelativeSyntax);
  }
}
