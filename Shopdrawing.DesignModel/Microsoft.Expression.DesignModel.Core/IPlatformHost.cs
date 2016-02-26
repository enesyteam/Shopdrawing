using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IPlatformHost
	{
		IDocumentContext CreateDefaultContext(IDocumentLocator documentLocator);

		IDocumentContext CreateSystemThemeContext(IDocumentLocator documentLocator);

		void SetPlatform(IPlatform platform);
	}
}