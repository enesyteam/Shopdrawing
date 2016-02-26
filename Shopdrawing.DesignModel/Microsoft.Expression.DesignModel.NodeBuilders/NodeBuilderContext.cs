using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public sealed class NodeBuilderContext
	{
		private IDocumentContext documentContext;

		private IPlatform platform;

		private DocumentCompositeNode scopeNode;

		private Type targetType;

		private HashSet<object> builtObjects = new HashSet<object>();

		public IDocumentContext DocumentContext
		{
			get
			{
				return this.documentContext;
			}
		}

		private IDocumentNodeBuilderFactory DocumentNodeBuilderFactory
		{
			get
			{
				return this.platform.DocumentNodeBuilderFactory;
			}
		}

		public bool IsForcingBuildAnimatedValue
		{
			get
			{
				return this.DocumentNodeBuilderFactory.IsForcingBuildAnimatedValue;
			}
		}

		public DocumentNodeNameScope NameScope
		{
			get
			{
				if (this.scopeNode == null)
				{
					return null;
				}
				return this.scopeNode.FindNameScopeForChildren();
			}
		}

		public DocumentCompositeNode ScopeNode
		{
			get
			{
				return this.scopeNode;
			}
		}

		public Type TargetType
		{
			get
			{
				return this.targetType;
			}
		}

		public ITypeResolver TypeResolver
		{
			get
			{
				return this.documentContext.TypeResolver;
			}
		}

		public NodeBuilderContext(IDocumentContext documentContext, IPlatform platform)
		{
			this.documentContext = documentContext;
			this.platform = platform;
		}

		public DocumentCompositeNode BuildNode(Type instanceType)
		{
			return this.DocumentNodeBuilderFactory.BuildNode(this.documentContext, instanceType);
		}

		public DocumentNode BuildNode(object instance)
		{
			return this.DocumentNodeBuilderFactory.BuildNode(this, instance);
		}

		public DocumentNode BuildNode(Type instanceType, object instance)
		{
			return this.DocumentNodeBuilderFactory.BuildNode(this, instanceType, instance);
		}

		public IDocumentNodeChildBuilder GetChildBuilder(Type instanceType)
		{
			return this.platform.DocumentNodeChildBuilderFactory.GetChildBuilder(instanceType);
		}

		public IDocumentNodePropertyBuilder GetPropertyBuilder(Type instanceType)
		{
			return this.platform.DocumentNodePropertyBuilderFactory.GetPropertyBuilder(this.TypeResolver, instanceType);
		}

		public IDisposable PushCycleGuard(object value, out bool shouldBuild)
		{
			NodeBuilderContext.CycleGuard cycleGuard = new NodeBuilderContext.CycleGuard(this, value);
			shouldBuild = !cycleGuard.AlreadyExists;
			return cycleGuard;
		}

		public IDisposable PushScopeNode(DocumentCompositeNode scopeNode)
		{
			if (scopeNode == null)
			{
				throw new ArgumentNullException("scopeNode");
			}
			IDisposable changeScopeNode = new NodeBuilderContext.ChangeScopeNode(this, this.scopeNode);
			this.scopeNode = scopeNode;
			return changeScopeNode;
		}

		public IDisposable PushTargetType(Type targetType)
		{
			IDisposable changeTargetType = new NodeBuilderContext.ChangeTargetType(this, this.targetType);
			this.targetType = targetType;
			return changeTargetType;
		}

		private sealed class ChangeScopeNode : IDisposable
		{
			private NodeBuilderContext context;

			private DocumentCompositeNode scopeNode;

			public ChangeScopeNode(NodeBuilderContext context, DocumentCompositeNode scopeNode)
			{
				this.context = context;
				this.scopeNode = scopeNode;
			}

			public void Dispose()
			{
				this.context.scopeNode = this.scopeNode;
			}
		}

		private sealed class ChangeTargetType : IDisposable
		{
			private NodeBuilderContext context;

			private Type targetType;

			public ChangeTargetType(NodeBuilderContext context, Type targetType)
			{
				this.context = context;
				this.targetType = targetType;
			}

			public void Dispose()
			{
				this.context.targetType = this.targetType;
			}
		}

		private struct CycleGuard : IDisposable
		{
			private NodeBuilderContext context;

			private object @value;

			public bool AlreadyExists
			{
				get
				{
					return this.@value == null;
				}
			}

			public CycleGuard(NodeBuilderContext context, object value)
			{
				this.context = context;
				this.@value = value;
				if (!this.context.builtObjects.Add(this.@value))
				{
					this.@value = null;
				}
			}

			public void Dispose()
			{
				if (this.@value != null)
				{
					this.context.builtObjects.Remove(this.@value);
					this.@value = null;
				}
			}
		}
	}
}