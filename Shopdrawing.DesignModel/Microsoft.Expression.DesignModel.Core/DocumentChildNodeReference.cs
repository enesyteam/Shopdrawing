using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class DocumentChildNodeReference : DocumentNodeReference
	{
		private DocumentCompositeNode parent;

		private int childIndex;

		public int ChildIndex
		{
			get
			{
				return this.childIndex;
			}
		}

		public override DocumentNode Node
		{
			get
			{
				return this.parent.Children[this.childIndex];
			}
			set
			{
				this.parent.Children[this.childIndex] = value;
			}
		}

		public DocumentCompositeNode Parent
		{
			get
			{
				return this.parent;
			}
		}

		public DocumentChildNodeReference(DocumentCompositeNode parent, int childIndex)
		{
			this.parent = parent;
			this.childIndex = childIndex;
		}

		public override bool Equals(object obj)
		{
			DocumentChildNodeReference documentChildNodeReference = obj as DocumentChildNodeReference;
			if (documentChildNodeReference == null || !this.parent.Equals(documentChildNodeReference.parent))
			{
				return false;
			}
			return this.childIndex.Equals(documentChildNodeReference.childIndex);
		}

		public override int GetHashCode()
		{
			return this.parent.GetHashCode() ^ this.childIndex.GetHashCode();
		}

		public override string ToString()
		{
			object[] str = new object[] { this.parent.ToString(), "[", this.childIndex, "]" };
			return string.Concat(str);
		}
	}
}