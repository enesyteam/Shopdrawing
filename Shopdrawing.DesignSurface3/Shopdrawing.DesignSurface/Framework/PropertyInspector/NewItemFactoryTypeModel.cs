// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.NewItemFactoryTypeModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public class NewItemFactoryTypeModel
  {
    private Type type;
    private NewItemFactory factory;
    private Size desiredSize;
    private IMessageLoggingService exceptionLogger;

    public string DisplayName
    {
      get
      {
        return this.factory.GetDisplayName(this.type);
      }
    }

    public Type Type
    {
      get
      {
        return this.type;
      }
    }

    public object Image
    {
      get
      {
        string imageName;
        Stream imageStream = this.factory.GetImageStream(this.type, this.desiredSize, out imageName);
        if (imageStream == null)
          return (object) null;
        System.Windows.Controls.Image image = new System.Windows.Controls.Image();
        try
        {
          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.BeginInit();
          bitmapImage.StreamSource = imageStream;
          bitmapImage.EndInit();
          image.Source = (ImageSource) bitmapImage;
        }
        catch (Exception ex)
        {
          this.ReportException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.NewItemFactoryIconLoadFailed, new object[2]
          {
            (object) this.factory.GetType().Name,
            (object) ex.Message
          }));
        }
        return (object) image;
      }
    }

    public Size DesiredSize
    {
      get
      {
        return this.desiredSize;
      }
      set
      {
        this.desiredSize = value;
      }
    }

    public NewItemFactory ItemFactory
    {
      get
      {
        return this.factory;
      }
    }

    public NewItemFactoryTypeModel(Type type, NewItemFactory factory)
    {
      this.type = type;
      this.factory = factory;
      this.desiredSize = new Size(16.0, 16.0);
    }

    public NewItemFactoryTypeModel(Type type, NewItemFactory factory, IMessageLoggingService exceptionLogger)
      : this(type, factory)
    {
      this.exceptionLogger = exceptionLogger;
    }

    public object CreateInstance()
    {
      return this.factory.CreateInstance(this.type);
    }

    private void ReportException(string message)
    {
      if (this.exceptionLogger == null)
        return;
      this.exceptionLogger.WriteLine(message);
    }
  }
}
