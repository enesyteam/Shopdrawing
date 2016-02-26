using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class VisualStateGroupMetadata : ClrObjectMetadata
	{
		public readonly static IPropertyId StatesProperty;

		public readonly static IPropertyId TransitionsProperty;

		public readonly static IPropertyId NameProperty;

		static VisualStateGroupMetadata()
		{
			VisualStateGroupMetadata.StatesProperty = (IPropertyId)ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "States", MemberAccessTypes.Public);
			VisualStateGroupMetadata.TransitionsProperty = (IPropertyId)ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "Transitions", MemberAccessTypes.Public);
			VisualStateGroupMetadata.NameProperty = (IPropertyId)ProjectNeutralTypes.VisualStateGroup.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
		}

		public VisualStateGroupMetadata(Type baseType) : base(baseType)
		{
		}
	}
}