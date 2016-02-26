using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IExceptionDictionary : IEnumerable<KeyValuePair<DocumentNode, Exception>>, IEnumerable
	{
		int Count
		{
			get;
		}

		IEnumerable<DocumentNode> Keys
		{
			get;
		}

		void Clear();

		bool Contains(ViewNode viewNode);

		Exception GetException(ViewNode viewNode);

		DocumentNode GetExceptionSource(ViewNode viewNode);

		void Remove(ViewNode viewNode);

		void SetException(ViewNode viewNode, DocumentNode exceptionTarget, Exception exception);
	}
}