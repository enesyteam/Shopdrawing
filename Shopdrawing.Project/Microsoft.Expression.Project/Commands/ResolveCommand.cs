using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal class ResolveCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandResolveName;
			}
		}

		public ResolveCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void InternalExectute()
		{
			if (!this.SaveSolution(true))
			{
				return;
			}
			base.Services.SourceControlProvider().ResolveConflicts((
				from item in base.GetFileItemAndDescendants(this.Solution())
				select item.DocumentReference.Path).ToArray<string>());
		}
	}
}