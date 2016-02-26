using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	public delegate IProjectStore ProjectCreator(DocumentReference documentReference, IServiceProvider serviceProvider);
}