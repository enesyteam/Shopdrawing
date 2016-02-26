using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class EditExternallyCommand : ItemCollectionCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandEditExternallyName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable || this.Solution() is WebProjectSolution)
				{
					return false;
				}
				return !(this.Selection().SingleOrNull<IDocumentItem>() is INamedProject);
			}
		}

		public EditExternallyCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private void EditItemExternally(IDocumentItem item)
		{
			bool flag;
			string path = item.DocumentReference.Path;
			if (PathHelper.FileExists(path))
			{
				if (item is INamedProject || item is VisualStudioSolution)
				{
					flag = true;
				}
				else
				{
					IProjectItem projectItem = item as IProjectItem;
					if (projectItem == null)
					{
						flag = false;
					}
					else
					{
						switch (projectItem.DocumentType.PreferredExternalEditCommand)
						{
							case PreferredExternalEditCommand.None:
							{
								return;
							}
							case PreferredExternalEditCommand.ShellEdit:
							{
								flag = false;
								goto Label0;
							}
							case PreferredExternalEditCommand.ShellOpen:
							{
								flag = true;
								goto Label0;
							}
						}
						throw new InvalidEnumArgumentException("item", (int)projectItem.DocumentType.PreferredExternalEditCommand, typeof(PreferredExternalEditCommand));
					}
				}
			Label0:
				ProcessStartInfo processStartInfo = new ProcessStartInfo(path)
				{
					UseShellExecute = true,
					Verb = (flag ? "Open" : "Edit")
				};
				try
				{
					Process.Start(processStartInfo);
				}
				catch (Win32Exception win32Exception)
				{
					if (win32Exception.NativeErrorCode != 1155)
					{
						throw;
					}
					else
					{
						processStartInfo.Verb = (flag ? "Edit" : "Open");
						Process.Start(processStartInfo);
					}
				}
			}
		}

		protected override void Execute(IDocumentItem item)
		{
			this.HandleBasicExceptions(() => this.InternalExecute(item));
		}

		private void InternalExecute(IDocumentItem item)
		{
			try
			{
				this.EditItemExternally(item);
			}
			catch (Win32Exception win32Exception)
			{
				this.DisplayCommandFailedExceptionMessage(win32Exception);
			}
		}

		protected override bool ShouldAddItem(IDocumentItem item)
		{
			IProjectItem projectItem = item as IProjectItem;
			if (!PathHelper.FileExists(item.DocumentReference.Path))
			{
				return false;
			}
			if (projectItem == null)
			{
				return true;
			}
			return projectItem.DocumentType.PreferredExternalEditCommand != PreferredExternalEditCommand.None;
		}
	}
}