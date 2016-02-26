using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IDesignTimeResourceResolver
	{
		void Enqueue(Action resolutionAction);

		bool Resolve(IDocumentContext documentContext, string missingResourceName);
	}
}