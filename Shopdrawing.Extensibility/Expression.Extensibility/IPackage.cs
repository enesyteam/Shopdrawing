using System;

namespace Microsoft.Expression.Extensibility
{
	public interface IPackage
	{
		void Load(IServices services);

		void Unload();
	}
}