// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class SceneNodeFactory
  {
    private SceneNodeFactory.SceneNodeTypeHandlerFactory typeHandlerFactory = new SceneNodeFactory.SceneNodeTypeHandlerFactory();

    public SceneNode Instantiate(SceneViewModel viewModel, DocumentNode node)
    {
      return this.typeHandlerFactory.GetSceneNodeFactory((ITypeResolver) viewModel.ProjectContext, node.Type).Instantiate(viewModel, node);
    }

    public SceneNode Instantiate(SceneViewModel viewModel, ITypeId targetType)
    {
      IType type = viewModel.ProjectContext.ResolveType(targetType);
      return this.typeHandlerFactory.GetSceneNodeFactory((ITypeResolver) viewModel.ProjectContext, type).Instantiate(viewModel, (ITypeId) type);
    }

    private sealed class SceneNodeTypeHandlerFactory : TypeIdHandlerFactory<SceneNodeFactory.SceneNodeTypeHandler>
    {
      public SceneNode.ConcreteSceneNodeFactory GetSceneNodeFactory(ITypeResolver typeResolver, IType type)
      {
        return this.GetHandler((IMetadataResolver) typeResolver, type).Factory;
      }

      protected override void Initialize()
      {
        base.Initialize();
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.UIElement, (SceneNode.ConcreteSceneNodeFactory) Base2DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.FrameworkElement, (SceneNode.ConcreteSceneNodeFactory) BaseFrameworkElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.AccessText, (SceneNode.ConcreteSceneNodeFactory) AccessTextElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Binding, (SceneNode.ConcreteSceneNodeFactory) BindingSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.BitmapImage, (SceneNode.ConcreteSceneNodeFactory) BitmapImageNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Border, (SceneNode.ConcreteSceneNodeFactory) BorderElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Brush, (SceneNode.ConcreteSceneNodeFactory) BrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Transform, (SceneNode.ConcreteSceneNodeFactory) TransformNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TransformGroup, (SceneNode.ConcreteSceneNodeFactory) TransformGroupNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.RotateTransform, (SceneNode.ConcreteSceneNodeFactory) RotateTransformNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ScaleTransform, (SceneNode.ConcreteSceneNodeFactory) ScaleTransformNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.SkewTransform, (SceneNode.ConcreteSceneNodeFactory) SkewTransformNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TranslateTransform, (SceneNode.ConcreteSceneNodeFactory) TranslateTransformNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.CompositeTransform, (SceneNode.ConcreteSceneNodeFactory) CompositeTransformNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.SolidColorBrush, (SceneNode.ConcreteSceneNodeFactory) SolidColorBrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.GradientBrush, (SceneNode.ConcreteSceneNodeFactory) GradientBrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.GradientStop, (SceneNode.ConcreteSceneNodeFactory) GradientStopNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.GradientStopCollection, (SceneNode.ConcreteSceneNodeFactory) GradientStopCollectionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.LinearGradientBrush, (SceneNode.ConcreteSceneNodeFactory) LinearGradientBrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.RadialGradientBrush, (SceneNode.ConcreteSceneNodeFactory) RadialGradientBrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TileBrush, (SceneNode.ConcreteSceneNodeFactory) TileBrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Canvas, (SceneNode.ConcreteSceneNodeFactory) CanvasElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Camera, SceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ColumnDefinition, (SceneNode.ConcreteSceneNodeFactory) ColumnDefinitionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ComboBox, (SceneNode.ConcreteSceneNodeFactory) ComboBoxElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Condition, (SceneNode.ConcreteSceneNodeFactory) ConditionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Control, (SceneNode.ConcreteSceneNodeFactory) ControlElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ContentControl, (SceneNode.ConcreteSceneNodeFactory) ContentControlElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ContentPresenter, (SceneNode.ConcreteSceneNodeFactory) ContentPresenterElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ContextMenu, (SceneNode.ConcreteSceneNodeFactory) ContextMenuElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ControlTemplate, (SceneNode.ConcreteSceneNodeFactory) ControlTemplateElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.Expander, (SceneNode.ConcreteSceneNodeFactory) ExpanderElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.FrameworkTemplate, (SceneNode.ConcreteSceneNodeFactory) FrameworkTemplateElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DataTemplate, (SceneNode.ConcreteSceneNodeFactory) DataTemplateElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.DataGrid, DataGridElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.DataGridColumn, DataGridColumnNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Decorator, (SceneNode.ConcreteSceneNodeFactory) DecoratorElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ResourceDictionary, (SceneNode.ConcreteSceneNodeFactory) ResourceDictionaryNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DictionaryEntry, (SceneNode.ConcreteSceneNodeFactory) DictionaryEntryNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.DockPanel, (SceneNode.ConcreteSceneNodeFactory) DockPanelElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.StackPanel, (SceneNode.ConcreteSceneNodeFactory) StackPanelElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.GeometryModel3D, (SceneNode.ConcreteSceneNodeFactory) GeometryModel3DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.FlowDocumentScrollViewer, (SceneNode.ConcreteSceneNodeFactory) FlowDocumentScrollViewerElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Grid, (SceneNode.ConcreteSceneNodeFactory) GridElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.GridView, (SceneNode.ConcreteSceneNodeFactory) GridViewElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TextBlock, (SceneNode.ConcreteSceneNodeFactory) TextBlockElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ItemsControl, (SceneNode.ConcreteSceneNodeFactory) ItemsControlElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.FlowDocument, (SceneNode.ConcreteSceneNodeFactory) FlowDocumentElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Inline, (SceneNode.ConcreteSceneNodeFactory) TextElementSceneElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Run, RunElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.InlineUIContainer, (SceneNode.ConcreteSceneNodeFactory) InlineUIContainerElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ImageBrush, (SceneNode.ConcreteSceneNodeFactory) ImageBrushNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ObjectDataProvider, (SceneNode.ConcreteSceneNodeFactory) ObjectDataProviderSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Paragraph, (SceneNode.ConcreteSceneNodeFactory) ParagraphElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TextElement, (SceneNode.ConcreteSceneNodeFactory) TextElementSceneElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Thickness, (SceneNode.ConcreteSceneNodeFactory) ThicknessNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ToolBar, (SceneNode.ConcreteSceneNodeFactory) ToolBarElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.TreeViewItem, (SceneNode.ConcreteSceneNodeFactory) TreeViewItemElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.AmbientLight, (SceneNode.ConcreteSceneNodeFactory) AmbientLightElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DiffuseMaterial, (SceneNode.ConcreteSceneNodeFactory) DiffuseMaterialNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DirectionalLight, (SceneNode.ConcreteSceneNodeFactory) DirectionalLightElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PointLight, (SceneNode.ConcreteSceneNodeFactory) PointLightElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.SpotLight, (SceneNode.ConcreteSceneNodeFactory) SpotLightElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MaterialGroup, (SceneNode.ConcreteSceneNodeFactory) MaterialGroupNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Material, (SceneNode.ConcreteSceneNodeFactory) MaterialNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MediaElement, (SceneNode.ConcreteSceneNodeFactory) MediaSceneElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MenuItem, (SceneNode.ConcreteSceneNodeFactory) MenuItemElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Model3DGroup, (SceneNode.ConcreteSceneNodeFactory) Model3DGroupElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ModelVisual3D, (SceneNode.ConcreteSceneNodeFactory) ModelVisual3DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ModelUIElement3D, (SceneNode.ConcreteSceneNodeFactory) ModelUIElement3DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ContainerUIElement3D, (SceneNode.ConcreteSceneNodeFactory) ContainerUIElement3DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Viewport2DVisual3D, (SceneNode.ConcreteSceneNodeFactory) Viewport2DVisual3DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MultiTrigger, (SceneNode.ConcreteSceneNodeFactory) MultiTriggerNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Image, (SceneNode.ConcreteSceneNodeFactory) ImageElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Page, (SceneNode.ConcreteSceneNodeFactory) PageElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Panel, (SceneNode.ConcreteSceneNodeFactory) PanelElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Path, (SceneNode.ConcreteSceneNodeFactory) PathElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PerspectiveCamera, (SceneNode.ConcreteSceneNodeFactory) PerspectiveCameraElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.OrthographicCamera, (SceneNode.ConcreteSceneNodeFactory) OrthographicCameraElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MatrixCamera, (SceneNode.ConcreteSceneNodeFactory) MatrixCameraElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Popup, (SceneNode.ConcreteSceneNodeFactory) PopupElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Trigger, (SceneNode.ConcreteSceneNodeFactory) TriggerNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Rectangle, (SceneNode.ConcreteSceneNodeFactory) RectangleElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.RowDefinition, (SceneNode.ConcreteSceneNodeFactory) RowDefinitionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Application, (SceneNode.ConcreteSceneNodeFactory) ApplicationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Object, SceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Setter, (SceneNode.ConcreteSceneNodeFactory) SetterSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Shape, (SceneNode.ConcreteSceneNodeFactory) ShapeElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.PrimitiveShape, PrimitiveShapeElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.CompositeShape, CompositeShapeElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.CompositeContentShape, CompositeContentShapeElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.Arc, ArcShapeElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Style, (SceneNode.ConcreteSceneNodeFactory) StyleNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TextBox, (SceneNode.ConcreteSceneNodeFactory) TextBoxElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.RichTextBox, (SceneNode.ConcreteSceneNodeFactory) RichTextBoxElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Viewport3D, (SceneNode.ConcreteSceneNodeFactory) Viewport3DElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TriggerBase, (SceneNode.ConcreteSceneNodeFactory) TriggerBaseNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Storyboard, (SceneNode.ConcreteSceneNodeFactory) StoryboardTimelineSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Timeline, (SceneNode.ConcreteSceneNodeFactory) TimelineSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MediaTimeline, (SceneNode.ConcreteSceneNodeFactory) MediaTimelineSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.IKeyFrame, (SceneNode.ConcreteSceneNodeFactory) KeyFrameSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.IKeyFrameAnimation, (SceneNode.ConcreteSceneNodeFactory) KeyFrameAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.AnimationTimeline, (SceneNode.ConcreteSceneNodeFactory) AnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DoubleAnimationUsingPath, (SceneNode.ConcreteSceneNodeFactory) PathAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PointAnimationUsingPath, (SceneNode.ConcreteSceneNodeFactory) PathAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.MatrixAnimationUsingPath, (SceneNode.ConcreteSceneNodeFactory) PathAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Window, (SceneNode.ConcreteSceneNodeFactory) WindowElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.EventTrigger, (SceneNode.ConcreteSceneNodeFactory) EventTriggerNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.TriggerAction, (SceneNode.ConcreteSceneNodeFactory) TriggerActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.BeginStoryboard, (SceneNode.ConcreteSceneNodeFactory) BeginActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PauseStoryboard, (SceneNode.ConcreteSceneNodeFactory) PauseActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ResumeStoryboard, (SceneNode.ConcreteSceneNodeFactory) ResumeActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.SkipStoryboardToFill, (SceneNode.ConcreteSceneNodeFactory) SkipToFillActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.StopStoryboard, (SceneNode.ConcreteSceneNodeFactory) StopActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.RemoveStoryboard, (SceneNode.ConcreteSceneNodeFactory) RemoveActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Uri, (SceneNode.ConcreteSceneNodeFactory) UriNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.Viewbox, (SceneNode.ConcreteSceneNodeFactory) ViewboxElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.VisualState, (SceneNode.ConcreteSceneNodeFactory) VisualStateSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.VisualStateGroup, (SceneNode.ConcreteSceneNodeFactory) VisualStateGroupSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.VisualStateManager, (SceneNode.ConcreteSceneNodeFactory) VisualStateManagerSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.VisualTransition, (SceneNode.ConcreteSceneNodeFactory) VisualStateTransitionSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.WrapPanel, (SceneNode.ConcreteSceneNodeFactory) WrapPanelElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.XmlDataProvider, (SceneNode.ConcreteSceneNodeFactory) XmlDataProviderSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Effect, (SceneNode.ConcreteSceneNodeFactory) EffectNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ByteAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ColorAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DecimalAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DoubleAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Int16Animation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Int32Animation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Int64Animation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Point3DAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PointAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Rotation3DAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.RectAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.SingleAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.SizeAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ThicknessAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Vector3DAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.VectorAnimation, (SceneNode.ConcreteSceneNodeFactory) FromToAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ColorAnimationUsingKeyFrames, (SceneNode.ConcreteSceneNodeFactory) KeyFrameAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ColorKeyFrame, (SceneNode.ConcreteSceneNodeFactory) KeyFrameSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DoubleAnimationUsingKeyFrames, (SceneNode.ConcreteSceneNodeFactory) KeyFrameAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.DoubleKeyFrame, (SceneNode.ConcreteSceneNodeFactory) KeyFrameSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PointAnimationUsingKeyFrames, (SceneNode.ConcreteSceneNodeFactory) KeyFrameAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.PointKeyFrame, (SceneNode.ConcreteSceneNodeFactory) KeyFrameSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ObjectAnimationUsingKeyFrames, (SceneNode.ConcreteSceneNodeFactory) KeyFrameAnimationSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.ObjectKeyFrame, (SceneNode.ConcreteSceneNodeFactory) KeyFrameSceneNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.Behavior, (SceneNode.ConcreteSceneNodeFactory) BehaviorNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.BehaviorTriggerBase, (SceneNode.ConcreteSceneNodeFactory) BehaviorTriggerBaseNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.BehaviorEventTriggerBase, (SceneNode.ConcreteSceneNodeFactory) BehaviorEventTriggerBaseNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.BehaviorTriggerAction, (SceneNode.ConcreteSceneNodeFactory) BehaviorTriggerActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.BehaviorEventTrigger, (SceneNode.ConcreteSceneNodeFactory) BehaviorEventTriggerNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.InvokeCommandAction, (SceneNode.ConcreteSceneNodeFactory) InvokeCommandActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.BehaviorTargetedTriggerAction, (SceneNode.ConcreteSceneNodeFactory) BehaviorTargetedTriggerActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.GoToStateAction, (SceneNode.ConcreteSceneNodeFactory) GoToStateActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.NavigationMenuAction, (SceneNode.ConcreteSceneNodeFactory) NavigationMenuActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.ConditionBehavior, (SceneNode.ConcreteSceneNodeFactory) ConditionBehaviorNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.ConditionalExpression, (SceneNode.ConcreteSceneNodeFactory) ConditionalExpressionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.ComparisonCondition, (SceneNode.ConcreteSceneNodeFactory) ComparisonConditionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.DataStateBehavior, (SceneNode.ConcreteSceneNodeFactory) DataStateBehaviorNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.ChangePropertyAction, ChangePropertyActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.LayoutPath, (SceneNode.ConcreteSceneNodeFactory) LayoutPathNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.PathListBox, (SceneNode.ConcreteSceneNodeFactory) PathListBoxElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.PathListBoxItem, (SceneNode.ConcreteSceneNodeFactory) PathListBoxItemElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.PathPanel, (SceneNode.ConcreteSceneNodeFactory) PathPanelElement.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.ActivateStateAction, (SceneNode.ConcreteSceneNodeFactory) ActivateStateActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.NavigateToScreenAction, (SceneNode.ConcreteSceneNodeFactory) NavigateToScreenActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.PlaySketchFlowAnimationAction, (SceneNode.ConcreteSceneNodeFactory) PlaySketchFlowAnimationActionNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(ProjectNeutralTypes.SketchFlowAnimationTrigger, (SceneNode.ConcreteSceneNodeFactory) SketchFlowAnimationTriggerNode.Factory));
        this.RegisterHandler(new SceneNodeFactory.SceneNodeTypeHandler(PlatformTypes.Annotation, (SceneNode.ConcreteSceneNodeFactory) AnnotationSceneNode.Factory));
      }

      protected override ITypeId GetBaseType(SceneNodeFactory.SceneNodeTypeHandler handler)
      {
        return handler.BaseType;
      }
    }

    private sealed class SceneNodeTypeHandler
    {
      private ITypeId baseType;
      private SceneNode.ConcreteSceneNodeFactory factory;

      public ITypeId BaseType
      {
        get
        {
          return this.baseType;
        }
      }

      public SceneNode.ConcreteSceneNodeFactory Factory
      {
        get
        {
          return this.factory;
        }
      }

      public SceneNodeTypeHandler(ITypeId baseType, SceneNode.ConcreteSceneNodeFactory factory)
      {
        this.baseType = baseType;
        this.factory = factory;
      }
    }
  }
}
