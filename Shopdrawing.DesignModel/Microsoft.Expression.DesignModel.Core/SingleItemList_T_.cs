using System;

namespace Microsoft.Expression.DesignModel.Core
{
	internal sealed class SingleItemList<T> : FrugalListBase<T>
	{
		private const int SIZE = 1;

		private T _loneEntry;

		public override int Capacity
		{
			get
			{
				return 1;
			}
		}

		public SingleItemList()
		{
		}

		public override FrugalListStoreState Add(T value)
		{
			if (this._count != 0)
			{
				return FrugalListStoreState.ThreeItemList;
			}
			this._loneEntry = value;
			SingleItemList<T> singleItemList = this;
			singleItemList._count = singleItemList._count + 1;
			return FrugalListStoreState.Success;
		}

		public override void Clear()
		{
			this._loneEntry = default(T);
			this._count = 0;
		}

		public override object Clone()
		{
			SingleItemList<T> singleItemList = new SingleItemList<T>();
			singleItemList.Promote(this);
			return singleItemList;
		}

		public override bool Contains(T value)
		{
			return this._loneEntry.Equals(value);
		}

		public override void CopyTo(T[] array, int index)
		{
			array[index] = this._loneEntry;
		}

		public override T EntryAt(int index)
		{
			return this._loneEntry;
		}

		public override int IndexOf(T value)
		{
			if (this._loneEntry.Equals(value))
			{
				return 0;
			}
			return -1;
		}

		public override void Insert(int index, T value)
		{
			if (this._count >= 1 || index >= 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._loneEntry = value;
			SingleItemList<T> singleItemList = this;
			singleItemList._count = singleItemList._count + 1;
		}

		public override void Promote(FrugalListBase<T> oldList)
		{
			if (1 != oldList.Count)
			{
				throw new ArgumentException("oldList");
			}
			this.SetCount(1);
			this.SetAt(0, oldList.EntryAt(0));
		}

		public void Promote(SingleItemList<T> oldList)
		{
			this.SetCount(oldList.Count);
			this.SetAt(0, oldList.EntryAt(0));
		}

		public override bool Remove(T value)
		{
			if (!this._loneEntry.Equals(value))
			{
				return false;
			}
			this._loneEntry = default(T);
			SingleItemList<T> singleItemList = this;
			singleItemList._count = singleItemList._count - 1;
			return true;
		}

		public override void RemoveAt(int index)
		{
			if (index != 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._loneEntry = default(T);
			SingleItemList<T> singleItemList = this;
			singleItemList._count = singleItemList._count - 1;
		}

		public override void SetAt(int index, T value)
		{
			this._loneEntry = value;
		}

		private void SetCount(int value)
		{
			if (value < 0 || value > 1)
			{
				throw new ArgumentOutOfRangeException("Count");
			}
			this._count = value;
		}

		public override void Sort()
		{
		}

		public override T[] ToArray()
		{
			return new T[] { this._loneEntry };
		}
	}
}