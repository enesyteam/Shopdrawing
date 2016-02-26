using Microsoft.Expression.DesignModel.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public class ViewObjectCollection<TPlatformObjectType, TViewObjectType> : ICollection<TViewObjectType>, IEnumerable<TViewObjectType>, IEnumerable
	where TViewObjectType : IViewObject
	{
		private IPlatform platform;

		private ICollection<TPlatformObjectType> platformCollection;

		public int Count
		{
			get
			{
				return this.platformCollection.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.platformCollection.IsReadOnly;
			}
		}

		public ViewObjectCollection(IPlatform platform, ICollection<TPlatformObjectType> platformCollection)
		{
			this.platform = platform;
			this.platformCollection = platformCollection;
		}

		public void Add(TViewObjectType item)
		{
			if (!this.VerifyPlatformType(item))
			{
				return;
			}
			this.platformCollection.Add((TPlatformObjectType)item.PlatformSpecificObject);
		}

		public void Clear()
		{
			this.platformCollection.Clear();
		}

		public bool Contains(TViewObjectType item)
		{
			if (!this.VerifyPlatformType(item))
			{
				return false;
			}
			return this.platformCollection.Contains((TPlatformObjectType)item.PlatformSpecificObject);
		}

		public void CopyTo(TViewObjectType[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", (object)arrayIndex, "Array index out of bounds.");
			}
			if (arrayIndex + this.platformCollection.Count > (int)array.Length)
			{
				throw new ArgumentException("Insufficient space in destination array.");
			}
			int num = 0;
			foreach (TPlatformObjectType tPlatformObjectType in this.platformCollection)
			{
				IViewObject viewObject = this.platform.ViewObjectFactory.Instantiate(tPlatformObjectType);
				if (viewObject is TViewObjectType)
				{
					int num1 = num;
					num = num1 + 1;
					array[arrayIndex + num1] = (TViewObjectType)viewObject;
				}
				else
				{
					int num2 = num;
					num = num2 + 1;
					array[arrayIndex + num2] = default(TViewObjectType);
				}
			}
		}

		public IEnumerator<TViewObjectType> GetEnumerator()
		{
			return this.WrapEnumerator();
		}

		public bool Remove(TViewObjectType item)
		{
			if (!this.VerifyPlatformType(item))
			{
				return false;
			}
			return this.platformCollection.Remove((TPlatformObjectType)item.PlatformSpecificObject);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.WrapEnumerator();
		}

		private bool VerifyPlatformType(TViewObjectType viewObject)
		{
			if (viewObject.PlatformSpecificObject is TPlatformObjectType)
			{
				return true;
			}
			return false;
		}

		private ViewObjectCollectionEnumerator<TViewObjectType> WrapEnumerator()
		{
			return new ViewObjectCollectionEnumerator<TViewObjectType>(this.platformCollection.GetEnumerator(), this.platform.ViewObjectFactory);
		}
	}
}