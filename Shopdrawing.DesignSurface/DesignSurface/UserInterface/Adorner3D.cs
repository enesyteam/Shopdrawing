// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Adorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class Adorner3D : IAdorner
  {
    protected static readonly Color RedAdornerColor = Color.FromScRgb(0.5f, 1f, 0.0f, 0.0f);
    protected static readonly Color GreenAdornerColor = Color.FromScRgb(0.5f, 0.0f, 1f, 0.0f);
    protected static readonly Color BlueAdornerColor = Color.FromScRgb(0.5f, 0.0f, 0.0f, 1f);
    protected static readonly Color SolidRedAdornerColor = Color.FromScRgb(1f, 1f, 0.0f, 0.0f);
    protected static readonly Color SolidGreenAdornerColor = Color.FromScRgb(1f, 0.0f, 1f, 0.0f);
    protected static readonly Color SolidBlueAdornerColor = Color.FromScRgb(1f, 0.0f, 0.0f, 1f);
    protected static readonly SolidColorBrush RedBrush = new SolidColorBrush(Adorner3D.RedAdornerColor);
    protected static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Adorner3D.GreenAdornerColor);
    protected static readonly SolidColorBrush BlueBrush = new SolidColorBrush(Adorner3D.BlueAdornerColor);
    protected static readonly SolidColorBrush SolidRedBrush = new SolidColorBrush(Adorner3D.SolidRedAdornerColor);
    protected static readonly SolidColorBrush SolidGreenBrush = new SolidColorBrush(Adorner3D.SolidGreenAdornerColor);
    protected static readonly SolidColorBrush SolidBlueBrush = new SolidColorBrush(Adorner3D.SolidBlueAdornerColor);
    protected static readonly Material RedLook = (Material) new DiffuseMaterial((Brush) Adorner3D.RedBrush);
    protected static readonly Material GreenLook = (Material) new DiffuseMaterial((Brush) Adorner3D.GreenBrush);
    protected static readonly Material BlueLook = (Material) new DiffuseMaterial((Brush) Adorner3D.BlueBrush);
    protected static readonly Material SolidRedLook = (Material) new DiffuseMaterial((Brush) Adorner3D.SolidRedBrush);
    protected static readonly Material SolidGreenLook = (Material) new DiffuseMaterial((Brush) Adorner3D.SolidGreenBrush);
    protected static readonly Material SolidBlueLook = (Material) new DiffuseMaterial((Brush) Adorner3D.SolidBlueBrush);
    private Adorner3D.TransformVia axis;
    private AdornerSet3D adornerSet;
    private bool isActive;
    private Model3DGroup adornerModel;
    private ModelVisual3D adornerVisual;
    private bool isProxyGeometry;

    public IAdornerSet AdornerSet
    {
      get
      {
        return (IAdornerSet) this.adornerSet;
      }
    }

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
      set
      {
        this.isActive = value;
      }
    }

    internal bool IsProxyGeometry
    {
      get
      {
        return this.isProxyGeometry;
      }
      set
      {
        this.isProxyGeometry = value;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.Element.Viewport.DesignerContext;
      }
    }

    AdornerElementSet IAdorner.ElementSet
    {
      get
      {
        return this.AdornerSet.ElementSet;
      }
    }

    public Base3DElement Element
    {
      get
      {
        return this.adornerSet.Element;
      }
    }

    public Model3DGroup AdornerModel
    {
      get
      {
        return this.adornerModel;
      }
      set
      {
        this.adornerModel = value;
        this.adornerVisual = new ModelVisual3D();
        this.adornerVisual.Content = (Model3D) value;
      }
    }

    public ModelVisual3D AdornerVisual
    {
      get
      {
        return this.adornerVisual;
      }
    }

    public Adorner3D.TransformVia Axis
    {
      get
      {
        return this.axis;
      }
    }

    public IPlatform Platform
    {
      get
      {
        return this.Element.Platform;
      }
    }

    static Adorner3D()
    {
      Adorner3D.RedBrush.Freeze();
      Adorner3D.GreenBrush.Freeze();
      Adorner3D.BlueBrush.Freeze();
      Adorner3D.SolidRedBrush.Freeze();
      Adorner3D.SolidGreenBrush.Freeze();
      Adorner3D.SolidBlueBrush.Freeze();
      Adorner3D.RedLook.Freeze();
      Adorner3D.GreenLook.Freeze();
      Adorner3D.BlueLook.Freeze();
      Adorner3D.SolidRedLook.Freeze();
      Adorner3D.SolidGreenLook.Freeze();
      Adorner3D.SolidBlueLook.Freeze();
    }

    protected Adorner3D(AdornerSet3D adornerSet)
    {
      this.adornerSet = adornerSet;
    }

    protected Adorner3D(AdornerSet3D adornerSet, Adorner3D.TransformVia direction)
      : this(adornerSet)
    {
      this.axis = direction;
    }

    public bool DoesIncludeModel3D(Model3D item)
    {
      return Adorner3D.DoesContain((Model3D) this.AdornerModel, item);
    }

    public static bool DoesContain(Model3D tree, Model3D item)
    {
      if (item == tree)
        return true;
      Model3DGroup model3Dgroup = tree as Model3DGroup;
      if (model3Dgroup != null && model3Dgroup.Children != null)
      {
        foreach (Model3D tree1 in model3Dgroup.Children)
        {
          if (Adorner3D.DoesContain(tree1, item))
            return true;
        }
      }
      return false;
    }

    public virtual void PositionAndOrientGeometry()
    {
    }

    public void SetName(DependencyObject target, string name)
    {
      CommonProperties.SetName((object) target, name, (IPlatformMetadata) this.Platform.Metadata);
    }

    public enum TransformVia
    {
      XAxis,
      YAxis,
      ZAxis,
    }
  }
}
