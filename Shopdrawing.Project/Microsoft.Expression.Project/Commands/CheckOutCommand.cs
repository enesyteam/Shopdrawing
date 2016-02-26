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
	internal class CheckOutCommand : SourceControlCommand
	{
		private bool recursive;

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandCheckOutName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
				if (!this.recursive)
				{
					if (!base.IsAvailable || documentItem == null)
					{
						return false;
					}
					return SourceControlStatusCache.GetCachedStatus(documentItem) == SourceControlStatus.CheckedIn;
				}
				if (!base.IsAvailable || documentItem == null)
				{
					return false;
				}
				return base.GetFileItemAndDescendants(documentItem).Any<IDocumentItem>((IDocumentItem item) => SourceControlStatusCache.GetCachedStatus(item) == SourceControlStatus.CheckedIn);
			}
		}

		public CheckOutCommand(bool recursive, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			this.recursive = recursive;
		}

		protected override void InternalExectute()
		{
			List<IDocumentItem> documentItems;
			IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
			if (documentItem != null)
			{
				documentItems = (!this.recursive ? new List<IDocumentItem>()
				{
					documentItem
				} : (
					from item in base.GetFileItemAndDescendants(documentItem)
					where SourceControlStatusCache.GetCachedStatus(item) == SourceControlStatus.CheckedIn
					select item).ToList<IDocumentItem>());
				string[] array = (
					from item in documentItems
					select item.DocumentReference.Path).ToArray<string>();
				if (base.SourceControlProvider.Checkout(array) == SourceControlSuccess.Success)
				{
					SourceControlStatusCache.SetCachedStatus(documentItems, SourceControlStatus.CheckedOut);
					return;
				}
				SourceControlStatusCache.UpdateStatus(documentItems, base.SourceControlProvider);
			}
		}
	}
}