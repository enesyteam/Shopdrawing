using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ITypeDeclaration
	{
		bool AddMethod(Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters);
	}
}