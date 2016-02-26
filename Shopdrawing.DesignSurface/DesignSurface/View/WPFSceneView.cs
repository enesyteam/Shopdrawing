// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.WPFSceneView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.View
{
  [DebuggerDisplay("{DebuggerDisplayValue}")]
  public sealed class WPFSceneView : SceneView
  {
    public override IViewVisual HitTestRoot
    {
      get
      {
        if (this.hitTestRoot == null)
          this.hitTestRoot = this.Platform.ViewObjectFactory.Instantiate((object) this.Artboard.ContentArea) as IViewVisual;
        return this.hitTestRoot;
      }
    }

    public override FrameworkElement ViewRootContainer
    {
      get
      {
        return this.Artboard.ContentArea;
      }
    }

    public override IViewObject ViewRoot
    {
      get
      {
        return this.Artboard.EditableContentObject;
      }
    }

    protected override object ScopedInstance
    {
      get
      {
        SceneNode sceneNode = this.allowViewScoping ? this.viewModel.ViewRoot : this.viewModel.RootNode;
        if (sceneNode == null)
          return (object) null;
        IViewObject correspondingViewObject = this.GetCorrespondingViewObject(sceneNode.DocumentNodePath);
        if (correspondingViewObject == null)
          return (object) null;
        return correspondingViewObject.PlatformSpecificObject;
      }
    }

    public WPFSceneView(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewHost, viewModel, true)
    {
    }

    private WPFSceneView(ISceneViewHost viewHost, SceneViewModel viewModel, bool isRootView)
      : base(viewHost, viewModel, isRootView)
    {
    }

    protected override INameScope CreateNameScope()
    {
      return (INameScope) new NameScope();
    }

    protected override Artboard CreateArtboard()
    {
      return new Artboard(this.ProjectContext.Platform.ViewObjectFactory, new ViewExceptionCallback(((SceneView) this).OnExceptionWithUnknownSource));
    }

    public override GeneralTransform GetComputedTransformToHitArea()
    {
      return (GeneralTransform) null;
    }

    public override Matrix GetComputedTransformToRootVerified(IViewObject element)
    {
      return this.GetComputedTransformToRoot(element);
    }

    public override Matrix GetComputedTransformToRoot(IViewObject element)
    {
      if (element == null)
        return Matrix.Identity;
      return ElementUtilities.GetComputedTransform((Visual) this.ViewRootContainer, element.PlatformSpecificObject as Visual);
    }

    public override Matrix GetComputedTransformFromRoot(IViewObject element)
    {
      return ElementUtilities.GetComputedTransform(element.PlatformSpecificObject as Visual, (Visual) this.ViewRootContainer);
    }

    public override void EnsureVisible(IAdorner adorner, bool scrollNow)
    {
      IClickable clickable = adorner as IClickable;
      if (adorner == null || clickable == null || !(adorner.ElementSet.PrimaryElement is BaseFrameworkElement))
        return;
      Matrix matrix = Matrix.Identity;
      Visual visual = adorner.ElementSet.PrimaryElement.ViewObject.PlatformSpecificObject as Visual;
      AdornerSet adornerSet = adorner.AdornerSet as AdornerSet;
      if (adornerSet != null)
      {
        matrix = adornerSet.Matrix;
        visual = (Visual) this.Artboard.AdornerLayer;
      }
      Point clickablePoint = clickable.GetClickablePoint(matrix);
      Rect rect = new Rect(clickablePoint.X - 3.0, clickablePoint.Y - 3.0, 6.0, 6.0);
      this.EnsureVisibleInternal(visual, rect, scrollNow);
    }

    public override void EnsureVisible(IViewObject element, bool scrollNow)
    {
      this.EnsureVisibleInternal(element.PlatformSpecificObject as Visual, new Rect(), scrollNow);
    }

    public override IDisposable EnsureCrossDocumentUpdateContext()
    {
      return (IDisposable) new WPFSceneView.CrossDocumentUpdateToken((SceneView) this);
    }

    public override bool IsMatrixTransform(IViewObject from, IViewObject to)
    {
      try
      {
        return this.ComputeTransformToVisual(from, to) is MatrixTransform;
      }
      catch (InvalidOperationException ex)
      {
        return false;
      }
    }

    public override Point TransformPoint(IViewObject from, IViewObject to, Point point)
    {
      try
      {
        return this.ComputeTransformToVisual(from, to).Transform(point);
      }
      catch (InvalidOperationException ex)
      {
        return point;
      }
    }

    public override Rect TransformBounds(IViewObject from, IViewObject to, Rect bounds)
    {
      try
      {
        return this.ComputeTransformToVisual(from, to).TransformBounds(bounds);
      }
      catch (InvalidOperationException ex)
      {
        return bounds;
      }
    }

    public override GeneralTransform ComputeTransformToVisual(IViewObject from, IViewObject to)
    {
      return this.ComputeTransformToVisual(from, to == null ? (Visual) null : to.PlatformSpecificObject as Visual);
    }

    public override GeneralTransform ComputeTransformToVisual(IViewObject from, Visual to)
    {
      Visual visual = from == null ? (Visual) null : from.PlatformSpecificObject as Visual;
      if (visual != null && to != null && (Visual) visual.FindCommonVisualAncestor((DependencyObject) to) != null)
      {
        GeneralTransform generalTransform = visual.TransformToVisual(to);
        if (generalTransform != null)
          return generalTransform;
      }
      return (GeneralTransform) Transform.Identity;
    }

    public override double GetDefinitionActualSize(IViewObject definition)
    {
      ColumnDefinition columnDefinition = definition.PlatformSpecificObject as ColumnDefinition;
      RowDefinition rowDefinition = definition.PlatformSpecificObject as RowDefinition;
      if (columnDefinition != null)
        return columnDefinition.ActualWidth;
      if (rowDefinition != null)
        return rowDefinition.ActualHeight;
      throw new ArgumentException();
    }

    public override Rect GetActualBounds(IViewObject element)
    {
      FrameworkElement element1 = element == null ? (FrameworkElement) null : element.PlatformSpecificObject as FrameworkElement;
      if (element1 == null)
        return new Rect();
      return ElementUtilities.GetActualBounds(element1);
    }

    public override Rect GetActualBoundsInParent(IViewObject element)
    {
      FrameworkElement element1 = element == null ? (FrameworkElement) null : element.PlatformSpecificObject as FrameworkElement;
      if (element1 == null)
        return new Rect();
      return ElementUtilities.GetActualBoundsInParent(element1);
    }

    public override Size GetRenderSize(IViewObject element)
    {
      UIElement uiElement = element.PlatformSpecificObject as UIElement;
      if (uiElement == null)
        return new Size();
      return uiElement.RenderSize;
    }

    public override Point PointToScreen(IViewObject relative, Point point)
    {
      Visual visual = relative.PlatformSpecificObject as Visual;
      if (visual != null)
        return visual.PointToScreen(point);
      return point;
    }

    public override Point PointFromScreen(IViewObject relative, Point point)
    {
      Visual visual = relative.PlatformSpecificObject as Visual;
      if (visual != null)
        return visual.PointFromScreen(point);
      return point;
    }

    public override object ConvertToWpfValue(object obj)
    {
      return obj;
    }

    public override object ConvertFromWpfValue(object obj)
    {
      return obj;
    }

    public override Microsoft.Expression.DesignModel.DocumentModel.DocumentNode ConvertToWpfValueAsDocumentNode(object obj)
    {
      return this.ViewModel.CreateSceneNode(obj).DocumentNode;
    }

    public override Microsoft.Expression.DesignModel.DocumentModel.DocumentNode ConvertFromWpfValueAsDocumentNode(object obj)
    {
      return this.ViewModel.CreateSceneNode(obj).DocumentNode;
    }

    public override System.Windows.Media.Geometry GetRenderedGeometryAsWpf(SceneElement shapeElement)
    {
      return ((Shape) shapeElement.ViewObject.PlatformSpecificObject).RenderedGeometry;
    }

    public override Viewport3DElement GetFirstHitViewport3D(Point point)
    {
      return this.hitTestHelper.PerformHitTestForViewport3D(new PointHitTestParameters(point));
    }

    public override void Activated()
    {
      base.Activated();
      if (!this.ShouldActivate)
        return;
      this.CodeEditor.Activated();
      this.RefreshErrors();
      this.RestoreFocusAsync();
      this.ProcessPostponedUpdate(false);
      this.UpdateReferences();
    }

    public override void EnsureActiveViewUpdated()
    {
      base.EnsureActiveViewUpdated();
      this.InvalidateAndUpdateApplicationResourceDictionary();
    }

    public override void RefreshApplicationResourceDictionary()
    {
      this.InvalidateAndUpdateApplicationResourceDictionary();
    }

    public override ImageCapturer GetCapturer(Size targetSize)
    {
      return (ImageCapturer) new FrameworkElementCapturer(this, targetSize);
    }

    protected override void AttachInternal(SceneView parent)
    {
      this.SwapRootNameScopes(parent as WPFSceneView);
      base.AttachInternal(parent);
    }

    protected override void DetachInternal(SceneView parent)
    {
      this.SwapRootNameScopes(parent as WPFSceneView);
      base.DetachInternal(parent);
    }

    private void SwapRootNameScopes(WPFSceneView parent)
    {
      if (parent == null)
        return;
      INameScope nameScope = this.rootNameScope;
      this.rootNameScope = parent.rootNameScope;
      parent.rootNameScope = nameScope;
    }

    protected override void UpdateInternal(bool updateInstances, bool updateReferences)
    {
      base.UpdateInternal(updateInstances, updateReferences);
      if (this.IsClosing || !updateInstances)
        return;
      this.InvalidateAndUpdateApplicationResourceDictionary();
    }

    protected override bool SetViewContentInternal(ViewContentType contentType, ViewNode target, object content)
    {
      FrameworkElement frameworkElement = (FrameworkElement) null;
      object obj = content;
      bool flag = false;
      if (contentType == ViewContentType.Content)
      {
        if (this.currentScopedInstance == content && this.currentContentType == ViewContentType.Content)
        {
          this.UpdateViewContentIfNecessary(false);
          return false;
        }
        if (target != null && content != null)
        {
          ViewContent viewContent = this.ProvideViewContent(target, content);
          contentType = viewContent.ViewContentType;
          content = viewContent.Content;
          flag = viewContent.IsSizeFixed;
          frameworkElement = content as FrameworkElement;
        }
        else
          contentType = ViewContentType.DefaultMessage;
        if (this.currentScopedInstance == null && content == null && this.currentContentType == ViewContentType.DefaultMessage)
          return false;
      }
      if (contentType == ViewContentType.Content)
      {
        this.currentScopedInstance = obj;
      }
      else
      {
        this.currentScopedInstance = (object) null;
        frameworkElement = (FrameworkElement) null;
      }
      this.currentContentType = contentType;
      this.ClearAdornerSets();
      if (flag && contentType == ViewContentType.Content && (!double.IsNaN(frameworkElement.Width) && !double.IsNaN(frameworkElement.Height)) || this.ViewModel.PreferredSize == Size.Empty)
      {
        this.Artboard.ContentArea.MinWidth = 20.0;
        this.Artboard.ContentArea.MinHeight = 20.0;
      }
      else
      {
        this.Artboard.ContentArea.MinWidth = this.ViewModel.PreferredSize.Width;
        this.Artboard.ContentArea.MinHeight = this.ViewModel.PreferredSize.Height;
      }
      if (frameworkElement != null)
      {
        this.Artboard.EditableContent = (FrameworkElement) null;
        this.SetApplicationResourceDictionary((ResourceDictionary) null);
        this.Artboard.EditableContent = frameworkElement;
      }
      else
        this.Artboard.SafelyRemoveEditableContent();
      this.Artboard.ContentArea.SetValue(TextElement.FontFamilyProperty, TextElement.FontFamilyProperty.DefaultMetadata.DefaultValue);
      this.Artboard.ContentArea.SetValue(TextElement.FontSizeProperty, TextElement.FontSizeProperty.DefaultMetadata.DefaultValue);
      switch (contentType)
      {
        case ViewContentType.DefaultMessage:
          this.IsDefaultMessageDisplayed = true;
          this.MessageContent = (object) null;
          break;
        case ViewContentType.Content:
          this.IsDefaultMessageDisplayed = false;
          this.MessageContent = (object) null;
          NameScope.SetNameScope((DependencyObject) frameworkElement, this.rootNameScope);
          break;
        default:
          this.IsDefaultMessageDisplayed = false;
          this.MessageContent = content;
          break;
      }
      return true;
    }

    protected override void SetApplicationResourceDictionary(ResourceDictionary resources)
    {
      try
      {
        this.Artboard.ResourcesHost.Resources = resources;
      }
      catch (Exception ex)
      {
        this.SetViewException(ex);
      }
    }

    public override void InvalidateAndUpdateApplicationResourceDictionary()
    {
      if (this.ShuttingDown)
        return;
      this.TrySetApplicationResourceDictionary((IEnumerable<ViewNode>) this.GetUpdatedApplicationResourceDictionaries());
    }

    private void TrySetApplicationResourceDictionary(IEnumerable<ViewNode> resources)
    {
      ResourceDictionary resources1 = this.Artboard.ResourcesHost.Resources;
      List<ResourceDictionary> list = new List<ResourceDictionary>();
      foreach (ViewNode viewNode in resources)
      {
        ResourceDictionary resourceDictionary = viewNode.Instance as ResourceDictionary;
        if (resourceDictionary != null)
          list.Add(resourceDictionary);
      }
      try
      {
        if (resources1 != null && Enumerable.SequenceEqual<ResourceDictionary>((IEnumerable<ResourceDictionary>) resources1.MergedDictionaries, (IEnumerable<ResourceDictionary>) list))
          return;
        if (resources1 != null)
        {
          resources1.Clear();
          resources1.MergedDictionaries.Clear();
        }
        else
          resources1 = new ResourceDictionary();
        foreach (ResourceDictionary resourceDictionary in list)
          resources1.MergedDictionaries.Add(resourceDictionary);
        this.SetApplicationResourceDictionary(resources1);
      }
      catch (Exception ex)
      {
        resources1.Clear();
        this.SetViewException(ex);
      }
    }

    protected override SceneView CreateSceneView()
    {
      return (SceneView) new WPFSceneView(this.viewHost, this.viewModel, false);
    }

    private class CrossDocumentUpdateToken : IDisposable
    {
      private IDisposable crossDocumentContextToken;
      private CrossDocumentUpdateContext updateContext;

      public CrossDocumentUpdateToken(SceneView sceneView)
      {
        this.updateContext = new CrossDocumentUpdateContext((IViewRootResolver) sceneView.DesignerContext.ViewRootResolver);
        this.crossDocumentContextToken = sceneView.InstanceBuilderContext.ChangeCrossDocumentUpdateContext((ICrossDocumentUpdateContext) this.updateContext);
        this.updateContext.BeginUpdate(false);
      }

      void IDisposable.Dispose()
      {
        if (this.updateContext != null)
        {
          this.updateContext.EndUpdate();
          this.updateContext = (CrossDocumentUpdateContext) null;
        }
        if (this.crossDocumentContextToken != null)
        {
          this.crossDocumentContextToken.Dispose();
          this.crossDocumentContextToken = (IDisposable) null;
        }
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
