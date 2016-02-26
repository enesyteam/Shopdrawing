using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;

namespace Microsoft.Expression.Project.Commands
{
	internal class UndoPendingChangesCommand : SourceControlCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandUndoPendingChangesName;
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

		public UndoPendingChangesCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override void InternalExectute()
		{
			Dictionary<string, string> strs;
			IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
			if (documentItem != null)
			{
				if (!this.SaveSolution(true))
				{
					return;
				}
				using (IDisposable disposable = this.SuspendWatchers())
				{
					List<IDocumentItem> list = base.GetFileItemAndDescendants(documentItem).Where<IDocumentItem>(new Func<IDocumentItem, bool>(this.FileHasPendingChange)).ToList<IDocumentItem>();
					string[] array = (
						from item in list
						select item.DocumentReference.Path).ToArray<string>();
					base.SourceControlProvider.RevertChange(array, out strs);
					foreach (string key in strs.Keys)
					{
						DocumentReference documentReference = DocumentReference.Create(key);
						if (!documentReference.IsValidPathFormat)
						{
							continue;
						}
						string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(documentReference);
						if (directoryNameOrRoot == null || !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryNameOrRoot))
						{
							continue;
						}
						try
						{
							if (!Directory.EnumerateFiles(directoryNameOrRoot, "*.*", SearchOption.AllDirectories).Any<string>())
							{
								Directory.Delete(directoryNameOrRoot);
							}
						}
						catch (IOException oException)
						{
						}
						catch (SecurityException securityException)
						{
						}
					}
				}
				SourceControlStatusCache.UpdateStatus(this.Solution().Descendants.AppendItem<IDocumentItem>(this.Solution()), base.SourceControlProvider);
			}
		}
	}
}