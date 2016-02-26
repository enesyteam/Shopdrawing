// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.DockAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal abstract class DockAdorner : Adorner, IClickable
  {
    internal static double Padding = 4.0;
    private static Dictionary<Dock, Drawing> dockToDrawingMap = new Dictionary<Dock, Drawing>();
    internal static Drawing DockBottomIcon = (Drawing) Application.Current.FindResource((object) "DockBottomIcon");
    internal static Drawing DockTopIcon = (Drawing) Application.Current.FindResource((object) "DockTopIcon");
    internal static Drawing DockLeftIcon = (Drawing) Application.Current.FindResource((object) "DockLeftIcon");
    internal static Drawing DockRightIcon = (Drawing) Application.Current.FindResource((object) "DockRightIcon");
    internal static Drawing DockFillIcon = (Drawing) Application.Current.FindResource((object) "DockFillIcon");
    private Dock dock;
    private Drawing drawing;

    public Dock Dock
    {
      get
      {
        return this.dock;
      }
    }

    protected Drawing Icon
    {
      get
      {
        return this.drawing;
      }
    }

    static DockAdorner()
    {
      DockAdorner.dockToDrawingMap[Dock.Bottom] = DockAdorner.DockBottomIcon;
      DockAdorner.dockToDrawingMap[Dock.Top] = DockAdorner.DockTopIcon;
      DockAdorner.dockToDrawingMap[Dock.Left] = DockAdorner.DockLeftIcon;
      DockAdorner.dockToDrawingMap[Dock.Right] = DockAdorner.DockRightIcon;
    }

    public DockAdorner(AdornerSet adornerSet, Dock dock)
      : base(adornerSet)
    {
      this.dock = dock;
      this.drawing = DockAdorner.GetAdornerDrawing(this.Dock);
    }

    public abstract Point GetClickablePoint(Matrix matrix);

    protected static Drawing GetAdornerDrawing(Dock dock)
    {
      return DockAdorner.dockToDrawingMap[dock];
    }

    protected static Transform RemoveScaleFromCanonicalTransform(CanonicalTransform canonicalTransform)
    {
      TransformCollection children = canonicalTransform.TransformGroup.Children;
      TransformCollection transformCollection = new TransformCollection();
      foreach (Transform transform in children)
      {
        if (!typeof (ScaleTransform).IsAssignableFrom(transform.GetType()))
          transformCollection.Add(transform);
      }
      return (Transform) new TransformGroup()
      {
        Children = transformCollection
      };
    }
  }
}
