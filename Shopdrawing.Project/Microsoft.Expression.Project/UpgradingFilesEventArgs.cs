using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal sealed class UpgradingFilesEventArgs : EventArgs
	{
		public List<DocumentReference> Files
		{
			get;
			private set;
		}

		private UpgradingFilesEventArgs()
		{
		}

		public UpgradingFilesEventArgs(List<DocumentReference> filesToBeUpgraded)
		{
			this.Files = filesToBeUpgraded;
		}
	}
}