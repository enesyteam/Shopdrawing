using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;
using System.Xml;

namespace Microsoft.Expression.Project
{
	internal static class DeepZoomHelper
	{
		public static DocumentReference CreateDeepZoomDirectoryReference(DocumentReference deepZoomPath, string directoryExtension)
		{
			if (deepZoomPath == null)
			{
				throw new ArgumentNullException("deepZoomPath");
			}
			if (string.IsNullOrEmpty(directoryExtension))
			{
				throw new ArgumentNullException("directoryExtension");
			}
			string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(deepZoomPath.Path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(deepZoomPath.Path);
			return DocumentReference.CreateFromRelativePath(directoryNameOrRoot, Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(string.Concat(fileNameWithoutExtension, directoryExtension)));
		}

		public static string[] GetDirectoryExtensions(string fileName)
		{
			string[] strArrays;
			if (!Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(fileName))
			{
				return null;
			}
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(fileName))
				{
					if (xmlReader != null && xmlReader.Read() && xmlReader.IsStartElement())
					{
						string[] strArrays1 = null;
						if (string.Compare(xmlReader.Name, "Image", StringComparison.OrdinalIgnoreCase) == 0)
						{
							strArrays1 = new string[] { "_files" };
						}
						else if (string.Compare(xmlReader.Name, "Collection", StringComparison.OrdinalIgnoreCase) == 0)
						{
							strArrays1 = new string[] { "_files", "_images" };
						}
						if (strArrays1 != null)
						{
							Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(fileName);
							Path.GetFileNameWithoutExtension(fileName);
							string[] strArrays2 = strArrays1;
							int num = 0;
							while (num < (int)strArrays2.Length)
							{
								string str = strArrays2[num];
								if (Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(DeepZoomHelper.CreateDeepZoomDirectoryReference(DocumentReference.Create(fileName), str).Path))
								{
									num++;
								}
								else
								{
									strArrays = null;
									return strArrays;
								}
							}
							strArrays = strArrays1;
							return strArrays;
						}
					}
				}
				return null;
			}
			catch (Exception exception)
			{
				return null;
			}
			return strArrays;
		}

		public static bool IsDeepZoomDocument(string fileName)
		{
			return DeepZoomHelper.GetDirectoryExtensions(fileName) != null;
		}
	}
}