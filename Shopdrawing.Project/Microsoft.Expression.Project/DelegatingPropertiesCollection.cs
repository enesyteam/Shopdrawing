using System;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class DelegatingPropertiesCollection : IPropertiesCollection, IDisposable
	{
		private bool isDisposed;

		private DelegatingPropertiesCollection.GetProperty getPropertyDelegate;

		private DelegatingPropertiesCollection.SetProperty setPropertyDelegate;

		public string this[string propertyKey]
		{
			get
			{
				if (this.isDisposed)
				{
					throw new ObjectDisposedException("CallbackPropertiesCollection");
				}
				return this.getPropertyDelegate(propertyKey);
			}
			set
			{
				if (this.isDisposed)
				{
					throw new ObjectDisposedException("CallbackPropertiesCollection");
				}
				this.setPropertyDelegate(propertyKey, value);
			}
		}

		public DelegatingPropertiesCollection(DelegatingPropertiesCollection.GetProperty getPropertyDelegate, DelegatingPropertiesCollection.SetProperty setPropertyDelegate)
		{
			if (getPropertyDelegate == null)
			{
				throw new ArgumentNullException("getPropertyDelegate");
			}
			if (setPropertyDelegate == null)
			{
				throw new ArgumentNullException("setPropertyDelegate");
			}
			this.getPropertyDelegate = getPropertyDelegate;
			this.setPropertyDelegate = setPropertyDelegate;
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool isDisposing)
		{
			this.getPropertyDelegate = null;
			this.setPropertyDelegate = null;
		}

		~DelegatingPropertiesCollection()
		{
			this.Dispose(false);
		}

		public delegate string GetProperty(string propertyName);

		public delegate void SetProperty(string propertyName, string value);
	}
}