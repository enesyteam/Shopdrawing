using System;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal interface IModule2
	{
		AssemblyName GetAssemblyNameFromAssemblyRef(Token token);

		int RowCount(System.Reflection.Adds.MetadataTable metadataTableIndex);
	}
}