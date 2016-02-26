using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeModelService
	{
		bool ShouldSaveFiles
		{
			get;
		}

		ITypeDeclaration FindTypeInFile(object solution, object projectItem, IEnumerable<string> locations, string typeName);
	}
}