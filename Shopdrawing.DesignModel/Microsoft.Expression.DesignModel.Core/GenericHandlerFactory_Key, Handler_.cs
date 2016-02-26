using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class GenericHandlerFactory<Key, Handler>
	{
		private List<Handler> handlers;

		private Dictionary<Key, Handler> handlerCache;

		public ICollection<Handler> Handlers
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

		protected GenericHandlerFactory()
		{
		}

		protected void CacheHandler(Key key, Handler handler)
		{
			this.InitializeIfNecessary();
			this.handlerCache[key] = handler;
		}

		[Conditional("DEBUG")]
		private void CheckNoRegisteredHandler(Key key)
		{
			foreach (Handler handler in this.handlers)
			{
				if (!key.Equals(this.GetKey(handler)))
				{
					continue;
				}
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string typeHandlerFactoryHandlerForTypeExists = ExceptionStringTable.TypeHandlerFactoryHandlerForTypeExists;
				object[] str = new object[] { key.ToString() };
				throw new InvalidOperationException(string.Format(currentCulture, typeHandlerFactoryHandlerForTypeExists, str));
			}
		}

		protected virtual Handler DetermineBestHandler(Handler handler, Key key)
		{
			return this.GetDefaultHandler(key);
		}

		protected bool GetCachedHandler(Key key, out Handler handler)
		{
			this.InitializeIfNecessary();
			return this.handlerCache.TryGetValue(key, out handler);
		}

		protected virtual Handler GetDefaultHandler(Key key)
		{
			return default(Handler);
		}

		protected Handler GetHandler(Key key)
		{
			Handler handler;
			if (!this.GetCachedHandler(key, out handler))
			{
				handler = this.DetermineBestHandler(this.GetDefaultHandler(key), key);
				if (handler != null)
				{
					this.CacheHandler(key, handler);
				}
			}
			return handler;
		}

		protected abstract Key GetKey(Handler handler);

		protected virtual void Initialize()
		{
			this.handlers = new List<Handler>();
			this.handlerCache = new Dictionary<Key, Handler>();
		}

		protected void InitializeIfNecessary()
		{
			if (!this.IsInitialized)
			{
				this.Initialize();
			}
		}

		protected void RegisterHandler(Handler handler)
		{
			this.InitializeIfNecessary();
			this.handlers.Add(handler);
			this.handlerCache.Clear();
		}

		protected void UnregisterHandler(Handler handler)
		{
			this.InitializeIfNecessary();
			this.handlers.Remove(handler);
			this.handlerCache.Clear();
		}
	}
}