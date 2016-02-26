using System;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface IPropertiesCollection
	{
		string this[string propertyKey]
		{
			get;
			set;
		}
	}
}