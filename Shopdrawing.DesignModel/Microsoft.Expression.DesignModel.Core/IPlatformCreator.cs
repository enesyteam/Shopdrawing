using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IPlatformCreator
	{
		IPlatform CreatePlatform(IPlatformRuntimeContext platformRuntimeContext, IPlatformReferenceContext platformReferenceContext, IPlatformService platformService, IPlatformHost platformHost);

		object GetProperty(string propertyName);

		void Initialize();

		void RegisterAssembly(Assembly assembly);

		void ReleasePlatform(IPlatform platform);

		void SetProperty(string propertyName, object propertyValue);

		void Shutdown();

		event EventHandler<PlatformEventArgs> PlatformCreated;

		event EventHandler<PlatformEventArgs> PlatformDisposing;
	}
}