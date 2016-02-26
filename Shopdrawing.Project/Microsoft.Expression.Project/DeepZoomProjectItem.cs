using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	internal sealed class DeepZoomProjectItem : ProjectItem
	{
		public DeepZoomProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(project, documentReference, documentType, serviceProvider)
		{
		}
	}
}