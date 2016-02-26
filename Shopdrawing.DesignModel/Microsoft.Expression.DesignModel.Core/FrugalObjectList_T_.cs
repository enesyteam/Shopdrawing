using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Core
{
	internal class FrugalObjectList<T>
	{
		internal FrugalListBase<T> _listStore;

		public int Capacity
		{
			get
			{
				if (this._listStore == null)
				{
					return 0;
				}
				return this._listStore.Capacity;
			}
			set
			{
				FrugalListBase<T> singleItemList;
				int capacity = 0;
				if (this._listStore != null)
				{
					capacity = this._listStore.Capacity;
				}
				if (capacity < value)
				{
					if (value == 1)
					{
						singleItemList = new SingleItemList<T>();
					}
					else if (value <= 3)
					{
						singleItemList = new ThreeItemList<T>();
					}
					else if (value > 6)
					{
						singleItemList = new ArrayItemList<T>(value);
					}
					else
					{
						singleItemList = new SixItemList<T>();
					}
					if (this._listStore != null)
					{
						singleItemList.Promote(this._listStore);
					}
					this._listStore = singleItemList;
				}
			}
		}

		public int Count
		{
			get
			{
				if (this._listStore == null)
				{
					return 0;
				}
				return this._listStore.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				if (this._listStore == null || index >= this._listStore.Count || index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._listStore.EntryAt(index);
			}
			set
			{
				if (this._listStore == null || index >= this._listStore.Count || index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this._listStore.SetAt(index, value);
			}
		}

		public FrugalObjectList()
		{
		}

		public FrugalObjectList(int size)
		{
			this.Capacity = size;
		}

		public int Add(T value)
		{
			if (this._listStore == null)
			{
				this._listStore = new SingleItemList<T>();
			}
			FrugalListStoreState frugalListStoreState = this._listStore.Add(value);
			if (frugalListStoreState != FrugalListStoreState.Success)
			{
				if (FrugalListStoreState.ThreeItemList == frugalListStoreState)
				{
					ThreeItemList<T> threeItemList = new ThreeItemList<T>();
					threeItemList.Promote(this._listStore);
					threeItemList.Add(value);
					this._listStore = threeItemList;
				}
				else if (FrugalListStoreState.SixItemList != frugalListStoreState)
				{
					if (FrugalListStoreState.Array != frugalListStoreState)
					{
						throw new InvalidOperationException();
					}
					ArrayItemList<T> arrayItemList = new ArrayItemList<T>(this._listStore.Count + 1);
					arrayItemList.Promote(this._listStore);
					this._listStore = arrayItemList;
					arrayItemList.Add(value);
					this._listStore = arrayItemList;
				}
				else
				{
					SixItemList<T> sixItemList = new SixItemList<T>();
					sixItemList.Promote(this._listStore);
					this._listStore = sixItemList;
					sixItemList.Add(value);
					this._listStore = sixItemList;
				}
			}
			return this._listStore.Count - 1;
		}

		public void Clear()
		{
			if (this._listStore != null)
			{
				this._listStore.Clear();
			}
		}

		public FrugalObjectList<T> Clone()
		{
			FrugalObjectList<T> frugalObjectList = new FrugalObjectList<T>();
			if (this._listStore != null)
			{
				frugalObjectList._listStore = (FrugalListBase<T>)this._listStore.Clone();
			}
			return frugalObjectList;
		}

		public bool Contains(T value)
		{
			if (this._listStore == null || this._listStore.Count <= 0)
			{
				return false;
			}
			return this._listStore.Contains(value);
		}

		public void CopyTo(T[] array, int index)
		{
			if (this._listStore != null && this._listStore.Count > 0)
			{
				this._listStore.CopyTo(array, index);
			}
		}

		public void EnsureIndex(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = index + 1 - this.Count;
			if (num > 0)
			{
				this.Capacity = index + 1;
				T t = default(T);
				for (int i = 0; i < num; i++)
				{
					this._listStore.Add(t);
				}
			}
		}

		public int IndexOf(T value)
		{
			if (this._listStore == null || this._listStore.Count <= 0)
			{
				return -1;
			}
			return this._listStore.IndexOf(value);
		}

		public void Insert(int index, T value)
		{
			if (index != 0 && (this._listStore == null || index > this._listStore.Count || index < 0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int capacity = 1;
			if (this._listStore != null && this._listStore.Count == this._listStore.Capacity)
			{
				capacity = this.Capacity + 1;
			}
			this.Capacity = capacity;
			this._listStore.Insert(index, value);
		}

		public bool Remove(T value)
		{
			if (this._listStore == null || this._listStore.Count <= 0)
			{
				return false;
			}
			return this._listStore.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (this._listStore == null || index >= this._listStore.Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._listStore.RemoveAt(index);
		}

		public void Sort()
		{
			if (this._listStore != null && this._listStore.Count > 0)
			{
				this._listStore.Sort();
			}
		}

		public T[] ToArray()
		{
			if (this._listStore == null || this._listStore.Count <= 0)
			{
				return null;
			}
			return this._listStore.ToArray();
		}
	}
}