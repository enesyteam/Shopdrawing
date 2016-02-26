using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class WarningDictionary : PersistentViewNodeDictionary<string>, IWarningDictionary, IEnumerable<KeyValuePair<DocumentNode, string>>, IEnumerable
	{
		public WarningDictionary(ViewNodeManager viewNodeManager) : base(viewNodeManager)
		{
		}

		public string GetWarning(ViewNode viewNode)
		{
			PersistentData<string> data = base.GetData(viewNode);
			if (data == null)
			{
				return null;
			}
			return data.Data;
		}

		public DocumentNode GetWarningSource(ViewNode viewNode)
		{
			PersistentData<string> data = base.GetData(viewNode);
			if (data == null)
			{
				return null;
			}
			return data.InnerSource;
		}

		public void SetWarning(ViewNode viewNode, DocumentNode warningSource, string warning)
		{
			base.SetData(viewNode, warningSource, warning);
		}
	}
}