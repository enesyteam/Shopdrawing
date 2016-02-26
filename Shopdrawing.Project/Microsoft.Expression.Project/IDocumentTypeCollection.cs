using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface IDocumentTypeCollection : IEnumerable<IDocumentType>, IEnumerable
	{
		int Count
		{
			get;
		}

		IDocumentType this[string name]
		{
			get;
		}

		IDocumentType UnknownDocumentType
		{
			get;
		}
	}
}