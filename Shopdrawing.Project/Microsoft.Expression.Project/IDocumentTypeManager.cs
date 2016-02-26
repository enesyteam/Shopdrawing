using System;

namespace Microsoft.Expression.Project
{
	public interface IDocumentTypeManager
	{
		IDocumentTypeCollection DocumentTypes
		{
			get;
		}

		void Register(IDocumentType documentType);

		void Unregister(IDocumentType documentType);
	}
}