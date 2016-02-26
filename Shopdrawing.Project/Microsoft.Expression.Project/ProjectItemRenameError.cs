using System;

namespace Microsoft.Expression.Project
{
	public enum ProjectItemRenameError
	{
		EmptyString,
		StartsWithPeriod,
		ContainsInvalidCharacters,
		IsReservedName,
		DuplicateName,
		Exception
	}
}