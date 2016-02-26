using System;

namespace Microsoft.Expression.Project
{
	public interface IMSBuildItem
	{
		string Include
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		string GetMetadata(string name);

		void SetMetadata(string name, string value);
	}
}