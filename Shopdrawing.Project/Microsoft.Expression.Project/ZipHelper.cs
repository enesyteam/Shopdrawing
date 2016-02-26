using Microsoft.Expression.SubsetFontTask.Zip;
using System;

namespace Microsoft.Expression.Project
{
	public static class ZipHelper
	{
		internal static ZipArchive CreateZipArchive(string location)
		{
			ZipArchive zipArchive = new ZipArchive(location);
			GC.SuppressFinalize(zipArchive);
			return zipArchive;
		}
	}
}