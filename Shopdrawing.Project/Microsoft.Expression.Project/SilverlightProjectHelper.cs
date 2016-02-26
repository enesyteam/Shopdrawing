using Microsoft.VisualStudio.Silverlight;
using System;

namespace Microsoft.Expression.Project
{
	public static class SilverlightProjectHelper
	{
		private static byte[] SilverlightPublicKeyToken;

		static SilverlightProjectHelper()
		{
			SilverlightProjectHelper.SilverlightPublicKeyToken = new byte[] { 124, 236, 133, 215, 190, 167, 121, 142 };
		}

		public static bool IsSilverlightAssembly(string path)
		{
			bool flag;
			AssemblyMetadataHelper.IMetaDataDispenserEx dispenser = AssemblyMetadataHelper.GetDispenser();
			AssemblyMetadataHelper.IMetaDataAssemblyImport metaDataAssemblyImport = AssemblyMetadataHelper.OpenScope(dispenser, path);
			if (metaDataAssemblyImport != null)
			{
				try
				{
					AssemblyNameVersion assemblyNameVersion = AssemblyMetadataHelper.GetAssemblyNameVersion(metaDataAssemblyImport);
					if (assemblyNameVersion == null || !assemblyNameVersion.Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase) || !ProjectAssemblyHelper.ComparePublicKeyTokens(SilverlightProjectHelper.SilverlightPublicKeyToken, assemblyNameVersion.PublicKeyToken))
					{
						bool flag1 = false;
						AssemblyNameVersion[] assemblyReferenceNameVersion = AssemblyMetadataHelper.GetAssemblyReferenceNameVersion(metaDataAssemblyImport);
						if (assemblyReferenceNameVersion != null)
						{
							AssemblyNameVersion[] assemblyNameVersionArray = assemblyReferenceNameVersion;
							int num = 0;
							while (num < (int)assemblyNameVersionArray.Length)
							{
								AssemblyNameVersion assemblyNameVersion1 = assemblyNameVersionArray[num];
								if (!assemblyNameVersion1.Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase))
								{
									num++;
								}
								else
								{
									flag1 = true;
									if (!ProjectAssemblyHelper.ComparePublicKeyTokens(SilverlightProjectHelper.SilverlightPublicKeyToken, assemblyNameVersion1.PublicKeyToken))
									{
										break;
									}
									flag = true;
									return flag;
								}
							}
							if (!flag1)
							{
								flag = true;
								return flag;
							}
						}
						return false;
					}
					else
					{
						flag = true;
					}
				}
				finally
				{
					if (metaDataAssemblyImport != null)
					{
						AssemblyMetadataHelper.ReleaseAssemblyImport(metaDataAssemblyImport);
					}
					if (dispenser != null)
					{
						AssemblyMetadataHelper.ReleaseDispenser(dispenser);
					}
				}
				return flag;
			}
			return false;
		}
	}
}