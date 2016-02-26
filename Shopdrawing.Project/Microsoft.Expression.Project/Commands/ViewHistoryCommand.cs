using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.Commands
{
	internal class ViewHistoryCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandViewHistoryName;
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
				return this.IsValidStatusForHistory(documentItem);
			}
		}

		public ViewHistoryCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void InternalExectute()
		{
			IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
			if (documentItem != null)
			{
				ISourceControlProvider sourceControlProvider = base.SourceControlProvider;
				string[] path = new string[] { documentItem.DocumentReference.Path };
				sourceControlProvider.History(path);
			}
		}

		private bool IsValidStatusForHistory(IDocumentItem documentItem)
		{
			SourceControlStatus cachedStatus = SourceControlStatusCache.GetCachedStatus(documentItem);
			if (cachedStatus == SourceControlStatus.None)
			{
				return false;
			}
			return cachedStatus != SourceControlStatus.Add;
		}
	}
}