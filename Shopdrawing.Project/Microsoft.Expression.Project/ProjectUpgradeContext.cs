using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	internal sealed class ProjectUpgradeContext : IProjectActionContext
	{
		private IServiceProvider serviceProvider;

		public IServiceProvider ServiceProvider
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public ProjectUpgradeContext(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public bool CanOverwrite(DocumentReference documentReference)
		{
			return true;
		}

		public bool HandleException(DocumentReference documentReference, Exception e)
		{
			return true;
		}

		public bool IsSourceControlled(DocumentReference documentReference)
		{
			return true;
		}
	}
}