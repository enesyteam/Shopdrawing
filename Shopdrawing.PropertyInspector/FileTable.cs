// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.FileTable
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface
{
    internal static class FileTable
    {
        private static FileResourceManager resourceManager = new FileResourceManager(typeof(FileTable).Assembly);

        public static FrameworkElement GetElement(string name)
        {
            return FileTable.resourceManager.GetElement(name);
        }

        public static FontFamily GetFontFamily(string name)
        {
            return FileTable.resourceManager.GetFontFamily(name);
        }

        public static ResourceDictionary GetResourceDictionary(string name)
        {
            return FileTable.resourceManager.GetResourceDictionary(name);
        }

        public static Style GetStyle(string name)
        {
            return FileTable.resourceManager.GetStyle(name);
        }

        public static DataTemplate GetDataTemplate(string name)
        {
            return FileTable.resourceManager.GetDataTemplate(name);
        }

        public static ImageSource GetImageSource(string name)
        {
            return FileTable.resourceManager.GetImageSource(name);
        }

        public static Cursor GetCursor(string name)
        {
            return FileTable.resourceManager.GetCursor(name);
        }

        public static byte[] GetByteArray(string name)
        {
            return FileTable.resourceManager.GetByteArray(name);
        }

        public static Model3DGroup GetModel3DGroup(string name)
        {
            return FileTable.resourceManager.GetModel3DGroup(name);
        }

        public static DrawingImage GetDrawingImage(string name)
        {
            return FileTable.resourceManager.GetDrawingImage(name);
        }
    }
}
