using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public abstract class ViewContentManager
	{
		public virtual Type BaseType
		{
			get
			{
				return typeof(object);
			}
		}

		public abstract IViewContentProviderFactory Factory
		{
			get;
		}

		protected ViewContentManager()
		{
		}

		public virtual object GetStandardStyle(Type targetType, IDocumentContext documentContext, IInstanceBuilderContext instanceBuilderContext)
		{
			return null;
		}

		public abstract ViewContent ProvideContent(ContentProviderParameters contentProviderContext);

		protected static bool ShouldSkipProperty(ReferenceStep property, ICollection<IProperty> propertiesToSkip)
		{
			bool flag;
			if (property == null)
			{
				return true;
			}
			DependencyPropertyReferenceStep dependencyPropertyReferenceStep = property as DependencyPropertyReferenceStep;
			using (IEnumerator<IProperty> enumerator = propertiesToSkip.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProperty current = enumerator.Current;
					if (!current.Equals(property))
					{
						if (dependencyPropertyReferenceStep == null)
						{
							continue;
						}
						DependencyPropertyReferenceStep dependencyPropertyReferenceStep1 = current as DependencyPropertyReferenceStep;
						if (dependencyPropertyReferenceStep1 == null || dependencyPropertyReferenceStep.DependencyProperty != dependencyPropertyReferenceStep1.DependencyProperty)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					else
					{
						flag = true;
						return flag;
					}
				}
				return false;
			}
			return flag;
		}

		public virtual bool UpdateContent(ContentProviderParameters contentProviderContext)
		{
			return false;
		}

		internal class ViewContentProviderFactory : TypeHandlerFactory<ViewContentProvider>, IViewContentProviderFactory
		{
			public ViewContentProviderFactory(Action initializer) : base(initializer)
			{
			}

			protected override Type GetBaseType(ViewContentProvider handler)
			{
				return handler.BaseType;
			}

			public ViewContentProvider GetProvider(object platformObject)
			{
				if (platformObject == null)
				{
					return null;
				}
				return base.GetHandler(platformObject.GetType());
			}

			public void Register(ViewContentProvider value)
			{
				base.RegisterHandler(value);
			}

			public void Unregister(ViewContentProvider value)
			{
				base.UnregisterHandler(value);
			}
		}
	}
}