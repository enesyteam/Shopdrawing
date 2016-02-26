using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	internal class ResourceReferenceCreator
	{
		private ProjectItemBase resourceItem;

		private string relativePathToReference;

		private string ResourceBuildAction
		{
			get
			{
				return this.resourceItem.Properties["BuildAction"];
			}
		}

		public ResourceReferenceCreator(ProjectItemBase resourceItem)
		{
			this.resourceItem = resourceItem;
		}

		private string CreateComponentPackUriReference()
		{
			string str = (this.resourceItem.Project.TargetAssembly == null ? "" : this.resourceItem.Project.TargetAssembly.Name);
			return string.Concat("/", str, ";component", this.resourceItem.ProjectRelativeDocumentReference);
		}

		private string CreateNoneReference()
		{
			return string.Empty;
		}

		private string CreateRelativePackUriReference()
		{
			return this.resourceItem.ProjectRelativeDocumentReference;
		}

		private string CreateRelativeReference()
		{
			return this.relativePathToReference;
		}

		public string CreateResourceReferenceFromDocument(DocumentReference referencingDocument)
		{
			ResourceReferenceCreator.UriSyntax uriSyntax = this.DetermineUriSyntax(referencingDocument);
			return this.NormalizeReference(this.GenerateReference(uriSyntax));
		}

		private string CreateSiteOfOriginPackUriReference()
		{
			return string.Concat("pack://siteoforigin:,,,", this.resourceItem.ProjectRelativeDocumentReference);
		}

		private ResourceReferenceCreator.UriSyntax DetermineSyntaxForCompiledItemType()
		{
			if ((this.relativePathToReference == null ? false : !this.relativePathToReference.StartsWith("..", StringComparison.OrdinalIgnoreCase)))
			{
				return ResourceReferenceCreator.UriSyntax.Relative;
			}
			return ResourceReferenceCreator.UriSyntax.ComponentPackUri;
		}

		private ResourceReferenceCreator.UriSyntax DetermineSyntaxForContentItemType()
		{
			if (this.IsWpfProject())
			{
				return ResourceReferenceCreator.UriSyntax.Relative;
			}
			return ResourceReferenceCreator.UriSyntax.RelativePackUri;
		}

		private ResourceReferenceCreator.UriSyntax DetermineSyntaxForDifferentProjectReference()
		{
			string resourceBuildAction = this.ResourceBuildAction;
			string str = resourceBuildAction;
			if (resourceBuildAction != null)
			{
				if (str == "Content" || str == "None")
				{
					return ResourceReferenceCreator.UriSyntax.None;
				}
				if (!(str == "Resource") && !(str == "BlendEmbeddedResource") && !(str == "DesignTimeOnly"))
				{
				}
			}
			return ResourceReferenceCreator.UriSyntax.ComponentPackUri;
		}

		private ResourceReferenceCreator.UriSyntax DetermineSyntaxForNoneItemType()
		{
			if (this.IsWpfProject())
			{
				return ResourceReferenceCreator.UriSyntax.SiteOfOriginPackUri;
			}
			return ResourceReferenceCreator.UriSyntax.RelativePackUri;
		}

		private ResourceReferenceCreator.UriSyntax DetermineSyntaxForNoReferencingDocument()
		{
			return ResourceReferenceCreator.UriSyntax.ComponentPackUri;
		}

		private ResourceReferenceCreator.UriSyntax DetermineSyntaxForSameProjectReference(DocumentReference referencingDocument)
		{
			ResourceReferenceCreator.UriSyntax uriSyntax = ResourceReferenceCreator.UriSyntax.None;
			string relativePathForDocumentReference = this.GetRelativePathForDocumentReference(referencingDocument);
			if (relativePathForDocumentReference == null)
			{
				return ResourceReferenceCreator.UriSyntax.None;
			}
			this.relativePathToReference = relativePathForDocumentReference;
			if (this.resourceItem.DocumentType == null || !(this.resourceItem.DocumentType.Name == DocumentTypeNamesHelper.Font))
			{
				string resourceBuildAction = this.ResourceBuildAction;
				string str = resourceBuildAction;
				if (resourceBuildAction != null)
				{
					switch (str)
					{
						case "Content":
						{
							uriSyntax = this.DetermineSyntaxForContentItemType();
							return uriSyntax;
						}
						case "BlendEmbeddedFont":
						{
							uriSyntax = ResourceReferenceCreator.UriSyntax.ComponentPackUri;
							return uriSyntax;
						}
						case "None":
						{
							uriSyntax = this.DetermineSyntaxForNoneItemType();
							return uriSyntax;
						}
					}
				}
				uriSyntax = this.DetermineSyntaxForCompiledItemType();
			}
			else
			{
				uriSyntax = ResourceReferenceCreator.UriSyntax.ComponentPackUri;
			}
			return uriSyntax;
		}

		private ResourceReferenceCreator.UriSyntax DetermineUriSyntax(DocumentReference referencingDocument)
		{
			ResourceReferenceCreator.UriSyntax uriSyntax;
			if (referencingDocument != null)
			{
				uriSyntax = (!this.IsResourceInSameProjectAs(referencingDocument) ? this.DetermineSyntaxForDifferentProjectReference() : this.DetermineSyntaxForSameProjectReference(referencingDocument));
			}
			else
			{
				uriSyntax = this.DetermineSyntaxForNoReferencingDocument();
			}
			return uriSyntax;
		}

		private string GenerateReference(ResourceReferenceCreator.UriSyntax uriSyntax)
		{
			switch (uriSyntax)
			{
				case ResourceReferenceCreator.UriSyntax.None:
				{
					return this.CreateNoneReference();
				}
				case ResourceReferenceCreator.UriSyntax.Relative:
				{
					return this.CreateRelativeReference();
				}
				case ResourceReferenceCreator.UriSyntax.RelativePackUri:
				{
					return this.CreateRelativePackUriReference();
				}
				case ResourceReferenceCreator.UriSyntax.ComponentPackUri:
				{
					return this.CreateComponentPackUriReference();
				}
				case ResourceReferenceCreator.UriSyntax.SiteOfOriginPackUri:
				{
					return this.CreateSiteOfOriginPackUriReference();
				}
			}
			throw new InvalidOperationException();
		}

		private string GetRelativePathForDocumentReference(DocumentReference referencingDocument)
		{
			string str = this.resourceItem.ProjectRelativeDocumentReference.TrimStart(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectorySeparatorCharacters());
			DocumentReference documentReference = null;
			if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(str))
			{
				return null;
			}
			documentReference = DocumentReference.Create(Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(this.resourceItem.Project.ProjectRoot.Path, str));
			return referencingDocument.GetRelativePath(documentReference);
		}

		private bool IsResourceInSameProjectAs(DocumentReference referencingDocument)
		{
			return this.resourceItem.Project.FindItem(referencingDocument) != null;
		}

		private bool IsWpfProject()
		{
			if (this.resourceItem.Project.TargetFramework == null)
			{
				return false;
			}
			return this.resourceItem.Project.TargetFramework.Identifier == ".NETFramework";
		}

		private string NormalizeReference(string reference)
		{
			return reference.Replace(Path.DirectorySeparatorChar, '/');
		}

		private enum UriSyntax
		{
			None,
			Relative,
			RelativePackUri,
			ComponentPackUri,
			SiteOfOriginPackUri
		}
	}
}