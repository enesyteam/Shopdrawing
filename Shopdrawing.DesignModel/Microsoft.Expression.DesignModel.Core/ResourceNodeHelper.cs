using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class ResourceNodeHelper
	{
		public static IEnumerable<Uri> FindReferencedDictionaries(DocumentCompositeNode node)
		{
			HashSet<Uri> uris = new HashSet<Uri>();
			IPropertyId resourcesProperty = node.Type.Metadata.ResourcesProperty;
			if (resourcesProperty != null)
			{
				DocumentCompositeNode item = node.Properties[resourcesProperty] as DocumentCompositeNode;
				if (item != null)
				{
					node = item;
				}
			}
			if (PlatformTypes.ResourceDictionary.IsAssignableFrom(node.Type))
			{
				ResourceNodeHelper.FindReferencedDictionariesInternal(node, uris);
			}
			return uris;
		}

		private static void FindReferencedDictionariesInternal(DocumentCompositeNode resourceDictionaryNode, HashSet<Uri> dictionaries)
		{
			if (resourceDictionaryNode != null)
			{
				DocumentCompositeNode item = resourceDictionaryNode.Properties[KnownProperties.ResourceDictionaryMergedDictionariesProperty] as DocumentCompositeNode;
				if (item != null)
				{
					for (int i = 0; i < item.Children.Count; i++)
					{
						DocumentCompositeNode documentCompositeNode = item.Children[i] as DocumentCompositeNode;
						if (documentCompositeNode != null)
						{
							ResourceNodeHelper.FindReferencedDictionaryFromSource(documentCompositeNode, dictionaries);
						}
					}
				}
				ResourceNodeHelper.FindReferencedDictionaryFromSource(resourceDictionaryNode, dictionaries);
			}
		}

		private static void FindReferencedDictionaryFromSource(DocumentCompositeNode resourceDictionaryNode, HashSet<Uri> dictionaries)
		{
			DocumentCompositeNode rootNode;
			Uri uriValue = resourceDictionaryNode.GetUriValue(KnownProperties.ResourceDictionarySourceProperty);
			if (uriValue != null)
			{
				uriValue = resourceDictionaryNode.Context.MakeDesignTimeUri(uriValue);
				if (uriValue != null && uriValue.IsAbsoluteUri && !dictionaries.Contains(uriValue))
				{
					dictionaries.Add(uriValue);
					DocumentCompositeNode documentCompositeNode = null;
					IDocumentRoot documentRoot = null;
					try
					{
						documentRoot = resourceDictionaryNode.Context.GetDocumentRoot(uriValue.OriginalString);
						if (documentRoot != null)
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
					}
					catch (IOException oException)
					{
					}
					catch (NotSupportedException notSupportedException)
					{
					}
					if (documentCompositeNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(documentCompositeNode.Type))
					{
						ResourceNodeHelper.FindReferencedDictionariesInternal(documentCompositeNode, dictionaries);
					}
				}
			}
		}

		public static DocumentNode GetResourceEntryKey(DocumentCompositeNode entryNode)
		{
			return DocumentNodeHelper.GetResourceEntryKey(entryNode);
		}

		public static DocumentNode GetResourceKey(DocumentCompositeNode node)
		{
			IPropertyId resourceProperty = ResourceNodeHelper.GetResourceProperty(node);
			if (resourceProperty == null)
			{
				return null;
			}
			return node.Properties[resourceProperty];
		}

		public static IPropertyId GetResourceProperty(DocumentNode node)
		{
			if (DocumentNodeUtilities.IsDynamicResource(node))
			{
				return KnownProperties.DynamicResourceResourceKeyProperty;
			}
			if (DocumentNodeUtilities.IsStaticResource(node))
			{
				return KnownProperties.StaticResourceResourceKeyProperty;
			}
			return null;
		}

		public static ISupportsResources GetResourcesCollection(DocumentNode node)
		{
			DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
			if (documentCompositeNode != null)
			{
				IType type = documentCompositeNode.Type;
				IPropertyId resourcesProperty = type.Metadata.ResourcesProperty;
				if (resourcesProperty != null)
				{
					return new ResourceNodeHelper.ResourceContainerAdapter(documentCompositeNode, resourcesProperty);
				}
				if (PlatformTypes.ResourceDictionary.IsAssignableFrom(type))
				{
					return new ResourceNodeHelper.ResourceDictionaryAdapter(documentCompositeNode);
				}
			}
			return null;
		}

		public static ResourceReferenceType GetResourceType(DocumentNode node)
		{
			if (DocumentNodeUtilities.IsDynamicResource(node))
			{
				return ResourceReferenceType.Dynamic;
			}
			return ResourceReferenceType.Static;
		}

		public static bool IsResourceContainer(ISupportsResources value, DocumentNode childNode)
		{
			ResourceNodeHelper.ResourceContainerAdapter resourceContainerAdapter = value as ResourceNodeHelper.ResourceContainerAdapter;
			if (resourceContainerAdapter == null)
			{
				return false;
			}
			if (childNode == null)
			{
				return true;
			}
			return !resourceContainerAdapter.ResourcesProperty.Equals(childNode.SitePropertyKey);
		}

		public static bool IsResourceDictionary(ISupportsResources value)
		{
			return value is ResourceNodeHelper.ResourceDictionaryAdapter;
		}

		public static void SetResourceEntryKey(DocumentCompositeNode entryNode, DocumentNode keyNode)
		{
			ITypeResolver typeResolver = entryNode.TypeResolver;
			if (entryNode.TypeResolver.IsCapabilitySet(PlatformCapability.NameSupportedAsKey) && (keyNode == null || PlatformTypes.String.IsAssignableFrom(keyNode.Type)) && entryNode.Properties[KnownProperties.DictionaryEntryKeyProperty] == null)
			{
				DocumentCompositeNode item = entryNode.Properties[KnownProperties.DictionaryEntryValueProperty] as DocumentCompositeNode;
				if (item != null)
				{
					IPropertyId nameProperty = item.NameProperty;
					if (nameProperty != null && item.Properties[nameProperty] != null)
					{
						item.Properties[nameProperty] = keyNode;
						return;
					}
				}
			}
			entryNode.Properties[KnownProperties.DictionaryEntryKeyProperty] = keyNode;
		}

		public static void SetResourceKey(DocumentCompositeNode node, DocumentNode valueNode)
		{
			IPropertyId resourceProperty = ResourceNodeHelper.GetResourceProperty(node);
			node.Properties[resourceProperty] = valueNode;
		}

		private sealed class ResourceContainerAdapter : ISupportsResources
		{
			private DocumentCompositeNode node;

			private IPropertyId resourcesProperty;

			public DocumentCompositeNode HostNode
			{
				get
				{
					return this.node;
				}
			}

			public DocumentCompositeNode Resources
			{
				get
				{
					return this.node.Properties[this.resourcesProperty] as DocumentCompositeNode;
				}
				set
				{
					this.node.Properties[this.resourcesProperty] = value;
				}
			}

			public IPropertyId ResourcesProperty
			{
				get
				{
					return this.resourcesProperty;
				}
			}

			public ResourceContainerAdapter(DocumentCompositeNode node, IPropertyId resourcesProperty)
			{
				this.node = node;
				this.resourcesProperty = resourcesProperty;
			}
		}

		private sealed class ResourceDictionaryAdapter : ISupportsResources
		{
			private DocumentCompositeNode resourceDictionary;

			public DocumentCompositeNode HostNode
			{
				get
				{
					return this.resourceDictionary;
				}
			}

			public DocumentCompositeNode Resources
			{
				get
				{
					return this.resourceDictionary;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public ResourceDictionaryAdapter(DocumentCompositeNode resourceDictionary)
			{
				this.resourceDictionary = resourceDictionary;
			}
		}
	}
}