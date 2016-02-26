using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;

namespace Microsoft.Expression.Project
{
	internal class AssemblyCache
	{
		private const string defaultProjectAssembliesFolder = "Microsoft\\Expression\\Blend\\Project Assemblies";

		private string location;

		public AssemblyCache()
		{
			this.location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Expression\\Blend\\Project Assemblies");
		}

		public void Clean()
		{
			try
			{
				ProjectPathHelper.CleanDirectory(this.location, true);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!(exception is ArgumentException) && !(exception is IOException) && !(exception is NotSupportedException) && !(exception is UnauthorizedAccessException))
				{
					throw;
				}
			}
		}

		public string CreateDirectory()
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.location))
			{
				Directory.CreateDirectory(this.location);
			}
			string str = null;
			while (true)
			{
				str = Path.Combine(this.location, Path.GetRandomFileName());
				try
				{
					if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str))
					{
						Directory.CreateDirectory(str);
						break;
					}
				}
				catch (IOException oException)
				{
					if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str))
					{
						throw;
					}
				}
			}
			return str;
		}

		public bool VerifyDirectory(string path)
		{
			return path.StartsWith(this.location, StringComparison.OrdinalIgnoreCase);
		}
	}
}