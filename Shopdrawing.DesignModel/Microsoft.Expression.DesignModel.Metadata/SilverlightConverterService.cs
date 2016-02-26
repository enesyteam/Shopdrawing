using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class SilverlightConverterService
	{
		public bool IsSilverlightProject
		{
			get;
			private set;
		}

		public SilverlightConverterService(bool isSilverlightProject)
		{
			this.IsSilverlightProject = isSilverlightProject;
		}
	}
}