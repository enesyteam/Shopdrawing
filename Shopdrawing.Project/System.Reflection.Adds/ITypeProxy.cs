using System;

namespace System.Reflection.Adds
{
	internal interface ITypeProxy
	{
		ITypeUniverse TypeUniverse
		{
			get;
		}

		Type GetResolvedType();
	}
}