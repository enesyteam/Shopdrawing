using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal class AddToSourceControlCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddToSourceControl;
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
				return base.GetFileItemAndDescendants(documentItem).Any<IDocumentItem>(new Func<IDocumentItem, bool>(this.FileIsNotUnderSourceControl));
			}
		}

		public AddToSourceControlCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected bool FileIsNotUnderSourceControl(IDocumentItem item)
		{
			return SourceControlStatusCache.GetCachedStatus(item) == SourceControlStatus.None;
		}

		protected override void InternalExectute()
		{
			IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
			if (documentItem != null)
			{
				IEnumerable<IDocumentItem> array = (
					from item in base.GetFileItemAndDescendants(documentItem)
					where this.FileIsNotUnderSourceControl(item)
					select item).ToArray<IDocumentItem>();
				if (base.SourceControlProvider.Add((
					from item in array
					select item.DocumentReference.Path).ToArray<string>()) == SourceControlSuccess.Success)
				{
					SourceControlStatusCache.SetCachedStatus(array, SourceControlStatus.Add);
					return;
				}
				SourceControlStatusCache.UpdateStatus(array, base.SourceControlProvider);
			}
		}
	}
}