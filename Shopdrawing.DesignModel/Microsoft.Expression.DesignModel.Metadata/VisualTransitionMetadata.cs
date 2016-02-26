using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class VisualTransitionMetadata : ClrObjectMetadata
	{
		public readonly static IPropertyId FromStateNameProperty;

		public readonly static IPropertyId ToStateNameProperty;

		public readonly static IPropertyId GeneratedDurationProperty;

		public readonly static IPropertyId StoryboardProperty;

		static VisualTransitionMetadata()
		{
			VisualTransitionMetadata.FromStateNameProperty = (IPropertyId)ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "From", MemberAccessTypes.Public);
			VisualTransitionMetadata.ToStateNameProperty = (IPropertyId)ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "To", MemberAccessTypes.Public);
			VisualTransitionMetadata.GeneratedDurationProperty = (IPropertyId)ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "GeneratedDuration", MemberAccessTypes.Public);
			VisualTransitionMetadata.StoryboardProperty = (IPropertyId)ProjectNeutralTypes.VisualTransition.GetMember(MemberType.LocalProperty, "Storyboard", MemberAccessTypes.Public);
		}

		public VisualTransitionMetadata(Type baseType) : base(baseType)
		{
		}
	}
}