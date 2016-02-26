using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class SaveAllCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandSaveAllName;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled || BuildManager.Building || this.Solution() == null)
				{
					return false;
				}
				return base.Services.OpenDocuments().Any<IDocument>((IDocument document) => document.IsDirty);
			}
		}

		public SaveAllCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => this.SaveSolution(false));
		}
	}
}