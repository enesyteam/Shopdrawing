using System;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface IParameterDeclaration
	{
		string Name
		{
			get;
		}

		Type ParameterType
		{
			get;
		}
	}
}