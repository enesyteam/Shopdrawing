using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class IndexedClrPropertyReferenceStep : ClrPropertyReferenceStep
	{
		public const char IndexStartChar = '[';

		public const char IndexEndChar = ']';

		private int index;

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat('[', this.index.ToString(CultureInfo.InvariantCulture), ']');
			}
		}

		private IndexedClrPropertyReferenceStep(IType declaringType, string memberName, IType valueType, ClrPropertyImplementationBase implementation, int index) : base(declaringType, memberName, valueType, implementation, index)
		{
			this.index = index;
		}

		protected override object[] GetIndexParameters()
		{
			return new object[] { this.index };
		}

		public static IndexedClrPropertyReferenceStep GetReferenceStep(IPlatformMetadata platformMetadata, ITypeId declaringTypeId, int index)
		{
			return IndexedClrPropertyReferenceStep.GetReferenceStep(((PlatformTypes)platformMetadata).DefaultTypeResolver, declaringTypeId, index);
		}

		public static IndexedClrPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, ITypeId declaringTypeId, int index)
		{
			IType type = typeResolver.ResolveType(declaringTypeId);
			return IndexedClrPropertyReferenceStep.GetReferenceStep(typeResolver, type.RuntimeType, index, true);
		}

		public static IndexedClrPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type declaringType, int index)
		{
			return IndexedClrPropertyReferenceStep.GetReferenceStep(typeResolver, declaringType, index, true);
		}

		public static IndexedClrPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type declaringType, int index, bool throwOnFailure)
		{
			Type[] typeArray = new Type[] { typeof(int) };
			string str = "Item";
			System.Reflection.PropertyInfo property = declaringType.GetProperty(str, BindingFlags.Instance | BindingFlags.Public, null, null, typeArray, null);
			if (property == null)
			{
				if (typeof(IList).IsAssignableFrom(declaringType))
				{
					property = typeof(IList).GetProperty(str, BindingFlags.Instance | BindingFlags.Public, null, null, typeArray, null);
				}
				if (property == null)
				{
					if (throwOnFailure)
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string typeDoesNotDeclareAnIndexer = ExceptionStringTable.TypeDoesNotDeclareAnIndexer;
						object[] name = new object[] { declaringType.Name };
						throw new ArgumentException(string.Format(currentCulture, typeDoesNotDeclareAnIndexer, name), "declaringType");
					}
					return null;
				}
			}
			IType type = typeResolver.GetType(declaringType);
			IType propertyTypeId = PlatformTypeHelper.GetPropertyTypeId(typeResolver, property);
			IPlatformMetadata platformMetadata = typeResolver.PlatformMetadata;
			object[] objArray = new object[] { index };
			ClrPropertyImplementationBase localClrPropertyImplementation = new LocalClrPropertyImplementation(platformMetadata, property, null, objArray);
			return new IndexedClrPropertyReferenceStep(type, str, propertyTypeId, localClrPropertyImplementation, index);
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] name = new object[] { base.DeclaringType.Name, this.Name };
			return string.Format(invariantCulture, "{0}{1}", name);
		}
	}
}