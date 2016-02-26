using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class ApplicationMetadata : ClrObjectMetadata
	{
		public readonly static IPropertyId ResourcesProperty;

		public readonly static IPropertyId StartupUriProperty;

		protected override IPropertyId ResourcesPropertyInternal
		{
			get
			{
				return ApplicationMetadata.ResourcesProperty;
			}
		}

		static ApplicationMetadata()
		{
			ApplicationMetadata.ResourcesProperty = (IPropertyId)PlatformTypes.Application.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
			ApplicationMetadata.StartupUriProperty = (IPropertyId)PlatformTypes.Application.GetMember(MemberType.LocalProperty, "StartupUri", MemberAccessTypes.Public);
		}

		public ApplicationMetadata(Type baseType) : base(baseType)
		{
		}
	}
}