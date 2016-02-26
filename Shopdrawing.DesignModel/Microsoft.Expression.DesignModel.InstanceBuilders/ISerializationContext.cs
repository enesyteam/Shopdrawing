using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface ISerializationContext
	{
		object Owner
		{
			get;
			set;
		}

		Guid RegistryToken
		{
			get;
		}

		Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager ViewNodeManager
		{
			get;
		}

		void AddPostponedReference(ViewNode source);

		ViewNodeId GetId(ViewNode viewNode);

		IEnumerable<IProperty> GetPostponedReferences(ViewNode parent);

		ViewNode GetViewNode(ViewNodeId id);

		bool IsValid(ViewNodeId id);
	}
}