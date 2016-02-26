using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public abstract class DependencyObjectInstanceBuilderBase<T> : ClrObjectInstanceBuilder
	where T : class
	{
		public override Type BaseType
		{
			get
			{
				return typeof(T);
			}
		}

		protected DependencyObjectInstanceBuilderBase()
		{
		}

		public override bool AllowPostponedResourceUpdate(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource)
		{
			if (viewNode.Type.Metadata.NameProperty == propertyKey)
			{
				return false;
			}
			DocumentNodePath editingContainer = context.ViewNodeManager.EditingContainer;
			if (editingContainer != null && DependencyObjectInstanceBuilderBase<T>.IsEvaluatedResourceWithinNodePath(editingContainer, viewNode, propertyKey, evaluatedResource))
			{
				return false;
			}
			if (context.IsSerializationScope && !PlatformTypes.UIElement.IsAssignableFrom(viewNode.Type))
			{
				return false;
			}
			if (evaluatedResource.Node != null && PlatformTypes.Visual.IsAssignableFrom(evaluatedResource.Node.Type))
			{
				return false;
			}
			DocumentCompositeNode node = evaluatedResource.Node as DocumentCompositeNode;
			if (node != null && node.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportNonSharedResources) && node.Properties[DesignTimeProperties.SharedProperty] != null && !node.GetValue<bool>(DesignTimeProperties.SharedProperty))
			{
				return false;
			}
			return true;
		}

		public override ViewNode GetViewNode(IInstanceBuilderContext context, DocumentNode documentNode)
		{
			if (context.IsSerializationScope)
			{
				return new InstantiatedElementViewNode(context.ViewNodeManager, documentNode);
			}
			return base.GetViewNode(context, documentNode);
		}

		private static bool IsEvaluatedResourceWithinNodePath(DocumentNodePath nodePath, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource)
		{
			DocumentNodePath correspondingNodePath = null;
			if (nodePath != null && nodePath.Count > 1)
			{
				for (int i = nodePath.Count - 1; i >= 1; i--)
				{
					if (nodePath[i].Container == evaluatedResource.Node && nodePath[i - 1].Target == viewNode.DocumentNode && nodePath[i].PropertyKey == propertyKey)
					{
						if (correspondingNodePath == null)
						{
							correspondingNodePath = viewNode.ViewNodeManager.GetCorrespondingNodePath(viewNode);
						}
						if (correspondingNodePath.Count == i && correspondingNodePath.IsAncestorOf(nodePath))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		protected abstract bool IsIncrementalChange(IInstanceBuilderContext context, ViewNode viewNode, IProperty property);

		protected abstract bool IsSealed(T instance);

		public override void ModifyValue(IInstanceBuilderContext context, ViewNode target, object onlyThisInstance, IProperty propertyKey, object value, PropertyModification modification)
		{
			IProperty shadowProperty;
			if (!context.IsSerializationScope)
			{
				IInstantiatedElementViewNode instantiatedElementViewNode = target as IInstantiatedElementViewNode;
				if (instantiatedElementViewNode != null && onlyThisInstance == null && instantiatedElementViewNode.InstantiatedElements.First != null)
				{
					if (context.UseShadowProperties)
					{
						shadowProperty = DesignTimeProperties.GetShadowProperty(propertyKey, target.DocumentNode.Type);
					}
					else
					{
						shadowProperty = null;
					}
					IProperty property = shadowProperty;
					if (property != null && DesignTimeProperties.UseShadowPropertyForInstanceBuilding(target.TypeResolver, property))
					{
						propertyKey = property;
					}
					ReferenceStep referenceStep = propertyKey as ReferenceStep;
					if (referenceStep != null)
					{
						foreach (object instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
						{
							if (modification != PropertyModification.Set)
							{
								referenceStep.ClearValue(instantiatedElement);
							}
							else
							{
								InstanceBuilderOperations.SetValue(instantiatedElement, referenceStep, value);
							}
						}
						return;
					}
				}
			}
			if (propertyKey.DeclaringType.Metadata.IsNameProperty(propertyKey))
			{
				string valueAsString = null;
				if (context.IsSerializationScope)
				{
					DocumentPrimitiveNode documentPrimitiveNode = value as DocumentPrimitiveNode;
					if (documentPrimitiveNode != null)
					{
						valueAsString = DocumentPrimitiveNode.GetValueAsString(documentPrimitiveNode);
					}
				}
				else
				{
					valueAsString = value as string;
					if (string.IsNullOrEmpty(valueAsString) && (ProjectNeutralTypes.VisualStateGroup.IsAssignableFrom(target.Type) || ProjectNeutralTypes.VisualState.IsAssignableFrom(target.Type)))
					{
						valueAsString = ((DocumentCompositeNode)target.DocumentNode).GetValueAsString(target.DocumentNode.NameProperty);
					}
				}
				if (valueAsString != null && string.IsNullOrEmpty(valueAsString))
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string instanceBuilderNamePropertyNotValid = ExceptionStringTable.InstanceBuilderNamePropertyNotValid;
					object[] objArray = new object[] { valueAsString };
					throw new InstanceBuilderException(string.Format(currentCulture, instanceBuilderNamePropertyNotValid, objArray), target.DocumentNode);
				}
			}
			base.ModifyValue(context, target, onlyThisInstance, propertyKey, value, modification);
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			InstanceState instanceState = target.InstanceState;
			if (target.Instance is DocumentNode && instanceState.IsPropertyInvalid && !instanceState.IsChildInvalid && !instanceState.IsDescendantInvalid)
			{
				IInstantiatedElementViewNode instantiatedElementViewNode = target as IInstantiatedElementViewNode;
				if (instantiatedElementViewNode != null && instantiatedElementViewNode.InstantiatedElements.First != null)
				{
					bool flag = true;
					using (IEnumerator<IProperty> enumerator = instanceState.InvalidProperties.GetEnumerator())
					{
						do
						{
							if (!enumerator.MoveNext())
							{
								break;
							}
							flag = this.IsIncrementalChange(context, target, enumerator.Current);
						}
						while (flag);
					}
					if (flag)
					{
						return;
					}
				}
			}
			bool flag1 = false;
			if (instanceState != InstanceState.Valid)
			{
				T instance = (T)(target.Instance as T);
				if (instance != null && this.IsSealed(instance))
				{
					flag1 = true;
				}
				IInstantiatedElementViewNode instantiatedElementViewNode1 = target as IInstantiatedElementViewNode;
				if (instantiatedElementViewNode1 != null && instantiatedElementViewNode1.InstantiatedElements.First != null)
				{
					foreach (object instantiatedElement in instantiatedElementViewNode1.InstantiatedElements)
					{
						instance = (T)(instantiatedElement as T);
						if (instance == null || !this.IsSealed(instance))
						{
							continue;
						}
						flag1 = true;
						break;
					}
				}
			}
			if (flag1)
			{
				InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			}
			base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
		}

		public override void UpdateProperty(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
		{
			string instance;
			string str;
			IPropertyId shadowProperty;
			ReferenceStep referenceStep = propertyKey as ReferenceStep;
			ViewNode item = viewNode.Properties[propertyKey];
			if (item != null && DocumentNodeUtilities.IsBinding(item.DocumentNode) && referenceStep != null)
			{
				ReferenceStep referenceStep1 = referenceStep;
				if (context.UseShadowProperties)
				{
					shadowProperty = DesignTimeProperties.GetShadowProperty(propertyKey, viewNode.DocumentNode.Type);
				}
				else
				{
					shadowProperty = null;
				}
				IPropertyId propertyId = shadowProperty;
				if (propertyId != null && DesignTimeProperties.UseShadowPropertyForInstanceBuilding(context.DocumentContext.TypeResolver, propertyId))
				{
					referenceStep1 = propertyId as ReferenceStep;
				}
				if (referenceStep1 != null)
				{
					IInstantiatedElementViewNode instantiatedElementViewNode = viewNode as IInstantiatedElementViewNode;
					if (instantiatedElementViewNode == null)
					{
						referenceStep1.ClearValue(viewNode.Instance);
					}
					else
					{
						foreach (object instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
						{
							referenceStep1.ClearValue(instantiatedElement);
						}
					}
				}
			}
			if (propertyKey.DeclaringType.Metadata.IsNameProperty(propertyKey) && InstanceBuilderOperations.GetIsInlinedResourceWithoutNamescope(viewNode))
			{
				InstanceBuilderOperations.UpdatePropertyWithoutApply(context, viewNode, propertyKey, valueNode);
				return;
			}
			INameScope nameScope = context.NameScope;
			bool flag = (nameScope == null ? false : propertyKey.DeclaringType.Metadata.IsNameProperty(propertyKey));
			if (flag)
			{
				ViewNode item1 = viewNode.Properties[propertyKey];
				if (item1 != null)
				{
					str = item1.Instance as string;
				}
				else
				{
					str = null;
				}
				string str1 = str;
				if (!string.IsNullOrEmpty(str1) && nameScope.FindName(str1) != null)
				{
					context.NameScope.UnregisterName(str1);
				}
			}
			if ((!context.IsSerializationScope || !(propertyKey is Event)) && (context.IsSerializationScope || !DesignTimeProperties.IsDocumentOnlyDesignTimeProperty(propertyKey)))
			{
				base.UpdateProperty(context, viewNode, propertyKey, valueNode);
			}
			else
			{
				InstanceBuilderOperations.UpdatePropertyWithoutApply(context, viewNode, propertyKey, valueNode);
			}
			if (flag)
			{
				ViewNode viewNode1 = viewNode.Properties[propertyKey];
				if (viewNode1 != null)
				{
					instance = viewNode1.Instance as string;
				}
				else
				{
					instance = null;
				}
				string str2 = instance;
				if (!string.IsNullOrEmpty(str2))
				{
					try
					{
						if (!str2.StartsWith("~", StringComparison.Ordinal) && !str2.Contains("."))
						{
							if (nameScope.FindName(str2) != null)
							{
								nameScope.UnregisterName(str2);
							}
							nameScope.RegisterName(str2, viewNode.Instance);
						}
					}
					catch (ArgumentException argumentException1)
					{
						ArgumentException argumentException = argumentException1;
						ViewNodeManager viewNodeManager = viewNode1.ViewNodeManager;
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string instanceBuilderUnableToRegisterName = ExceptionStringTable.InstanceBuilderUnableToRegisterName;
						object[] objArray = new object[] { str2 };
						viewNodeManager.OnException(viewNode1, new InstanceBuilderException(string.Format(currentCulture, instanceBuilderUnableToRegisterName, objArray), argumentException, viewNode1.DocumentNode, viewNode), false);
					}
				}
			}
		}
	}
}