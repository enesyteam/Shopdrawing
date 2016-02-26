using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class GenericContext
	{
		public Type[] MethodArgs
		{
			get;
			protected set;
		}

		public Type[] TypeArgs
		{
			get;
			protected set;
		}

		public GenericContext(Type[] typeArgs, Type[] methodArgs)
		{
			this.TypeArgs = (typeArgs == null ? Type.EmptyTypes : typeArgs);
			this.MethodArgs = (methodArgs == null ? Type.EmptyTypes : methodArgs);
		}

		public GenericContext(MethodBase methodTypeContext) : this(methodTypeContext.DeclaringType.GetGenericArguments(), methodTypeContext.GetGenericArguments())
		{
		}

		private static string ArrayToString<T>(T[] a)
		{
			if (a == null)
			{
				return "empty";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < (int)a.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(a[i]);
			}
			return stringBuilder.ToString();
		}

		public GenericContext DeleteMethodArgs()
		{
			if ((int)this.MethodArgs.Length == 0)
			{
				return this;
			}
			return new GenericContext(this.TypeArgs, null);
		}

		public override bool Equals(object obj)
		{
			GenericContext genericContext = (GenericContext)obj;
			if (genericContext == null)
			{
				return false;
			}
			if (!GenericContext.IsArrayEqual<Type>(this.TypeArgs, genericContext.TypeArgs))
			{
				return false;
			}
			return GenericContext.IsArrayEqual<Type>(this.MethodArgs, genericContext.MethodArgs);
		}

		private static int GetArrayHashCode<T>(T[] a)
		{
			int hashCode = 0;
			for (int i = 0; i < (int)a.Length; i++)
			{
				hashCode = hashCode + a[i].GetHashCode() * i;
			}
			return hashCode;
		}

		public override int GetHashCode()
		{
			return GenericContext.GetArrayHashCode<Type>(this.TypeArgs) * 32768 + GenericContext.GetArrayHashCode<Type>(this.MethodArgs);
		}

		private static bool IsArrayEqual<T>(T[] a1, T[] a2)
		where T : Type
		{
			if ((int)a1.Length != (int)a2.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)a1.Length; i++)
			{
				if (!a1[i].Equals(a2[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNullOrEmptyMethodArgs(GenericContext context)
		{
			if (context != null && (int)context.MethodArgs.Length != 0)
			{
				return false;
			}
			return true;
		}

		public static bool IsNullOrEmptyTypeArgs(GenericContext context)
		{
			if (context != null && (int)context.TypeArgs.Length != 0)
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return string.Concat("Type: ", GenericContext.ArrayToString<Type>(this.TypeArgs), ", Method: ", GenericContext.ArrayToString<Type>(this.MethodArgs));
		}

		public virtual GenericContext VerifyAndUpdateMethodArguments(int expectedNumberOfMethodArgs)
		{
			if ((int)this.MethodArgs.Length != expectedNumberOfMethodArgs)
			{
				throw new ArgumentException(MetadataStringTable.InvalidMetadataSignature);
			}
			return this;
		}
	}
}