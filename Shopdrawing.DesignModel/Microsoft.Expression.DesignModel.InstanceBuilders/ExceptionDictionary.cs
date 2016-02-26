using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class ExceptionDictionary : PersistentViewNodeDictionary<Exception>, IExceptionDictionary, IEnumerable<KeyValuePair<DocumentNode, Exception>>, IEnumerable
	{
		public ExceptionDictionary(ViewNodeManager viewNodeManager) : base(viewNodeManager)
		{
		}

		public Exception GetException(ViewNode viewNode)
		{
			PersistentData<Exception> data = base.GetData(viewNode);
			if (data == null)
			{
				return null;
			}
			return data.Data;
		}

		public DocumentNode GetExceptionSource(ViewNode viewNode)
		{
			PersistentData<Exception> data = base.GetData(viewNode);
			if (data == null)
			{
				return null;
			}
			return data.InnerSource;
		}

		public void SetException(ViewNode viewNode, DocumentNode exceptionTarget, Exception exception)
		{
			base.SetData(viewNode, exceptionTarget, exception);
		}
	}
}