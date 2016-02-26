// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SilverlightSceneView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  [DebuggerDisplay("{DebuggerDisplayValue}")]
  [Prototype]
  public sealed class SilverlightSceneView : SceneView
  {
    private IPlatformSpecificView platformSurface;
    private ImageHost imageHost;
    private bool isRootInstanceKnownInvalid;
    private int settingContentReentranceDepth;
    private bool disposedExceptionCaught;
    private bool catastrohpicFailureCaught;

    public override IViewVisual HitTestRoot
    {
      get
      {
        return this.hitTestRoot;
      }
    }

    public override IViewPanel OverlayLayer
    {
      get
      {
        return this.Platform.ViewObjectFactory.Instantiate(this.imageHost.OverlayLayer) as IViewPanel;
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
        if (sceneNode != null)
        {
          IViewObject correspondingViewObject = this.GetCorrespondingViewObject(sceneNode.DocumentNodePath);
          if (correspondingViewObject != null)
            return correspondingViewObject.PlatformSpecificObject;
        }
        return (object) null;
      }
    }

    public SilverlightSceneView(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewHost, viewModel, true)
    {
    }

    private SilverlightSceneView(ISceneViewHost viewHost, SceneViewModel viewModel, bool isRootView)
      : base(viewHost, viewModel, isRootView)
    {
    }

    protected override INameScope CreateNameScope()
    {
      return (INameScope) new SilverlightNameScope();
    }

    protected override Artboard CreateArtboard()
    {
      this.platformSurface = this.viewModel.ProjectContext.Platform.CreateSurface();
      this.platformSurface.UnhandledException += new UnhandledExceptionEventHandler(this.SilverlightImageHost_UnhandledException);
      this.imageHost = this.platformSurface.CreateImageHost();
      this.imageHost.DocumentSizeChanged += new EventHandler(this.ImageHost_DocumentSizeChanged);
      this.Document.DocumentRoot.TypesChanged += new EventHandler(this.DocumentRoot_TypesChanged);
      SilverlightArtboard silverlightArtboard = new SilverlightArtboard(this.platformSurface, this.Platform.ViewObjectFactory, new ViewExceptionCallback(((SceneView) this).OnExceptionWithUnknownSource), this.imageHost);
      this.platformSurface.BaseUri = this.ViewModel.ProjectContext.MakeDesignTimeUri(new Uri("/", UriKind.Relative), (string) null).OriginalString;
      return (Artboard) silverlightArtboard;
    }

    private void ImageHost_DocumentSizeChanged(object sender, EventArgs e)
    {
      this.UpdateRootBounds();
    }

    private void DocumentRoot_TypesChanged(object sender, EventArgs e)
    {
      this.imageHost.Redraw(true);
    }

    [Conditional("DEBUG")]
    public void TakeImageHostSnapshot()
    {
      MethodInfo method = this.imageHost.GetType().GetMethod("TakeSnapshot");
      if (!(method != (MethodInfo) null))
        return;
      method.Invoke((object) this.imageHost, (object[]) null);
    }

    public override IDisposable EnsureCrossDocumentUpdateContext()
    {
      return (IDisposable) new SilverlightSceneView.CrossDocumentUpdateToken(this);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Document.DocumentRoot.TypesChanged -= new EventHandler(this.DocumentRoot_TypesChanged);
        base.Dispose(disposing);
        if (this.imageHost != null)
        {
          this.imageHost.SizeChanged -= new SizeChangedEventHandler(this.ImageHost_DocumentSizeChanged);
          this.imageHost.Dispose();
          this.imageHost = (ImageHost) null;
        }
        if (this.platformSurface == null)
          return;
        this.platformSurface.UnhandledException -= new UnhandledExceptionEventHandler(this.SilverlightImageHost_UnhandledException);
        this.platformSurface.Dispose();
        this.platformSurface = (IPlatformSpecificView) null;
      }
      else
        base.Dispose(disposing);
    }

    private void SilverlightImageHost_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
      if (this.IsClosing || this.settingContentReentranceDepth <= 0 && sender != this.imageHost.Child)
        return;
      this.OnExceptionWithUnknownSource(args.ExceptionObject as Exception);
    }

    public override Matrix GetComputedTransformToRootVerified(IViewObject element)
    {
      return this.platformSurface.TransformToVisualVerified(element, (IViewObject) this.HitTestRoot);
    }

    public override Matrix GetComputedTransformToRoot(IViewObject element)
    {
      return this.platformSurface.TransformToVisual(element, (IViewObject) this.HitTestRoot);
    }

    public override Matrix GetComputedTransformFromRoot(IViewObject element)
    {
      return this.platformSurface.TransformToVisual((IViewObject) this.HitTestRoot, element);
    }

    public override void EnsureVisible(IAdorner adorner, bool scrollNow)
    {
      IClickable clickable = adorner as IClickable;
      if (adorner == null || clickable == null || !(adorner.ElementSet.PrimaryElement is BaseFrameworkElement))
        return;
      Matrix identity = Matrix.Identity;
      AdornerSet adornerSet = adorner.AdornerSet as AdornerSet;
      Matrix matrix;
      Visual visual;
      if (adornerSet != null)
      {
        matrix = adornerSet.Matrix;
        visual = (Visual) this.Artboard.AdornerLayer;
      }
      else
      {
        matrix = this.platformSurface.TransformToVisual(adorner.ElementSet.PrimaryElement.ViewObject, (IViewObject) this.HitTestRoot);
        visual = (Visual) this.ViewRootContainer;
      }
      Point clickablePoint = clickable.GetClickablePoint(matrix);
      Rect rect = new Rect(clickablePoint.X - 3.0, clickablePoint.Y - 3.0, 6.0, 6.0);
      this.EnsureVisibleInternal(visual, rect, scrollNow);
    }

    public override void EnsureVisible(IViewObject element, bool scrollNow)
    {
      Rect actualBounds = this.GetActualBounds(element);
      if (actualBounds.IsEmpty)
        return;
      this.EnsureVisibleInternal((Visual) this.ViewRootContainer, this.platformSurface.TransformBounds(element, (IViewObject) this.HitTestRoot, actualBounds), scrollNow);
    }

    protected override void UpdateLayoutInternal()
    {
      SceneNode sceneNode = this.allowViewScoping ? this.viewModel.ViewRoot : this.viewModel.RootNode;
      if (sceneNode == null)
        return;
      IViewVisual viewVisual = this.GetCorrespondingViewObject(sceneNode.DocumentNodePath) as IViewVisual;
      if (viewVisual == null)
        return;
      viewVisual.UpdateLayout();
    }

    public override bool IsMatrixTransform(IViewObject from, IViewObject to)
    {
      return this.platformSurface.IsMatrixTransform(from, to);
    }

    public override Point TransformPoint(IViewObject from, IViewObject to, Point point)
    {
      return this.platformSurface.TransformPoint(from, to, point);
    }

    public override Rect TransformBounds(IViewObject from, IViewObject to, Rect bounds)
    {
      return this.platformSurface.TransformBounds(from, to, bounds);
    }

    public override GeneralTransform ComputeTransformToVisual(IViewObject from, IViewObject to)
    {
      return (GeneralTransform) new MatrixTransform(this.platformSurface.TransformToVisual(from, to));
    }

    public override GeneralTransform ComputeTransformToVisual(IViewObject from, Visual to)
    {
      GeneralTransform generalTransform = this.ViewRootContainer.TransformToVisual(to);
      return (GeneralTransform) new MatrixTransform(this.platformSurface.TransformToVisual(from, (IViewObject) this.HitTestRoot) * VectorUtilities.GetMatrixFromTransform(generalTransform));
    }

    public override double GetDefinitionActualSize(IViewObject definition)
    {
      return this.platformSurface.GetDefinitionActualSize(definition.PlatformSpecificObject);
    }

    public override Rect GetActualBounds(IViewObject element)
    {
      return this.platformSurface.GetActualBounds(element);
    }

    public override Rect GetActualBoundsInParent(IViewObject element)
    {
      return this.platformSurface.GetActualBoundsInParent(element);
    }

    public override Size GetRenderSize(IViewObject element)
    {
      return this.platformSurface.GetActualBounds(element).Size;
    }

    public override Point PointToScreen(IViewObject relative, Point point)
    {
      return this.ViewRootContainer.PointToScreen(this.platformSurface.TransformToVisual(relative, (IViewObject) this.HitTestRoot).Transform(point));
    }

    public override Point PointFromScreen(IViewObject relative, Point point)
    {
      return this.platformSurface.TransformToVisual((IViewObject) this.HitTestRoot, relative).Transform(this.ViewRootContainer.PointFromScreen(point));
    }

    public override object ConvertToWpfValue(object obj)
    {
      return this.ViewModel.DesignerContext.PlatformConverter.ConvertToWpf(this.Document.DocumentContext, obj);
    }

    public override object ConvertFromWpfValue(object obj)
    {
      return this.ViewModel.DesignerContext.PlatformConverter.ConvertToSilverlight(this.Document.DocumentContext, obj);
    }

    public override DocumentNode ConvertToWpfValueAsDocumentNode(object obj)
    {
      return this.ViewModel.DesignerContext.PlatformConverter.ConvertToWpfAsDocumentNode(this.Document.DocumentContext, obj);
    }

    public override DocumentNode ConvertFromWpfValueAsDocumentNode(object obj)
    {
      return this.ViewModel.DesignerContext.PlatformConverter.ConvertToSilverlightAsDocumentNode(this.Document.DocumentContext, obj);
    }

    public override System.Windows.Media.Geometry GetRenderedGeometryAsWpf(SceneElement shapeElement)
    {
      IViewObject element = shapeElement != null ? shapeElement.ViewObject : (IViewObject) null;
      if (element == null)
          return (System.Windows.Media.Geometry)new PathGeometry();
      Rect actualBounds = this.GetActualBounds(element);
      if (!actualBounds.IsEmpty && (Brush) shapeElement.GetComputedValueAsWpf(ShapeElement.StrokeProperty) != null)
      {
        double num1 = (double) shapeElement.GetComputedValue(ShapeElement.StrokeThicknessProperty);
        if (!double.IsNaN(num1) && !double.IsInfinity(num1))
        {
          double num2 = Math.Min(actualBounds.Width, num1) / 2.0;
          double num3 = Math.Min(actualBounds.Height, num1) / 2.0;
          actualBounds.Inflate(-num2, -num3);
        }
      }
      System.Windows.Media.Geometry geometry = this.platformSurface.GetShapeRenderedGeometry(element.PlatformSpecificObject, actualBounds);
      if (geometry == null)
      {
        if (ProjectNeutralTypes.PrimitiveShape.IsAssignableFrom((ITypeId) shapeElement.Type))
            geometry = shapeElement.GetComputedValueAsWpf(PrimitiveShapeElement.RenderedGeometryProperty) as System.Windows.Media.Geometry;
        else if (PlatformTypes.Path.IsAssignableFrom((ITypeId) shapeElement.Type))
            geometry = shapeElement.GetComputedValueAsWpf(PathElement.DataProperty) as System.Windows.Media.Geometry;
      }
      if (geometry == null)
          geometry = (System.Windows.Media.Geometry)new PathGeometry();
      if (!PlatformTypes.Rectangle.Equals((object) shapeElement.Type) && !PlatformTypes.Ellipse.Equals((object) shapeElement.Type))
      {
        MatrixTransform matrixTransform = shapeElement.GetComputedValueAsWpf(ShapeElement.GeometryTransformProperty) as MatrixTransform;
        if (matrixTransform != null && !matrixTransform.Matrix.IsIdentity)
        {
          Matrix matrix = geometry.Transform != null ? geometry.Transform.Value * matrixTransform.Matrix : matrixTransform.Matrix;
          geometry.Transform = (Transform) new MatrixTransform(matrix);
        }
      }
      return geometry;
    }

    public override GeneralTransform GetComputedTransformToHitArea()
    {
      return ((SilverlightArtboard) this.Artboard).SilverlightRootTransform;
    }

    public override Viewport3DElement GetFirstHitViewport3D(Point point)
    {
      return (Viewport3DElement) null;
    }

    public override SceneElement GetElementAtPoint(Point point, HitTestModifier hitTestModifier, InvisibleObjectHitTestModifier invisibleObjectHitTestModifier, ICollection<BaseFrameworkElement> ignoredElements)
    {
      if (this.imageHost.RootInstance == null)
        return (SceneElement) null;
      Point point1 = point;
      GeneralTransform transformToHitArea = this.GetComputedTransformToHitArea();
      if (transformToHitArea != null)
        point1 = transformToHitArea.Transform(point);
      return base.GetElementAtPoint(point1, hitTestModifier, invisibleObjectHitTestModifier, ignoredElements);
    }

    public override IList<SceneElement> GetElementsInRectangle(Rect rectangle, HitTestModifier hitTestModifier, InvisibleObjectHitTestModifier invisibleObjectHitTestModifier, bool skipFullyContainedSelectionInObject)
    {
      GeneralTransform transformToHitArea = this.GetComputedTransformToHitArea();
      if (transformToHitArea != null)
        rectangle = new Rect(transformToHitArea.Transform(rectangle.TopLeft), transformToHitArea.Transform(rectangle.BottomRight));
      return base.GetElementsInRectangle(rectangle, hitTestModifier, invisibleObjectHitTestModifier, skipFullyContainedSelectionInObject);
    }

    public override void Activated()
    {
      base.Activated();
      if (!this.ShouldActivate)
        return;
      this.imageHost.Activate();
      this.CodeEditor.Activated();
      this.RefreshErrors();
      this.platformSurface.BaseUri = this.ViewModel.ProjectContext.MakeDesignTimeUri(new Uri("/", UriKind.Relative), (string) null).OriginalString;
      this.RestoreFocusAsync();
    }

    public override void EnsureActiveViewUpdated()
    {
      if (!this.IsClosing && this.postponedUpdate == SceneView.PostponedUpdate.UpdateInvalidated)
        this.imageHost.Activate();
      base.EnsureActiveViewUpdated();
      if (this.IsClosing)
        return;
      this.EnsureApplicationResourceDictionaryIsCurrent();
      this.UpdateLayout();
    }

    public override void Deactivated()
    {
      base.Deactivated();
      this.imageHost.Deactivate();
    }

    public override ImageCapturer GetCapturer(Size targetSize)
    {
      return (ImageCapturer) new VisualCapturer(this, targetSize);
    }

    protected override void AttachInternal(SceneView parent)
    {
      SilverlightSceneView silverlightSceneView = parent as SilverlightSceneView;
      if (silverlightSceneView != null)
        this.imageHost.Attach(silverlightSceneView.imageHost);
      base.AttachInternal(parent);
    }

    protected override void DetachInternal(SceneView parent)
    {
      SilverlightSceneView silverlightSceneView = parent as SilverlightSceneView;
      if (silverlightSceneView != null)
        this.imageHost.Detach(silverlightSceneView.imageHost);
      base.DetachInternal(parent);
    }

    protected override bool ShouldFilterViewException(Exception exception)
    {
        ObjectDisposedException objectDisposedException = exception as ObjectDisposedException;
        if (objectDisposedException != null && string.Equals(objectDisposedException.ObjectName, "NativeObject", StringComparison.Ordinal))
        {
            if (!this.disposedExceptionCaught)
            {
                base.DesignerContext.PlatformContextChanger.UpdateForPlatformChange(this, null, true, true);
            }
            this.disposedExceptionCaught = true;
        }
        if (!this.IsExceptionCatastrophicFailure(exception.Message) && (exception.InnerException == null || !this.IsExceptionCatastrophicFailure(exception.InnerException.Message)) || this.catastrohpicFailureCaught)
        {
            return base.ShouldFilterViewException(exception);
        }
        UIThreadDispatcherHelper.BeginInvoke(DispatcherPriority.ContextIdle, new DispatcherOperationCallback((object o) =>
        {
            base.UpdateLayout();
            return null;
        }), null);
        this.catastrohpicFailureCaught = true;
        return true;
    }

    private FormattedException FilterCatastrophicFailure(FormattedException formattedException)
    {
      if (this.IsExceptionCatastrophicFailure(formattedException.Message) || formattedException.InnerException != null && this.IsExceptionCatastrophicFailure(formattedException.InnerException.Message))
        formattedException = this.ExceptionFormatter.Format(new Exception(ExceptionStringTable.SilverlightSceneViewEUnexpectedRebuild, formattedException.SourceException));
      return formattedException;
    }

    private bool IsExceptionCatastrophicFailure(string exceptionMessage)
    {
      return exceptionMessage.Contains("8000FFFF");
    }

    protected override bool SetViewContentInternal(ViewContentType contentType, ViewNode target, object content)
    {
      if (this.settingContentReentranceDepth > 1)
        return false;
      if (contentType == ViewContentType.Error)
      {
        FormattedException formattedException = content as FormattedException;
        if (formattedException != null)
          content = (object) this.FilterCatastrophicFailure(formattedException);
      }
      try
      {
        ++this.settingContentReentranceDepth;
        return this.SetViewContentWorker(contentType, target, content);
      }
      finally
      {
        --this.settingContentReentranceDepth;
      }
    }

    private bool SetViewContentWorker(ViewContentType contentType, ViewNode target, object content)
    {
      object obj = content;
      if (contentType == ViewContentType.Content)
      {
        if (!this.isRootInstanceKnownInvalid && this.postponedUpdate == SceneView.PostponedUpdate.None && (this.currentScopedInstance == content && this.currentContentType == ViewContentType.Content))
        {
          this.ApplyNameScope(this.rootNameScope as SilverlightNameScope);
          this.UpdateViewContentIfNecessary(false);
          return false;
        }
        if (content != null)
        {
          ViewContent viewContent = this.ProvideViewContent(target, content);
          contentType = viewContent.ViewContentType;
          content = viewContent.Content;
          if (content != null && contentType == ViewContentType.Content && !this.Document.ProjectContext.ResolveType(PlatformTypes.FrameworkElement).RuntimeType.IsAssignableFrom(content.GetType()))
            contentType = ViewContentType.DefaultMessage;
        }
        else
          contentType = ViewContentType.DefaultMessage;
        if (!this.isRootInstanceKnownInvalid && this.currentScopedInstance == null && (content == null && this.currentContentType == ViewContentType.DefaultMessage))
          return false;
      }
      ViewContentType viewContentType = this.currentContentType;
      this.currentContentType = contentType;
      if (contentType == ViewContentType.Content)
      {
        this.currentScopedInstance = obj;
        this.imageHost.RootInstance = content;
        this.hitTestRoot = this.Platform.ViewObjectFactory.Instantiate(this.imageHost.HitTestRoot) as IViewVisual;
        if (this.currentContentType != contentType)
          return true;
        this.isRootInstanceKnownInvalid = false;
        if (viewContentType != ViewContentType.Content)
          this.imageHost.Redraw(true);
        this.UpdateRootBounds();
        this.UpdateLayout();
        this.disposedExceptionCaught = false;
        this.catastrohpicFailureCaught = false;
      }
      else
      {
        this.imageHost.RootInstance = (object) null;
        this.currentScopedInstance = (object) null;
      }
      this.ClearAdornerSets();
      bool flag = false;
      if (contentType == ViewContentType.Content)
      {
        ReferenceStep referenceStep1 = this.ProjectContext.ResolveProperty(BaseFrameworkElement.ActualWidthProperty) as ReferenceStep;
        ReferenceStep referenceStep2 = this.ProjectContext.ResolveProperty(BaseFrameworkElement.ActualHeightProperty) as ReferenceStep;
        double num1 = (double) referenceStep1.GetValue(content);
        double num2 = (double) referenceStep2.GetValue(content);
        if (!double.IsNaN(num1) && !double.IsNaN(num2))
        {
          this.SetContentSize(new Size(num1, num2));
          flag = true;
        }
      }
      if (!flag)
      {
        if (this.ViewModel.PreferredSize != Size.Empty)
          this.SetContentSize(new Size(this.ViewModel.PreferredSize.Width, this.ViewModel.PreferredSize.Height));
        else
          this.SetContentSize(new Size(20.0, 20.0));
      }
      switch (contentType)
      {
        case ViewContentType.DefaultMessage:
          this.IsDefaultMessageDisplayed = true;
          this.MessageContent = (object) null;
          break;
        case ViewContentType.Content:
          this.IsDefaultMessageDisplayed = false;
          this.MessageContent = (object) null;
          this.ApplyNameScope(this.rootNameScope as SilverlightNameScope);
          break;
        default:
          this.IsDefaultMessageDisplayed = false;
          this.MessageContent = content;
          break;
      }
      return true;
    }

    private void SetContentSize(Size size)
    {
      this.Artboard.ContentArea.Width = size.Width;
      this.Artboard.ContentArea.Height = size.Height;
    }

    private void ApplyNameScope(SilverlightNameScope nameScope)
    {
      foreach (KeyValuePair<string, object> keyValuePair in nameScope.InternalDictionary)
        this.platformSurface.SetName(keyValuePair.Value, keyValuePair.Key);
    }

    private void UpdateRootBounds()
    {
      IViewObject viewRoot = this.ViewRoot;
      Rect rect = viewRoot != null ? this.GetActualBounds(viewRoot) : Rect.Empty;
      if (!rect.IsEmpty)
      {
        this.Artboard.ContentArea.Width = rect.Width;
        this.Artboard.ContentArea.Height = rect.Height;
      }
      else
      {
        this.Artboard.ContentArea.Width = 0.0;
        this.Artboard.ContentArea.Height = 0.0;
      }
      this.Artboard.InvalidateArrange();
    }

    private void EnsureApplicationResourceDictionaryIsCurrent()
    {
      if (this.ShuttingDown)
        return;
      this.imageHost.TrySetApplicationResourceDictionary(this.InstanceBuilderContext, (IEnumerable<ViewNode>) this.GetUpdatedApplicationResourceDictionaries());
    }

    protected override void SetApplicationResourceDictionary(ResourceDictionary resources)
    {
    }

    public override void InvalidateAndUpdateApplicationResourceDictionary()
    {
    }

    public override void RefreshApplicationResourceDictionary()
    {
      this.EnsureApplicationResourceDictionaryIsCurrent();
    }

    protected override SceneView CreateSceneView()
    {
      this.isRootInstanceKnownInvalid = true;
      return (SceneView) new SilverlightSceneView(this.viewHost, this.viewModel, false);
    }

    protected override void UpdateInternal(bool updateInstances, bool updateReferences)
    {
      if (this.IsClosing)
        return;
      if (updateInstances)
        this.EnsureApplicationResourceDictionaryIsCurrent();
      base.UpdateInternal(updateInstances, updateReferences);
      if (!updateReferences)
        return;
      this.imageHost.Redraw(false);
    }

    public override void ShutdownVisualTree()
    {
      base.ShutdownVisualTree();
      this.imageHost.ShutdownVisualTree();
      SilverlightNameScope silverlightNameScope = this.rootNameScope as SilverlightNameScope;
      if (silverlightNameScope != null)
        silverlightNameScope.InternalDictionary.Clear();
      DesignSurfacePlatformCaches.GetOrCreateCache<Dictionary<string, List<IEasingFunctionDefinition>>>(this.Platform.Metadata, DesignSurfacePlatformCaches.EasingFunctionsCache).Clear();
    }

    protected override void ActivateHost()
    {
      this.imageHost.Activate();
    }

    protected override void DeactivateHost()
    {
      this.imageHost.Deactivate();
    }

    private class CrossDocumentUpdateToken : IDisposable
    {
      private IDisposable appResourcesHostToken;
      private IDisposable crossDocumentContextToken;
      private IResourceDictionaryHost resourcesHost;
      private CrossDocumentUpdateContext updateContext;

      public CrossDocumentUpdateToken(SilverlightSceneView sceneView)
      {
        bool shouldDelay = !sceneView.IsEditingOutOfPlace && sceneView.Document == sceneView.Document.ApplicationSceneDocument;
        if (shouldDelay)
        {
          this.resourcesHost = (IResourceDictionaryHost) sceneView.imageHost;
          this.appResourcesHostToken = sceneView.InstanceBuilderContext.ChangeResourceDictionaryHost(this.resourcesHost);
          this.resourcesHost.BeginInstanceBuilding(sceneView.InstanceBuilderContext);
        }
        this.updateContext = new CrossDocumentUpdateContext((IViewRootResolver) sceneView.DesignerContext.ViewRootResolver);
        this.crossDocumentContextToken = sceneView.InstanceBuilderContext.ChangeCrossDocumentUpdateContext((ICrossDocumentUpdateContext) this.updateContext);
        this.updateContext.BeginUpdate(shouldDelay);
      }

      void IDisposable.Dispose()
      {
        if (this.appResourcesHostToken != null)
        {
          this.appResourcesHostToken.Dispose();
          this.appResourcesHostToken = (IDisposable) null;
        }
        if (this.resourcesHost != null)
        {
          this.resourcesHost.EndInstanceBuilding();
          this.resourcesHost = (IResourceDictionaryHost) null;
        }
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
