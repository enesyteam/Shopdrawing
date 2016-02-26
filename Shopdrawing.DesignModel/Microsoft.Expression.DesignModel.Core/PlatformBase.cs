using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.NodeBuilders;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class PlatformBase : IPlatform, IViewNodeManagerFactory, IDisposable
	{
		private IPlatformHost platformHost;

		private IDocumentNodeBuilderFactory documentNodeBuilderFactory;

		private IDocumentNodeChildBuilderFactory documentNodeChildBuilderFactory;

		private IDocumentNodePropertyBuilderFactory documentNodePropertyBuilderFactory;

		private IInstanceBuilderFactory instanceBuilderFactory;

		private IViewObjectFactory viewObjectFactory;

		private bool isDisposed;

		public IDocumentNodeBuilderFactory DocumentNodeBuilderFactory
		{
			get
			{
				return this.documentNodeBuilderFactory;
			}
		}

		public IDocumentNodeChildBuilderFactory DocumentNodeChildBuilderFactory
		{
			get
			{
				return this.documentNodeChildBuilderFactory;
			}
		}

		public IDocumentNodePropertyBuilderFactory DocumentNodePropertyBuilderFactory
		{
			get
			{
				return this.documentNodePropertyBuilderFactory;
			}
		}

		public IEffectManager EffectManager
		{
			get
			{
				return this.CreateEffectManager();
			}
		}

		public abstract IPlatformGeometryHelper GeometryHelper
		{
			get;
		}

		public IInstanceBuilderFactory InstanceBuilderFactory
		{
			get
			{
				return this.instanceBuilderFactory;
			}
		}

		public abstract IPlatformTypes Metadata
		{
			get;
		}

		public virtual bool NeedsContextUpdate
		{
			get
			{
				return false;
			}
		}

		public IPlatformHost PlatformHost
		{
			get
			{
				return this.platformHost;
			}
			set
			{
				if (this.platformHost != value)
				{
					this.platformHost = value;
					this.platformHost.SetPlatform(this);
				}
			}
		}

		public abstract Microsoft.Expression.DesignModel.Core.ThemeManager ThemeManager
		{
			get;
		}

		public virtual IViewResourceDictionary ThemeResources
		{
			get
			{
				return null;
			}
		}

		public abstract ViewContentManager ViewContentProvider
		{
			get;
		}

		public IViewObjectFactory ViewObjectFactory
		{
			get
			{
				return this.viewObjectFactory;
			}
		}

		public abstract IViewTextObjectFactory ViewTextObjectFactory
		{
			get;
		}

		protected PlatformBase()
		{
			PlatformBase platformBase = this;
			this.documentNodeBuilderFactory = new Microsoft.Expression.DesignModel.NodeBuilders.DocumentNodeBuilderFactory(this, new Action(platformBase.RegisterNodeBuilders));
			PlatformBase platformBase1 = this;
			this.documentNodeChildBuilderFactory = new Microsoft.Expression.DesignModel.NodeBuilders.DocumentNodeChildBuilderFactory(new Action(platformBase1.RegisterNodeChildBuilders));
			PlatformBase platformBase2 = this;
			this.documentNodePropertyBuilderFactory = new Microsoft.Expression.DesignModel.NodeBuilders.DocumentNodePropertyBuilderFactory(new Action(platformBase2.RegisterNodePropertyBuilders));
			PlatformBase platformBase3 = this;
			this.instanceBuilderFactory = new Microsoft.Expression.DesignModel.InstanceBuilders.InstanceBuilderFactory(new Action(platformBase3.RegisterInstanceBuilders));
			PlatformBase platformBase4 = this;
			this.viewObjectFactory = new Microsoft.Expression.DesignModel.ViewObjects.ViewObjectFactory(new Action(platformBase4.RegisterViewObjects));
		}

		public virtual void ActivatePlatformContext()
		{
		}

		protected abstract IEffectManager CreateEffectManager();

		protected abstract IInstanceDictionary CreateInstanceDictionary(ViewNodeManager manager);

		public abstract IViewPanel CreateOverlayLayer();

		public abstract IPlatformSpecificView CreateSurface();

		protected abstract ViewNodeManager CreateViewNodeManager();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				if (this.Metadata != null)
				{
					this.Metadata.Dispose();
				}
				this.isDisposed = true;
			}
		}

		public virtual string GetDeploymentInformation(Uri rootUri, out string deploymentLocation)
		{
			deploymentLocation = null;
			return null;
		}

		public abstract void Initialize();

		ViewNodeManager Microsoft.Expression.DesignModel.InstanceBuilders.IViewNodeManagerFactory.Create()
		{
			return this.CreateViewNodeManager();
		}

		IInstanceDictionary Microsoft.Expression.DesignModel.InstanceBuilders.IViewNodeManagerFactory.CreateInstanceDictionary(ViewNodeManager manager)
		{
			return this.CreateInstanceDictionary(manager);
		}

		public virtual void RefreshProjectSpecificMetadata(ITypeResolver typeResolver, ITypeMetadataFactory typeMetadataFactory)
		{
		}

		protected abstract void RegisterInstanceBuilders();

		protected abstract void RegisterNodeBuilders();

		protected abstract void RegisterNodeChildBuilders();

		protected abstract void RegisterNodePropertyBuilders();

		protected abstract void RegisterViewObjects();

		public abstract void Shutdown();

		public abstract IViewVisual TryGetElementForException(Exception exception);
	}
}