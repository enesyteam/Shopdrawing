using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal interface IReflectionAssembly
	{
		Assembly ReflectionAssembly
		{
			get;
		}
	}
}