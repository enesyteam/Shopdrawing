using System;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeAidPropertyInfo : ICodeAidMemberInfo
	{
		ICodeAidTypeInfo PropertyType
		{
			get;
		}
	}
}