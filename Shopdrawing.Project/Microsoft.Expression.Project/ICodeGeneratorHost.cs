using System.CodeDom.Compiler;

namespace Microsoft.Expression.Project
{
	public interface ICodeGeneratorHost
	{
		System.CodeDom.Compiler.CodeDomProvider CodeDomProvider
		{
			get;
		}
	}
}