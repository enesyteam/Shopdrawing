using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class UserControlViewNode : ViewNode
	{
		private IInstanceBuilderContext childContext;

		public override IInstanceBuilderContext ChildContext
		{
			get
			{
				return this.childContext;
			}
			set
			{
				if (this.childContext != value)
				{
					IDisposable disposable = this.childContext;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.childContext = value;
				}
			}
		}

		public UserControlViewNode(Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode) : base(manager, documentNode)
		{
		}

		public UserControlViewNode(Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeManager manager, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode, Microsoft.Expression.DesignModel.InstanceBuilders.InstanceState instanceState, object instance) : base(manager, documentNode, instanceState, instance)
		{
		}
	}
}