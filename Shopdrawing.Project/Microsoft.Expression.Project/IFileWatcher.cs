using System;

namespace Microsoft.Expression.Project
{
	internal interface IFileWatcher
	{
		void Deactivate();

		void Reactivate();
	}
}