using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class InstanceState
	{
		private readonly static InstanceState valid;

		private readonly static InstanceState invalid;

		private readonly static InstanceState descendantInvalid;

		private readonly static InstanceState uninitialized;

		private InstanceStateType stateType;

		private List<IProperty> invalidProperties;

		private SortedSet<int> invalidChildIndices;

		private InstanceState.ChildOperation childOperation;

		public DocumentNodeChangeAction ChildAction
		{
			get
			{
				if (this.invalidChildIndices != null)
				{
					return DocumentNodeChangeAction.Replace;
				}
				return this.childOperation.Action;
			}
		}

		public int ChildIndex
		{
			get
			{
				return this.childOperation.Index;
			}
		}

		public static InstanceState DescendantInvalid
		{
			get
			{
				return InstanceState.descendantInvalid;
			}
		}

		public static InstanceState Invalid
		{
			get
			{
				return InstanceState.invalid;
			}
		}

		public ICollection<int> InvalidChildIndices
		{
			get
			{
				return this.invalidChildIndices;
			}
		}

		public IList<IProperty> InvalidProperties
		{
			get
			{
				return this.invalidProperties;
			}
		}

		public bool IsChildInvalid
		{
			get
			{
				if (this.childOperation != null)
				{
					return true;
				}
				return this.invalidChildIndices != null;
			}
		}

		public bool IsDescendantInvalid
		{
			get
			{
				if (this.stateType == InstanceStateType.DescendantInvalid)
				{
					return true;
				}
				return this.stateType == InstanceStateType.ChildAndDescendantInvalid;
			}
		}

		public bool IsPropertyInvalid
		{
			get
			{
				return this.invalidProperties != null;
			}
		}

		public bool IsPropertyOrChildInvalid
		{
			get
			{
				if (this.stateType == InstanceStateType.ChildAndDescendantInvalid)
				{
					return true;
				}
				return this.stateType == InstanceStateType.PropertyOrChildInvalid;
			}
		}

		public InstanceStateType StateType
		{
			get
			{
				return this.stateType;
			}
		}

		public static InstanceState Uninitialized
		{
			get
			{
				return InstanceState.uninitialized;
			}
		}

		public static InstanceState Valid
		{
			get
			{
				return InstanceState.valid;
			}
		}

		static InstanceState()
		{
			InstanceState.valid = new InstanceState(InstanceStateType.Valid);
			InstanceState.invalid = new InstanceState(InstanceStateType.Invalid);
			InstanceState.descendantInvalid = new InstanceState(InstanceStateType.DescendantInvalid);
			InstanceState.uninitialized = new InstanceState(InstanceStateType.Uninitialized);
		}

		private InstanceState(InstanceStateType stateType)
		{
			this.stateType = stateType;
		}

		public InstanceState(DocumentNodeChange args)
		{
			this.stateType = InstanceStateType.PropertyOrChildInvalid;
			if (!args.IsPropertyChange)
			{
				this.childOperation = new InstanceState.ChildOperation(args.ChildIndex, args.Action);
				return;
			}
			this.invalidProperties = new List<IProperty>(1)
			{
				args.PropertyKey
			};
		}

		public InstanceState(InstanceState instanceState, InstanceStateType instanceStateType)
		{
			this.stateType = instanceStateType;
			this.invalidProperties = instanceState.invalidProperties;
			this.invalidChildIndices = instanceState.invalidChildIndices;
			this.childOperation = instanceState.childOperation;
		}

		public InstanceState(IEnumerable<int> invalidChildIndices, InstanceStateType instanceStateType)
		{
			this.stateType = instanceStateType;
			this.invalidChildIndices = new SortedSet<int>(invalidChildIndices);
		}

		public InstanceState(IProperty propertyKey)
		{
			this.stateType = InstanceStateType.PropertyOrChildInvalid;
			this.invalidProperties = new List<IProperty>(1)
			{
				propertyKey
			};
		}

		public InstanceState(int childIndex, DocumentNodeChangeAction action)
		{
			this.stateType = InstanceStateType.PropertyOrChildInvalid;
			this.childOperation = new InstanceState.ChildOperation(childIndex, action);
		}

		public bool ContainsInvalidProperty(IPropertyId propertyKey)
		{
			bool flag;
			if (this.invalidProperties != null)
			{
				List<IProperty>.Enumerator enumerator = this.invalidProperties.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Equals(propertyKey))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.stateType.ToString());
			if (this.invalidProperties != null)
			{
				foreach (IPropertyId invalidProperty in this.invalidProperties)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(invalidProperty.Name);
				}
			}
			else if (this.childOperation != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(this.childOperation.ToString());
			}
			return stringBuilder.ToString();
		}

		private class ChildOperation
		{
			private int index;

			private DocumentNodeChangeAction action;

			public DocumentNodeChangeAction Action
			{
				get
				{
					return this.action;
				}
			}

			public int Index
			{
				get
				{
					return this.index;
				}
			}

			public ChildOperation(int index, DocumentNodeChangeAction action)
			{
				this.index = index;
				this.action = action;
			}

			public override string ToString()
			{
				return string.Concat(this.action.ToString(), ",", this.index.ToString());
			}
		}
	}
}