using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class TypeIdHandlerFactory<TypeHandler>
	{
		private List<TypeHandler> handlers;

		private Dictionary<ITypeId, KeyValuePair<ITypeId, TypeHandler>> handlerCache;

		public ICollection<TypeHandler> Handlers
		{
			get
			{
				this.InitializeIfNecessary();
				return this.handlers;
			}
		}

		protected bool IsInitialized
		{
			get
			{
				return this.handlers != null;
			}
		}

		protected TypeIdHandlerFactory()
		{
		}

		protected void CacheHandler(ITypeId type, TypeHandler handler)
		{
			this.InitializeIfNecessary();
			this.handlerCache[type] = new KeyValuePair<ITypeId, TypeHandler>(type, handler);
		}

		[Conditional("DEBUG")]
		private void CheckNoRegisteredHandler(ITypeId type)
		{
			foreach (TypeHandler handler in this.handlers)
			{
				if (type != this.GetBaseType(handler))
				{
					continue;
				}
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string typeHandlerFactoryHandlerForTypeExists = ExceptionStringTable.TypeHandlerFactoryHandlerForTypeExists;
				object[] name = new object[] { type.Name };
				throw new InvalidOperationException(string.Format(currentCulture, typeHandlerFactoryHandlerForTypeExists, name));
			}
		}

		protected TypeHandler DetermineBestHandler(TypeHandler handler, IMetadataResolver typeResolver, IType type)
		{
			this.InitializeIfNecessary();
			ITypeId typeId = typeResolver.ResolveType(PlatformTypes.Object);
			ITypeId implementingType = typeId;
			foreach (TypeHandler typeHandler in this.handlers)
			{
				IType type1 = typeResolver.ResolveType(this.GetBaseType(typeHandler));
				if (type1 == null || !type1.IsAssignableFrom(type) || !typeId.IsAssignableFrom(type1) && (!type1.IsInterface || type1.IsAssignableFrom(implementingType)))
				{
					continue;
				}
				handler = typeHandler;
				typeId = type1;
				implementingType = this.GetImplementingType(type1, type);
			}
			return handler;
		}

		protected abstract ITypeId GetBaseType(TypeHandler handler);

		protected bool GetCachedHandler(ITypeId type, out KeyValuePair<ITypeId, TypeHandler> handler)
		{
			this.InitializeIfNecessary();
			return this.handlerCache.TryGetValue(type, out handler);
		}

		protected virtual TypeHandler GetDefaultHandler(ITypeId type)
		{
			return default(TypeHandler);
		}

		protected TypeHandler GetHandler(IMetadataResolver typeResolver, IType type)
		{
			KeyValuePair<ITypeId, TypeHandler> keyValuePair;
			if (!this.GetCachedHandler(type, out keyValuePair) || keyValuePair.Key != type)
			{
				TypeHandler typeHandler = this.DetermineBestHandler(this.GetDefaultHandler(type), typeResolver, type);
				this.CacheHandler(type, typeHandler);
				keyValuePair = new KeyValuePair<ITypeId, TypeHandler>(type, typeHandler);
			}
			return keyValuePair.Value;
		}

		protected ITypeId GetImplementingType(IType baseType, IType targetType)
		{
			if (!baseType.IsInterface)
			{
				return baseType;
			}
			IType type = targetType;
			while (type.BaseType != null && baseType.IsAssignableFrom(type.BaseType))
			{
				type = type.BaseType;
			}
			return type;
		}

		protected virtual void Initialize()
		{
			this.handlers = new List<TypeHandler>();
			this.handlerCache = new Dictionary<ITypeId, KeyValuePair<ITypeId, TypeHandler>>();
		}

		protected void InitializeIfNecessary()
		{
			if (!this.IsInitialized)
			{
				this.Initialize();
			}
		}

		protected void RegisterHandler(TypeHandler handler)
		{
			this.InitializeIfNecessary();
			this.handlers.Add(handler);
			this.handlerCache.Clear();
		}

		protected void UnregisterHandler(TypeHandler handler)
		{
			this.InitializeIfNecessary();
			this.handlers.Remove(handler);
			this.handlerCache.Clear();
		}
	}
}