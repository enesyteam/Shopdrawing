using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Extensibility
{
	public interface IServices : IServiceProvider
	{
		IEnumerable<IAddIn> AddIns
		{
			get;
		}

		IEnumerable<IPackage> Packages
		{
			get;
		}

		int Version
		{
			get;
		}

		void AddService(Type serviceType, object serviceInstance);

		void ExcludeAddIn(string fileName);

		T GetService<T>()
		where T : class;

		IAddIn LoadAddIn(string fileName);

		IEnumerable<IAddIn> LoadAddIns(string fileSpec);

		void RegisterPackage(IPackage package);

		void RemoveService(Type serviceType);

		void UnloadAddIn(IAddIn addIn);

		void UnregisterPackage(IPackage package);
	}
}