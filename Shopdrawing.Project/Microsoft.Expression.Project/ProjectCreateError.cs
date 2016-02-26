using System;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectCreateError : IProjectCreateError
	{
		private string identifier;

		private string message;

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public ProjectCreateError(string identifier, string message)
		{
			this.identifier = identifier;
			this.message = message;
		}
	}
}