using System;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal interface ITypeSignatureBlob : ITypeProxy
	{
		byte[] Blob
		{
			get;
		}

		Module DeclaringScope
		{
			get;
		}
	}
}