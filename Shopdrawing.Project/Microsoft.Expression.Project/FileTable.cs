using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	internal static class FileTable
	{
		private static FileResourceManager resourceManager;

		static FileTable()
		{
			Microsoft.Expression.Project.FileTable.resourceManager = new FileResourceManager(typeof(Microsoft.Expression.Project.FileTable).Assembly);
		}

		public static byte[] GetByteArray(string name)
		{
			return Microsoft.Expression.Project.FileTable.resourceManager.GetByteArray(name);
		}

		public static FrameworkElement GetElement(string name)
		{
			return Microsoft.Expression.Project.FileTable.resourceManager.GetElement(name);
		}

		public static ImageSource GetImageSource(string name)
		{
			return Microsoft.Expression.Project.FileTable.resourceManager.GetImageSource(name);
		}

		public static ResourceDictionary GetResourceDictionary(string name)
		{
			return Microsoft.Expression.Project.FileTable.resourceManager.GetResourceDictionary(name);
		}
	}
}