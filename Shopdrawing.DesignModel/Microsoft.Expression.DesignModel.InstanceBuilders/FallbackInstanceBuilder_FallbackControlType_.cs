using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class FallbackInstanceBuilder<FallbackControlType> : IInstanceBuilder
	where FallbackControlType : new()
	{
		public Type BaseType
		{
			get
			{
				return typeof(object);
			}
		}

		public virtual Type ReplacementType
		{
			get
			{
				return null;
			}
		}

		public FallbackInstanceBuilder()
		{
		}

		public bool AllowPostponedResourceUpdate(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource)
		{
			return false;
		}

		public AttachmentOrder GetAttachmentOrder(IInstanceBuilderContext context, ViewNode viewNode)
		{
			return AttachmentOrder.PostInitialization;
		}

		public ViewNode GetViewNode(IInstanceBuilderContext context, DocumentNode documentNode)
		{
			throw new InvalidOperationException();
		}

		public void Initialize(IInstanceBuilderContext context, ViewNode viewNode, bool isNewInstance)
		{
		}

		public bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			DocumentNode documentNode = viewNode.DocumentNode;
			DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
			bool flag = false;
			if (!PlatformTypes.FrameworkElement.IsAssignableFrom(viewNode.Type))
			{
				if (documentCompositeNode != null && documentCompositeNode.SupportsChildren && documentCompositeNode.Children.Count > 0)
				{
					IList listAdapter = InstanceBuilderOperations.GetListAdapter(viewNode.Instance);
					if (listAdapter != null)
					{
						listAdapter.Clear();
					}
				}
				flag = true;
				viewNode.Instance = null;
			}
			else
			{
				IPlatformMetadata platformMetadata = documentNode.TypeResolver.PlatformMetadata;
				if (!context.IsSerializationScope)
				{
					try
					{
						ViewNode viewNode1 = viewNode;
						FallbackControlType fallbackControlType = default(FallbackControlType);
						viewNode1.Instance = (fallbackControlType == null ? Activator.CreateInstance<FallbackControlType>() : default(FallbackControlType));
					}
					catch (Exception exception)
					{
						viewNode.Instance = null;
						flag = true;
					}
				}
				else
				{
					DocumentCompositeNode documentCompositeNode1 = context.DocumentContext.CreateNode(typeof(FallbackControlType));
					ViewNodeId id = context.SerializationContext.GetId(viewNode);
					documentCompositeNode1.Properties[DesignTimeProperties.ViewNodeIdProperty] = context.DocumentContext.CreateNode(typeof(string), ViewNodeManager.ViewNodeIdConverter.ConvertToInvariantString(id));
					viewNode.Instance = documentCompositeNode1;
				}
				if (viewNode.Instance != null && documentCompositeNode != null)
				{
					if (documentCompositeNode != null)
					{
						DocumentNode exceptionSource = context.ExceptionDictionary.GetExceptionSource(viewNode);
						Type runtimeType = documentNode.TypeResolver.ResolveType(PlatformTypes.FrameworkElement).RuntimeType;
						foreach (IProperty property in context.GetProperties(viewNode))
						{
							try
							{
								ViewNode viewNode2 = InstanceBuilderOperations.UpdatePropertyWithoutApply(context, viewNode, property, documentCompositeNode.Properties[property]);
								if (viewNode2 != null && viewNode2.Instance != null && viewNode2.DocumentNode != exceptionSource)
								{
									Type type = (context.IsSerializationScope ? ((DocumentNode)viewNode2.Instance).TargetType : viewNode2.Instance.GetType());
									ReferenceStep referenceStep = property as ReferenceStep;
									if (referenceStep != null && referenceStep.TargetType.IsAssignableFrom(runtimeType) && PlatformTypeHelper.GetPropertyType(referenceStep).IsAssignableFrom(type))
									{
										InstanceBuilderOperations.SetValue(viewNode.Instance, referenceStep, viewNode2.Instance);
									}
								}
							}
							catch
							{
							}
						}
					}
					IProperty property1 = platformMetadata.ResolveProperty(KnownProperties.FrameworkElementMinWidthProperty);
					InstanceBuilderOperations.SetValue(viewNode.Instance, property1, FallbackInstanceBuilder<FallbackControlType>.MinSize(context));
					IProperty property2 = platformMetadata.ResolveProperty(KnownProperties.FrameworkElementMinHeightProperty);
					InstanceBuilderOperations.SetValue(viewNode.Instance, property2, FallbackInstanceBuilder<FallbackControlType>.MinSize(context));
				}
			}
			if (flag)
			{
				viewNode.Instance = ClrObjectInstanceBuilder.InvalidObjectSentinel;
			}
			viewNode.InstanceState = InstanceState.Valid;
			return true;
		}

		private static object MinSize(IInstanceBuilderContext context)
		{
			object obj = 50;
			if (context.IsSerializationScope)
			{
				obj = context.DocumentContext.CreateNode(typeof(double), 50);
			}
			return obj;
		}

		public void ModifyValue(IInstanceBuilderContext context, ViewNode target, object onlyThisInstance, IProperty propertyKey, object value, PropertyModification modification)
		{
		}

		public void OnChildRemoving(IInstanceBuilderContext context, ViewNode parent, ViewNode child)
		{
		}

		public void OnDescendantUpdated(IInstanceBuilderContext context, ViewNode viewNode, ViewNode child, InstanceState childState)
		{
		}

		public void OnInitialized(IInstanceBuilderContext context, ViewNode target, object instance)
		{
		}

		public void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
		}

		public bool ShouldTryExpandExpression(IInstanceBuilderContext context, ViewNode viewNode, IPropertyId propertyKey, DocumentNode expressionNode)
		{
			return false;
		}

		public void UpdateChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, DocumentNodeChangeAction action, DocumentNode childNode)
		{
			throw new InvalidOperationException();
		}

		public void UpdateInstance(IInstanceBuilderContext context, ViewNode viewNode)
		{
			throw new InvalidOperationException();
		}

		public void UpdateProperty(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
		{
			throw new InvalidOperationException();
		}
	}
}