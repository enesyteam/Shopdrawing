using System;
using System.Collections;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface IProjectTypeCollection : ICollection, IEnumerable
	{
		IProjectType this[string name]
		{
			get;
		}

		IProjectType this[int index]
		{
			get;
		}

		void Add(IProjectType value);

		bool Contains(IProjectType value);

		void Remove(IProjectType value);
	}
}