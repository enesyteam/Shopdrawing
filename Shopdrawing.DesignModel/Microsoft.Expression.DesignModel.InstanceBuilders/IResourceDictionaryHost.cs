using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IResourceDictionaryHost
	{
		void BeginInstanceBuilding(IInstanceBuilderContext context);

		void EndInstanceBuilding();

		void OnResourceDictionaryInstantiated(IInstanceBuilderContext context, ViewNode viewNode);
	}
}