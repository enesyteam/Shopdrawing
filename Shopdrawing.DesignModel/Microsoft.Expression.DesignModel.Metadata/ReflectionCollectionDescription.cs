using System;
using System.Collections;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class ReflectionCollectionDescription : CollectionAdapterDescription
	{
		private MethodInfo addMethod;

		private MethodInfo clearMethod;

		private MethodInfo insertMethod;

		private MethodInfo removeAtMethod;

		private PropertyInfo itemProperty;

		public MethodInfo AddMethod
		{
			get
			{
				return this.addMethod;
			}
		}

		public MethodInfo ClearMethod
		{
			get
			{
				return this.clearMethod;
			}
		}

		public MethodInfo InsertMethod
		{
			get
			{
				return this.insertMethod;
			}
		}

		public PropertyInfo ItemProperty
		{
			get
			{
				return this.itemProperty;
			}
		}

		public MethodInfo RemoveAtMethod
		{
			get
			{
				return this.removeAtMethod;
			}
		}

		public ReflectionCollectionDescription(System.Type type, System.Type itemType, MethodInfo addMethod, MethodInfo clearMethod, MethodInfo insertMethod, MethodInfo removeAtMethod, PropertyInfo itemProperty) : base(type, itemType)
		{
			this.addMethod = addMethod;
			this.clearMethod = clearMethod;
			this.insertMethod = insertMethod;
			this.removeAtMethod = removeAtMethod;
			this.itemProperty = itemProperty;
		}

		public override ICollection GetCollectionAdapter(object instance)
		{
			return new ReflectionCollectionDescription.ReflectionCollectionAdapter((ICollection)instance, this);
		}

		private sealed class ReflectionCollectionAdapter : IList, ICollection, IEnumerable
		{
			private ICollection collection;

			private ReflectionCollectionDescription description;

			public int Count
			{
				get
				{
					return this.collection.Count;
				}
			}

			public bool IsFixedSize
			{
				get
				{
					return false;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					return this.collection.IsSynchronized;
				}
			}

			public object this[int index]
			{
				get
				{
					PropertyInfo itemProperty = this.description.ItemProperty;
					ICollection collections = this.collection;
					object[] objArray = new object[] { index };
					return itemProperty.GetValue(collections, objArray);
				}
				set
				{
					PropertyInfo itemProperty = this.description.ItemProperty;
					ICollection collections = this.collection;
					object[] objArray = new object[] { index };
					itemProperty.SetValue(collections, value, objArray);
				}
			}

			public object SyncRoot
			{
				get
				{
					return this.collection.SyncRoot;
				}
			}

			public ReflectionCollectionAdapter(ICollection collection, ReflectionCollectionDescription description)
			{
				this.collection = collection;
				this.description = description;
			}

			public int Add(object item)
			{
				MethodInfo addMethod = this.description.AddMethod;
				ICollection collections = this.collection;
				object[] objArray = new object[] { item };
				addMethod.Invoke(collections, objArray);
				return this.collection.Count - 1;
			}

			public void Clear()
			{
				this.description.ClearMethod.Invoke(this.collection, null);
			}

			public bool Contains(object item)
			{
				return this.IndexOf(item) >= 0;
			}

			public void CopyTo(Array array, int index)
			{
				this.collection.CopyTo(array, index);
			}

			public IEnumerator GetEnumerator()
			{
				return this.collection.GetEnumerator();
			}

			public int IndexOf(object item)
			{
				throw new NotImplementedException();
			}

			public void Insert(int index, object value)
			{
				MethodInfo insertMethod = this.description.InsertMethod;
				ICollection collections = this.collection;
				object[] objArray = new object[] { index, value };
				insertMethod.Invoke(collections, objArray);
			}

			public void Remove(object item)
			{
				this.RemoveAt(this.IndexOf(item));
			}

			public void RemoveAt(int index)
			{
				MethodInfo removeAtMethod = this.description.RemoveAtMethod;
				ICollection collections = this.collection;
				object[] objArray = new object[] { index };
				removeAtMethod.Invoke(collections, objArray);
			}
		}
	}
}