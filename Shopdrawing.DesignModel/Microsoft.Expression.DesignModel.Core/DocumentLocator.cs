using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public class DocumentLocator : IDocumentLocator
	{
		private string path;

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		public DocumentLocator(string path)
		{
			this.path = path;
		}
	}
}