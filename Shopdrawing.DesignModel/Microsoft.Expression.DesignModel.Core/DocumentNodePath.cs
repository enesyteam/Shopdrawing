using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class DocumentNodePath
	{
		private FrugalStructList<NodePathEntry> nodes;

		public DocumentNode ContainerNode
		{
			get
			{
				NodePathEntry item = this.nodes[this.nodes.Count - 1];
				return item.Container;
			}
		}

		public DocumentNode ContainerOwner
		{
			get
			{
				if (this.nodes.Count <= 1)
				{
					return null;
				}
				NodePathEntry item = this.nodes[this.nodes.Count - 2];
				return item.Target;
			}
		}

		public IProperty ContainerOwnerProperty
		{
			get
			{
				NodePathEntry item = this.nodes[this.nodes.Count - 1];
				return item.PropertyKey;
			}
		}

		public int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public NodePathEntry this[int index]
		{
			get
			{
				return this.nodes[index];
			}
		}

		public DocumentNode Node
		{
			get
			{
				NodePathEntry item = this.nodes[this.nodes.Count - 1];
				return item.Target;
			}
		}

		public DocumentNode RootNode
		{
			get
			{
				return this.nodes[0].Container;
			}
		}

		internal DocumentNodePath()
		{
			this.nodes = new FrugalStructList<NodePathEntry>();
		}

		public DocumentNodePath(DocumentNode documentRoot, DocumentNode node) : this()
		{
			this.nodes.Add(new NodePathEntry(documentRoot, node));
		}

		public DocumentNodePath(IEnumerable<NodePathEntry> entries) : this()
		{
			foreach (NodePathEntry entry in entries)
			{
				this.nodes.Add(entry);
			}
		}

		private void AppendNodePathEntries(DocumentNodePath source, int numberOfEntries)
		{
			if (numberOfEntries > source.nodes.Count)
			{
				throw new ArgumentException(ExceptionStringTable.DocumentNodePathNumberOfNodesExceeded, "numberOfEntries");
			}
			for (int i = 0; i < numberOfEntries; i++)
			{
				this.nodes.Add(source.nodes[i]);
			}
		}

		public bool Contains(DocumentNode nodeBeingLookedFor)
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				NodePathEntry item = this.nodes[i];
				DocumentNode container = item.Container;
				DocumentNode target = item.Target;
				if (container == nodeBeingLookedFor)
				{
					return true;
				}
				while (target != container)
				{
					if (target == null)
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string documentNodePathContainerIsNotAncestor = ExceptionStringTable.DocumentNodePathContainerIsNotAncestor;
						object[] name = new object[] { container.Type.Name, item.Target.Type.Name };
						throw new InvalidOperationException(string.Format(currentCulture, documentNodePathContainerIsNotAncestor, name));
					}
					if (target == nodeBeingLookedFor)
					{
						return true;
					}
					target = target.Parent;
				}
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj != this)
			{
				DocumentNodePath documentNodePath = obj as DocumentNodePath;
				if (documentNodePath == null || documentNodePath.nodes.Count != this.nodes.Count)
				{
					flag = false;
				}
				else
				{
					flag = true;
					int count = this.nodes.Count - 1;
					while (count >= 0)
					{
						if (this.nodes[count].Equals(documentNodePath.nodes[count]))
						{
							count--;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public DocumentNodePath GetContainerNodePath()
		{
			return this.GetPathInContainer(this.ContainerNode);
		}

		public DocumentNodePath GetContainerOwnerPath()
		{
			DocumentNodePath subpath = null;
			if (this.nodes.Count > 1)
			{
				subpath = this.GetSubpath(this.nodes.Count - 1);
			}
			return subpath;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				NodePathEntry item = this.nodes[i];
				hashCode = hashCode ^ item.GetHashCode();
			}
			return hashCode;
		}

		public DocumentNodePath GetParent()
		{
			DocumentNodePath containerOwnerPath = null;
			DocumentNode node = this.Node;
			if (node == this.ContainerNode)
			{
				containerOwnerPath = this.GetContainerOwnerPath();
			}
			else if (node.Parent != null)
			{
				containerOwnerPath = this.GetPathInContainer(node.Parent);
			}
			return containerOwnerPath;
		}

		public void GetPathAsNodes(List<DocumentNode> newNodeList)
		{
			newNodeList.Clear();
			for (int i = 0; i < this.nodes.Count; i++)
			{
				NodePathEntry item = this.nodes[i];
				DocumentNode container = item.Container;
				DocumentNode target = item.Target;
				int count = newNodeList.Count;
				while (target != container)
				{
					if (target == null)
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string documentNodePathContainerIsNotAncestor = ExceptionStringTable.DocumentNodePathContainerIsNotAncestor;
						object[] name = new object[] { container.Type.Name, item.Target.Type.Name };
						throw new InvalidOperationException(string.Format(currentCulture, documentNodePathContainerIsNotAncestor, name));
					}
					newNodeList.Insert(count, target);
					target = target.Parent;
				}
				newNodeList.Insert(count, container);
			}
		}

		public List<DocumentNode> GetPathAsNodes()
		{
			List<DocumentNode> documentNodes = new List<DocumentNode>();
			this.GetPathAsNodes(documentNodes);
			return documentNodes;
		}

		public string GetPathAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<DocumentNode> pathAsNodes = this.GetPathAsNodes();
			DocumentNode documentNode = null;
			for (int i = 0; i < pathAsNodes.Count; i++)
			{
				DocumentNode item = pathAsNodes[i];
				bool flag = true;
				if (documentNode != null && documentNode == item.Parent)
				{
					if (!item.IsProperty)
					{
						int num = item.Parent.Children.IndexOf(item);
						CultureInfo invariantCulture = CultureInfo.InvariantCulture;
						object[] objArray = new object[] { num };
						stringBuilder.Append(string.Format(invariantCulture, "[{0}]", objArray));
						flag = false;
					}
					else
					{
						CultureInfo cultureInfo = CultureInfo.InvariantCulture;
						object[] name = new object[] { item.SitePropertyKey.Name };
						stringBuilder.Append(string.Format(cultureInfo, ".{0}", name));
						flag = false;
					}
				}
				if (flag)
				{
					string str = item.Type.Name;
					if (documentNode == null)
					{
						CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
						object[] objArray1 = new object[] { str };
						stringBuilder.Append(string.Format(invariantCulture1, "{0}", objArray1));
					}
					else
					{
						CultureInfo cultureInfo1 = CultureInfo.InvariantCulture;
						object[] objArray2 = new object[] { str };
						stringBuilder.Append(string.Format(cultureInfo1, ".{0}", objArray2));
					}
				}
				documentNode = item;
			}
			return stringBuilder.ToString();
		}

		public DocumentNodePath GetPathInContainer(DocumentNode node)
		{
			DocumentNodePath subpath = this.GetSubpath(this.nodes.Count - 1);
			NodePathEntry item = this.nodes[this.nodes.Count - 1];
			subpath.nodes.Add(new NodePathEntry(item.PropertyKey, item.Container, node, item.ViewKey));
			return subpath;
		}

		public DocumentNodePath GetPathInSubContainer(IProperty propertyKey, DocumentNode newContainer)
		{
			DocumentNodePath subpath = this.GetSubpath(this.nodes.Count);
			subpath.nodes.Add(new NodePathEntry(propertyKey, newContainer, newContainer));
			return subpath;
		}

		private DocumentNodePath GetSubpath(int numberOfEntries)
		{
			DocumentNodePath documentNodePath = new DocumentNodePath();
			documentNodePath.AppendNodePathEntries(this, numberOfEntries);
			return documentNodePath;
		}

		public bool IsAncestorOf(DocumentNodePath other)
		{
			if (other == this)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			int count = this.nodes.Count;
			if (count <= other.nodes.Count)
			{
				int num = 0;
				while (num < count && this.nodes[num].Equals(other.nodes[num]))
				{
					num++;
				}
				if (num >= count)
				{
					return true;
				}
				if (this.nodes[num].Container == other.nodes[num].Container && this.nodes[num].Target.IsAncestorOf(other.nodes[num].Target))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsValid()
		{
			return this.IsValid(false);
		}

		public bool IsValid(bool verifyIsInDocument)
		{
			for (int i = this.nodes.Count - 1; i >= 0; i--)
			{
				DocumentNode container = this.nodes[i].Container;
				DocumentNode target = this.nodes[i].Target;
				if (verifyIsInDocument && (!container.IsInDocument || !target.IsInDocument))
				{
					return false;
				}
				while (target != null && target != container)
				{
					target = target.Parent;
				}
				if (target != container)
				{
					return false;
				}
			}
			return true;
		}

		public bool PathReevaluates(ExpressionEvaluator evaluator)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = this.nodes.Count - 1; i > 0; i--)
			{
				DocumentNode target = this.nodes[i - 1].Target;
				if (target != null)
				{
					DocumentCompositeNode documentCompositeNode = target as DocumentCompositeNode;
					IPropertyId propertyKey = this.nodes[i].PropertyKey;
					if (documentCompositeNode != null && propertyKey != null)
					{
						DocumentNode item = documentCompositeNode.Properties[propertyKey];
						if (item != null && item.Type.IsExpression)
						{
							item = evaluator.EvaluateExpression(this.GetSubpath(i), item);
						}
						if (item == null || item != this.nodes[i].Container)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				NodePathEntry item = this.nodes[i];
				stringBuilder.Append(item.ToString());
			}
			return stringBuilder.ToString();
		}

		[Conditional("DEBUG")]
		private void VerifyInSubtree(DocumentNode ancestor, DocumentNode potentialDescendant)
		{
			do
			{
				if (ancestor == potentialDescendant)
				{
					return;
				}
				potentialDescendant = potentialDescendant.Parent;
			}
			while (potentialDescendant != null);
		}
	}
}