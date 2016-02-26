using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Microsoft.Expression.Utility.Globalization
{
	public class WorkaroundComponentResourceManager : ComponentResourceManager
	{
		public WorkaroundComponentResourceManager(Type type) : base(type)
		{
		}

		public override ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
		{
			ResourceSet resourceSet;
			try
			{
				resourceSet = base.GetResourceSet(culture, createIfNotExists, tryParents);
			}
			catch (MissingSatelliteAssemblyException missingSatelliteAssemblyException)
			{
				resourceSet = null;
			}
			return resourceSet;
		}
	}
}