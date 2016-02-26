// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.RawAnnotation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public class RawAnnotation
  {
    public string Id { get; set; }

    public string Text { get; set; }

    public double Top { get; set; }

    public double Left { get; set; }

    public string Author { get; set; }

    public string AuthorInitials { get; set; }

    public int SerialNumber { get; set; }

    public bool VisibleAtRuntime { get; set; }

    public DateTime Timestamp { get; set; }

    public RawAnnotation()
    {
    }

    public RawAnnotation(AnnotationSceneNode annotation)
    {
      this.Id = annotation.Id;
      this.Text = annotation.Text;
      this.Top = annotation.Top;
      this.Left = annotation.Left;
      this.Author = annotation.Author;
      this.AuthorInitials = annotation.AuthorInitials;
      this.SerialNumber = annotation.SerialNumber;
      this.VisibleAtRuntime = annotation.VisibleAtRuntime;
      this.Timestamp = annotation.Timestamp;
    }
  }
}
