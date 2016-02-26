using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	[Serializable]
	public class FrameworkNameDictionary<T> : Dictionary<FrameworkName, T>
	{
		public FrameworkNameDictionary() : base(FrameworkNameEqualityComparer.Instance)
		{
		}

		protected FrameworkNameDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}