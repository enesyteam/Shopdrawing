using System;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal interface ITypeReference : ITypeProxy
	{
		Module DeclaringScope
		{
			get;
		}

		string FullName
		{
			get;
		}

		string RawName
		{
			get;
		}

		Token ResolutionScope
		{
			get;
		}

		Token TypeRefToken
		{
			get;
		}
	}
}