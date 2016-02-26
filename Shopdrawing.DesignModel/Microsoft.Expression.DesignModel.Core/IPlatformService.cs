using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IPlatformService
	{
		IEnumerable<string> RegisteredPlatformCreators
		{
			get;
		}

		IPlatformCreator GetPlatformCreator(string identifier);

		object GetProperty(string platform, string propertyName);

		void RegisterPlatformCreator(string identifier, IPlatformCreator platformCreator);

		void RegisterPlatformCreator(string identifier, PlatformCreatorCallback callback);

		void SetProperty(string platform, string propertyName, object propertyValue);

		void UnregisterPlatformCreator(string identifier);

		event EventHandler<PlatformEventArgs> PlatformCreated;

		event EventHandler<PlatformEventArgs> PlatformDisposing;
	}
}