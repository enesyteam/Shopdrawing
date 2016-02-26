using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class ViewNode : IDisposable
	{
		private Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode;

		private IList<ViewNode> children;

		private IDictionary<IProperty, ViewNode> properties;

		private Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState instanceState;

		private Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager;

		private object instance;

		private ViewNode parent;

		private IProperty propertyKey;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IInstanceBuilderContext ChildContext
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<ViewNode> Children
		{
			get
			{
				if (this.children == null)
				{
					this.children = new ViewNode.ViewNodeChildrenCollection(this);
				}
				return this.children;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Microsoft.Expression.DesignModel.DocumentModel.DocumentNode DocumentNode
		{
			get
			{
				return this.documentNode;
			}
			set
			{
				this.documentNode = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				if (this.instance != value)
				{
					object obj = this.instance;
					this.instance = value;
					this.manager.OnInstanceChanged(this, obj, this.instance);
				}
				this.instanceState = Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState.Uninitialized;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState InstanceState
		{
			get
			{
				return this.instanceState;
			}
			set
			{
				this.instanceState = value;
			}
		}

		public System.Type InstanceType
		{
			get
			{
				System.Type type = null;
				if (this.instance != null)
				{
					type = this.instance.GetType();
				}
				return type;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsChild
		{
			get
			{
				return this.propertyKey == null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsProperty
		{
			get
			{
				return this.propertyKey != null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewNode Parent
		{
			get
			{
				return this.parent;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDictionary<IProperty, ViewNode> Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ViewNode.ViewNodePropertyDictionary(this);
				}
				return this.properties;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IProperty SitePropertyKey
		{
			get
			{
				return this.propertyKey;
			}
		}

		public System.Type TargetType
		{
			get
			{
				return this.documentNode.TargetType;
			}
		}

		public IType Type
		{
			get
			{
				return this.documentNode.Type;
			}
		}

		public ITypeResolver TypeResolver
		{
			get
			{
				return this.documentNode.TypeResolver;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager ViewNodeManager
		{
			get
			{
				return this.manager;
			}
		}

		public ViewNode(Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode) : this(manager, documentNode, Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState.Invalid, null)
		{
		}

		public ViewNode(Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode, Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState instanceState, object instance)
		{
			this.documentNode = documentNode;
			this.instanceState = instanceState;
			this.instance = instance;
			this.manager = manager;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.instance = null;
				this.documentNode = null;
				this.manager = null;
				this.parent = null;
				this.propertyKey = null;
				this.ChildContext = null;
			}
		}

		public bool IsAncestorOf(ViewNode descendant)
		{
			while (descendant != null)
			{
				if (this == descendant)
				{
					return true;
				}
				descendant = descendant.Parent;
			}
			return false;
		}

		public void MergeState(Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState newState)
		{
			switch (newState.StateType)
			{
				case InstanceStateType.Invalid:
				{
					this.instanceState = Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState.Invalid;
					return;
				}
				case InstanceStateType.Uninitialized:
				{
					return;
				}
				case InstanceStateType.PropertyOrChildInvalid:
				case InstanceStateType.ChildAndDescendantInvalid:
				{
					switch (this.instanceState.StateType)
					{
						case InstanceStateType.Valid:
						{
							this.instanceState = newState;
							return;
						}
						case InstanceStateType.Invalid:
						case InstanceStateType.Uninitialized:
						{
							return;
						}
						case InstanceStateType.PropertyOrChildInvalid:
						case InstanceStateType.ChildAndDescendantInvalid:
						{
							if (this.instanceState.IsPropertyInvalid != newState.IsPropertyInvalid)
							{
								this.instanceState = Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState.Invalid;
								return;
							}
							if (this.instanceState.IsPropertyInvalid && newState.IsPropertyInvalid)
							{
								if (this.instanceState.InvalidProperties.Contains(newState.InvalidProperties[0]))
								{
									return;
								}
								this.instanceState.InvalidProperties.Add(newState.InvalidProperties[0]);
								return;
							}
							else if (!this.instanceState.IsChildInvalid || !newState.IsChildInvalid)
							{
								if (this.instanceState.IsDescendantInvalid || !newState.IsDescendantInvalid)
								{
									return;
								}
								this.instanceState = new Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState(newState, InstanceStateType.ChildAndDescendantInvalid);
								return;
							}
							else
							{
								DocumentCompositeNode documentNode = (DocumentCompositeNode)this.DocumentNode;
								if (this.instanceState.ChildAction != DocumentNodeChangeAction.Replace || newState.ChildAction != DocumentNodeChangeAction.Replace)
								{
									if (this.instanceState.InvalidChildIndices == null && this.instanceState.ChildIndex == newState.ChildIndex && this.instanceState.ChildAction == newState.ChildAction && (this.instanceState.ChildAction != DocumentNodeChangeAction.Add || this.Children.Count == documentNode.Children.Count - 1) && (this.instanceState.ChildAction != DocumentNodeChangeAction.Remove || this.Children.Count == documentNode.Children.Count + 1))
									{
										return;
									}
									this.instanceState = Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState.Invalid;
									return;
								}
								else
								{
									if (this.instanceState.InvalidChildIndices == null && (newState.InvalidChildIndices != null || this.instanceState.ChildIndex != newState.ChildIndex))
									{
										int childIndex = this.instanceState.ChildIndex;
										this.instanceState = new Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState(new List<int>(), this.instanceState.StateType);
										this.instanceState.InvalidChildIndices.Add(childIndex);
									}
									if (this.instanceState.InvalidChildIndices == null)
									{
										return;
									}
									if (newState.InvalidChildIndices == null)
									{
										this.instanceState.InvalidChildIndices.Add(newState.ChildIndex);
										return;
									}
									using (IEnumerator<int> enumerator = newState.InvalidChildIndices.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											int current = enumerator.Current;
											this.instanceState.InvalidChildIndices.Add(current);
										}
										return;
									}
								}
							}
							break;
						}
						case InstanceStateType.DescendantInvalid:
						{
							this.instanceState = new Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState(newState, InstanceStateType.ChildAndDescendantInvalid);
							return;
						}
						default:
						{
							return;
						}
					}
					break;
				}
				case InstanceStateType.DescendantInvalid:
				{
					switch (this.instanceState.StateType)
					{
						case InstanceStateType.Valid:
						{
							this.instanceState = newState;
							return;
						}
						case InstanceStateType.Invalid:
						case InstanceStateType.Uninitialized:
						case InstanceStateType.DescendantInvalid:
						{
							return;
						}
						case InstanceStateType.PropertyOrChildInvalid:
						case InstanceStateType.ChildAndDescendantInvalid:
						{
							this.instanceState = new Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState(this.instanceState, InstanceStateType.ChildAndDescendantInvalid);
							return;
						}
						default:
						{
							return;
						}
					}
					break;
				}
				default:
				{
					return;
				}
			}
		}

		private void OnViewNodeAdded(ViewNode parent, ViewNode child)
		{
			this.manager.OnViewNodeAdded(parent, child);
		}

		private void OnViewNodeRemoving(ViewNode parent, ViewNode child)
		{
			this.manager.OnViewNodeRemoving(parent, child);
			if (child.children != null)
			{
				child.Children.Clear();
			}
			if (child.properties != null)
			{
				child.Properties.Clear();
			}
		}

		public override string ToString()
		{
			return string.Concat(this.documentNode.ToString(), " ", this.instanceState.ToString());
		}

		private class ViewNodeChildrenCollection : ViewNode.ViewNodeCollectionBase, IList<ViewNode>, ICollection<ViewNode>, IEnumerable<ViewNode>, IEnumerable
		{
			private List<ViewNode> items;

			public int Count
			{
				get
				{
					return this.items.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public ViewNode this[int index]
			{
				get
				{
					return this.items[index];
				}
				set
				{
					if (this[index] != value)
					{
						base.PreRemoveInternal(this[index]);
						this.items[index] = value;
						base.PostAddInternal(value);
					}
				}
			}

			public ViewNodeChildrenCollection(ViewNode parent) : base(parent)
			{
				this.items = new List<ViewNode>();
			}

			public void Add(ViewNode item)
			{
				this.items.Add(item);
				base.PostAddInternal(item);
			}

			public void Clear()
			{
				foreach (ViewNode item in this.items)
				{
					base.PreRemoveInternal(item);
				}
				this.items.Clear();
			}

			public bool Contains(ViewNode item)
			{
				return this.items.Contains(item);
			}

			public void CopyTo(ViewNode[] array, int arrayIndex)
			{
				this.items.CopyTo(array, arrayIndex);
			}

			public IEnumerator<ViewNode> GetEnumerator()
			{
				return this.items.GetEnumerator();
			}

			public int IndexOf(ViewNode item)
			{
				return this.items.IndexOf(item);
			}

			public void Insert(int index, ViewNode item)
			{
				this.items.Insert(index, item);
				base.PostAddInternal(item);
			}

			public bool Remove(ViewNode item)
			{
				base.PreRemoveInternal(item);
				return this.items.Remove(item);
			}

			public void RemoveAt(int index)
			{
				base.PreRemoveInternal(this[index]);
				this.items.RemoveAt(index);
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.items.GetEnumerator();
			}
		}

		private abstract class ViewNodeCollectionBase
		{
			protected ViewNode Parent
			{
				get;
				private set;
			}

			public ViewNodeCollectionBase(ViewNode parent)
			{
				this.Parent = parent;
			}

			protected void PostAddInternal(ViewNode child)
			{
				child.parent = this.Parent;
				child.propertyKey = null;
				this.Parent.OnViewNodeAdded(this.Parent, child);
			}

			protected void PostAddInternal(ViewNode child, IProperty propertyKey)
			{
				child.parent = this.Parent;
				child.propertyKey = propertyKey;
				this.Parent.OnViewNodeAdded(this.Parent, child);
			}

			protected void PreRemoveInternal(ViewNode child)
			{
				this.Parent.OnViewNodeRemoving(this.Parent, child);
				child.Dispose();
			}
		}

		private class ViewNodePropertyDictionary : ViewNode.ViewNodeCollectionBase, IDictionary<IProperty, ViewNode>, ICollection<KeyValuePair<IProperty, ViewNode>>, IEnumerable<KeyValuePair<IProperty, ViewNode>>, IEnumerable
		{
			private Dictionary<IProperty, ViewNode> items;

			public int Count
			{
				get
				{
					return this.items.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public ViewNode this[IProperty key]
			{
				get
				{
					ViewNode viewNode;
					IProperty property = this.ResolvePropertyInternal(key);
					if (this.items.TryGetValue(property, out viewNode))
					{
						return viewNode;
					}
					return null;
				}
				set
				{
					IProperty property = this.ResolvePropertyInternal(key);
					ViewNode item = this[property];
					if (item != null)
					{
						base.PreRemoveInternal(item);
					}
					if (value == null)
					{
						this.items.Remove(property);
						return;
					}
					this.items[property] = value;
					base.PostAddInternal(value, property);
				}
			}

			public ICollection<IProperty> Keys
			{
				get
				{
					return this.items.Keys;
				}
			}

			public ICollection<ViewNode> Values
			{
				get
				{
					return this.items.Values;
				}
			}

			public ViewNodePropertyDictionary(ViewNode parent) : base(parent)
			{
				this.items = new Dictionary<IProperty, ViewNode>();
			}

			public void Add(IProperty key, ViewNode value)
			{
				this[key] = value;
			}

			public void Add(KeyValuePair<IProperty, ViewNode> item)
			{
				this[item.Key] = item.Value;
			}

			public void Clear()
			{
				foreach (KeyValuePair<IProperty, ViewNode> item in this.items)
				{
					base.PreRemoveInternal(item.Value);
				}
				this.items.Clear();
			}

			public bool Contains(KeyValuePair<IProperty, ViewNode> item)
			{
				return ((ICollection<KeyValuePair<IProperty, ViewNode>>)this.items).Contains(item);
			}

			public bool ContainsKey(IProperty key)
			{
				return this[key] != null;
			}

			public void CopyTo(KeyValuePair<IProperty, ViewNode>[] array, int arrayIndex)
			{
				((ICollection<KeyValuePair<IProperty, ViewNode>>)this.items).CopyTo(array, arrayIndex);
			}

			public IEnumerator<KeyValuePair<IProperty, ViewNode>> GetEnumerator()
			{
				return this.items.GetEnumerator();
			}

			public bool Remove(IProperty key)
			{
				ViewNode item = this[key];
				if (item == null)
				{
					return false;
				}
				base.PreRemoveInternal(item);
				return this.items.Remove(key);
			}

			public bool Remove(KeyValuePair<IProperty, ViewNode> item)
			{
				if (!this.Contains(item))
				{
					return false;
				}
				base.PreRemoveInternal(item.Value);
				return this.items.Remove(item.Key);
			}

			private IProperty ResolvePropertyInternal(IProperty property)
			{
				if (property.PropertyType.PlatformMetadata == base.Parent.DocumentNode.PlatformMetadata)
				{
					return property;
				}
				return base.Parent.DocumentNode.TypeResolver.ResolveProperty(property);
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.items.GetEnumerator();
			}

			public bool TryGetValue(IProperty key, out ViewNode value)
			{
				return this.items.TryGetValue(key, out value);
			}
		}
	}
}