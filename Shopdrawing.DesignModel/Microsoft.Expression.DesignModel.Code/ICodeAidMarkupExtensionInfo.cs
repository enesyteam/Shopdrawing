using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeAidMarkupExtensionInfo : ICodeAidMemberInfo
	{
		IEnumerable<ICodeAidMemberInfo> Properties
		{
			get;
		}
	}
}