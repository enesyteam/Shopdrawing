using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectActionContext
	{
		IServiceProvider ServiceProvider
		{
			get;
		}

		bool CanOverwrite(DocumentReference documentReference);

		bool HandleException(DocumentReference documentReference, Exception exception);

		bool IsSourceControlled(DocumentReference documentReference);
	}
}