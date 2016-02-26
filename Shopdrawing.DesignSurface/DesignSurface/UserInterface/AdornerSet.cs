// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class AdornerSet : ContainerVisual, IAdornerSet
  {
    private AdornerSetOrder order;
    private ToolBehaviorContext toolContext;
    private AdornerElementSet adornedElementSet;
    private List<Adorner> adornerList;
    private Matrix matrix;

    public AdornerSetOrder Order
    {
      get
      {
        return this.order;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.toolContext.View.ViewModel;
      }
    }

    public SceneElement Element
    {
      get
      {
        return this.ElementSet.PrimaryElement;
      }
    }

    public AdornerElementSet ElementSet
    {
      get
      {
        return this.adornedElementSet;
      }
    }

    internal ToolBehaviorContext ToolContext
    {
      get
      {
        return this.toolContext;
      }
    }

    public virtual ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) null;
      }
    }

    internal Matrix Matrix
    {
      get
      {
        return this.matrix;
      }
    }

    protected virtual bool NeedsRebuild { get; set; }

    protected List<Adorner> AdornerList
    {
      get
      {
        return this.adornerList;
      }
      set
      {
        this.adornerList = value;
      }
    }

    public IAdornerCollection Adorners
    {
      get
      {
        this.Update();
        return (IAdornerCollection) new AdornerCollection((IList) this.adornerList);
      }
    }

    internal AdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
      : this(toolContext, adornedElement, AdornerSetOrderTokens.MediumPriority)
    {
    }

    internal AdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet)
      : this(toolContext, adornedElementSet, AdornerSetOrderTokens.MediumPriority)
    {
    }

    internal AdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement, AdornerSetOrder order)
      : this(toolContext, toolContext.View.Artboard.AdornerLayer.CreateOrGetAdornerElementSetForElement(adornedElement), order)
    {
    }

    internal AdornerSet(ToolBehaviorContext toolContext, AdornerElementSet adornedElementSet, AdornerSetOrder order)
    {
      if (toolContext == null)
        throw new ArgumentNullException("toolContext");
      if (adornedElementSet == null)
        throw new ArgumentNullException("adornedElementSet");
      this.order = order;
      this.toolContext = toolContext;
      this.adornedElementSet = adornedElementSet;
    }

    public void Update()
    {
      this.OnRedrawing();
      if (this.adornerList == null || this.NeedsRebuild)
      {
        this.NeedsRebuild = false;
        this.UpdateChildrenVisuals();
      }
      foreach (Adorner adorner in this.adornerList)
        adorner.Redraw();
    }

    public void InvalidateRender()
    {
      if (this.adornerList == null)
        return;
      foreach (Adorner adorner in this.adornerList)
        adorner.InvalidateRender();
    }

    protected virtual void OnRedrawing()
    {
    }

    public void InvalidateStructure()
    {
      this.NeedsRebuild = true;
    }

    public virtual void OnRemove()
    {
      if (this.adornerList == null)
        return;
      foreach (Adorner adorner in this.adornerList)
        adorner.OnRemove();
    }

    protected virtual void UpdateChildrenVisuals()
    {
      this.Children.Clear();
      this.adornerList = new List<Adorner>();
      this.CreateAdorners();
    }

    public virtual Cursor GetCursor(IAdorner adorner)
    {
      return Cursors.Arrow;
    }

    internal void SetMatrix(Matrix matrix)
    {
      this.matrix = matrix;
      this.InvalidateRender();
    }

    protected virtual void CreateAdorners()
    {
    }

    protected void AddAdorner(Adorner adorner)
    {
      if (this.adornerList == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornerSetAddAdornerCalledWithNullList);
      this.adornerList.Add(adorner);
      this.Children.Add((Visual) adorner);
    }

    public virtual Matrix GetTransformMatrix(IViewObject targetViewObject)
    {
      return this.ElementSet.GetTransformMatrix(targetViewObject);
    }

    public virtual Matrix GetTransformMatrixToAdornerLayer()
    {
      return this.ElementSet.GetTransformMatrixToAdornerLayer();
    }
  }
}
