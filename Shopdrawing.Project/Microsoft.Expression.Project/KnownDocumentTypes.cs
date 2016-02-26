using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;

namespace Microsoft.Expression.Project
{
	internal sealed class KnownDocumentTypes
	{
		private IDocumentTypeManager documentTypeManager;

		private IDocumentType sceneDocumentType;

		private ICodeDocumentType csharpDocumentType;

		private ICodeDocumentType visualBasicDocumentType;

		private IDocumentType javascriptDocumentType;

		private ICodeDocumentType defaultCodeDocumentType;

		public ICodeDocumentType CSharpDocumentType
		{
			get
			{
				if (this.csharpDocumentType == null || this.csharpDocumentType == this.documentTypeManager.DocumentTypes.UnknownDocumentType)
				{
					this.csharpDocumentType = this.documentTypeManager.DocumentTypes.CSharpDocumentType();
				}
				return this.csharpDocumentType;
			}
		}

		public ICodeDocumentType DefaultCodeDocumentType
		{
			get
			{
				if (this.defaultCodeDocumentType == null)
				{
					this.defaultCodeDocumentType = this.CSharpDocumentType;
				}
				return this.defaultCodeDocumentType;
			}
			set
			{
				this.defaultCodeDocumentType = value;
			}
		}

		public IDocumentType JavascriptDocumentType
		{
			get
			{
				if (this.javascriptDocumentType == null || this.javascriptDocumentType == this.documentTypeManager.DocumentTypes.UnknownDocumentType)
				{
					this.javascriptDocumentType = this.documentTypeManager.DocumentTypes.JavaScriptDocumentType();
				}
				return this.javascriptDocumentType;
			}
		}

		public IDocumentType SceneDocumentType
		{
			get
			{
				if (this.sceneDocumentType == null || this.sceneDocumentType == this.documentTypeManager.DocumentTypes.UnknownDocumentType)
				{
					this.sceneDocumentType = this.documentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Xaml];
				}
				return this.sceneDocumentType;
			}
		}

		public ICodeDocumentType VisualBasicDocumentType
		{
			get
			{
				if (this.visualBasicDocumentType == null || this.visualBasicDocumentType == this.documentTypeManager.DocumentTypes.UnknownDocumentType)
				{
					this.visualBasicDocumentType = this.documentTypeManager.DocumentTypes.VisualBasicDocumentType();
				}
				return this.visualBasicDocumentType;
			}
		}

		public KnownDocumentTypes(IDocumentTypeManager documentTypeManager)
		{
			this.documentTypeManager = documentTypeManager;
		}
	}
}