using System;

namespace Microsoft.Expression.DesignModel.Core
{
	internal sealed class ThreeItemList<T> : FrugalListBase<T>
	{
		private const int SIZE = 3;

		private T _entry0;

		private T _entry1;

		private T _entry2;

		public override int Capacity
		{
			get
			{
				return 3;
			}
		}

		public ThreeItemList()
		{
		}

		public override FrugalListStoreState Add(T value)
		{
			switch (this._count)
			{
				case 0:
				{
					this._entry0 = value;
					break;
				}
				case 1:
				{
					this._entry1 = value;
					break;
				}
				case 2:
				{
					this._entry2 = value;
					break;
				}
				default:
				{
					return FrugalListStoreState.SixItemList;
				}
			}
			ThreeItemList<T> threeItemList = this;
			threeItemList._count = threeItemList._count + 1;
			return FrugalListStoreState.Success;
		}

		public override void Clear()
		{
			this._entry0 = default(T);
			this._entry1 = default(T);
			this._entry2 = default(T);
			this._count = 0;
		}

		public override object Clone()
		{
			ThreeItemList<T> threeItemList = new ThreeItemList<T>();
			threeItemList.Promote(this);
			return threeItemList;
		}

		public override bool Contains(T value)
		{
			return -1 != this.IndexOf(value);
		}

		public override void CopyTo(T[] array, int index)
		{
			array[index] = this._entry0;
			if (this._count >= 2)
			{
				array[index + 1] = this._entry1;
				if (this._count == 3)
				{
					array[index + 2] = this._entry2;
				}
			}
		}

		public override T EntryAt(int index)
		{
			switch (index)
			{
				case 0:
				{
					return this._entry0;
				}
				case 1:
				{
					return this._entry1;
				}
				case 2:
				{
					return this._entry2;
				}
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public override int IndexOf(T value)
		{
			if (this._entry0.Equals(value))
			{
				return 0;
			}
			if (this._count > 1)
			{
				if (this._entry1.Equals(value))
				{
					return 1;
				}
				if (3 == this._count && this._entry2.Equals(value))
				{
					return 2;
				}
			}
			return -1;
		}

		public override void Insert(int index, T value)
		{
			if (this._count >= 3)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			switch (index)
			{
				case 0:
				{
					this._entry2 = this._entry1;
					this._entry1 = this._entry0;
					this._entry0 = value;
					break;
				}
				case 1:
				{
					this._entry2 = this._entry1;
					this._entry1 = value;
					break;
				}
				case 2:
				{
					this._entry2 = value;
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException("index");
				}
			}
			ThreeItemList<T> threeItemList = this;
			threeItemList._count = threeItemList._count + 1;
		}

		public override void Promote(FrugalListBase<T> oldList)
		{
			int count = oldList.Count;
			if (3 < count)
			{
				throw new ArgumentException("oldList");
			}
			this.SetCount(oldList.Count);
			switch (count)
			{
				case 0:
				{
					return;
				}
				case 1:
				{
					this.SetAt(0, oldList.EntryAt(0));
					return;
				}
				case 2:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					return;
				}
				case 3:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					this.SetAt(2, oldList.EntryAt(2));
					return;
				}
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public void Promote(SingleItemList<T> oldList)
		{
			this.SetCount(oldList.Count);
			this.SetAt(0, oldList.EntryAt(0));
		}

		public void Promote(ThreeItemList<T> oldList)
		{
			int count = oldList.Count;
			this.SetCount(oldList.Count);
			switch (count)
			{
				case 0:
				{
					return;
				}
				case 1:
				{
					this.SetAt(0, oldList.EntryAt(0));
					return;
				}
				case 2:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					return;
				}
				case 3:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					this.SetAt(2, oldList.EntryAt(2));
					return;
				}
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public override bool Remove(T value)
		{
			if (this._entry0.Equals(value))
			{
				this.RemoveAt(0);
				return true;
			}
			if (this._count > 1)
			{
				if (this._entry1.Equals(value))
				{
					this.RemoveAt(1);
					return true;
				}
				if (3 == this._count && this._entry2.Equals(value))
				{
					this.RemoveAt(2);
					return true;
				}
			}
			return false;
		}

		public override void RemoveAt(int index)
		{
			ThreeItemList<T> threeItemList;
			switch (index)
			{
				case 0:
				{
					this._entry0 = this._entry1;
					this._entry1 = this._entry2;
					this._entry2 = default(T);
					threeItemList = this;
					threeItemList._count = threeItemList._count - 1;
					return;
				}
				case 1:
				{
					this._entry1 = this._entry2;
					this._entry2 = default(T);
					threeItemList = this;
					threeItemList._count = threeItemList._count - 1;
					return;
				}
				case 2:
				{
					this._entry2 = default(T);
					threeItemList = this;
					threeItemList._count = threeItemList._count - 1;
					return;
				}
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public override void SetAt(int index, T value)
		{
			switch (index)
			{
				case 0:
				{
					this._entry0 = value;
					return;
				}
				case 1:
				{
					this._entry1 = value;
					return;
				}
				case 2:
				{
					this._entry2 = value;
					return;
				}
			}
			throw new ArgumentOutOfRangeException("index");
		}

		private void SetCount(int value)
		{
			if (value < 0 || value > 3)
			{
				throw new ArgumentOutOfRangeException("Count");
			}
			this._count = value;
		}

		public override void Sort()
		{
			T t;
			T t1;
			T t2;
			if (this._count == 1)
			{
				return;
			}
			if (this._count != 2)
			{
				if (((IComparable<T>)(object)this._entry0).CompareTo(this._entry1) >= 0)
				{
					t = this._entry1;
					t2 = this._entry0;
				}
				else
				{
					t = this._entry0;
					t2 = this._entry1;
				}
				if (((IComparable<T>)(object)t).CompareTo(this._entry2) >= 0)
				{
					t1 = t;
					t = this._entry2;
				}
				else if (((IComparable<T>)(object)t2).CompareTo(this._entry2) >= 0)
				{
					t1 = this._entry2;
				}
				else
				{
					t1 = t2;
					t2 = this._entry2;
				}
				this._entry0 = t;
				this._entry1 = t1;
				this._entry2 = t2;
			}
			else if (((IComparable<T>)(object)this._entry0).CompareTo(this._entry1) > 0)
			{
				T t3 = this._entry0;
				this._entry0 = this._entry1;
				this._entry1 = t3;
				return;
			}
		}

		public override T[] ToArray()
		{
			T[] tArray = new T[this._count];
			tArray[0] = this._entry0;
			if (this._count >= 2)
			{
				tArray[1] = this._entry1;
				if (this._count == 3)
				{
					tArray[2] = this._entry2;
				}
			}
			return tArray;
		}
	}
}