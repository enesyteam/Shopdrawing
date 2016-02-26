using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Conversion
{
	internal class ConversionTarget
	{
		internal Microsoft.Expression.Framework.Documents.DocumentReference DocumentReference
		{
			get
			{
				if (this.IsSolution)
				{
					return this.Solution.DocumentReference;
				}
				return this.ProjectStore.DocumentReference;
			}
		}

		internal bool IsProject
		{
			get
			{
				return this.ProjectStore != null;
			}
		}

		internal bool IsSolution
		{
			get
			{
				return this.Solution != null;
			}
		}

		internal IProjectStore ProjectStore
		{
			get;
			private set;
		}

		internal ISolutionManagement Solution
		{
			get;
			private set;
		}

		internal ConversionTarget(ISolutionManagement solution)
		{
			this.Solution = solution;
		}

		internal ConversionTarget(IProjectStore projectStore)
		{
			this.ProjectStore = projectStore;
		}
	}
}