using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions.Documents
{
	public static class DocumentServiceExtensions
	{
		public static ICodeDocumentType CSharpDocumentType(this IDocumentTypeCollection source)
		{
			return source[DocumentTypeNamesHelper.CSharp] as ICodeDocumentType;
		}

		public static ICodeDocumentType DefaultCodeDocumentType(this IDocumentTypeCollection source)
		{
			return source.CSharpDocumentType();
		}

		public static IDocumentTypeCollection DocumentTypes(this IServiceProvider source)
		{
			return source.DocumentTypeManager().DocumentTypes;
		}

		public static ICodeDocumentType JavaScriptDocumentType(this IDocumentTypeCollection source)
		{
			return source[DocumentTypeNamesHelper.JavaScript] as ICodeDocumentType;
		}

		public static IDocumentCollection OpenDocuments(this IServiceProvider source)
		{
			return source.DocumentService().Documents;
		}

		public static ICodeDocumentType VisualBasicDocumentType(this IDocumentTypeCollection source)
		{
			return source[DocumentTypeNamesHelper.VisualBasic] as ICodeDocumentType;
		}
	}
}