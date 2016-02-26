using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public enum InstanceStateType
	{
		Valid,
		Invalid,
		Uninitialized,
		PropertyOrChildInvalid,
		DescendantInvalid,
		ChildAndDescendantInvalid
	}
}