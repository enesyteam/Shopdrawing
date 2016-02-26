using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyPropertyInfo : PropertyInfo
	{
		private readonly MetadataOnlyModule m_resolver;

		private readonly Token m_PropertyToken;

		private readonly PropertyAttributes m_attrib;

		private readonly Token m_declaringClassToken;

		private readonly Type m_propertyType;

		private readonly GenericContext m_context;

		private readonly string m_name;

		private readonly Token m_setterToken;

		private readonly Token m_getterToken;

		public override PropertyAttributes Attributes
		{
			get
			{
				return this.m_attrib;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !this.m_getterToken.IsNil;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.m_setterToken.IsNil;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.m_resolver.GetGenericType(this.m_declaringClassToken, this.m_context);
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Property;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_PropertyToken;
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.m_propertyType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public MetadataOnlyPropertyInfo(MetadataOnlyModule resolver, Token propToken, Type[] typeArgs, Type[] methodArgs)
		{
			int num;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num1;
			int num2;
			UnusedIntPtr unusedIntPtr;
			int num3;
			PropertyAttributes propertyAttribute;
			Token token;
			Token token1;
			Token token2;
			uint num4;
			this.m_resolver = resolver;
			this.m_PropertyToken = propToken;
			this.m_context = new GenericContext(typeArgs, methodArgs);
			IMetadataImport rawImport = this.m_resolver.RawImport;
			StringBuilder stringBuilder = new StringBuilder(256);
			rawImport.GetPropertyProps(this.m_PropertyToken, out this.m_declaringClassToken, null, 0, out num, out propertyAttribute, out embeddedBlobPointer, out num1, out num2, out unusedIntPtr, out num3, out token, out token1, out token2, 1, out num4);
			this.m_attrib = propertyAttribute;
			stringBuilder.Capacity = num;
			rawImport.GetPropertyProps(this.m_PropertyToken, out this.m_declaringClassToken, stringBuilder, num, out num, out propertyAttribute, out embeddedBlobPointer, out num1, out num2, out unusedIntPtr, out num3, out token, out token1, out token2, 1, out num4);
			this.m_name = stringBuilder.ToString();
			byte[] numArray = this.m_resolver.ReadEmbeddedBlob(embeddedBlobPointer, num1);
			int num5 = 0;
			SignatureUtil.ExtractCallingConvention(numArray, ref num5);
			SignatureUtil.ExtractInt(numArray, ref num5);
			this.m_propertyType = SignatureUtil.ExtractType(numArray, ref num5, this.m_resolver, this.m_context);
			this.m_setterToken = token;
			this.m_getterToken = token1;
		}

		public override bool Equals(object obj)
		{
			MetadataOnlyPropertyInfo metadataOnlyPropertyInfo = obj as MetadataOnlyPropertyInfo;
			if (metadataOnlyPropertyInfo == null)
			{
				return false;
			}
			if (!metadataOnlyPropertyInfo.m_resolver.Equals(this.m_resolver) || !metadataOnlyPropertyInfo.m_PropertyToken.Equals(this.m_PropertyToken))
			{
				return false;
			}
			return this.DeclaringType.Equals(metadataOnlyPropertyInfo.DeclaringType);
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			List<MethodInfo> methodInfos = new List<MethodInfo>();
			MethodInfo getMethod = this.GetGetMethod(nonPublic);
			if (getMethod != null)
			{
				methodInfos.Add(getMethod);
			}
			MethodInfo setMethod = this.GetSetMethod(nonPublic);
			if (setMethod != null)
			{
				methodInfos.Add(setMethod);
			}
			return methodInfos.ToArray();
		}

		public override object GetConstantValue()
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.m_resolver.GetCustomAttributeData(this.MetadataToken);
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			if (this.m_getterToken.IsNil)
			{
				return null;
			}
			MethodInfo genericMethodInfo = this.m_resolver.GetGenericMethodInfo(this.m_getterToken, this.m_context);
			if (!nonPublic && !genericMethodInfo.IsPublic)
			{
				return null;
			}
			return genericMethodInfo;
		}

		public override int GetHashCode()
		{
			Token mPropertyToken = this.m_PropertyToken;
			return this.m_resolver.GetHashCode() * 32767 + mPropertyToken.GetHashCode();
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			MethodInfo getMethod = this.GetGetMethod(true);
			if (getMethod != null)
			{
				return getMethod.GetParameters();
			}
			return new ParameterInfo[0];
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			if (this.m_setterToken.IsNil)
			{
				return null;
			}
			MethodInfo genericMethodInfo = this.m_resolver.GetGenericMethodInfo(this.m_setterToken, this.m_context);
			if (!nonPublic && !genericMethodInfo.IsPublic)
			{
				return null;
			}
			return genericMethodInfo;
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return string.Concat(this.DeclaringType.ToString(), ".", this.Name);
		}
	}
}