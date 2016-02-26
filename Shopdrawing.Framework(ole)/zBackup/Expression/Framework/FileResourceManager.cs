// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.FileResourceManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.Framework
{
  public sealed class FileResourceManager
  {
    private ResourceManager resourceManager;
    private string componentName;

    public FileResourceManager(Assembly resourceAssembly)
    {
      string str = resourceAssembly.ManifestModule.Name;
      if (str.EndsWith(".DLL", true, CultureInfo.InvariantCulture) || str.EndsWith(".EXE", true, CultureInfo.InvariantCulture))
        str = str.Substring(0, str.Length - 4);
      this.resourceManager = new ResourceManager(str + ".g", resourceAssembly);
      this.componentName = resourceAssembly.ToString();
    }

    public FontFamily GetFontFamily(string name)
    {
      return (FontFamily) this.LoadObject(name);
    }

    public FrameworkElement GetElement(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetElementFileResource);
      FrameworkElement frameworkElement = (FrameworkElement) this.LoadObject(name);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetElementFileResource);
      return frameworkElement;
    }

    public Style GetStyle(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetStyleFileResource);
      Style style = ((FrameworkElement) this.LoadObject(name)).Tag as Style;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetStyleFileResource);
      return style;
    }

    public DataTemplate GetDataTemplate(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetDataTemplateFileResource);
      DataTemplate dataTemplate = (DataTemplate) ((ResourceDictionary) this.LoadObject(name))[(object) "root"];
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetDataTemplateFileResource);
      return dataTemplate;
    }

    public ResourceDictionary GetResourceDictionary(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadFileResource);
      object obj = this.LoadObject(name);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadFileResource);
      ResourceDictionary resourceDictionary = obj as ResourceDictionary;
      if (resourceDictionary != null)
        return resourceDictionary;
      FrameworkElement frameworkElement = obj as FrameworkElement;
      if (frameworkElement != null)
        return frameworkElement.Resources;
      return (ResourceDictionary) null;
    }

    private object LoadObject(string name)
    {
      name = name.ToLower(CultureInfo.InvariantCulture);
      name = name.Replace("\\", "/");
      return Application.LoadComponent(new Uri(this.componentName + ";component/" + Path.ChangeExtension(name, ".xaml"), UriKind.RelativeOrAbsolute));
    }

    public ImageSource GetImageSource(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetImageFileResource);
      ImageSource imageSourceCore = this.GetImageSourceCore(name);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetImageFileResource);
      return imageSourceCore;
    }

    private ImageSource GetImageSourceCore(string name)
    {
      name = name.ToLower(CultureInfo.InvariantCulture);
      name = name.Replace("\\", "/");
      Stream stream = (Stream) this.resourceManager.GetObject(name, Thread.CurrentThread.CurrentUICulture);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.GetImageFileResource, "Loaded " + name);
      BitmapImage bitmapImage = new BitmapImage();
      bitmapImage.BeginInit();
      bitmapImage.StreamSource = stream;
      bitmapImage.EndInit();
      bitmapImage.Freeze();
      return (ImageSource) bitmapImage;
    }

    public Cursor GetCursor(string name)
    {
      name = name.ToLower(CultureInfo.InvariantCulture);
      name = name.Replace("\\", "/");
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetCursorFileResource);
      Stream cursorStream = (Stream) this.resourceManager.GetObject(name, Thread.CurrentThread.CurrentUICulture);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.GetCursorFileResource, "Loaded " + name);
      Cursor cursor = new Cursor(cursorStream);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetCursorFileResource);
      return cursor;
    }

    public DrawingImage GetDrawingImage(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetImageFileResource);
      DrawingImage drawingImage = (DrawingImage) this.LoadObject(name);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetImageFileResource);
      return drawingImage;
    }

    public Model3DGroup GetModel3DGroup(string name)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetModel3DFileResource);
      Model3DGroup model3Dgroup = (Model3DGroup) this.LoadObject(name);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetModel3DFileResource);
      return model3Dgroup;
    }

    public byte[] GetByteArray(string name)
    {
      name = name.ToLower(CultureInfo.InvariantCulture);
      name = name.Replace("\\", "/");
      Stream stream = (Stream) this.resourceManager.GetObject(name, Thread.CurrentThread.CurrentUICulture);
      byte[] buffer = new byte[stream.Length];
      stream.Read(buffer, 0, buffer.Length);
      return buffer;
    }
  }
}
