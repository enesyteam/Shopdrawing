using Microsoft.Expression.Framework.Documents;
using System;
using System.Diagnostics;

namespace Microsoft.Expression.Project
{
	public interface IOutOfBrowserDeploymentService
	{
		ProcessStartInfo TryPerformOutOfBrowserDeployment(IProject startupProject, DocumentReference serverRoot, Uri baseWebServerUri, out bool shouldStartServer);
	}
}