using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IInstanceDictionary : IEnumerable<KeyValuePair<object, ViewNode>>, IEnumerable
	{
		IDictionary<DocumentNode, object> DataSourceCache
		{
			get;
		}

		IList<ViewNode> InstantiatedElementRoots
		{
			get;
		}

		void Clear();

		ViewNode GetViewNode(object viewObject, bool allowCrossView);

		void OnInstanceChanged(ViewNode viewNode, object oldInstance, object newInstance);

		void OnViewNodeRemoved(ViewNode viewNode);
	}
}