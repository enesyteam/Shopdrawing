using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class MarkupExtensionInstanceBuilderBase : ClrObjectInstanceBuilder
	{
		private static Comparison<DocumentNode> DocumentNodeComparer;

		static MarkupExtensionInstanceBuilderBase()
		{
			MarkupExtensionInstanceBuilderBase.DocumentNodeComparer = (DocumentNode n1, DocumentNode n2) => {
				if (n1.ContainerSourceContext == null || n2.ContainerSourceContext == null)
				{
					return 0;
				}
				return n1.ContainerSourceContext.TextRange.Offset - n2.ContainerSourceContext.TextRange.Offset;
			};
		}

		public MarkupExtensionInstanceBuilderBase()
		{
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			IConstructorArgumentNodeCollection constructorArgumentNodeCollections;
			if (viewNode.Instance == null)
			{
				DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
				if (documentNode == null)
				{
					return base.Instantiate(context, viewNode);
				}
				IType type = documentNode.Type;
				IConstructor bestConstructor = documentNode.GetBestConstructor(out constructorArgumentNodeCollections);
				if (bestConstructor == null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string instanceBuilderCannotInstantiateType = ExceptionStringTable.InstanceBuilderCannotInstantiateType;
					object[] name = new object[] { type.Name };
					throw new InstanceBuilderException(string.Format(currentCulture, instanceBuilderCannotInstantiateType, name), documentNode);
				}
				IConstructorArgumentProperties constructorArgumentProperties = type.GetConstructorArgumentProperties();
				IPropertyId[] propertyIdArray = new IPropertyId[constructorArgumentNodeCollections.Count];
				IPropertyId[] propertyIdArray1 = propertyIdArray;
				propertyIdArray1 = propertyIdArray;
				if (!context.IsSerializationScope)
				{
					object[] objArray = new object[constructorArgumentNodeCollections.Count];
					for (int i = 0; i < constructorArgumentNodeCollections.Count; i++)
					{
						IParameter item = bestConstructor.Parameters[i];
						IProperty property = constructorArgumentProperties[item.Name];
						propertyIdArray1[i] = property;
						objArray[i] = MarkupExtensionInstanceBuilderBase.InstantiateConstructorArgument(context, viewNode, item, property, constructorArgumentNodeCollections[i]);
					}
					try
					{
						viewNode.Instance = bestConstructor.Invoke(objArray);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						CultureInfo cultureInfo = CultureInfo.CurrentCulture;
						string str = ExceptionStringTable.InstanceBuilderCannotInstantiateType;
						object[] name1 = new object[] { type.Name };
						throw new InstanceBuilderException(string.Format(cultureInfo, str, name1), exception, documentNode);
					}
				}
				else
				{
					DocumentNode[] documentNodeArray = new DocumentNode[constructorArgumentNodeCollections.Count];
					for (int j = 0; j < constructorArgumentNodeCollections.Count; j++)
					{
						IParameter parameter = bestConstructor.Parameters[j];
						IProperty item1 = constructorArgumentProperties[parameter.Name];
						propertyIdArray1[j] = item1;
						documentNodeArray[j] = (DocumentNode)MarkupExtensionInstanceBuilderBase.InstantiateConstructorArgument(context, viewNode, parameter, item1, constructorArgumentNodeCollections[j]);
					}
					type = (IType)type.Clone(context.DocumentContext.TypeResolver);
					DocumentCompositeNode documentCompositeNode = context.DocumentContext.CreateNode(type);
					documentCompositeNode.SetConstructor(bestConstructor, documentNodeArray);
					viewNode.Instance = documentCompositeNode;
				}
				if (viewNode.Instance != null)
				{
					List<DocumentNode> documentNodes = null;
					foreach (IProperty property1 in context.GetProperties(viewNode))
					{
						if (Array.IndexOf<IPropertyId>(propertyIdArray1, property1) >= 0)
						{
							continue;
						}
						if (documentNodes == null)
						{
							documentNodes = new List<DocumentNode>();
						}
						documentNodes.Add(documentNode.Properties[property1]);
					}
					if (documentNodes != null)
					{
						documentNodes.Sort(MarkupExtensionInstanceBuilderBase.DocumentNodeComparer);
						foreach (DocumentNode documentNode1 in documentNodes)
						{
							this.UpdateProperty(context, viewNode, documentNode1.SitePropertyKey, documentNode1);
						}
					}
				}
			}
			viewNode.InstanceState = InstanceState.Valid;
			return true;
		}

		private static object InstantiateConstructorArgument(IInstanceBuilderContext context, ViewNode viewNode, IParameter constructorArgument, IProperty constructorArgumentProperty, DocumentNode constructorArgumentNode)
		{
			object defaultValue;
			if (constructorArgumentProperty != null)
			{
				return InstanceBuilderOperations.UpdatePropertyWithoutApply(context, viewNode, constructorArgumentProperty, constructorArgumentNode).Instance;
			}
			if (constructorArgumentNode != null)
			{
				ViewNode viewNode1 = InstanceBuilderOperations.UpdateChildWithoutApply(context, viewNode, viewNode.Children.Count, DocumentNodeChangeAction.Add, constructorArgumentNode);
				return viewNode1.Instance;
			}
			IDocumentContext documentContext = viewNode.DocumentNode.Context;
			Type targetType = viewNode.TargetType;
			Type runtimeType = constructorArgument.ParameterType.RuntimeType;
			ReferenceStep referenceStep = constructorArgumentProperty as ReferenceStep;
			if (referenceStep == null || !referenceStep.HasDefaultValue(targetType))
			{
				bool flag = documentContext.TypeResolver.InTargetAssembly(constructorArgument.ParameterType);
				defaultValue = InstanceBuilderOperations.InstantiateType(runtimeType, flag);
			}
			else
			{
				defaultValue = referenceStep.GetDefaultValue(targetType);
			}
			if (context.IsSerializationScope)
			{
				defaultValue = documentContext.CreateNode(runtimeType, defaultValue);
			}
			return defaultValue;
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
		}
	}
}