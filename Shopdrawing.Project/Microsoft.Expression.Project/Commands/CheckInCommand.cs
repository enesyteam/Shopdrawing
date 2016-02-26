using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal class CheckInCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandCheckInName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
				if (!base.IsAvailable || documentItem == null)
				{
					return false;
				}
				if (base.IsDirectoryBasedProjectOrFolder(documentItem))
				{
					return true;
				}
				return base.GetFileItemAndDescendants(documentItem).Any<IDocumentItem>(new Func<IDocumentItem, bool>(this.FileHasPendingChange));
			}
		}

		public CheckInCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void InternalExectute()
		{
			IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
			if (documentItem != null)
			{
				if (!this.SaveSolution(true))
				{
					return;
				}
				string[] array = (
					from item in base.GetFileItemAndDescendants(documentItem)
					where base.FileHasPendingChange(item)
					select item.DocumentReference.Path).ToArray<string>();
				base.SourceControlProvider.CheckIn(array);
				SourceControlStatusCache.ClearStatusCache();
				SourceControlStatusCache.UpdateStatus(base.GetFileItemAndDescendants(this.Solution()), base.SourceControlProvider);
			}
		}
	}
}