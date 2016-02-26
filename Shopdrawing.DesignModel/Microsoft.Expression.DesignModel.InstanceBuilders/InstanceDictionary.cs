using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	internal class InstanceDictionary : IInstanceDictionary, IEnumerable<KeyValuePair<object, ViewNode>>, IEnumerable
	{
		protected ViewNodeManager viewNodeManager;

		private Dictionary<DocumentNode, object> dataSourceCache = new Dictionary<DocumentNode, object>();

		private Dictionary<object, ViewNode> reverseDictionary = new Dictionary<object, ViewNode>();

		private IList<ViewNode> instantiatedElementRoots = new List<ViewNode>();

		public IDictionary<DocumentNode, object> DataSourceCache
		{
			get
			{
				return this.dataSourceCache;
			}
		}

		public IList<ViewNode> InstantiatedElementRoots
		{
			get
			{
				return this.instantiatedElementRoots;
			}
		}

		public InstanceDictionary(ViewNodeManager viewNodeManager)
		{
			this.viewNodeManager = viewNodeManager;
		}

		public void Clear()
		{
			this.dataSourceCache.Clear();
			this.reverseDictionary.Clear();
			this.instantiatedElementRoots.Clear();
		}

		public IEnumerator<KeyValuePair<object, ViewNode>> GetEnumerator()
		{
			return this.reverseDictionary.GetEnumerator();
		}

		public virtual ViewNode GetViewNode(object viewObject, bool allowCrossView)
		{
			ViewNode viewNode = null;
			this.reverseDictionary.TryGetValue(viewObject, out viewNode);
			return viewNode;
		}

		public void OnInstanceChanged(ViewNode viewNode, object oldInstance, object newInstance)
		{
			if (this.ShouldStoreInReverseDictionary(oldInstance))
			{
				this.reverseDictionary.Remove(oldInstance);
			}
			if (this.ShouldStoreInReverseDictionary(newInstance))
			{
				this.reverseDictionary[newInstance] = viewNode;
			}
		}

		public void OnViewNodeRemoved(ViewNode viewNode)
		{
			object instance = viewNode.Instance;
			if (this.ShouldStoreInReverseDictionary(instance))
			{
				this.reverseDictionary.Remove(instance);
			}
		}

		protected virtual bool ShouldStoreInReverseDictionary(object instance)
		{
			if (instance == null)
			{
				return false;
			}
			return !instance.GetType().IsValueType;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.reverseDictionary).GetEnumerator();
		}
	}
}