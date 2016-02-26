// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.AdornerLayer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class AdornerLayer : UIElement
  {
    private Dictionary<AdornerElementSet, AdornerLayer.ElementInfo> elementMap = new Dictionary<AdornerElementSet, AdornerLayer.ElementInfo>();
    private List<AdornerElementSet> adornerElementSets = new List<AdornerElementSet>();
    private List<AdornerSet3DContainer> adornerSet3DContainers = new List<AdornerSet3DContainer>();
    private List<Viewport3DElement> proxiedLightContainers = new List<Viewport3DElement>();
    private VisualCollection children;
    private SceneView sceneView;
    private int suspendUpdateCount;

    internal List<AdornerSet3DContainer> AdornerSet3DContainers
    {
      get
      {
        return this.adornerSet3DContainers;
      }
    }

    public List<Viewport3DElement> ContainersWithProxiedLights
    {
      get
      {
        return this.proxiedLightContainers;
      }
    }

    internal SceneView SceneView
    {
      get
      {
        return this.sceneView;
      }
      set
      {
        this.sceneView = value;
      }
    }

    protected override int VisualChildrenCount
    {
      get
      {
        return this.children.Count;
      }
    }

    public event EventHandler<AdornerPropertyChangedEventArgs> AdornerPropertyChanged;

    public AdornerLayer()
    {
      this.children = new VisualCollection((Visual) this);
      this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
    }

    public void FirePropertyChanged(string propertyName, object newValue)
    {
      if (this.AdornerPropertyChanged == null)
        return;
      this.AdornerPropertyChanged((object) this, new AdornerPropertyChangedEventArgs(propertyName, newValue));
    }

    internal void TearDown()
    {
      this.LayoutUpdated -= new EventHandler(this.OnLayoutUpdated);
      this.sceneView = (SceneView) null;
    }

    public void Hide()
    {
      this.Visibility = Visibility.Collapsed;
    }

    public void Show()
    {
      this.Visibility = Visibility.Visible;
    }

    public void Add(IAdornerSet adornerSet)
    {
      if (adornerSet == null)
        throw new ArgumentNullException("adornerSet");
      AdornerSet adornerSet2D;
      if ((adornerSet2D = adornerSet as AdornerSet) != null)
      {
        this.AddAdornerSet2D(adornerSet2D);
      }
      else
      {
        AdornerSet3D adornerSet3D;
        if ((adornerSet3D = adornerSet as AdornerSet3D) == null)
          throw new ArgumentException(ExceptionStringTable.AdornerSetInstanceMustDeriveFromAdornerSetOrAdornerSet3D);
        this.AddAdornerSet3D(adornerSet3D);
      }
    }

    public void Remove(IAdornerSet adornerSet)
    {
      if (adornerSet == null)
        throw new ArgumentNullException("adornerSet");
      AdornerSet adornerSet2D;
      if ((adornerSet2D = adornerSet as AdornerSet) != null)
      {
        this.RemoveAdornerSet2D(adornerSet2D);
      }
      else
      {
        AdornerSet3D adornerSet3D;
        if ((adornerSet3D = adornerSet as AdornerSet3D) == null)
          throw new ArgumentException(ExceptionStringTable.AdornerSetInstanceMustDeriveFromAdornerSetOrAdornerSet3D);
        this.RemoveAdornerSet3D(adornerSet3D);
      }
    }

    public void ClearAdornerElementSets()
    {
      this.adornerElementSets.Clear();
    }

    public AdornerElementSet CreateOrGetAdornerElementSetForElement(SceneElement element)
    {
      IList<AdornerElementSet> containingElement = this.GetAdornerElementSetsContainingElement(element);
      if (containingElement.Count >= 1)
      {
        foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) containingElement)
        {
          if (!adornerElementSet.AdornsMultipleElements)
            return adornerElementSet;
        }
      }
      AdornerElementSet adornerElementSet1 = new AdornerElementSet(element);
      this.adornerElementSets.Add(adornerElementSet1);
      return adornerElementSet1;
    }

    private IList<AdornerElementSet> GetAdornerElementSetsContainingElement(SceneElement sceneElement)
    {
      List<AdornerElementSet> list = new List<AdornerElementSet>();
      foreach (AdornerElementSet adornerElementSet in this.adornerElementSets)
      {
        if (adornerElementSet.Elements.Contains(sceneElement))
          list.Add(adornerElementSet);
      }
      foreach (AdornerElementSet adornerElementSet in this.elementMap.Keys)
      {
        if (adornerElementSet.Elements.Contains(sceneElement) && !list.Contains(adornerElementSet))
          list.Add(adornerElementSet);
      }
      return (IList<AdornerElementSet>) list;
    }

    public ICollection<AdornerSet> Get2DAdornerSets(SceneElement sceneElement)
    {
      List<AdornerSet> list = new List<AdornerSet>();
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) this.GetAdornerElementSetsContainingElement(sceneElement))
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo != null)
          list.AddRange((IEnumerable<AdornerSet>) elementInfo.AdornerSets);
      }
      return (ICollection<AdornerSet>) new ReadOnlyCollection<AdornerSet>((IList<AdornerSet>) list);
    }

    public ICollection<IAdornerSet> GetAdornerSets(SceneElement sceneElement)
    {
      List<IAdornerSet> list = new List<IAdornerSet>();
      Base3DElement base3Delement = sceneElement as Base3DElement;
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) this.GetAdornerElementSetsContainingElement(base3Delement != null ? (SceneElement) base3Delement.Viewport : sceneElement))
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo != null)
        {
          if (base3Delement != null && elementInfo.AdornerSet3DContainer != null)
          {
            foreach (AdornerSet3D adornerSet3D in elementInfo.AdornerSet3DContainer.AdornerSets)
            {
              if (adornerSet3D.Element == base3Delement)
                list.Add((IAdornerSet) adornerSet3D);
            }
          }
          foreach (AdornerSet adornerSet in elementInfo.AdornerSets)
            list.Add((IAdornerSet) adornerSet);
        }
      }
      return (ICollection<IAdornerSet>) list;
    }

    public void Update2D()
    {
      if (this.Visibility != Visibility.Visible)
        return;
      this.UpdateElementInfo((SceneElement) null, AdornerLayer.ElementInfoUpdateSource.Explicit);
      foreach (AdornerLayer.ElementInfo elementInfo in this.elementMap.Values)
        elementInfo.Update2D();
    }

    public void UpdateAdorners()
    {
      this.UpdateAdorners(AdornerLayer.ElementInfoUpdateSource.Explicit);
    }

    private void UpdateAdorners(AdornerLayer.ElementInfoUpdateSource source)
    {
      if (this.Visibility != Visibility.Visible)
        return;
      this.UpdateElementInfo((SceneElement) null, source);
      foreach (AdornerLayer.ElementInfo elementInfo in this.elementMap.Values)
      {
        elementInfo.Update2D();
        elementInfo.Update3D();
      }
    }

    public void InvalidateAdornerVisuals()
    {
      foreach (AdornerLayer.ElementInfo elementInfo in this.elementMap.Values)
        elementInfo.InvalidateRender((Base3DElement) null);
    }

    public void InvalidateAdornerVisuals(SceneElement sceneElement)
    {
      if (sceneElement == null)
        throw new ArgumentNullException("sceneElement");
      if (!sceneElement.IsAttached)
        return;
      Base3DElement optionalBase3DElement = sceneElement as Base3DElement;
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) this.GetAdornerElementSetsContainingElement(sceneElement))
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo != null)
          elementInfo.InvalidateRender(optionalBase3DElement);
        adornerElementSet.Invalidate();
      }
      if (optionalBase3DElement == null)
        return;
      Viewport3DElement viewport = optionalBase3DElement.Viewport;
      if (viewport == null)
        return;
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) this.GetAdornerElementSetsContainingElement((SceneElement) viewport))
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo != null)
          elementInfo.InvalidateRender(optionalBase3DElement);
      }
    }

    public void InvalidateAdornersStructure(SceneElement sceneElement)
    {
      if (sceneElement == null)
        throw new ArgumentNullException("sceneElement");
      if (!sceneElement.IsAttached)
        return;
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) this.GetAdornerElementSetsContainingElement(sceneElement))
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo != null)
          elementInfo.InvalidateStructure();
      }
    }

    public bool IsProxied(SceneElement element)
    {
      Base3DElement base3Delement = element as Base3DElement;
      if (base3Delement != null)
        return this.ContainersWithProxiedLights.Contains(base3Delement.Viewport);
      return false;
    }

    public void ClearProxyGeometries()
    {
      this.proxiedLightContainers = new List<Viewport3DElement>();
    }

    public IDisposable SuspendUpdates()
    {
      return (IDisposable) new AdornerLayer.UpdateSuspender(this);
    }

    internal AdornerSet3DContainer GetAdornerSet3DContainer(Viewport3DElement viewport3DElement)
    {
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) this.GetAdornerElementSetsContainingElement((SceneElement) viewport3DElement))
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo != null && elementInfo.AdornerSet3DContainer != null)
          return elementInfo.AdornerSet3DContainer;
      }
      return (AdornerSet3DContainer) null;
    }

    protected override Visual GetVisualChild(int index)
    {
      return this.children[index];
    }

    protected override Size MeasureCore(Size availableSize)
    {
      return new Size();
    }

    protected override void ArrangeCore(Rect finalRect)
    {
    }

    private void AddAdornerSet2D(AdornerSet adornerSet2D)
    {
      AdornerElementSet elementSet = adornerSet2D.ElementSet;
      if (elementSet == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornerElementSetMayNotBeNull);
      if (adornerSet2D.Parent != this)
        this.children.Add((Visual) adornerSet2D);
      AdornerLayer.ElementInfo createElementInfo = this.GetOrCreateElementInfo(elementSet);
      createElementInfo.Update(elementSet, this, AdornerLayer.ElementInfoUpdateSource.Explicit);
      createElementInfo.AddAdornerSet(adornerSet2D);
    }

    private void AddAdornerSet3D(AdornerSet3D adornerSet3D)
    {
      Base3DElement element = adornerSet3D.Element;
      if (element == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornedBase3DElementMayNotBeNull);
      Viewport3DElement viewport = element.Viewport;
      if (viewport == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornedViewport3DElementMayNotBeNull);
      AdornerLayer.ElementInfo createElementInfo = this.GetOrCreateElementInfo(this.CreateOrGetAdornerElementSetForElement((SceneElement) viewport));
      if (createElementInfo.AdornerSet3DContainer == null)
      {
        createElementInfo.AdornerSet3DContainer = new AdornerSet3DContainer(viewport);
        this.adornerSet3DContainers.Add(createElementInfo.AdornerSet3DContainer);
      }
      createElementInfo.Update((SceneElement) viewport, this, AdornerLayer.ElementInfoUpdateSource.Explicit);
      createElementInfo.AddAdornerSet(adornerSet3D);
      if (createElementInfo.AdornerSet3DContainer.Parent == this)
        return;
      this.children.Add((Visual) createElementInfo.AdornerSet3DContainer);
    }

    private void RemoveAdornerSet2D(AdornerSet adornerSet2D)
    {
      AdornerElementSet elementSet = adornerSet2D.ElementSet;
      if (elementSet == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornerElementSetMayNotBeNull);
      AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(elementSet);
      if (elementInfo != null && elementInfo.RemoveAdornerSet(adornerSet2D))
      {
        if (elementInfo.IsEmpty)
          this.elementMap.Remove(elementSet);
        this.children.Remove((Visual) adornerSet2D);
      }
      adornerSet2D.OnRemove();
    }

    private void RemoveAdornerSet3D(AdornerSet3D adornerSet3D)
    {
      Viewport3DElement adornedViewport = adornerSet3D.AdornerSet3DContainer.AdornedViewport;
      if (adornedViewport == null)
        throw new InvalidOperationException(ExceptionStringTable.AdornedViewport3DElementMayNotBeNull);
      IList<AdornerElementSet> containingElement = this.GetAdornerElementSetsContainingElement((SceneElement) adornedViewport);
      if (containingElement.Count <= 0)
        return;
      foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) containingElement)
      {
        AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
        if (elementInfo.AdornerSet3DContainer != null && elementInfo.AdornerSet3DContainer.AdornerSets.Contains(adornerSet3D) && (elementInfo != null && elementInfo.RemoveAdornerSet(adornerSet3D)))
        {
          AdornerSet3DContainer adornerSet3Dcontainer = elementInfo.AdornerSet3DContainer;
          if (adornerSet3Dcontainer.AdornerSetCount == 0)
          {
            elementInfo.AdornerSet3DContainer = (AdornerSet3DContainer) null;
            this.children.Remove((Visual) adornerSet3Dcontainer);
            this.adornerSet3DContainers.Remove(adornerSet3Dcontainer);
          }
          if (elementInfo.IsEmpty)
            this.elementMap.Remove(adornerElementSet);
        }
      }
    }

    private void OnLayoutUpdated(object sender, EventArgs e)
    {
      if (this.suspendUpdateCount > 0 || this.sceneView == null || (this.sceneView.IsUpdating || PresentationSource.FromVisual((Visual) this) == null))
        return;
      this.UpdateAdorners(AdornerLayer.ElementInfoUpdateSource.LayoutUpdated);
    }

    private void UpdateElementInfo(SceneElement sceneElement, AdornerLayer.ElementInfoUpdateSource source)
    {
      if (this.elementMap.Count == 0)
        return;
      List<AdornerElementSet> list = new List<AdornerElementSet>(1);
      if (sceneElement != null)
      {
        IList<AdornerElementSet> containingElement = this.GetAdornerElementSetsContainingElement(sceneElement);
        if (!sceneElement.IsAttached)
        {
          foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) containingElement)
            list.Add(adornerElementSet);
        }
        else
        {
          foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) containingElement)
            this.GetElementInfo(adornerElementSet).Update(adornerElementSet, this, source);
        }
      }
      else
      {
        foreach (KeyValuePair<AdornerElementSet, AdornerLayer.ElementInfo> keyValuePair in this.elementMap)
        {
          AdornerElementSet key = keyValuePair.Key;
          if (!key.IsAttached)
            list.Add(key);
          else
            keyValuePair.Value.Update(key, this, source);
        }
      }
      foreach (AdornerElementSet adornerElementSet in list)
        this.RemoveAllAdornerSets(adornerElementSet);
    }

    private void RemoveAllAdornerSets(AdornerElementSet adornerElementSet)
    {
      AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
      foreach (AdornerSet adornerSet in elementInfo.AdornerSets)
      {
        this.children.Remove((Visual) adornerSet);
        adornerSet.OnRemove();
      }
      if (elementInfo.AdornerSet3DContainer != null)
      {
        this.children.Remove((Visual) elementInfo.AdornerSet3DContainer);
        this.adornerSet3DContainers.Remove(elementInfo.AdornerSet3DContainer);
      }
      this.elementMap.Remove(adornerElementSet);
    }

    private AdornerLayer.ElementInfo GetOrCreateElementInfo(AdornerElementSet adornerElementSet)
    {
      AdornerLayer.ElementInfo elementInfo = this.GetElementInfo(adornerElementSet);
      if (elementInfo == null)
      {
        elementInfo = new AdornerLayer.ElementInfo();
        this.elementMap.Add(adornerElementSet, elementInfo);
      }
      return elementInfo;
    }

    private AdornerLayer.ElementInfo GetElementInfo(AdornerElementSet adornerElementSet)
    {
      AdornerLayer.ElementInfo elementInfo = (AdornerLayer.ElementInfo) null;
      this.elementMap.TryGetValue(adornerElementSet, out elementInfo);
      return elementInfo;
    }

    private enum ElementInfoUpdateSource
    {
      LayoutUpdated,
      Explicit,
    }

    private sealed class ElementInfo
    {
      private Point sharedCenter = new Point();
      private Size lastRenderSize = new Size(double.NaN, double.NaN);
      private Matrix primaryElementMatrix = Matrix.Identity;
      private List<AdornerSet> adornerSets = new List<AdornerSet>(1);
      private Matrix matrix;
      private AdornerSet3DContainer adornerSet3DContainer;

      public bool IsEmpty
      {
        get
        {
          if (this.adornerSets.Count != 0)
            return false;
          if (this.adornerSet3DContainer != null)
            return this.adornerSet3DContainer.AdornerSetCount == 0;
          return true;
        }
      }

      public List<AdornerSet> AdornerSets
      {
        get
        {
          return this.adornerSets;
        }
      }

      public AdornerSet3DContainer AdornerSet3DContainer
      {
        get
        {
          return this.adornerSet3DContainer;
        }
        set
        {
          this.adornerSet3DContainer = value;
        }
      }

      public void AddAdornerSet(AdornerSet adornerSet)
      {
        this.adornerSets.Add(adornerSet);
        this.UpdateMatrix(adornerSet, (AdornerLayer) adornerSet.Parent);
      }

      public bool RemoveAdornerSet(AdornerSet adornerSet)
      {
        return this.adornerSets.Remove(adornerSet);
      }

      public void AddAdornerSet(AdornerSet3D adornerSet)
      {
        this.adornerSet3DContainer.AddAdornerSet(adornerSet);
        this.adornerSet3DContainer.SetMatrix(this.matrix);
      }

      public bool RemoveAdornerSet(AdornerSet3D adornerSet)
      {
        return this.adornerSet3DContainer.RemoveAdornerSet(adornerSet);
      }

      public void Update2D()
      {
        foreach (AdornerSet adornerSet in this.adornerSets)
          adornerSet.Update();
      }

      public void Update3D()
      {
        if (this.adornerSet3DContainer == null)
          return;
        this.adornerSet3DContainer.Update((Base3DElement) null);
      }

      public void InvalidateRender(Base3DElement optionalBase3DElement)
      {
        foreach (AdornerSet adornerSet in this.adornerSets)
          adornerSet.InvalidateRender();
        if (this.adornerSet3DContainer == null)
          return;
        this.adornerSet3DContainer.InvalidateRender(optionalBase3DElement);
      }

      public void InvalidateStructure()
      {
        foreach (AdornerSet adornerSet in this.adornerSets)
          adornerSet.InvalidateStructure();
      }

      public void Update(SceneElement sceneElement, AdornerLayer adornerLayer, AdornerLayer.ElementInfoUpdateSource source)
      {
        foreach (AdornerElementSet adornerElementSet in (IEnumerable<AdornerElementSet>) adornerLayer.GetAdornerElementSetsContainingElement(sceneElement))
          this.Update(adornerElementSet, adornerLayer, source);
      }

      public void Update(AdornerElementSet adornerElementSet, AdornerLayer adornerLayer, AdornerLayer.ElementInfoUpdateSource source)
      {
        SceneElement primaryElement = adornerElementSet.PrimaryElement;
        bool flag = false;
        if (!adornerLayer.SceneView.IsInArtboard(primaryElement))
          return;
        Size size2 = primaryElement.Visual != null ? adornerLayer.SceneView.GetRenderSize(primaryElement.Visual) : Size.Empty;
        Transform transform = (Transform) new MatrixTransform(adornerElementSet.CalculatePrimaryElementTransformMatrixToAdornerLayer());
        Matrix matrix = transform == null ? Matrix.Identity : transform.Value;
        Point renderTransformOrigin = adornerElementSet.RenderTransformOrigin;
        if (!AdornerLayer.ElementInfo.AreClose(renderTransformOrigin, this.sharedCenter) || !AdornerLayer.ElementInfo.AreClose(this.lastRenderSize, size2) || (this.primaryElementMatrix != matrix || adornerElementSet.IsPrimaryTransformNonAffine))
        {
          flag = true;
          this.sharedCenter = renderTransformOrigin;
          this.lastRenderSize = size2;
          this.primaryElementMatrix = matrix;
        }
        ISceneInsertionPoint sceneInsertionPoint = adornerElementSet.PrimaryElement.ViewModel.ActiveSceneInsertionPoint;
        if (flag || source == AdornerLayer.ElementInfoUpdateSource.LayoutUpdated && adornerElementSet.NeedsUpdate)
        {
          adornerElementSet.Update();
          this.matrix = adornerElementSet.GetTransformMatrixToAdornerLayer();
          foreach (AdornerSet adornerSet in this.adornerSets)
            this.UpdateMatrix(adornerSet, adornerLayer);
          if (this.adornerSet3DContainer != null)
            this.adornerSet3DContainer.SetMatrix(this.matrix);
          if (sceneInsertionPoint != null && primaryElement.ParentElement is GridElement && (primaryElement.ParentElement == sceneInsertionPoint.SceneElement && primaryElement.ViewModel.IsInGridDesignMode))
            adornerLayer.InvalidateAdornerVisuals(primaryElement.ParentElement);
        }
        if (!(primaryElement is Base3DElement) || sceneInsertionPoint == null)
          return;
        Base3DElement base3Delement = sceneInsertionPoint.SceneElement as Base3DElement;
        if (base3Delement == null)
          return;
        adornerLayer.InvalidateAdornerVisuals((SceneElement) base3Delement);
      }

      private void UpdateMatrix(AdornerSet adornerSet, AdornerLayer adornerLayer)
      {
        Matrix matrixToAdornerLayer = adornerSet.GetTransformMatrixToAdornerLayer();
        adornerSet.SetMatrix(matrixToAdornerLayer);
      }

      private static bool AreClose(Point point1, Point point2)
      {
        if (AdornerLayer.ElementInfo.AreClose(point1.X, point2.X))
          return AdornerLayer.ElementInfo.AreClose(point1.Y, point2.Y);
        return false;
      }

      private static bool AreClose(Size size1, Size size2)
      {
        if (AdornerLayer.ElementInfo.AreClose(size1.Width, size2.Width))
          return AdornerLayer.ElementInfo.AreClose(size1.Height, size2.Height);
        return false;
      }

      private static bool AreClose(double value1, double value2)
      {
        return Math.Abs(value1 - value2) < 0.0 / 1.0;
      }
    }

    private class UpdateSuspender : IDisposable
    {
      private AdornerLayer adornerLayer;

      public UpdateSuspender(AdornerLayer adornerLayer)
      {
        this.adornerLayer = adornerLayer;
        ++this.adornerLayer.suspendUpdateCount;
      }

      public void Dispose()
      {
        if (this.adornerLayer.suspendUpdateCount <= 0)
          return;
        --this.adornerLayer.suspendUpdateCount;
      }
    }
  }
}
