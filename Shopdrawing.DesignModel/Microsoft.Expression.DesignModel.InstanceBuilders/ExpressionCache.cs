using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	[Serializable]
	internal sealed class ExpressionCache : Dictionary<DocumentNode, List<ViewNode>>, IExpressionCache
	{
		public ExpressionCache()
		{
		}

		public void CacheExpressionValue(ViewNode target, DocumentNode sourceNode)
		{
			List<ViewNode> viewNodes;
			base.TryGetValue(sourceNode, out viewNodes);
			if (viewNodes != null)
			{
				if (!viewNodes.Contains(target))
				{
					viewNodes.Add(target);
				}
				return;
			}
			viewNodes = new List<ViewNode>()
			{
				target
			};
			base[sourceNode] = viewNodes;
		}

		public List<ViewNode> GetExpressionValue(DocumentNode target)
		{
			List<ViewNode> viewNodes;
			List<ViewNode> viewNodes1 = new List<ViewNode>();
			List<ViewNode> viewNodes2 = null;
			if (base.TryGetValue(target, out viewNodes))
			{
				foreach (ViewNode viewNode in viewNodes)
				{
					if (viewNode.Parent != null)
					{
						if (DocumentNodeUtilities.IsBinding(target))
						{
							continue;
						}
						viewNodes1.Add(viewNode);
					}
					else
					{
						if (viewNodes2 == null)
						{
							viewNodes2 = new List<ViewNode>();
						}
						viewNodes2.Add(viewNode);
					}
				}
				if (viewNodes2 != null)
				{
					if (viewNodes2.Count != viewNodes.Count)
					{
						this.PurgePartialEntry(viewNodes, viewNodes2);
					}
					else
					{
						base.Remove(target);
					}
				}
			}
			return viewNodes1;
		}

		private static bool HasInvalidAncestor(IInstanceBuilderContext context, ViewNode source, out bool hitRoot)
		{
			ViewNode parent = source;
			hitRoot = false;
			while (parent != null)
			{
				if (parent.InstanceState == InstanceState.Invalid)
				{
					return true;
				}
				hitRoot = parent == context.ViewNodeManager.Root;
				parent = parent.Parent;
			}
			return false;
		}

		private void PurgePartialEntry(List<ViewNode> target, List<ViewNode> invalidEntries)
		{
			int num = 0;
			int item = 0;
			for (int i = 0; i < target.Count; i++)
			{
				if (num >= invalidEntries.Count || target[i] != invalidEntries[num])
				{
					target[item] = target[i];
					item++;
				}
				else
				{
					num++;
				}
			}
			for (int j = target.Count - 1; j >= item; j--)
			{
				target.RemoveAt(j);
			}
		}

		public List<ViewNode> Validate(IInstanceBuilderContext context, out List<ExpressionSite> sites)
		{
			ExpressionSite expressionSite;
			bool flag;
			DocumentNode item;
			DocumentNode documentNode;
			DocumentNode item1;
			List<DocumentNode> documentNodes = new List<DocumentNode>();
			List<ViewNode> viewNodes = new List<ViewNode>();
			List<ViewNode> viewNodes1 = new List<ViewNode>();
			sites = new List<ExpressionSite>();
			ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator(context.DocumentRootResolver);
			foreach (KeyValuePair<DocumentNode, List<ViewNode>> keyValuePair in this)
			{
				DocumentNode key = keyValuePair.Key;
				viewNodes1.Clear();
				foreach (ViewNode value in keyValuePair.Value)
				{
					ViewNode parent = value.Parent;
					if (parent != null)
					{
						DocumentNode documentNode1 = parent.DocumentNode;
						if (!value.IsProperty)
						{
							int num = value.Parent.Children.IndexOf(value);
							expressionSite = new ExpressionSite(num);
						}
						else
						{
							expressionSite = new ExpressionSite(value.SitePropertyKey);
						}
						bool flag1 = ExpressionCache.HasInvalidAncestor(context, parent, out flag);
						if (!flag)
						{
							continue;
						}
						if (!flag1)
						{
							DocumentNodePath correspondingNodePath = context.ViewNodeManager.GetCorrespondingNodePath(parent);
							DocumentCompositeNode node = (DocumentCompositeNode)correspondingNodePath.Node;
							if (expressionSite.IsProperty)
							{
								item = node.Properties[expressionSite.PropertyKey];
							}
							else if (node.Children == null)
							{
								if (expressionSite.ChildIndex < node.ConstructorArguments.Count)
								{
									documentNode = node.ConstructorArguments[expressionSite.ChildIndex];
								}
								else
								{
									documentNode = null;
								}
								item = documentNode;
							}
							else
							{
								if (expressionSite.ChildIndex < node.Children.Count)
								{
									item1 = node.Children[expressionSite.ChildIndex];
								}
								else
								{
									item1 = null;
								}
								item = item1;
							}
							DocumentNode documentNode2 = value.DocumentNode;
							bool flag2 = true;
							if (item != null)
							{
								if (!DocumentNodeUtilities.IsBinding(value.DocumentNode))
								{
									flag2 = false;
									documentNode2 = expressionEvaluator.EvaluateExpression(correspondingNodePath, item);
								}
								else
								{
									IPropertyId propertyId = value.DocumentNode.TypeResolver.ResolveProperty(Microsoft.Expression.DesignModel.Metadata.KnownProperties.BindingElementNameProperty);
									if (propertyId != null)
									{
										DocumentNode item2 = ((DocumentCompositeNode)value.DocumentNode).Properties[propertyId];
										if (item2 != null)
										{
											DocumentPrimitiveNode documentPrimitiveNode = item2 as DocumentPrimitiveNode;
											if (documentPrimitiveNode != null)
											{
												DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
												if (documentNodeStringValue != null)
												{
													string str = documentNodeStringValue.Value;
													object obj = context.NameScope.FindName(str);
													if (obj != null)
													{
														ViewNode viewNode = context.InstanceDictionary.GetViewNode(obj, false);
														InstanceState invalid = InstanceState.Invalid;
														if (viewNode != null && !ExpressionCache.HasInvalidAncestor(context, viewNode, out flag))
														{
															invalid = viewNode.InstanceState;
														}
														if (invalid != InstanceState.Invalid && (invalid.InvalidProperties == null || !invalid.InvalidProperties.Contains(viewNode.DocumentNode.NameProperty)))
														{
															documentNode2 = viewNode.DocumentNode;
															flag2 = false;
														}
														else if (parent.Instance is VisualBrush && expressionSite.IsProperty && Microsoft.Expression.DesignModel.Metadata.KnownProperties.VisualBrushVisualProperty.Equals(expressionSite.PropertyKey))
														{
															BindingOperations.ClearBinding((VisualBrush)parent.Instance, VisualBrush.VisualProperty);
														}
													}
												}
											}
										}
									}
								}
							}
							if (key != item == (documentNode2 != null) && (documentNode2 == null || documentNode2 == key) && !flag2)
							{
								continue;
							}
							viewNodes1.Add(value);
							viewNodes.Add(parent);
							sites.Add(expressionSite);
						}
						else
						{
							viewNodes1.Add(value);
						}
					}
					else
					{
						viewNodes1.Add(value);
					}
				}
				if (viewNodes1.Count != keyValuePair.Value.Count)
				{
					this.PurgePartialEntry(keyValuePair.Value, viewNodes1);
				}
				else
				{
					documentNodes.Add(keyValuePair.Key);
				}
			}
			foreach (DocumentNode documentNode3 in documentNodes)
			{
				base.Remove(documentNode3);
			}
			return viewNodes;
		}
	}
}