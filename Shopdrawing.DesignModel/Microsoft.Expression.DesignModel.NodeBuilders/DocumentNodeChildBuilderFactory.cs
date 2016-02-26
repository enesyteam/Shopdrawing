using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	internal sealed class DocumentNodeChildBuilderFactory : TypeHandlerFactory<IDocumentNodeChildBuilder>, IDocumentNodeChildBuilderFactory
	{
		private IDocumentNodeChildBuilder arrayChildBuilder;

		private IDocumentNodeChildBuilder collectionChildBuilder;

		public IDocumentNodeChildBuilder ArrayChildBuilder
		{
			get
			{
				return this.arrayChildBuilder;
			}
			set
			{
				this.arrayChildBuilder = value;
			}
		}

		public IDocumentNodeChildBuilder CollectionChildBuilder
		{
			get
			{
				return this.collectionChildBuilder;
			}
			set
			{
				this.collectionChildBuilder = value;
			}
		}

		public DocumentNodeChildBuilderFactory(Action initializer) : base(initializer)
		{
		}

		protected override Type GetBaseType(IDocumentNodeChildBuilder handler)
		{
			return handler.BaseType;
		}

		public IDocumentNodeChildBuilder GetChildBuilder(Type type)
		{
			return base.GetHandler(type);
		}

		protected override IDocumentNodeChildBuilder GetDefaultHandler(Type type)
		{
			base.InitializeIfNecessary();
			if (type.IsArray)
			{
				return this.arrayChildBuilder;
			}
			if (CollectionAdapterDescription.GetAdapterDescription(type) == null)
			{
				return null;
			}
			return this.collectionChildBuilder;
		}

		public void Register(IDocumentNodeChildBuilder builder)
		{
			base.RegisterHandler(builder);
		}

		public void Unregister(IDocumentNodeChildBuilder builder)
		{
			base.UnregisterHandler(builder);
		}
	}
}