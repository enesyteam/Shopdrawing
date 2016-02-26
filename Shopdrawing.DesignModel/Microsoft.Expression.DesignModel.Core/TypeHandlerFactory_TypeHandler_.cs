using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Core
{
	internal abstract class TypeHandlerFactory<TypeHandler>
	{
		private List<TypeHandler> handlers;

		private Dictionary<Type, TypeHandler> handlerCache;

		private Action initializer;

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

		protected TypeHandlerFactory(Action initializer)
		{
			this.initializer = initializer;
		}

		protected void CacheHandler(Type type, TypeHandler handler)
		{
			this.InitializeIfNecessary();
			this.handlerCache[type] = handler;
		}

		[Conditional("DEBUG")]
		private void CheckNoRegisteredHandler(Type type)
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

		protected TypeHandler DetermineBestHandler(TypeHandler handler, Type type)
		{
			this.InitializeIfNecessary();
			Type type1 = typeof(object);
			Type implementingType = typeof(object);
			foreach (TypeHandler typeHandler in this.handlers)
			{
				Type baseType = this.GetBaseType(typeHandler);
				if (!(baseType != null) || !baseType.IsAssignableFrom(type) && (!baseType.IsGenericTypeDefinition || !PlatformTypes.IsGenericTypeDefinitionOf(baseType, type)) || !type1.IsAssignableFrom(baseType) && (!baseType.IsInterface || baseType.IsAssignableFrom(implementingType)))
				{
					continue;
				}
				handler = typeHandler;
				type1 = baseType;
				implementingType = this.GetImplementingType(baseType, type);
			}
			return handler;
		}

		private static bool DoesTypeImplement(Type baseType, Type targetType)
		{
			if (baseType.IsAssignableFrom(targetType))
			{
				return true;
			}
			if (!baseType.IsGenericTypeDefinition)
			{
				return false;
			}
			return PlatformTypes.IsGenericTypeDefinitionOf(baseType, targetType);
		}

		protected abstract Type GetBaseType(TypeHandler handler);

		protected bool GetCachedHandler(Type type, out TypeHandler handler)
		{
			this.InitializeIfNecessary();
			return this.handlerCache.TryGetValue(type, out handler);
		}

		protected virtual TypeHandler GetDefaultHandler(Type type)
		{
			return default(TypeHandler);
		}

		protected TypeHandler GetHandler(Type type)
		{
			TypeHandler typeHandler;
			if (!this.GetCachedHandler(type, out typeHandler))
			{
				typeHandler = this.DetermineBestHandler(this.GetDefaultHandler(type), type);
				this.CacheHandler(type, typeHandler);
			}
			return typeHandler;
		}

		protected Type GetImplementingType(Type baseType, Type targetType)
		{
			if (!baseType.IsInterface && baseType.IsAssignableFrom(targetType))
			{
				return baseType;
			}
			Type type = targetType;
			while (type.BaseType != null && TypeHandlerFactory<TypeHandler>.DoesTypeImplement(baseType, type.BaseType))
			{
				type = type.BaseType;
			}
			return type;
		}

		protected void InitializeIfNecessary()
		{
			if (!this.IsInitialized)
			{
				this.handlers = new List<TypeHandler>();
				this.handlerCache = new Dictionary<Type, TypeHandler>();
				if (this.initializer != null)
				{
					this.initializer();
				}
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