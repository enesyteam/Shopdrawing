using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class VisualStateMetadata : ClrObjectMetadata
	{
		public readonly static IPropertyId StoryboardProperty;

		public readonly static IPropertyId NameProperty;

		static VisualStateMetadata()
		{
			VisualStateMetadata.StoryboardProperty = (IPropertyId)ProjectNeutralTypes.VisualState.GetMember(MemberType.LocalProperty, "Storyboard", MemberAccessTypes.Public);
			VisualStateMetadata.NameProperty = (IPropertyId)ProjectNeutralTypes.VisualState.GetMember(MemberType.LocalProperty, "Name", MemberAccessTypes.Public);
		}

		public VisualStateMetadata(Type baseType) : base(baseType)
		{
		}
	}
}