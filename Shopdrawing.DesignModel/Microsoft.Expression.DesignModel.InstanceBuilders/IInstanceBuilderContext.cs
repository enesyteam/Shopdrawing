using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IInstanceBuilderContext : IDisposable
	{
		bool AllowIncrementalTemplateUpdates
		{
			get;
		}

		bool AllowPostponingResourceEvaluation
		{
			get;
		}

		DocumentNode AlternateSiteNode
		{
			get;
		}

		ViewNode ContainerRoot
		{
			get;
		}

		ICrossDocumentUpdateContext CrossDocumentUpdateContext
		{
			get;
		}

		ICollection<string> CurrentlyInstantiatingUserControlPreviews
		{
			get;
		}

		bool DisableProjectionTransforms
		{
			get;
			set;
		}

		IDocumentContext DocumentContext
		{
			get;
		}

		IDocumentRootResolver DocumentRootResolver
		{
			get;
		}

		IEffectManager EffectManager
		{
			get;
		}

		IExceptionDictionary ExceptionDictionary
		{
			get;
		}

		IInstanceBuilderFactory InstanceBuilderFactory
		{
			get;
		}

		IInstanceDictionary InstanceDictionary
		{
			get;
		}

		bool IsSerializationScope
		{
			get;
		}

		bool IsStandalone
		{
			get;
		}

		ITypeMetadataFactory MetadataFactory
		{
			get;
		}

		INameScope NameScope
		{
			get;
		}

		IViewPanel OverlayLayer
		{
			get;
		}

		IInstanceBuilderContext ParentContext
		{
			get;
		}

		IPlatform Platform
		{
			get;
		}

		IResourceDictionaryHost ResourceDictionaryHost
		{
			get;
		}

		InstanceTypeReplacement RootTargetTypeReplacement
		{
			get;
			set;
		}

		ISerializationContext SerializationContext
		{
			get;
		}

		bool ShouldRegisterInstantiatedElements
		{
			get;
		}

		ITextBufferService TextBufferService
		{
			get;
		}

		ICollection<ViewNode> UserControlInstances
		{
			get;
		}

		bool UseShadowProperties
		{
			get;
		}

		Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager ViewNodeManager
		{
			get;
		}

		IWarningDictionary WarningDictionary
		{
			get;
		}

		IDisposable ChangeContainerRoot(ViewNode newContainerRoot);

		IDisposable ChangeCrossDocumentUpdateContext(ICrossDocumentUpdateContext context);

		IDisposable ChangeResourceDictionaryHost(IResourceDictionaryHost host);

		IDisposable ChangeSerializationContext(ISerializationContext serializationContext);

		string ConvertToWpfFontName(string gdiFontName);

		IDocumentContext CreateErrorDocumentContext(IDocumentContext sourceDocumentContext);

		IInstanceBuilderContext CreateStandaloneChildContext(IDocumentContext documentContext);

		IDisposable DisablePostponedResourceEvaluation();

		object EvaluateSystemResource(object resourceKey);

		IDisposable ForceAllowIncrementalTemplateUpdates(bool allow);

		IDisposable ForceRegistrationOfInstantiatedElements(bool forceRegistration);

		IDisposable ForceUseShadowProperties(bool useShadowProperties);

		ICollection<IProperty> GetProperties(ViewNode viewNode);

		DocumentNode GetPropertyValue(ViewNode viewNode, IPropertyId propertyKey);

		IInstanceBuilderContext GetViewContext(IDocumentRoot documentRoot);

		bool HasFont(string fontFamilyName);

		string ResolveFont(string fontFamilySource, object fontStretch, object fontStyle, object fontWeight, IDocumentContext documentContext);

		IDisposable SetCurrentlyInstantiatingUserControlPreview(string xamlSourcePath);

		bool ShouldDisableVisualStateManagerFor(ViewNode viewNode);

		bool ShouldInstantiatePreviewControl(IDocumentRoot documentRoot);

		void UpdateTextEditProxyIfEditing();
	}
}