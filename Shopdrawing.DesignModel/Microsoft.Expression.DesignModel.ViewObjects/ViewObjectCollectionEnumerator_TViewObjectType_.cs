using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	internal class ViewObjectCollectionEnumerator<TViewObjectType> : IEnumerator<TViewObjectType>, IDisposable, IEnumerator
	where TViewObjectType : IViewObject
	{
		private IEnumerator innerEnumerator;

		private IViewObjectFactory factory;

		private bool isDisposed;

		public TViewObjectType Current
		{
			get
			{
				return this.WrapCurrent();
			}
		}

		object System.Collections.IEnumerator.Current
		{
			get
			{
				return this.WrapCurrent();
			}
		}

		internal ViewObjectCollectionEnumerator(IEnumerator innerEnumerator, IViewObjectFactory factory)
		{
			this.innerEnumerator = innerEnumerator;
			this.factory = factory;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				IDisposable disposable = this.innerEnumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.innerEnumerator = null;
				this.isDisposed = true;
			}
		}

		~ViewObjectCollectionEnumerator()
		{
			this.Dispose(false);
		}

		public bool MoveNext()
		{
			return this.innerEnumerator.MoveNext();
		}

		public void Reset()
		{
			this.innerEnumerator.Reset();
		}

		private TViewObjectType WrapCurrent()
		{
			IViewObject viewObject = this.factory.Instantiate(this.innerEnumerator.Current);
			if (viewObject is TViewObjectType)
			{
				return (TViewObjectType)viewObject;
			}
			return default(TViewObjectType);
		}
	}
}