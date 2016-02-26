using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public sealed class ArrayInstanceBuilder : ClrObjectInstanceBuilder
	{
		public override Type BaseType
		{
			get
			{
				return typeof(Array);
			}
		}

		public ArrayInstanceBuilder()
		{
		}

		protected override object InstantiateTargetType(IInstanceBuilderContext context, ViewNode viewNode)
		{
			DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
			if (documentNode == null)
			{
				return base.InstantiateTargetType(context, viewNode);
			}
			object obj = null;
			Type targetType = documentNode.TargetType;
			if (targetType.IsArray)
			{
				Type elementType = targetType.GetElementType();
				if (elementType != null)
				{
					try
					{
						obj = Array.CreateInstance(elementType, documentNode.Children.Count);
					}
					catch
					{
					}
				}
			}
			return obj;
		}

		protected override void VerifyTargetType(Type targetType)
		{
			if (!targetType.IsArray)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string instanceBuilderTargetTypeIsNotArray = ExceptionStringTable.InstanceBuilderTargetTypeIsNotArray;
				object[] name = new object[] { targetType.Name };
				throw new InvalidOperationException(string.Format(currentCulture, instanceBuilderTargetTypeIsNotArray, name));
			}
		}
	}
}