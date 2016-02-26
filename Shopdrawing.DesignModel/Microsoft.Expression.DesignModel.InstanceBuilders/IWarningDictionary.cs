using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IWarningDictionary : IEnumerable<KeyValuePair<DocumentNode, string>>, IEnumerable
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

		string GetWarning(ViewNode viewNode);

		DocumentNode GetWarningSource(ViewNode viewNode);

		void Remove(ViewNode viewNode);

		void SetWarning(ViewNode viewNode, DocumentNode warningSource, string warning);
	}
}