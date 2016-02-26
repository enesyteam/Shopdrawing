using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	internal sealed class DocumentNodeBuilderFactory : TypeHandlerFactory<IDocumentNodeBuilder>, IDocumentNodeBuilderFactory
	{
		private IPlatform platform;

		private IDocumentNodeBuilder defaultBuilder;

		private int forcingBuildAnimatedValueCount;

		public IDocumentNodeBuilder DefaultBuilder
		{
			get
			{
				return this.defaultBuilder;
			}
			set
			{
				this.defaultBuilder = value;
			}
		}

		public IDisposable ForceBuildAnimatedValue
		{
			get
			{
				return new DocumentNodeBuilderFactory.ForceBuildAnimatedValueToken(this);
			}
		}

		public bool IsForcingBuildAnimatedValue
		{
			get
			{
				return this.forcingBuildAnimatedValueCount > 0;
			}
		}

		public DocumentNodeBuilderFactory(IPlatform platform, Action initializer) : base(initializer)
		{
			this.platform = platform;
		}

		public DocumentCompositeNode BuildNode(IDocumentContext documentContext, Type instanceType)
		{
			return this.GetBuilder(instanceType).BuildNode(documentContext, instanceType);
		}

		public DocumentNode BuildNode(IDocumentContext documentContext, object instance)
		{
			return this.BuildNode(new NodeBuilderContext(documentContext, this.platform), instance);
		}

		public DocumentNode BuildNode(NodeBuilderContext context, object instance)
		{
			Type type = instance.GetType();
			return this.GetBuilder(type).BuildNode(context, type, instance);
		}

		public DocumentNode BuildNode(IDocumentContext documentContext, Type instanceType, object instance)
		{
			return this.BuildNode(new NodeBuilderContext(documentContext, this.platform), instanceType, instance);
		}

		public DocumentNode BuildNode(NodeBuilderContext context, Type instanceType, object instance)
		{
			if (instance != null)
			{
				instanceType = instance.GetType();
			}
			return this.GetBuilder(instanceType).BuildNode(context, instanceType, instance);
		}

		protected override Type GetBaseType(IDocumentNodeBuilder handler)
		{
			return handler.BaseType;
		}

		public IDocumentNodeBuilder GetBuilder(Type instanceType)
		{
			return base.GetHandler(instanceType);
		}

		protected override IDocumentNodeBuilder GetDefaultHandler(Type type)
		{
			base.InitializeIfNecessary();
			return this.defaultBuilder;
		}

		public void Register(IDocumentNodeBuilder value)
		{
			base.RegisterHandler(value);
		}

		public void Unregister(IDocumentNodeBuilder value)
		{
			base.UnregisterHandler(value);
		}

		private class ForceBuildAnimatedValueToken : IDisposable
		{
			private DocumentNodeBuilderFactory factory;

			public ForceBuildAnimatedValueToken(DocumentNodeBuilderFactory factory)
			{
				this.factory = factory;
				DocumentNodeBuilderFactory documentNodeBuilderFactory = this.factory;
				documentNodeBuilderFactory.forcingBuildAnimatedValueCount = documentNodeBuilderFactory.forcingBuildAnimatedValueCount + 1;
			}

			public void Dispose()
			{
				if (this.factory != null)
				{
					DocumentNodeBuilderFactory documentNodeBuilderFactory = this.factory;
					documentNodeBuilderFactory.forcingBuildAnimatedValueCount = documentNodeBuilderFactory.forcingBuildAnimatedValueCount - 1;
					this.factory = null;
					GC.SuppressFinalize(this);
				}
			}
		}
	}
}