using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class InstantiatedElementViewNode : ViewNode, IInstantiatedElementViewNode
	{
		private InstantiatedElementList instantiatedElements;

		public InstantiatedElementList InstantiatedElements
		{
			get
			{
				return this.instantiatedElements;
			}
		}

		public InstantiatedElementViewNode(Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode) : base(manager, documentNode)
		{
			this.instantiatedElements = new InstantiatedElementList();
		}

		public InstantiatedElementViewNode(Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode, Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState instanceState, object instance) : base(manager, documentNode, instanceState, instance)
		{
			this.instantiatedElements = new InstantiatedElementList();
		}
	}
}