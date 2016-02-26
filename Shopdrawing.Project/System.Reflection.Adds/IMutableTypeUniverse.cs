using System;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal interface IMutableTypeUniverse : ITypeUniverse
	{
		void AddAssembly(Assembly assembly);
	}
}