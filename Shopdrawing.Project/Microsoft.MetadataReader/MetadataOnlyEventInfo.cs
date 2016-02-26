using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyEventInfo : EventInfo
	{
		private MetadataOnlyModule m_resolver;

		private int m_eventToken;

		private EventAttributes m_attrib;

		private int m_declaringClassToken;

		private int m_eventHandlerTypeToken;

		private GenericContext m_context;

		private string m_name;

		private Token m_addMethodToken;

		private Token m_removeMethodToken;

		private Token m_raiseMethodToken;

		public override EventAttributes Attributes
		{
			get
			{
				return this.m_attrib;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				Type genericType = this.m_resolver.GetGenericType(new Token(this.m_declaringClassToken), this.m_context);
				return genericType;
			}
		}

		public override Type EventHandlerType
		{
			get
			{
				Type genericType = this.m_resolver.GetGenericType(new Token(this.m_eventHandlerTypeToken), this.m_context);
				return genericType;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Event;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_eventToken;
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

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public MetadataOnlyEventInfo(MetadataOnlyModule resolver, Token eventToken, Type[] typeArgs, Type[] methodArgs)
		{
			this.m_resolver = resolver;
			this.m_eventToken = eventToken;
			this.m_context = new GenericContext(typeArgs, methodArgs);
			this.GetInfo();
		}

		public override bool Equals(object obj)
		{
			MetadataOnlyEventInfo metadataOnlyEventInfo = obj as MetadataOnlyEventInfo;
			if (metadataOnlyEventInfo == null)
			{
				return false;
			}
			if (!metadataOnlyEventInfo.m_resolver.Equals(this.m_resolver) || !metadataOnlyEventInfo.m_eventToken.Equals(this.m_eventToken))
			{
				return false;
			}
			return this.DeclaringType.Equals(metadataOnlyEventInfo.DeclaringType);
		}

		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			if (this.m_addMethodToken.IsNil)
			{
				return null;
			}
			MethodInfo genericMethodInfo = this.m_resolver.GetGenericMethodInfo(this.m_addMethodToken, this.m_context);
			if (!nonPublic && !genericMethodInfo.IsPublic)
			{
				return null;
			}
			return genericMethodInfo;
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

		public override int GetHashCode()
		{
			return this.m_resolver.GetHashCode() * 32767 + this.m_eventToken.GetHashCode();
		}

		private void GetInfo()
		{
			int num;
			int num1;
			int num2;
			int num3;
			int num4;
			int num5;
			uint num6;
			IMetadataImport rawImport = this.m_resolver.RawImport;
			StringBuilder stringBuilder = new StringBuilder(256);
			rawImport.GetEventProps(this.m_eventToken, out this.m_declaringClassToken, null, 0, out num, out num1, out this.m_eventHandlerTypeToken, out num2, out num4, out num3, out num5, 1, out num6);
			this.m_attrib = (EventAttributes)num1;
			stringBuilder.Capacity = num;
			rawImport.GetEventProps(this.m_eventToken, out this.m_declaringClassToken, stringBuilder, num, out num, out num1, out this.m_eventHandlerTypeToken, out num2, out num4, out num3, out num5, 1, out num6);
			this.m_name = stringBuilder.ToString();
			this.m_addMethodToken = new Token(num2);
			this.m_removeMethodToken = new Token(num4);
			this.m_raiseMethodToken = new Token(num3);
		}

		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			if (this.m_raiseMethodToken.IsNil)
			{
				return null;
			}
			MethodInfo genericMethodInfo = this.m_resolver.GetGenericMethodInfo(this.m_raiseMethodToken, this.m_context);
			if (!nonPublic && !genericMethodInfo.IsPublic)
			{
				return null;
			}
			return genericMethodInfo;
		}

		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			if (this.m_removeMethodToken.IsNil)
			{
				return null;
			}
			MethodInfo genericMethodInfo = this.m_resolver.GetGenericMethodInfo(this.m_removeMethodToken, this.m_context);
			if (!nonPublic && !genericMethodInfo.IsPublic)
			{
				return null;
			}
			return genericMethodInfo;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return string.Concat(this.DeclaringType.ToString(), ".", this.Name);
		}
	}
}