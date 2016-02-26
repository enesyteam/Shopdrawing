using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Code
{
	[CLSCompliant(true)]
	public interface ICodeAidTypeInfo : ICodeAidMemberInfo
	{
		IEnumerable<ICodeAidMemberInfo> AllAttachedProperties
		{
			get;
		}

		ICodeAidTypeInfo CollectionItemType
		{
			get;
		}

		ICodeAidTypeInfo DefaultContentPropertyType
		{
			get;
		}

		IEnumerable<ICodeAidMemberInfo> EnumerationValues
		{
			get;
		}

		IEnumerable<ICodeAidMemberInfo> Events
		{
			get;
		}

		bool IsDictionaryType
		{
			get;
		}

		IEnumerable<ICodeAidMemberInfo> Properties
		{
			get;
		}

		IEnumerable<ICodeAidMemberInfo> FilteredAttachedProperties(ICodeAidTypeInfo parentTypeInfo, IEnumerable<ICodeAidTypeInfo> ancestorTypeInfos);

		bool IsAssignableFrom(ICodeAidTypeInfo type);
	}
}