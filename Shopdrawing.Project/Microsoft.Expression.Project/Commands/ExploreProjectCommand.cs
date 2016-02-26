using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class ExploreProjectCommand : ProjectCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandOpenFolderName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				IDocumentItem documentItem = this.Selection().SingleOrNull<IDocumentItem>();
				if (documentItem == null)
				{
					return false;
				}
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(documentItem.DocumentReference.Path))
				{
					return true;
				}
				return Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(documentItem.DocumentReference.Path);
			}
		}

		public ExploreProjectCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				string path = this.Selection().First<IDocumentItem>().DocumentReference.Path;
				if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path))
				{
					path = Path.GetDirectoryName(path);
				}
				try
				{
					Process.Start(path);
				}
				catch (Win32Exception win32Exception)
				{
					this.DisplayCommandFailedExceptionMessage(win32Exception);
				}
			});
		}
	}
}