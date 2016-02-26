using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class PersistentViewNodeDictionary<T> : IEnumerable<KeyValuePair<DocumentNode, T>>, IEnumerable
	{
		private Dictionary<DocumentNode, List<PersistentData<T>>> dictionary;

		private int count;

		private ViewNodeManager viewNodeManager;

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public IEnumerable<DocumentNode> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public PersistentViewNodeDictionary(ViewNodeManager viewNodeManager)
		{
			this.viewNodeManager = viewNodeManager;
			this.count = 0;
		}

		public void Clear()
		{
			this.dictionary.Clear();
			this.count = 0;
		}

		private static bool CompareNodePaths(DocumentNodePath stored, DocumentNodePath fromViewNodeTree)
		{
			if (stored == fromViewNodeTree)
			{
				return true;
			}
			if (stored.Count != fromViewNodeTree.Count && stored.Count + 1 != fromViewNodeTree.Count)
			{
				return false;
			}
			int num = 0;
			int num1 = 0;
			while (num < stored.Count && num1 < fromViewNodeTree.Count)
			{
				NodePathEntry item = stored[num];
				NodePathEntry nodePathEntry = fromViewNodeTree[num1];
				if (!item.Equals(nodePathEntry))
				{
					if (item.Container != nodePathEntry.Container || num1 + 1 >= fromViewNodeTree.Count || item.Target != fromViewNodeTree[num1 + 1].Target)
					{
						return false;
					}
					num1++;
				}
				num1++;
				num++;
			}
			if (num != stored.Count)
			{
				return false;
			}
			return num1 == fromViewNodeTree.Count;
		}

		public bool Contains(ViewNode viewNode)
		{
			if (this.count <= 0)
			{
				return false;
			}
			return this.GetData(viewNode) != null;
		}

		public PersistentData<T> GetData(ViewNode viewNode)
		{
			List<PersistentData<T>> persistentDatas;
			PersistentData<T> persistentDatum;
			if (this.dictionary.TryGetValue(viewNode.DocumentNode, out persistentDatas))
			{
				DocumentNodePath correspondingNodePath = this.viewNodeManager.GetCorrespondingNodePath(viewNode);
				if (correspondingNodePath == null)
				{
					return null;
				}
				List<PersistentData<T>>.Enumerator enumerator = persistentDatas.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						PersistentData<T> current = enumerator.Current;
						if (!PersistentViewNodeDictionary<T>.CompareNodePaths(current.Target, correspondingNodePath))
						{
							continue;
						}
						persistentDatum = current;
						return persistentDatum;
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return persistentDatum;
			}
			return null;
		}

		public IEnumerator<KeyValuePair<DocumentNode, T>> GetEnumerator()
		{
			foreach (KeyValuePair<DocumentNode, List<PersistentData<T>>> keyValuePair in this.dictionary)
			{
				if (keyValuePair.Value == null || keyValuePair.Value.Count <= 0)
				{
					continue;
				}
				List<PersistentData<T>> value = keyValuePair.Value;
				int num = value.FindIndex((PersistentData<T> i) => {
					if (i.InnerSource == null)
					{
						return false;
					}
					return i.InnerSource.DocumentRoot != null;
				});
				if (num < 0)
				{
					num = 0;
				}
				DocumentNode innerSource = keyValuePair.Value[num].InnerSource;
				yield return new KeyValuePair<DocumentNode, T>(innerSource, keyValuePair.Value[num].Data);
			}
		}

		private void Remove(DocumentNode documentNode)
		{
			if (this.count > 0 && this.dictionary.Remove(documentNode))
			{
				PersistentViewNodeDictionary<T> persistentViewNodeDictionary = this;
				persistentViewNodeDictionary.count = persistentViewNodeDictionary.count - 1;
			}
		}

		public void Remove(ViewNode viewNode)
		{
			List<PersistentData<T>> persistentDatas;
			if (this.count > 0 && this.dictionary.TryGetValue(viewNode.DocumentNode, out persistentDatas))
			{
				DocumentNodePath correspondingNodePath = this.viewNodeManager.GetCorrespondingNodePath(viewNode);
				persistentDatas.RemoveAll((PersistentData<T> item) => PersistentViewNodeDictionary<T>.CompareNodePaths(item.Target, correspondingNodePath));
				if (persistentDatas.Count == 0)
				{
					this.Remove(viewNode.DocumentNode);
				}
			}
		}

		public void SetData(ViewNode target, DocumentNode innerSource, T data)
		{
			List<PersistentData<T>> persistentDatas;
			DocumentNodePath correspondingNodePath = this.viewNodeManager.GetCorrespondingNodePath(target);
			PersistentData<T> persistentDatum = new PersistentData<T>(data, correspondingNodePath, innerSource);
			if (!this.dictionary.TryGetValue(target.DocumentNode, out persistentDatas))
			{
				persistentDatas = new List<PersistentData<T>>()
				{
					persistentDatum
				};
				this.dictionary[target.DocumentNode] = persistentDatas;
				PersistentViewNodeDictionary<T> persistentViewNodeDictionary = this;
				persistentViewNodeDictionary.count = persistentViewNodeDictionary.count + 1;
				return;
			}
			for (int i = 0; i < persistentDatas.Count; i++)
			{
				if (PersistentViewNodeDictionary<T>.CompareNodePaths(persistentDatas[i].Target, correspondingNodePath))
				{
					persistentDatas[i] = persistentDatum;
					return;
				}
			}
			persistentDatas.Add(persistentDatum);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}