using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public abstract class DocumentNodeBuilderBase
	{
		private Type baseType;

		public Type BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		protected DocumentNodeBuilderBase(Type baseType)
		{
			this.baseType = baseType;
		}

		protected DocumentCompositeNode CreateCompositeNode(IDocumentContext documentContext, Type instanceType)
		{
			return this.CreateCompositeNode(documentContext, instanceType, documentContext.GetChildNodeType(instanceType));
		}

		protected virtual DocumentCompositeNode CreateCompositeNode(IDocumentContext documentContext, Type instanceType, Type childType)
		{
			ITypeResolver typeResolver = documentContext.TypeResolver;
			IType type = typeResolver.GetType(instanceType);
			if (type == null || !TypeHelper.IsAccessibleType(typeResolver, type))
			{
				return null;
			}
			IType type1 = null;
			if (childType != null)
			{
				type1 = typeResolver.GetType(childType);
				if (type1 == null || !TypeHelper.IsAccessibleType(typeResolver, type1))
				{
					return null;
				}
			}
			return new DocumentCompositeNode(documentContext, type, type1);
		}

		protected virtual DocumentNode CreatePrimitiveNode(NodeBuilderContext context, Type instanceType, object value)
		{
			IType type = context.TypeResolver.GetType(instanceType);
			if (type == null || !TypeHelper.IsAccessibleType(context.TypeResolver, type))
			{
				return null;
			}
			return new DocumentPrimitiveNode(context.DocumentContext, type, value);
		}

		public virtual Type GetChildType(Type instanceType)
		{
			CollectionAdapterDescription adapterDescription = CollectionAdapterDescription.GetAdapterDescription(instanceType);
			if (adapterDescription == null)
			{
				return CollectionAdapterDescription.GetGenericCollectionType(instanceType);
			}
			return adapterDescription.ItemType;
		}

		[Conditional("DEBUG")]
		protected virtual void VerifyInstanceType(Type instanceType, object instance)
		{
			if (!this.baseType.IsAssignableFrom(instanceType))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string nodeBuilderInstanceTypeDoesNotDeriveFromBuilderBaseType = ExceptionStringTable.NodeBuilderInstanceTypeDoesNotDeriveFromBuilderBaseType;
				object[] name = new object[] { instanceType.Name, this.baseType.Name };
				throw new InvalidOperationException(string.Format(currentCulture, nodeBuilderInstanceTypeDoesNotDeriveFromBuilderBaseType, name));
			}
			if (instance != null && !instanceType.IsInstanceOfType(instance))
			{
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				string nodeBuilderInstanceDoesNotMatchType = ExceptionStringTable.NodeBuilderInstanceDoesNotMatchType;
				object[] objArray = new object[] { instance.GetType().Name, instanceType.Name };
				throw new InvalidOperationException(string.Format(cultureInfo, nodeBuilderInstanceDoesNotMatchType, objArray));
			}
		}
	}
}