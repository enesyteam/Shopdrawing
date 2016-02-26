using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal interface IMutableMembers
	{
		void AddMember(IMember memberId);

		IMember GetMember(MemberType memberTypes, string uniqueName);
	}
}