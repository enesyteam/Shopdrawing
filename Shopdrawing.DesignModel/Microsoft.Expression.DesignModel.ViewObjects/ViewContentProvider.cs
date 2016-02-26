using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public abstract class ViewContentProvider
	{
		public virtual Type BaseType
		{
			get
			{
				return typeof(object);
			}
		}

		protected ViewContentProvider()
		{
		}

		public static T InstantiateTargetType<T>(IAssembly projectAssembly, IInstanceBuilderContext context, Type type)
		where T : class
		{
			if (!(type != null) || type.IsInterface)
			{
				return Activator.CreateInstance<T>();
			}
			return (T)ViewContentProvider.InstantiateTargetType(projectAssembly, context, type);
		}

		public static object InstantiateTargetType(IAssembly projectAssembly, IInstanceBuilderContext context, Type type)
		{
			bool flag = (projectAssembly == null ? false : projectAssembly.CompareTo(type.Assembly));
			IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(type);
			if (builder.ReplacementType == null)
			{
				return InstanceBuilderOperations.InstantiateType(type, flag);
			}
			return InstanceBuilderOperations.InstantiateType(builder.ReplacementType, true);
		}

		public abstract ViewContent ProvideContent(ContentProviderParameters contentProviderContext);

		public virtual bool UpdateContent(ContentProviderParameters contentProviderContext)
		{
			return false;
		}
	}
}