using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class MoveResult
	{
		public string Destination
		{
			get;
			set;
		}

		public bool MovedSuccessfully
		{
			get;
			set;
		}

		public string Source
		{
			get;
			set;
		}

		public MoveResult()
		{
		}
	}
}