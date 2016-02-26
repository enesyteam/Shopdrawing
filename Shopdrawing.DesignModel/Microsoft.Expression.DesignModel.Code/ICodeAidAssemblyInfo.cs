using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeAidAssemblyInfo : ICodeAidMemberInfo
	{
		string LongName
		{
			get;
		}

		IEnumerable<ICodeAidMemberInfo> Namespaces
		{
			get;
		}

		string ShortName
		{
			get;
		}
	}
}