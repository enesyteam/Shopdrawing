using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class ExpressionEvaluator
	{
		private IDocumentRootResolver documentRootResolver;

		public ExpressionEvaluator(IDocumentRootResolver documentRootResolver)
		{
			this.documentRootResolver = documentRootResolver;
		}

		public static DocumentNode EvaluateExpression(DocumentNode expression)
		{
			DocumentNodePath documentNodePath = new DocumentNodePath(expression.DocumentRoot.RootNode, expression);
			return (new ExpressionEvaluator(expression.Context)).EvaluateExpression(documentNodePath, expression);
		}

		public DocumentNode EvaluateExpression(DocumentNodePath context, DocumentNode expression)
		{
			DocumentCompositeNode documentCompositeNode = expression as DocumentCompositeNode;
			DocumentCompositeNode documentCompositeNode1 = documentCompositeNode;
			if (documentCompositeNode == null)
			{
				DocumentPrimitiveNode documentPrimitiveNode = expression as DocumentPrimitiveNode;
				DocumentPrimitiveNode documentPrimitiveNode1 = documentPrimitiveNode;
				if (documentPrimitiveNode != null)
				{
					DocumentNodeReferenceValue value = documentPrimitiveNode1.Value as DocumentNodeReferenceValue;
					if (value != null)
					{
						return value.Value;
					}
				}
			}
			else
			{
				if (DocumentNodeUtilities.IsTemplateBinding(documentCompositeNode1))
				{
					return this.EvaluateTemplateBinding(context, documentCompositeNode1);
				}
				if (documentCompositeNode1.Type.IsResource)
				{
					DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(documentCompositeNode1);
					if (resourceKey != null)
					{
						return this.EvaluateResource(context, ResourceNodeHelper.GetResourceType(documentCompositeNode1), resourceKey);
					}
				}
			}
			return expression;
		}

		public DocumentNode EvaluateProperty(DocumentNodePath nodePath, IPropertyId propertyKey)
		{
			return this.EvaluateExpression(nodePath, ((DocumentCompositeNode)nodePath.Node).Properties[propertyKey]);
		}

		public DocumentNode EvaluateResource(DocumentNodePath nodePath, DocumentNode keyNode)
		{
			return this.EvaluateResourceAndCollectionPath(nodePath, ResourceReferenceType.Static, keyNode, null, null, null);
		}

		public DocumentNode EvaluateResource(DocumentNodePath nodePath, ResourceReferenceType referenceType, DocumentNode keyNode)
		{
			return this.EvaluateResourceAndCollectionPath(nodePath, referenceType, keyNode, null, null, null);
		}

		public DocumentNode EvaluateResourceAndCollectionPath(DocumentNodePath nodePath, ResourceReferenceType referenceType, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots)
		{
			bool flag;
			return this.EvaluateResourceAndCollectionPath(nodePath, referenceType, keyNode, resourcesHostNodePath, relatedRoots, null, out flag);
		}

		public DocumentNode EvaluateResourceAndCollectionPath(DocumentNodePath nodePath, ResourceReferenceType referenceType, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, ICollection<string> warnings)
		{
			bool flag;
			return this.EvaluateResourceAndCollectionPath(nodePath, referenceType, keyNode, resourcesHostNodePath, relatedRoots, warnings, out flag);
		}

		public DocumentNode EvaluateResourceAndCollectionPath(DocumentNodePath nodePath, ResourceReferenceType referenceType, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, ICollection<string> warnings, out bool invalidForwardReference)
		{
			Uri uri;
			DocumentNode documentNode;
			IDocumentRoot applicationRoot;
			IDocumentRoot documentRoot = nodePath.RootNode.DocumentRoot;
			if (this.documentRootResolver != null)
			{
				applicationRoot = this.documentRootResolver.ApplicationRoot;
			}
			else
			{
				applicationRoot = null;
			}
			IDocumentRoot documentRoot1 = applicationRoot;
			bool flag = (documentRoot1 == null ? false : documentRoot1.RootNode != null);
			IDocumentRoot documentRoot2 = null;
			invalidForwardReference = false;
			if (referenceType != ResourceReferenceType.Static)
			{
				DocumentNode node = nodePath.Node;
				while (node != null)
				{
					if (flag && node.DocumentRoot != null && node == node.DocumentRoot.RootNode && PlatformTypes.ResourceDictionary.IsAssignableFrom(node.Type))
					{
						string documentUrl = node.Context.DocumentUrl;
						DocumentCompositeNode rootNode = documentRoot1.RootNode as DocumentCompositeNode;
						if (rootNode != null && Uri.TryCreate(documentUrl, UriKind.Absolute, out uri) && ResourceNodeHelper.FindReferencedDictionaries(rootNode).Contains<Uri>(uri))
						{
							documentRoot2 = node.DocumentRoot;
							break;
						}
					}
					DocumentNode documentNode1 = this.EvaluateResourceAtSpecificNode(node, keyNode, resourcesHostNodePath, relatedRoots, warnings);
					if (documentNode1 != null)
					{
						return documentNode1;
					}
					node = node.Parent;
					if (node == null || nodePath == null)
					{
						continue;
					}
					DocumentNode containerNode = nodePath.ContainerNode;
					DocumentNode styleForSetter = ExpressionEvaluator.GetStyleForSetter(node);
					if (styleForSetter == null || styleForSetter != containerNode)
					{
						styleForSetter = ExpressionEvaluator.GetStyleForResourceEntry(node);
						if (styleForSetter == null)
						{
							if (node != containerNode)
							{
								continue;
							}
							nodePath = null;
						}
						else
						{
							if (styleForSetter == containerNode)
							{
								nodePath = null;
							}
							node = styleForSetter.Parent;
						}
					}
					else
					{
						nodePath = nodePath.GetContainerOwnerPath();
						if (nodePath == null)
						{
							continue;
						}
						node = nodePath.Node;
					}
				}
			}
			else
			{
				DocumentNode documentNode2 = null;
				for (DocumentNode i = nodePath.Node; i != null; i = i.Parent)
				{
					ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(i);
					if (resourcesCollection != null)
					{
						ResourceSite resourceSite = new ResourceSite(i.Context, resourcesCollection);
						DocumentNode documentNode3 = null;
						if (ResourceNodeHelper.IsResourceDictionary(resourcesCollection))
						{
							int siteChildIndex = -1;
							if (documentNode2 != null)
							{
								siteChildIndex = documentNode2.SiteChildIndex;
							}
							documentNode3 = this.EvaluateResourceAtSpecificSite(resourceSite, keyNode, resourcesHostNodePath, relatedRoots, siteChildIndex, warnings);
						}
						else if (ResourceNodeHelper.IsResourceContainer(resourcesCollection, documentNode2))
						{
							documentNode3 = this.EvaluateResourceAtSpecificSite(resourceSite, keyNode, resourcesHostNodePath, relatedRoots, -1, warnings);
						}
						if (documentNode3 != null)
						{
							if (keyNode != null && keyNode.Parent != null && keyNode.Parent.Parent == i)
							{
								ITextRange nodeSpan = DocumentNodeHelper.GetNodeSpan(keyNode.Parent);
								ITextRange textRange = DocumentNodeHelper.GetNodeSpan(documentNode3);
								if (!TextRange.IsNull(textRange) && !TextRange.IsNull(nodeSpan) && nodeSpan.Offset < textRange.Offset)
								{
									documentNode3 = null;
									invalidForwardReference = true;
								}
							}
							if (documentNode3 != null)
							{
								return documentNode3;
							}
						}
					}
					documentNode2 = i;
				}
			}
			if (flag)
			{
				DocumentNode documentNode4 = this.EvaluateResourceAtSpecificNode(documentRoot1.RootNode, keyNode, resourcesHostNodePath, relatedRoots, warnings);
				if (documentNode4 != null)
				{
					if (relatedRoots != null && documentNode4.DocumentRoot != documentRoot2)
					{
						relatedRoots.Add(documentRoot1);
					}
					return documentNode4;
				}
			}
			if (documentRoot != null)
			{
				using (IEnumerator<IDocumentRoot> enumerator = documentRoot.DesignTimeResources.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IDocumentRoot current = enumerator.Current;
						DocumentNode documentNode5 = this.EvaluateResourceAtSpecificNode(current.RootNode, keyNode, resourcesHostNodePath, relatedRoots, warnings);
						if (documentNode5 == null)
						{
							continue;
						}
						if (relatedRoots != null && documentNode5.DocumentRoot != documentRoot2)
						{
							relatedRoots.Add(current);
						}
						documentNode = documentNode5;
						return documentNode;
					}
					return null;
				}
				return documentNode;
			}
			return null;
		}

		private DocumentNode EvaluateResourceAtSpecificNode(DocumentNode node, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, ICollection<string> warnings)
		{
			ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(node);
			if (resourcesCollection == null)
			{
				return null;
			}
			ResourceSite resourceSite = new ResourceSite(node.Context, resourcesCollection);
			return this.EvaluateResourceAtSpecificSite(resourceSite, keyNode, resourcesHostNodePath, relatedRoots, -1, warnings);
		}

		private DocumentNode EvaluateResourceAtSpecificSite(ResourceSite resourceSite, DocumentNode keyNode, ICollection<DocumentCompositeNode> resourcesHostNodePath, ICollection<IDocumentRoot> relatedRoots, int lastResourceToSearch, ICollection<string> warnings)
		{
			DocumentCompositeNode documentCompositeNode = resourceSite.FindResource(this.documentRootResolver, keyNode, resourcesHostNodePath, relatedRoots, lastResourceToSearch, warnings);
			if (documentCompositeNode == null)
			{
				return null;
			}
			return documentCompositeNode.Properties[KnownProperties.DictionaryEntryValueProperty];
		}

		public DocumentNode EvaluateTemplateBinding(DocumentNodePath nodePath, DocumentCompositeNode templateBindingNode)
		{
			DocumentNode documentNode = null;
			DependencyPropertyReferenceStep member = null;
			DocumentPrimitiveNode item = templateBindingNode.Properties[KnownProperties.TemplateBindingPropertyProperty] as DocumentPrimitiveNode;
			if (item != null)
			{
				DocumentNodeMemberValue value = item.Value as DocumentNodeMemberValue;
				if (value != null)
				{
					member = value.Member as DependencyPropertyReferenceStep;
				}
			}
			if (member != null)
			{
				DocumentNodePath containerOwnerPath = nodePath.GetContainerOwnerPath();
				if (containerOwnerPath != null)
				{
					documentNode = this.EvaluateProperty(containerOwnerPath, member);
					if (documentNode == null)
					{
						DocumentCompositeNode containerNode = nodePath.ContainerNode as DocumentCompositeNode;
						documentNode = this.EvaluateProperty(nodePath.GetPathInContainer(containerNode), member);
					}
				}
			}
			return documentNode;
		}

		private static DocumentNode GetStyleForResourceEntry(DocumentNode node)
		{
			if (PlatformTypes.DictionaryEntry.IsAssignableFrom(node.Type))
			{
				DocumentNode parent = node.Parent;
				if (parent != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(parent.Type) && parent.Parent != null && KnownProperties.StyleResourcesProperty.Equals(parent.SitePropertyKey))
				{
					return parent.Parent;
				}
			}
			return null;
		}

		private static DocumentNode GetStyleForSetter(DocumentNode node)
		{
			if (PlatformTypes.Setter.IsAssignableFrom(node.Type))
			{
				DocumentNode parent = node.Parent;
				if (parent != null && PlatformTypes.SetterBaseCollection.IsAssignableFrom(parent.Type) && parent.Parent != null && KnownProperties.StyleSettersProperty.Equals(parent.SitePropertyKey))
				{
					return parent.Parent;
				}
			}
			return null;
		}
	}
}