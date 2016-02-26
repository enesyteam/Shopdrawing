using Microsoft.Expression.DesignModel.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class TypeMetadataFactory : ITypeMetadataFactory
	{
		private readonly ITypeResolver typeResolver;

		private readonly Action<ITypeMetadataFactory> initializer;

		private TypeMetadataFactory.MetadataFactory factory;

		private Dictionary<Type, ITypeMetadata> metadata;

		public TypeMetadataFactory(ITypeResolver typeResolver, Action<ITypeMetadataFactory> initializer)
		{
			this.typeResolver = typeResolver;
			this.initializer = initializer;
			this.Reset();
		}

		public ITypeMetadata GetMetadata(Type type)
		{
			ITypeMetadata factory;
			if (!this.metadata.TryGetValue(type, out factory))
			{
				TypeMetadataFactory.MetadataEntry metadata = this.factory.GetMetadata(type);
				factory = metadata.Factory(type);
				factory.TypeResolver = this.typeResolver;
				this.metadata.Add(type, factory);
			}
			return factory;
		}

		private void Initialize()
		{
			if (this.initializer != null)
			{
				this.initializer(this);
			}
		}

		public void Register(Type type, MetadataFactoryCallback factory)
		{
			this.factory.RegisterMetadata(type, factory);
			this.metadata.Clear();
		}

		public void Reset()
		{
			this.factory = new TypeMetadataFactory.MetadataFactory(new Action(this.Initialize));
			this.metadata = new Dictionary<Type, ITypeMetadata>();
		}

		private sealed class MetadataEntry
		{
			private Type type;

			private MetadataFactoryCallback factory;

			public MetadataFactoryCallback Factory
			{
				get
				{
					return this.factory;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}

			public MetadataEntry(Type type, MetadataFactoryCallback factory)
			{
				this.type = type;
				this.factory = factory;
			}
		}

		private sealed class MetadataFactory : TypeHandlerFactory<TypeMetadataFactory.MetadataEntry>
		{
			private TypeMetadataFactory.MetadataEntry defaultHandler;

			public MetadataFactory(Action initializer) : base(initializer)
			{
			}

			protected override Type GetBaseType(TypeMetadataFactory.MetadataEntry metadata)
			{
				return metadata.Type;
			}

			protected override TypeMetadataFactory.MetadataEntry GetDefaultHandler(Type type)
			{
				return this.defaultHandler;
			}

			public TypeMetadataFactory.MetadataEntry GetMetadata(Type type)
			{
				return base.GetHandler(type);
			}

			public void RegisterMetadata(Type type, MetadataFactoryCallback factory)
			{
				base.RegisterHandler(new TypeMetadataFactory.MetadataEntry(type, factory));
			}
		}
	}
}