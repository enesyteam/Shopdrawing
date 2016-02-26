using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Text;

namespace Microsoft.Expression.DesignModel.Core
{
	public struct NodePathEntry
	{
		private IProperty propertyKey;

		private DocumentNode container;

		private DocumentNode target;

		private object viewKey;

		public DocumentNode Container
		{
			get
			{
				return this.container;
			}
		}

		public IProperty PropertyKey
		{
			get
			{
				return this.propertyKey;
			}
		}

		public DocumentNode Target
		{
			get
			{
				return this.target;
			}
		}

		public object ViewKey
		{
			get
			{
				return this.viewKey;
			}
		}

		public NodePathEntry(DocumentNode root) : this(null, root, root)
		{
		}

		public NodePathEntry(DocumentNode root, DocumentNode target) : this(null, root, target)
		{
		}

		public NodePathEntry(IProperty propertyKey, DocumentNode container, DocumentNode target) : this(propertyKey, container, target, null)
		{
		}

		public NodePathEntry(IProperty propertyKey, DocumentNode container, DocumentNode target, object viewKey)
		{
			this.propertyKey = propertyKey;
			this.container = container;
			this.target = target;
			this.viewKey = viewKey;
		}

		public bool Equals(NodePathEntry other)
		{
			if (this.target != other.target || this.container != other.container || this.propertyKey != other.propertyKey)
			{
				return false;
			}
			if (this.viewKey == null && other.viewKey == null)
			{
				return true;
			}
			if (this.viewKey == null)
			{
				return false;
			}
			return this.viewKey.Equals(other.viewKey);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NodePathEntry))
			{
				return false;
			}
			return this.Equals((NodePathEntry)obj);
		}

		public override int GetHashCode()
		{
			return this.target.GetHashCode() ^ this.container.GetHashCode() ^ (this.propertyKey != null ? this.propertyKey.GetHashCode() : 0) ^ (this.viewKey != null ? this.viewKey.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append((this.propertyKey != null ? this.propertyKey.ToString() : "null"));
			if (this.viewKey != null)
			{
				stringBuilder.Append("(");
				stringBuilder.Append(this.viewKey.ToString());
				stringBuilder.Append(")");
			}
			stringBuilder.Append("->");
			stringBuilder.Append(this.container.Type.Name);
			stringBuilder.Append(":");
			stringBuilder.Append(this.target.Type.Name);
			return stringBuilder.ToString();
		}
	}
}