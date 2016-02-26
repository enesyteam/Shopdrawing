using System;

namespace Microsoft.Expression.Extensibility.Project
{
	public interface IProjectCapabilityAdapter : IProjectAdapter
	{
		bool GetCapability<T>(IProject project, string name, out T value);
	}
}