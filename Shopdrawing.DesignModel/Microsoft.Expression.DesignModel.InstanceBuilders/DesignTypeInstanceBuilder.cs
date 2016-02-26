using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class DesignTypeInstanceBuilder : MarkupExtensionInstanceBuilderBase
	{
		public override Type BaseType
		{
			get
			{
				return typeof(DesignInstanceExtension);
			}
		}

		public DesignTypeInstanceBuilder()
		{
		}

		private Type GetTypeToInstantiate(IInstanceBuilderContext context, DesignInstanceExtension designMarkupExtension, out string warning)
		{
			warning = null;
			if (designMarkupExtension.Type == null)
			{
				return null;
			}
			if (designMarkupExtension.IsDesignTimeCreatable && !designMarkupExtension.CreateList)
			{
				return designMarkupExtension.Type;
			}
			if (!designMarkupExtension.IsDesignTimeCreatable && !DesignTypeInstanceBuilder.IsTypeEmittable(designMarkupExtension.Type))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string instanceBuilderDesignTypeCannotBeLateBound = ExceptionStringTable.InstanceBuilderDesignTypeCannotBeLateBound;
				object[] name = new object[] { designMarkupExtension.Type.Name };
				warning = string.Format(currentCulture, instanceBuilderDesignTypeCannotBeLateBound, name);
				return null;
			}
			DesignTypeResult typeToInstantiate = DesignTypeInstanceBuilder.GetTypeToInstantiate(context.DocumentContext.TypeResolver.PlatformMetadata, designMarkupExtension.Type, designMarkupExtension.CreateList, designMarkupExtension.IsDesignTimeCreatable);
			if (!typeToInstantiate.IsFailed)
			{
				return typeToInstantiate.DesignType;
			}
			if (string.IsNullOrEmpty(typeToInstantiate.Context))
			{
				warning = typeToInstantiate.TypeGenerationException.Message;
			}
			else
			{
				CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
				object[] message = new object[] { typeToInstantiate.TypeGenerationException.Message, typeToInstantiate.Context };
				warning = string.Format(currentUICulture, "{0} ({1})", message);
			}
			return null;
		}

		public static DesignTypeResult GetTypeToInstantiate(IPlatformMetadata platformMetadata, Type sourceType, bool createList, bool isDesignTimeCreatable)
		{
			Type designType = sourceType;
			DesignTypeGenerator designTypeGenerator = new DesignTypeGenerator(platformMetadata);
			if (!isDesignTimeCreatable)
			{
				DesignTypeResult designTypeResult = designTypeGenerator.GetDesignType(sourceType);
				if (designTypeResult.IsFailed)
				{
					return designTypeResult;
				}
				designType = designTypeResult.DesignType;
			}
			if (!(designType != null) || !createList)
			{
				return new DesignTypeResult(sourceType, designType);
			}
			return designTypeGenerator.GetXamlFriendlyListType(designType);
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			string str;
			if (viewNode.Instance == null)
			{
				DesignInstanceExtension instance = null;
				using (IDisposable disposable = context.ChangeSerializationContext(null))
				{
					base.Instantiate(context, viewNode);
					instance = viewNode.Instance as DesignInstanceExtension;
				}
				instance.Instance = null;
				Type typeToInstantiate = this.GetTypeToInstantiate(context, instance, out str);
				if (typeToInstantiate != null)
				{
					if (context.IsSerializationScope)
					{
						IType type = context.DocumentContext.TypeResolver.GetType(typeToInstantiate);
						context.DocumentContext.CreateNode(type);
						viewNode.Instance = context.DocumentContext.CreateNode(typeToInstantiate);
						viewNode.InstanceState = InstanceState.Valid;
					}
					else
					{
						instance.Instance = Activator.CreateInstance(typeToInstantiate);
						viewNode.Instance = instance;
						viewNode.InstanceState = InstanceState.Valid;
					}
				}
				else if (!string.IsNullOrEmpty(str))
				{
					context.WarningDictionary.SetWarning(viewNode, viewNode.DocumentNode, str);
				}
				if (viewNode.Instance == instance && instance.Instance == null)
				{
					viewNode.Instance = null;
					viewNode.InstanceState = InstanceState.Valid;
				}
			}
			return viewNode.Instance != null;
		}

		private static bool IsTypeEmittable(Type type)
		{
			if (typeof(ITypedList).IsAssignableFrom(type))
			{
				return false;
			}
			return !typeof(ICustomTypeDescriptor).IsAssignableFrom(type);
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
			if (target.InstanceState != InstanceState.Invalid)
			{
				InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			}
		}
	}
}