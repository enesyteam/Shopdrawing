using Microsoft.Expression.Framework.Documents;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal sealed class InvalidProjectStore : ProjectStoreBase
	{
		internal string InvalidStateDescription
		{
			get;
			private set;
		}

		private InvalidProjectStore(Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(documentReference)
		{
		}

		internal static IProjectStore CreateInstance(Microsoft.Expression.Framework.Documents.DocumentReference documentReference, string invalidStateDescription, IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			return new InvalidProjectStore(documentReference)
			{
				InvalidStateDescription = invalidStateDescription
			};
		}
	}
}