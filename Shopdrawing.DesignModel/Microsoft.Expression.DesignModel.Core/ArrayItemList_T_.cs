using System;

namespace Microsoft.Expression.DesignModel.Core
{
	internal sealed class ArrayItemList<T> : FrugalListBase<T>
	{
		private const int MINSIZE = 9;

		private const int GROWTH = 3;

		private const int LARGEGROWTH = 18;

		private T[] _entries;

		public override int Capacity
		{
			get
			{
				if (this._entries == null)
				{
					return 0;
				}
				return (int)this._entries.Length;
			}
		}

		public ArrayItemList()
		{
		}

		public ArrayItemList(int size)
		{
			size = size + 2;
			size = size - size % 3;
			this._entries = new T[size];
		}

		public override FrugalListStoreState Add(T value)
		{
			if (this._entries == null || this._count >= (int)this._entries.Length)
			{
				if (this._entries == null)
				{
					this._entries = new T[9];
				}
				else
				{
					int length = (int)this._entries.Length;
					length = (length >= 18 ? length + (length >> 2) : length + 3);
					T[] tArray = new T[length];
					Array.Copy(this._entries, 0, tArray, 0, (int)this._entries.Length);
					this._entries = tArray;
				}
				this._entries[this._count] = value;
				ArrayItemList<T> arrayItemList = this;
				arrayItemList._count = arrayItemList._count + 1;
			}
			else
			{
				this._entries[this._count] = value;
				ArrayItemList<T> arrayItemList1 = this;
				arrayItemList1._count = arrayItemList1._count + 1;
			}
			return FrugalListStoreState.Success;
		}

		public override void Clear()
		{
			for (int i = 0; i < this._count; i++)
			{
				this._entries[i] = default(T);
			}
			this._count = 0;
		}

		public override object Clone()
		{
			ArrayItemList<T> arrayItemList = new ArrayItemList<T>(this.Capacity);
			arrayItemList.Promote(this);
			return arrayItemList;
		}

		public override bool Contains(T value)
		{
			return -1 != this.IndexOf(value);
		}

		public override void CopyTo(T[] array, int index)
		{
			for (int i = 0; i < this._count; i++)
			{
				array[index + i] = this._entries[i];
			}
		}

		public override T EntryAt(int index)
		{
			return this._entries[index];
		}

		public override int IndexOf(T value)
		{
			for (int i = 0; i < this._count; i++)
			{
				if (this._entries[i].Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		public override void Insert(int index, T value)
		{
			if (this._entries == null || this._count >= (int)this._entries.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			Array.Copy(this._entries, index, this._entries, index + 1, this._count - index);
			this._entries[index] = value;
			ArrayItemList<T> arrayItemList = this;
			arrayItemList._count = arrayItemList._count + 1;
		}

		public override void Promote(FrugalListBase<T> oldList)
		{
			for (int i = 0; i < oldList.Count; i++)
			{
				if (this.Add(oldList.EntryAt(i)) != FrugalListStoreState.Success)
				{
					throw new ArgumentException("list is smaller than oldList", "oldList");
				}
			}
		}

		public void Promote(SixItemList<T> oldList)
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
				case 4:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					this.SetAt(2, oldList.EntryAt(2));
					this.SetAt(3, oldList.EntryAt(3));
					return;
				}
				case 5:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					this.SetAt(2, oldList.EntryAt(2));
					this.SetAt(3, oldList.EntryAt(3));
					this.SetAt(4, oldList.EntryAt(4));
					return;
				}
				case 6:
				{
					this.SetAt(0, oldList.EntryAt(0));
					this.SetAt(1, oldList.EntryAt(1));
					this.SetAt(2, oldList.EntryAt(2));
					this.SetAt(3, oldList.EntryAt(3));
					this.SetAt(4, oldList.EntryAt(4));
					this.SetAt(5, oldList.EntryAt(5));
					return;
				}
			}
			throw new ArgumentOutOfRangeException("oldList");
		}

		public void Promote(ArrayItemList<T> oldList)
		{
			int count = oldList.Count;
			if ((int)this._entries.Length < count)
			{
				throw new ArgumentException("list is smaller than oldList", "oldList");
			}
			this.SetCount(oldList.Count);
			for (int i = 0; i < count; i++)
			{
				this.SetAt(i, oldList.EntryAt(i));
			}
		}

		public override bool Remove(T value)
		{
			for (int i = 0; i < this._count; i++)
			{
				if (this._entries[i].Equals(value))
				{
					this.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public override void RemoveAt(int index)
		{
			int num = this._count - index - 1;
			if (num > 0)
			{
				Array.Copy(this._entries, index + 1, this._entries, index, num);
			}
			this._entries[this._count - 1] = default(T);
			ArrayItemList<T> arrayItemList = this;
			arrayItemList._count = arrayItemList._count - 1;
		}

		public override void SetAt(int index, T value)
		{
			this._entries[index] = value;
		}

		private void SetCount(int value)
		{
			if (value < 0 || value > (int)this._entries.Length)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			this._count = value;
		}

		public override void Sort()
		{
			Array.Sort<T>(this._entries, 0, this._count);
		}

		public override T[] ToArray()
		{
			T[] tArray = new T[this._count];
			for (int i = 0; i < this._count; i++)
			{
				tArray[i] = this._entries[i];
			}
			return tArray;
		}
	}
}