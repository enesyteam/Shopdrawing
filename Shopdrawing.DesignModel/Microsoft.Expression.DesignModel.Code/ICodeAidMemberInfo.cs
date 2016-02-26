using System;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeAidMemberInfo
	{
		string DescriptionSubtitle
		{
			get;
		}

		string DescriptionText
		{
			get;
		}

		string DescriptionTitle
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}