using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	internal sealed class DocumentNodePropertyBuilderFactory : TypeHandlerFactory<IDocumentNodePropertyBuilder>, IDocumentNodePropertyBuilderFactory
	{
		private IDocumentNodePropertyBuilder defaultPropertyBuilder;

		public IDocumentNodePropertyBuilder DefaultPropertyBuilder
		{
			get
			{
				return this.defaultPropertyBuilder;
			}
			set
			{
				this.defaultPropertyBuilder = value;
			}
		}

		public DocumentNodePropertyBuilderFactory(Action initializer) : base(initializer)
		{
		}

		protected override Type GetBaseType(IDocumentNodePropertyBuilder handler)
		{
			return handler.BaseType;
		}

		private IDocumentNodePropertyBuilder GetDefaultHandler(ITypeResolver typeResolver, Type type)
		{
			IDocumentNodePropertyBuilder documentNodePropertyBuilder;
			base.InitializeIfNecessary();
			IType type1 = typeResolver.GetType(type);
			if (type1 != null)
			{
				if (type.IsValueType && type1.TypeConverter != null && !PlatformTypes.TypeConverter.Equals(typeResolver.GetType(type1.TypeConverter.GetType())))
				{
					return null;
				}
				MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, type1);
				using (IEnumerator<IProperty> enumerator = type1.Metadata.Properties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IProperty current = enumerator.Current;
						if (!current.ShouldSerialize || !TypeHelper.IsSet(allowableMemberAccess, current.WriteAccess) && current.PropertyType.ItemType == null)
						{
							continue;
						}
						documentNodePropertyBuilder = this.defaultPropertyBuilder;
						return documentNodePropertyBuilder;
					}
					return null;
				}
				return documentNodePropertyBuilder;
			}
			return null;
		}

		private IDocumentNodePropertyBuilder GetHandler(ITypeResolver typeResolver, Type type)
		{
			IDocumentNodePropertyBuilder documentNodePropertyBuilder;
			if (!base.GetCachedHandler(type, out documentNodePropertyBuilder))
			{
				documentNodePropertyBuilder = base.DetermineBestHandler(this.GetDefaultHandler(typeResolver, type), type);
				base.CacheHandler(type, documentNodePropertyBuilder);
			}
			return documentNodePropertyBuilder;
		}

		public IDocumentNodePropertyBuilder GetPropertyBuilder(ITypeResolver typeResolver, Type type)
		{
			return this.GetHandler(typeResolver, type);
		}

		public void Register(IDocumentNodePropertyBuilder builder)
		{
			base.RegisterHandler(builder);
		}

		public void Unregister(IDocumentNodePropertyBuilder builder)
		{
			base.UnregisterHandler(builder);
		}
	}
}