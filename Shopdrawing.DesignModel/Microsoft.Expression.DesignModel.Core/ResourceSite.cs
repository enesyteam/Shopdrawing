using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class ResourceSite
	{
		private readonly IDocumentContext documentContext;

		private readonly ISupportsResources resourcesHost;

		public IDocumentContext DocumentContext
		{
			get
			{
				return this.documentContext;
			}
		}

		public DocumentCompositeNode HostNode
		{
			get
			{
				return this.resourcesHost.HostNode;
			}
		}

		public DocumentCompositeNode ResourcesDictionary
		{
			get
			{
				return this.resourcesHost.Resources;
			}
		}

		public ResourceSite(IDocumentContext documentContext, ISupportsResources resourcesHost)
		{
			if (documentContext == null)
			{
				throw new ArgumentNullException("documentContext");
			}
			if (resourcesHost == null)
			{
				throw new ArgumentNullException("resourcesHost");
			}
			this.documentContext = documentContext;
			this.resourcesHost = resourcesHost;
		}

		public ResourceSite(DocumentNode resourcesHost) : this(resourcesHost.Context, ResourceNodeHelper.GetResourcesCollection(resourcesHost))
		{
		}

		public DocumentCompositeNode CreateResource(string key, DocumentNode valueNode)
		{
			return this.CreateResource(this.documentContext.CreateNode(key), valueNode, -1);
		}

		public DocumentCompositeNode CreateResource(DocumentNode keyNode, DocumentNode valueNode, int index)
		{
			this.EnsureResourceCollection();
			DocumentCompositeNode documentCompositeNode = this.documentContext.CreateNode(PlatformTypes.DictionaryEntry);
			if (keyNode != null)
			{
				ResourceNodeHelper.SetResourceEntryKey(documentCompositeNode, keyNode);
			}
			documentCompositeNode.Properties[KnownProperties.DictionaryEntryValueProperty] = valueNode;
			IList<DocumentNode> children = this.resourcesHost.Resources.Children;
			if (index < 0)
			{
				index = children.Count;
			}
			children.Insert(index, documentCompositeNode);
			return documentCompositeNode;
		}

		public void EnsureResourceCollection()
		{
			DocumentCompositeNode resources = this.resourcesHost.Resources;
			if (resources == null || !resources.SupportsChildren)
			{
				resources = this.documentContext.CreateNode(PlatformTypes.ResourceDictionary);
				this.resourcesHost.Resources = resources;
			}
		}

		public DocumentCompositeNode FindResource(IDocumentRootResolver documentRootResolver, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, int numberOfChildrenToSearch, ICollection<string> warnings)
		{
			IDocumentRoot documentRoot = this.resourcesHost.HostNode.DocumentRoot;
			return this.FindResource(documentRootResolver, keyNode, resourcesHostNodePath, relatedRoots, new ResourceSite.ResourceDictionaryLink(null, documentRoot), numberOfChildrenToSearch, warnings);
		}

		private DocumentCompositeNode FindResource(IDocumentRootResolver documentRootResolver, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, ResourceSite.ResourceDictionaryLink container, int numberOfChildrenToSearch, ICollection<string> warnings)
		{
			if (resourcesHostNodePath != null)
			{
				resourcesHostNodePath.Add(this.resourcesHost.HostNode);
			}
			DocumentCompositeNode resources = this.resourcesHost.Resources;
			if (resources == null)
			{
				return null;
			}
			DocumentCompositeNode documentCompositeNode = ResourceSite.FindResource(keyNode, resources, numberOfChildrenToSearch);
			if (documentCompositeNode != null)
			{
				return documentCompositeNode;
			}
			if (documentRootResolver == null)
			{
				return null;
			}
			if (resources.TypeResolver.ResolveProperty(KnownProperties.ResourceDictionaryMergedDictionariesProperty) != null)
			{
				DocumentCompositeNode item = resources.Properties[KnownProperties.ResourceDictionaryMergedDictionariesProperty] as DocumentCompositeNode;
				if (item != null)
				{
					for (int i = item.Children.Count - 1; i >= 0; i--)
					{
						DocumentCompositeNode item1 = item.Children[i] as DocumentCompositeNode;
						if (item1 != null)
						{
							DocumentNode resolvedDocumentRootForSourceUri = this.GetResolvedDocumentRootForSourceUri(documentRootResolver, item1, warnings);
							if (resolvedDocumentRootForSourceUri == null || !PlatformTypes.ResourceDictionary.IsAssignableFrom(resolvedDocumentRootForSourceUri.Type))
							{
								documentCompositeNode = ResourceSite.FindResource(keyNode, item1, -1);
								if (documentCompositeNode != null)
								{
									return documentCompositeNode;
								}
							}
							else
							{
								documentCompositeNode = this.FindResourceInRelatedDocument(documentRootResolver, keyNode, resourcesHostNodePath, relatedRoots, container, resolvedDocumentRootForSourceUri);
								if (documentCompositeNode != null)
								{
									return documentCompositeNode;
								}
							}
						}
					}
				}
			}
			if (resources.TypeResolver.ResolveProperty(KnownProperties.ResourceDictionarySourceProperty) != null)
			{
				DocumentNode documentNode = this.GetResolvedDocumentRootForSourceUri(documentRootResolver, resources, warnings);
				if (documentNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(documentNode.Type))
				{
					return this.FindResourceInRelatedDocument(documentRootResolver, keyNode, resourcesHostNodePath, relatedRoots, container, documentNode);
				}
			}
			return null;
		}

		public DocumentCompositeNode FindResource(IDocumentRootResolver documentRootResolver, string key, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots)
		{
			return this.FindResource(documentRootResolver, this.documentContext.CreateNode(key), resourcesHostNodePath, relatedRoots, -1, null);
		}

		public DocumentCompositeNode FindResource(IDocumentRootResolver documentRootResolver, string key, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, ICollection<string> warnings)
		{
			return this.FindResource(documentRootResolver, this.documentContext.CreateNode(key), resourcesHostNodePath, relatedRoots, -1, warnings);
		}

		public static DocumentCompositeNode FindResource(DocumentNode key, DocumentCompositeNode resourcesCollection, int numberOfChildrenToSearch)
		{
			if (resourcesCollection != null && resourcesCollection.SupportsChildren)
			{
				IList<DocumentNode> children = resourcesCollection.Children;
				if (numberOfChildrenToSearch < 0)
				{
					numberOfChildrenToSearch = children.Count;
				}
				for (int i = 0; i < numberOfChildrenToSearch; i++)
				{
					DocumentCompositeNode item = children[i] as DocumentCompositeNode;
					if (item != null)
					{
						DocumentNode resourceEntryKey = ResourceNodeHelper.GetResourceEntryKey(item);
						if (resourceEntryKey == null)
						{
							DocumentCompositeNode documentCompositeNode = item.Properties[KnownProperties.DictionaryEntryValueProperty] as DocumentCompositeNode;
							if (documentCompositeNode != null)
							{
								resourceEntryKey = ResourceSite.GetImplicitKey(documentCompositeNode);
							}
						}
						if (resourceEntryKey != null && resourceEntryKey.Equals(key))
						{
							return item;
						}
					}
				}
			}
			return null;
		}

		private DocumentCompositeNode FindResourceInRelatedDocument(IDocumentRootResolver documentRootResolver, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, ResourceSite.ResourceDictionaryLink container, DocumentNode rootNode)
		{
			IDocumentRoot documentRoot = rootNode.DocumentRoot;
			for (ResourceSite.ResourceDictionaryLink i = container; i != null; i = i.Container)
			{
				if (i.DocumentRoot == documentRoot)
				{
					return null;
				}
			}
			ResourceSite resourceSite = new ResourceSite(rootNode);
			DocumentCompositeNode documentCompositeNode = resourceSite.FindResource(documentRootResolver, keyNode, resourcesHostNodePath, relatedRoots, new ResourceSite.ResourceDictionaryLink(container, documentRoot), -1, null);
			if (documentCompositeNode != null && relatedRoots != null)
			{
				relatedRoots.Add(documentRoot);
			}
			return documentCompositeNode;
		}

		private static DocumentNode GetImplicitKey(DocumentCompositeNode valueNode)
		{
			IPropertyId implicitDictionaryKeyProperty = valueNode.Type.Metadata.ImplicitDictionaryKeyProperty;
			if (implicitDictionaryKeyProperty == null)
			{
				return null;
			}
			return valueNode.Properties[implicitDictionaryKeyProperty];
		}

		private DocumentNode GetResolvedDocumentRootForSourceUri(IDocumentRootResolver documentRootResolver, DocumentCompositeNode resourceDictionaryNode, ICollection<string> warnings)
		{
			DocumentNode documentNode;
			DocumentCompositeNode rootNode;
			DocumentCompositeNode documentCompositeNode = null;
			IDocumentRoot documentRoot = null;
			Uri uriValue = resourceDictionaryNode.GetUriValue(KnownProperties.ResourceDictionarySourceProperty);
			Uri uri = uriValue;
			if (uri != null)
			{
				uri = this.documentContext.MakeDesignTimeUri(uri);
			}
			if (uri != null)
			{
				try
				{
					documentRoot = documentRootResolver.GetDocumentRoot(uri.OriginalString);
					if (documentRoot != null)
					{
						if (PlatformTypes.PlatformsCompatible(documentRoot.TypeResolver.PlatformMetadata, resourceDictionaryNode.TypeResolver.PlatformMetadata))
						{
							if (documentRoot.IsEditable)
							{
								rootNode = documentRoot.RootNode as DocumentCompositeNode;
							}
							else
							{
								rootNode = null;
							}
							documentCompositeNode = rootNode;
						}
						else
						{
							if (warnings != null)
							{
								CultureInfo currentCulture = CultureInfo.CurrentCulture;
								string resourceDictionaryTargetFrameworkNotMatching = StringTable.ResourceDictionaryTargetFrameworkNotMatching;
								object[] fullName = new object[] { uriValue, documentRoot.TypeResolver.PlatformMetadata.TargetFramework.FullName, resourceDictionaryNode.TypeResolver.PlatformMetadata.TargetFramework.FullName };
								warnings.Add(string.Format(currentCulture, resourceDictionaryTargetFrameworkNotMatching, fullName));
							}
							documentNode = null;
							return documentNode;
						}
					}
					return documentCompositeNode;
				}
				catch (IOException oException)
				{
					return documentCompositeNode;
				}
				catch (NotSupportedException notSupportedException)
				{
					return documentCompositeNode;
				}
				return documentNode;
			}
			return documentCompositeNode;
		}

		public string GetUniqueResourceKey(string prefix)
		{
			string str;
			string i = prefix;
			if (this.FindResource(null, i, null, null) != null)
			{
				int num = (ResourceSite.ParseKeyString(prefix, out str, out num) ? num + 1 : 1);
				for (i = string.Concat(str, num.ToString()); this.FindResource(null, i, null, null) != null; i = string.Concat(str, num.ToString(CultureInfo.InvariantCulture)))
				{
					num++;
				}
			}
			return i;
		}

		public static bool ParseKeyString(string name, out string prefix, out int suffixValue)
		{
			int length = name.Length;
			while (length > 0 && char.IsDigit(name[length - 1]))
			{
				length--;
			}
			if (length < 0 || length == name.Length)
			{
				prefix = name;
				suffixValue = -1;
				return false;
			}
			prefix = name.Substring(0, length);
			suffixValue = int.Parse(name.Substring(length, name.Length - length), CultureInfo.InvariantCulture);
			return true;
		}

		public bool RemoveResource(string key)
		{
			bool flag = false;
			if (this.resourcesHost.Resources != null)
			{
				DocumentCompositeNode documentCompositeNode = ResourceSite.FindResource(this.documentContext.CreateNode(key), this.resourcesHost.Resources, -1);
				if (documentCompositeNode != null)
				{
					flag = this.resourcesHost.Resources.Children.Remove(documentCompositeNode);
				}
			}
			return flag;
		}

		private sealed class ResourceDictionaryLink
		{
			private ResourceSite.ResourceDictionaryLink container;

			private IDocumentRoot documentRoot;

			public ResourceSite.ResourceDictionaryLink Container
			{
				get
				{
					return this.container;
				}
			}

			public IDocumentRoot DocumentRoot
			{
				get
				{
					return this.documentRoot;
				}
			}

			public ResourceDictionaryLink(ResourceSite.ResourceDictionaryLink container, IDocumentRoot documentRoot)
			{
				this.container = container;
				this.documentRoot = documentRoot;
			}
		}
	}
}