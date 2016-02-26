using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.NodeBuilders;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IPlatform : IViewNodeManagerFactory, IDisposable
	{
		IDocumentNodeBuilderFactory DocumentNodeBuilderFactory
		{
			get;
		}

		IDocumentNodeChildBuilderFactory DocumentNodeChildBuilderFactory
		{
			get;
		}

		IDocumentNodePropertyBuilderFactory DocumentNodePropertyBuilderFactory
		{
			get;
		}

		IEffectManager EffectManager
		{
			get;
		}

		IPlatformGeometryHelper GeometryHelper
		{
			get;
		}

		IInstanceBuilderFactory InstanceBuilderFactory
		{
			get;
		}

		IPlatformTypes Metadata
		{
			get;
		}

		bool NeedsContextUpdate
		{
			get;
		}

		IPlatformHost PlatformHost
		{
			get;
			set;
		}

		Microsoft.Expression.DesignModel.Core.ThemeManager ThemeManager
		{
			get;
		}

		IViewResourceDictionary ThemeResources
		{
			get;
		}

		ViewContentManager ViewContentProvider
		{
			get;
		}

		IViewObjectFactory ViewObjectFactory
		{
			get;
		}

		IViewTextObjectFactory ViewTextObjectFactory
		{
			get;
		}

		void ActivatePlatformContext();

		IViewPanel CreateOverlayLayer();

		IPlatformSpecificView CreateSurface();

		string GetDeploymentInformation(Uri rootUri, out string deploymentLocation);

		void RefreshProjectSpecificMetadata(ITypeResolver typeResolver, ITypeMetadataFactory typeMetadataFactory);

		IViewVisual TryGetElementForException(Exception exception);
	}
}