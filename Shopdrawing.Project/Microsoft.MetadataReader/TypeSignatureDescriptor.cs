using System;
using System.Runtime.CompilerServices;

namespace Microsoft.MetadataReader
{
	internal class TypeSignatureDescriptor
	{
		public Microsoft.MetadataReader.CustomModifiers CustomModifiers
		{
			get;
			set;
		}

		public bool IsPinned
		{
			get;
			set;
		}

		public System.Type Type
		{
			get;
			set;
		}

		public TypeSignatureDescriptor()
		{
		}
	}
}