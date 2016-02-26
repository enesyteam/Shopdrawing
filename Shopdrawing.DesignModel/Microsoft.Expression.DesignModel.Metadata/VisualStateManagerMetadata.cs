using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class VisualStateManagerMetadata : ClrObjectMetadata
	{
		public readonly static IPropertyId VisualStateGroupsProperty;

		public readonly static IPropertyId CustomVisualStateManagerProperty;

		static VisualStateManagerMetadata()
		{
			VisualStateManagerMetadata.VisualStateGroupsProperty = (IPropertyId)ProjectNeutralTypes.VisualStateManager.GetMember(MemberType.AttachedProperty, "VisualStateGroups", MemberAccessTypes.Public);
			VisualStateManagerMetadata.CustomVisualStateManagerProperty = (IPropertyId)ProjectNeutralTypes.VisualStateManager.GetMember(MemberType.AttachedProperty, "CustomVisualStateManager", MemberAccessTypes.Public);
		}

		public VisualStateManagerMetadata(Type baseType) : base(baseType)
		{
		}
	}
}