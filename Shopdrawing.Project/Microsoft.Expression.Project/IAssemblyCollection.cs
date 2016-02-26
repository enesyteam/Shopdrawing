using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Expression.Project
{
	public interface IAssemblyCollection : ICollection<ProjectAssembly>, IEnumerable<ProjectAssembly>, IEnumerable, INotifyCollectionChanges, INotifyCollectionChanged
	{
		ProjectAssembly Find(string name);
	}
}