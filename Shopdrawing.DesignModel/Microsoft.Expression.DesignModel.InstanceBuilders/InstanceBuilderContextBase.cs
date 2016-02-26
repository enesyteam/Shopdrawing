using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	[DebuggerDisplay("{System.IO.Path.GetFileName(this.DocumentContext.DocumentUrl)}")]
	public abstract class InstanceBuilderContextBase : IInstanceBuilderContext, IDisposable
	{
		private IPlatform platform;

		private IDocumentContext documentContext;

		private IResourceDictionaryHost resourceDictionaryHost;

		private ICrossDocumentUpdateContext crossDocumentUpdateContext;

		private IInstanceBuilderFactory instanceBuilderFactory;

		private IDocumentRootResolver documentRootResolver;

		private ITypeMetadataFactory metadataFactory;

		private IInstanceDictionary instanceDictionary;

		private IViewRootResolver viewRootResolver;

		private IExceptionDictionary exceptionDictionary;

		private Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager viewNodeManager;

		private INameScope nameScope;

		private ISerializationContext serializationContext;

		private IInstanceBuilderContext parentContext;

		private IWarningDictionary warningDictionary;

		private bool useShadowProperties;

		private bool shouldRegisterInstantiatedElements;

		private ITextBufferService textBufferService;

		private IEffectManager effectManager;

		private ICollection<ViewNode> userControlInstances;

		private ICollection<string> currentlyInstantiatingUserControlPreviews;

		private ViewNode containerRoot;

		private DocumentNode alternateSiteNode;

		public bool AllowIncrementalTemplateUpdates
		{
			get
			{
				return JustDecompileGenerated_get_AllowIncrementalTemplateUpdates();
			}
			set
			{
				JustDecompileGenerated_set_AllowIncrementalTemplateUpdates(value);
			}
		}

		private bool JustDecompileGenerated_AllowIncrementalTemplateUpdates_k__BackingField;

		public bool JustDecompileGenerated_get_AllowIncrementalTemplateUpdates()
		{
			return this.JustDecompileGenerated_AllowIncrementalTemplateUpdates_k__BackingField;
		}

		private void JustDecompileGenerated_set_AllowIncrementalTemplateUpdates(bool value)
		{
			this.JustDecompileGenerated_AllowIncrementalTemplateUpdates_k__BackingField = value;
		}

		public bool AllowPostponingResourceEvaluation
		{
			get
			{
				return JustDecompileGenerated_get_AllowPostponingResourceEvaluation();
			}
			set
			{
				JustDecompileGenerated_set_AllowPostponingResourceEvaluation(value);
			}
		}

		private bool JustDecompileGenerated_AllowPostponingResourceEvaluation_k__BackingField;

		public bool JustDecompileGenerated_get_AllowPostponingResourceEvaluation()
		{
			return this.JustDecompileGenerated_AllowPostponingResourceEvaluation_k__BackingField;
		}

		private void JustDecompileGenerated_set_AllowPostponingResourceEvaluation(bool value)
		{
			this.JustDecompileGenerated_AllowPostponingResourceEvaluation_k__BackingField = value;
		}

		public DocumentNode AlternateSiteNode
		{
			get
			{
				return this.alternateSiteNode;
			}
		}

		public virtual IAttachViewRoot AttachedViewRoot
		{
			get
			{
				return null;
			}
		}

		public ViewNode ContainerRoot
		{
			get
			{
				return this.containerRoot;
			}
		}

		public ICrossDocumentUpdateContext CrossDocumentUpdateContext
		{
			get
			{
				return this.crossDocumentUpdateContext;
			}
		}

		public ICollection<string> CurrentlyInstantiatingUserControlPreviews
		{
			get
			{
				return this.currentlyInstantiatingUserControlPreviews;
			}
		}

		public virtual IPlatform DesignerDefaultPlatform
		{
			get
			{
				return null;
			}
		}

		public bool DisableProjectionTransforms
		{
			get;
			set;
		}

		public IDocumentContext DocumentContext
		{
			get
			{
				return this.documentContext;
			}
		}

		public IDocumentRootResolver DocumentRootResolver
		{
			get
			{
				return JustDecompileGenerated_get_DocumentRootResolver();
			}
			set
			{
				JustDecompileGenerated_set_DocumentRootResolver(value);
			}
		}

		public IDocumentRootResolver JustDecompileGenerated_get_DocumentRootResolver()
		{
			return this.documentRootResolver;
		}

		protected void JustDecompileGenerated_set_DocumentRootResolver(IDocumentRootResolver value)
		{
			this.documentRootResolver = value;
		}

		public IEffectManager EffectManager
		{
			get
			{
				return this.effectManager;
			}
		}

		public IExceptionDictionary ExceptionDictionary
		{
			get
			{
				return this.exceptionDictionary;
			}
		}

		public IInstanceBuilderFactory InstanceBuilderFactory
		{
			get
			{
				return this.instanceBuilderFactory;
			}
		}

		public IInstanceDictionary InstanceDictionary
		{
			get
			{
				return this.instanceDictionary;
			}
		}

		public bool IsSerializationScope
		{
			get
			{
				return this.serializationContext != null;
			}
		}

		public virtual bool IsStandalone
		{
			get
			{
				return false;
			}
		}

		public ITypeMetadataFactory MetadataFactory
		{
			get
			{
				return this.metadataFactory;
			}
		}

		public INameScope NameScope
		{
			get
			{
				return this.nameScope;
			}
		}

		public abstract IViewPanel OverlayLayer
		{
			get;
		}

		public IInstanceBuilderContext ParentContext
		{
			get
			{
				return this.parentContext;
			}
		}

		public IPlatform Platform
		{
			get
			{
				return this.platform;
			}
		}

		public IResourceDictionaryHost ResourceDictionaryHost
		{
			get
			{
				return this.resourceDictionaryHost;
			}
		}

		public InstanceTypeReplacement RootTargetTypeReplacement
		{
			get;
			set;
		}

		public ISerializationContext SerializationContext
		{
			get
			{
				return this.serializationContext;
			}
		}

		public bool ShouldRegisterInstantiatedElements
		{
			get
			{
				return this.shouldRegisterInstantiatedElements;
			}
		}

		public ITextBufferService TextBufferService
		{
			get
			{
				return this.textBufferService;
			}
		}

		public ICollection<ViewNode> UserControlInstances
		{
			get
			{
				return this.userControlInstances;
			}
		}

		public bool UseShadowProperties
		{
			get
			{
				return this.useShadowProperties;
			}
		}

		public Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager ViewNodeManager
		{
			get
			{
				return this.viewNodeManager;
			}
		}

		protected IViewRootResolver ViewRootResolver
		{
			get
			{
				return this.viewRootResolver;
			}
		}

		public IWarningDictionary WarningDictionary
		{
			get
			{
				return this.warningDictionary;
			}
		}

		protected InstanceBuilderContextBase(IPlatform platform, IDocumentContext documentContext, IDocumentRootResolver documentRootResolver, IViewRootResolver viewRootResolver, ITypeMetadataFactory metadataFactory, INameScope nameScope, IInstanceBuilderContext parentContext, bool useShadowProperties, ITextBufferService textBufferService, DocumentNode alternateSiteNode)
		{
			this.documentContext = documentContext;
			this.platform = platform;
			this.effectManager = platform.EffectManager;
			this.instanceBuilderFactory = platform.InstanceBuilderFactory;
			this.documentRootResolver = documentRootResolver;
			this.viewRootResolver = viewRootResolver;
			this.metadataFactory = metadataFactory;
			this.AllowPostponingResourceEvaluation = true;
			this.viewNodeManager = this.platform.Create();
			this.viewNodeManager.Initialize(this);
			this.userControlInstances = new HashSet<ViewNode>();
			this.currentlyInstantiatingUserControlPreviews = new HashSet<string>();
			this.instanceDictionary = this.platform.CreateInstanceDictionary(this.viewNodeManager);
			this.exceptionDictionary = new Microsoft.Expression.DesignModel.InstanceBuilders.ExceptionDictionary(this.viewNodeManager);
			this.warningDictionary = new Microsoft.Expression.DesignModel.InstanceBuilders.WarningDictionary(this.viewNodeManager);
			this.nameScope = nameScope;
			this.useShadowProperties = useShadowProperties;
			this.shouldRegisterInstantiatedElements = true;
			this.parentContext = parentContext;
			this.textBufferService = textBufferService;
			this.alternateSiteNode = alternateSiteNode;
		}

		public IDisposable ChangeContainerRoot(ViewNode newContainerRoot)
		{
			INameScope instance = newContainerRoot.Instance as INameScope ?? this.nameScope;
			return new InstanceBuilderContextBase.ChangeContainerRootToken(this, newContainerRoot, instance);
		}

		private void ChangeContainerRootInternal(ViewNode containerRoot, INameScope nameScope)
		{
			if (containerRoot != null)
			{
				containerRoot.ViewNodeManager.OnNameScopeChanged(containerRoot, nameScope);
			}
			this.nameScope = nameScope;
			this.containerRoot = containerRoot;
		}

		public IDisposable ChangeCrossDocumentUpdateContext(ICrossDocumentUpdateContext context)
		{
			return new InstanceBuilderContextBase.ChangeCrossDocumentUpdateContextToken(this, context);
		}

		public IDisposable ChangeResourceDictionaryHost(IResourceDictionaryHost host)
		{
			return new InstanceBuilderContextBase.ChangeApplicationResourcesHostToken(this, host);
		}

		public IDisposable ChangeSerializationContext(ISerializationContext serializationContext)
		{
			return new InstanceBuilderContextBase.SetSerializationContextToken(this, serializationContext);
		}

		private void ChangeSerializationContextInternal(ISerializationContext serializationContext)
		{
			this.serializationContext = serializationContext;
		}

		public abstract string ConvertToWpfFontName(string gdiFontName);

		public virtual IDocumentContext CreateErrorDocumentContext(IDocumentContext sourceDocumentContext)
		{
			return null;
		}

		public virtual IInstanceBuilderContext CreateStandaloneChildContext(IDocumentContext documentContext)
		{
			return null;
		}

		public IDisposable DisablePostponedResourceEvaluation()
		{
			return new InstanceBuilderContextBase.ForcePostponedResourceEvaluationToken(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.viewNodeManager != null)
				{
					this.viewNodeManager.DisposeInternal();
					this.viewNodeManager = null;
				}
				this.effectManager = null;
				this.instanceBuilderFactory = null;
				this.metadataFactory = null;
				this.serializationContext = null;
				this.documentRootResolver = null;
				this.viewRootResolver = null;
				this.parentContext = null;
				this.nameScope = null;
				this.textBufferService = null;
				this.containerRoot = null;
				if (this.instanceDictionary != null)
				{
					this.instanceDictionary.Clear();
				}
				if (this.exceptionDictionary != null)
				{
					this.exceptionDictionary.Clear();
				}
				if (this.warningDictionary != null)
				{
					this.warningDictionary.Clear();
				}
				if (this.userControlInstances != null)
				{
					foreach (ViewNode userControlInstance in this.userControlInstances)
					{
						userControlInstance.Dispose();
					}
					this.userControlInstances.Clear();
				}
			}
		}

		public abstract object EvaluateSystemResource(object resourceKey);

		public IDisposable ForceAllowIncrementalTemplateUpdates(bool allow)
		{
			return new InstanceBuilderContextBase.ForceAllowIncrementalTemplateUpdatesToken(this, allow);
		}

		public IDisposable ForceRegistrationOfInstantiatedElements(bool shouldRegisterInstantiatedElements)
		{
			return new InstanceBuilderContextBase.ForceRegistrationOfInstantiatedElementsToken(this, shouldRegisterInstantiatedElements);
		}

		private void ForceRegistrationOfInstantiatedElementsInternal(bool shouldRegisterInstantiatedElements)
		{
			this.shouldRegisterInstantiatedElements = shouldRegisterInstantiatedElements;
		}

		public IDisposable ForceUseShadowProperties(bool useShadowProperties)
		{
			return new InstanceBuilderContextBase.ForceUseShadowPropertiesToken(this, useShadowProperties);
		}

		private void ForceUseShadowPropertiesInternal(bool useShadowProperties)
		{
			this.useShadowProperties = useShadowProperties;
		}

		public abstract ICollection<IProperty> GetProperties(ViewNode viewNode);

		public abstract DocumentNode GetPropertyValue(ViewNode viewNode, IPropertyId propertyKey);

		public IInstanceBuilderContext GetViewContext(IDocumentRoot documentRoot)
		{
			IInstanceBuilderContext instanceBuilderContext;
			instanceBuilderContext = (this.CrossDocumentUpdateContext == null ? this.viewRootResolver.GetViewContext(documentRoot) : this.CrossDocumentUpdateContext.GetViewContext(documentRoot));
			return instanceBuilderContext;
		}

		public abstract bool HasFont(string fontFamilyName);

		public abstract string ResolveFont(string fontFamilySource, object fontStretch, object fontStyle, object fontWeight, IDocumentContext documentContext);

		public IDisposable SetCurrentlyInstantiatingUserControlPreview(string xamlSourcePath)
		{
			return new InstanceBuilderContextBase.InstantiatingUserControlPreviewToken(this, xamlSourcePath);
		}

		public abstract bool ShouldDisableVisualStateManagerFor(ViewNode viewNode);

		public virtual bool ShouldInstantiatePreviewControl(IDocumentRoot documentRoot)
		{
			return !documentRoot.TypeResolver.ProjectAssembly.IsLoaded;
		}

		public static void TransferViewRootResolver(InstanceBuilderContextBase source, InstanceBuilderContextBase target)
		{
			target.viewRootResolver = source.viewRootResolver;
		}

		public virtual void UpdateTextEditProxyIfEditing()
		{
		}

		private class ChangeApplicationResourcesHostToken : IDisposable
		{
			private InstanceBuilderContextBase context;

			private IResourceDictionaryHost originalHost;

			public ChangeApplicationResourcesHostToken(InstanceBuilderContextBase context, IResourceDictionaryHost host)
			{
				this.context = context;
				this.originalHost = this.context.resourceDictionaryHost;
				this.context.resourceDictionaryHost = host;
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.resourceDictionaryHost = this.originalHost;
					this.context = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private class ChangeContainerRootToken : IDisposable
		{
			private ViewNode containerRoot;

			private INameScope containerNameScope;

			private InstanceBuilderContextBase context;

			public ChangeContainerRootToken(InstanceBuilderContextBase context, ViewNode newRoot, INameScope newNameScope)
			{
				this.context = context;
				this.containerRoot = context.containerRoot;
				this.containerNameScope = context.nameScope;
				context.ChangeContainerRootInternal(newRoot, newNameScope);
			}

			public void Dispose()
			{
				this.context.ChangeContainerRootInternal(this.containerRoot, this.containerNameScope);
			}
		}

		private class ChangeCrossDocumentUpdateContextToken : IDisposable
		{
			private InstanceBuilderContextBase context;

			private ICrossDocumentUpdateContext originalCrossDocumentUpdateContext;

			public ChangeCrossDocumentUpdateContextToken(InstanceBuilderContextBase context, ICrossDocumentUpdateContext crossDocumentUpdateContext)
			{
				this.context = context;
				this.originalCrossDocumentUpdateContext = this.context.crossDocumentUpdateContext;
				this.context.crossDocumentUpdateContext = crossDocumentUpdateContext;
			}

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.crossDocumentUpdateContext = this.originalCrossDocumentUpdateContext;
					this.context = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private class ForceAllowIncrementalTemplateUpdatesToken : IDisposable
		{
			private InstanceBuilderContextBase context;

			private bool oldAllow;

			public ForceAllowIncrementalTemplateUpdatesToken(InstanceBuilderContextBase context, bool allow)
			{
				this.context = context;
				this.oldAllow = this.context.AllowIncrementalTemplateUpdates;
				this.context.AllowIncrementalTemplateUpdates = allow;
			}

			public void Dispose()
			{
				this.context.AllowIncrementalTemplateUpdates = this.oldAllow;
			}
		}

		private class ForcePostponedResourceEvaluationToken : IDisposable
		{
			private InstanceBuilderContextBase context;

			private bool oldAllow;

			public ForcePostponedResourceEvaluationToken(InstanceBuilderContextBase context)
			{
				this.context = context;
				this.oldAllow = this.context.AllowPostponingResourceEvaluation;
				this.context.AllowPostponingResourceEvaluation = false;
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.context.AllowPostponingResourceEvaluation = this.oldAllow;
					this.context = null;
				}
			}

			void System.IDisposable.Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		private class ForceRegistrationOfInstantiatedElementsToken : IDisposable
		{
			private bool shouldRegisterInstantiatedElements;

			private InstanceBuilderContextBase context;

			public ForceRegistrationOfInstantiatedElementsToken(InstanceBuilderContextBase context, bool shouldRegisterInstantiatedElements)
			{
				this.context = context;
				this.shouldRegisterInstantiatedElements = context.shouldRegisterInstantiatedElements;
				this.context.ForceRegistrationOfInstantiatedElementsInternal(shouldRegisterInstantiatedElements);
			}

			public void Dispose()
			{
				this.context.ForceRegistrationOfInstantiatedElementsInternal(this.shouldRegisterInstantiatedElements);
			}
		}

		private class ForceUseShadowPropertiesToken : IDisposable
		{
			private bool useShadowProperties;

			private InstanceBuilderContextBase context;

			public ForceUseShadowPropertiesToken(InstanceBuilderContextBase context, bool useShadowProperties)
			{
				this.context = context;
				this.useShadowProperties = context.useShadowProperties;
				this.context.ForceUseShadowPropertiesInternal(useShadowProperties);
			}

			public void Dispose()
			{
				this.context.ForceUseShadowPropertiesInternal(this.useShadowProperties);
			}
		}

		private class InstantiatingUserControlPreviewToken : IDisposable
		{
			private string xamlSourcePath;

			private InstanceBuilderContextBase context;

			public InstantiatingUserControlPreviewToken(InstanceBuilderContextBase context, string xamlSourcePath)
			{
				this.context = context;
				this.xamlSourcePath = xamlSourcePath;
				context.currentlyInstantiatingUserControlPreviews.Add(xamlSourcePath);
			}

			public void Dispose()
			{
				this.context.currentlyInstantiatingUserControlPreviews.Remove(this.xamlSourcePath);
			}
		}

		private class SetSerializationContextToken : IDisposable
		{
			private ISerializationContext serializationContext;

			private InstanceBuilderContextBase context;

			public SetSerializationContextToken(InstanceBuilderContextBase context, ISerializationContext serializationContext)
			{
				this.context = context;
				this.serializationContext = context.serializationContext;
				context.ChangeSerializationContextInternal(serializationContext);
			}

			public void Dispose()
			{
				this.context.ChangeSerializationContextInternal(this.serializationContext);
			}
		}
	}
}