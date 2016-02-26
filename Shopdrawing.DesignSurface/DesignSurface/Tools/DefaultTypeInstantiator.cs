// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DefaultTypeInstantiator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class DefaultTypeInstantiator
  {
    public static readonly IPropertyId GlyphsUnicodeStringProperty = (IPropertyId) PlatformTypes.Glyphs.GetMember(MemberType.LocalProperty, "UnicodeString", MemberAccessTypes.Public);
    public static readonly IPropertyId GlyphsFontRenderingSizeEmProperty = (IPropertyId) PlatformTypes.Glyphs.GetMember(MemberType.LocalProperty, "FontRenderingEmSize", MemberAccessTypes.Public);
    public static readonly IPropertyId GlyphsFillProperty = (IPropertyId) PlatformTypes.Glyphs.GetMember(MemberType.LocalProperty, "Fill", MemberAccessTypes.Public);
    public static readonly IPropertyId ListViewViewProperty = (IPropertyId) PlatformTypes.ListView.GetMember(MemberType.LocalProperty, "View", MemberAccessTypes.Public);
    private static ITypeId[] SizeNonExceptions = new ITypeId[15]
    {
      PlatformTypes.Control,
      ProjectNeutralTypes.HeaderedContentControl,
      ProjectNeutralTypes.HeaderedItemsControl,
      PlatformTypes.ItemsControl,
      PlatformTypes.UserControl,
      PlatformTypes.ScrollViewer,
      ProjectNeutralTypes.TabControl,
      ProjectNeutralTypes.TreeView,
      PlatformTypes.ListView,
      PlatformTypes.ListBox,
      ProjectNeutralTypes.GridSplitter,
      PlatformTypes.ProgressBar,
      PlatformTypes.Button,
      PlatformTypes.ComboBox,
      PlatformTypes.RichTextBox
    };
    internal static ITypeId[] InteractiveElementTypes = new ITypeId[33]
    {
      PlatformTypes.Button,
      PlatformTypes.RepeatButton,
      PlatformTypes.ToggleButton,
      PlatformTypes.CheckBox,
      PlatformTypes.RadioButton,
      PlatformTypes.ContextMenu,
      PlatformTypes.Menu,
      PlatformTypes.ListView,
      ProjectNeutralTypes.TreeView,
      PlatformTypes.Selector,
      ProjectNeutralTypes.Label,
      PlatformTypes.TextBlock,
      PlatformTypes.TextBox,
      PlatformTypes.RichTextBox,
      PlatformTypes.PasswordBox,
      PlatformTypes.Slider,
      PlatformTypes.Hyperlink,
      ProjectNeutralTypes.TabControl,
      PlatformTypes.DocumentViewer,
      PlatformTypes.Image,
      PlatformTypes.MediaElement,
      ProjectNeutralTypes.GridSplitter,
      PlatformTypes.ResizeGrip,
      PlatformTypes.Window,
      ProjectNeutralTypes.Expander,
      PlatformTypes.ToolBar,
      PlatformTypes.NavigationWindow,
      PlatformTypes.GroupBox,
      PlatformTypes.ScrollBar,
      PlatformTypes.Thumb,
      ProjectNeutralTypes.TabPanel,
      ProjectNeutralTypes.Frame,
      PlatformTypes.Page
    };
    private SceneView sceneView;
    private IMessageDisplayService messageManager;

    public SceneView SceneView
    {
      get
      {
        return this.sceneView;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.sceneView.ViewModel;
      }
    }

    protected virtual bool ShouldUseDefaultInitializer
    {
      get
      {
        return true;
      }
    }

    public DefaultTypeInstantiator(SceneView sceneView)
    {
      this.sceneView = sceneView;
      this.messageManager = sceneView.ViewModel.DesignerContext.MessageDisplayService;
    }

    public static string TypeNameCallback(SceneElement sceneElement)
    {
      return (string) null;
    }

    public virtual SceneNode CreatePrototypeInstance(ITypeId instanceType)
    {
      SceneView sceneView = this.sceneView;
      if (sceneView != null && sceneView.IsEditable)
        return this.CreateRawInstance(instanceType);
      return (SceneNode) null;
    }

    public virtual SceneNode CreateInstance(ITypeId instanceType, ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.PropertyInspectorFromCreate);
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.CreateElement);
      SceneView sceneView = this.sceneView;
      if (sceneView == null || !sceneView.IsEditable || insertionPoint == null)
        return (SceneNode) null;
      SceneNode rawInstance = this.CreateRawInstance(instanceType);
      if (rawInstance == null || !insertionPoint.CanInsert((ITypeId) rawInstance.Type))
        return (SceneNode) null;
      if (rect.IsEmpty)
        rect = new Rect(0.0, 0.0, double.NaN, double.NaN);
      if (double.IsInfinity(rect.Width))
        rect.Width = double.NaN;
      if (double.IsInfinity(rect.Height))
        rect.Height = double.NaN;
      using (this.ViewModel.ForceBaseValue())
      {
        using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitCreateControlFormat, new object[1]
        {
          (object) instanceType.Name
        })))
        {
          IExpandable expandable = insertionPoint.SceneElement as IExpandable;
          if (expandable != null)
            insertionPoint.SceneNode.SetValue(expandable.ExpansionProperty, (object) true);
          bool flag = this.ShouldUseDefaultInitializer && Enumerable.Any<FeatureProvider>(DefaultTypeInstantiator.GetDefaultInitializers(rawInstance));
          IList<SceneNode> nodes = (IList<SceneNode>) null;
          if (!flag)
          {
            nodes = this.CreateNodeTreeOnInsertion(rawInstance);
            this.ApplyBeforeInsertionDefaultsToElements(nodes, rawInstance, new DefaultTypeInstantiator.SceneElementNamingCallback(DefaultTypeInstantiator.TypeNameCallback));
          }
          SceneNode layoutTarget = DefaultTypeInstantiator.GetLayoutTarget(rawInstance);
          SceneElement selectionTarget = DefaultTypeInstantiator.GetSelectionTarget(rawInstance);
          this.ViewModel.ElementSelectionSet.Clear();
          insertionPoint.Insert(rawInstance);
          editTransaction.Update();
          this.ApplyDefaultInitializers(rawInstance);
          if (action != null)
          {
            action(rawInstance);
            editTransaction.Update();
          }
          if (!flag)
            this.ApplyAfterInsertionDefaultsToElements(nodes, rawInstance);
          SceneElement sceneElement1 = layoutTarget as SceneElement;
          EffectNode effectNode = layoutTarget as EffectNode;
          if (sceneElement1 != null && sceneElement1.IsViewObjectValid)
          {
            if (selectionTarget != null)
              this.ViewModel.ElementSelectionSet.SetSelection(selectionTarget);
            this.SetLayout(insertionPoint, rect, rawInstance, layoutTarget, editTransaction);
          }
          else if (effectNode != null)
          {
            this.ViewModel.ChildPropertySelectionSet.SetSelection((SceneNode) effectNode);
          }
          else
          {
            for (SceneNode sceneNode = layoutTarget; sceneNode != null; sceneNode = sceneNode.Parent)
            {
              SceneElement sceneElement2 = sceneNode as SceneElement;
              if (sceneElement2 != null && sceneElement2.Visual != null && sceneElement2.Visual is IViewVisual)
              {
                this.sceneView.EnsureVisible(sceneElement2.Visual);
                break;
              }
            }
          }
          if (this.ViewModel.DesignerContext.AmbientPropertyManager != null)
            this.ViewModel.DesignerContext.AmbientPropertyManager.ApplyAmbientProperties(rawInstance);
          editTransaction.Commit();
        }
      }
      return rawInstance;
    }

    private void SetLayout(ISceneInsertionPoint insertionPoint, Rect rect, SceneNode node, SceneNode layoutTarget, SceneEditTransaction undo)
    {
      if (!PlatformTypes.UIElement.IsAssignableFrom((ITypeId) node.Type))
        return;
      bool flag1 = false;
      bool flag2 = false;
      if (node.IsSet(BaseFrameworkElement.WidthProperty) == PropertyState.Set && double.IsNaN(rect.Width))
      {
        rect.Width = (double) node.GetLocalOrDefaultValueAsWpf(BaseFrameworkElement.WidthProperty);
        flag1 = true;
      }
      if (node.IsSet(BaseFrameworkElement.HeightProperty) == PropertyState.Set && double.IsNaN(rect.Height))
      {
        rect.Height = (double) node.GetLocalOrDefaultValueAsWpf(BaseFrameworkElement.HeightProperty);
        flag2 = true;
      }
      using (this.ViewModel.ForceBaseValue())
      {
        BaseFrameworkElement child = layoutTarget as BaseFrameworkElement;
        if (child == null)
          return;
        SetRectMode setRectMode = SetRectMode.Default;
        ILayoutDesigner designerForChild = child.ViewModel.GetLayoutDesignerForChild((SceneElement) child, true);
        if (double.IsNaN(rect.Width) || double.IsNaN(rect.Height))
        {
          setRectMode = SetRectMode.CreateDefault;
          bool flag3 = PlatformTypes.Control.IsAssignableFrom((ITypeId) node.Type) || PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) node.Type);
          for (int index = 0; index < DefaultTypeInstantiator.SizeNonExceptions.Length; ++index)
          {
            if (DefaultTypeInstantiator.SizeNonExceptions[index].Equals((object) node.Type))
              flag3 = false;
          }
          if (!flag3)
          {
            BaseFrameworkElement frameworkElement = insertionPoint.SceneNode as BaseFrameworkElement;
            if (frameworkElement != null)
            {
              Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
              double val1_1 = 100.0;
              double val1_2 = 100.0;
              if (ProjectNeutralTypes.GridSplitter.IsAssignableFrom((ITypeId) node.Type))
                val1_1 = 5.0;
              else if (PlatformTypes.Button.IsAssignableFrom((ITypeId) node.Type))
                val1_1 = 75.0;
              else if (PlatformTypes.ComboBox.IsAssignableFrom((ITypeId) node.Type))
                val1_1 = 120.0;
              if (PlatformTypes.ProgressBar.IsAssignableFrom((ITypeId) node.Type))
                val1_2 = 10.0;
              else if (PlatformTypes.Button.IsAssignableFrom((ITypeId) node.Type) || PlatformTypes.ComboBox.IsAssignableFrom((ITypeId) node.Type))
                val1_2 = double.NaN;
              if ((designerForChild.GetWidthConstraintMode(child) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike)
                val1_1 = Math.Min(val1_1, computedTightBounds.Width);
              if ((designerForChild.GetHeightConstraintMode(child) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike)
                val1_2 = Math.Min(val1_2, computedTightBounds.Height);
              rect = new Rect(rect.Left, rect.Top, double.IsNaN(rect.Width) ? val1_1 : rect.Width, double.IsNaN(rect.Height) ? val1_2 : rect.Height);
            }
          }
        }
        LayoutOverrides layoutOverrides = LayoutOverrides.None;
        LayoutOverrides overridesToIgnore = LayoutOverrides.None;
        Rect rect1 = rect;
        if (double.IsNaN(rect.Width))
        {
          layoutOverrides |= LayoutOverrides.Width;
          rect1.Width = 0.0;
        }
        else
          overridesToIgnore |= LayoutOverrides.Width;
        if (double.IsNaN(rect.Height))
        {
          layoutOverrides |= LayoutOverrides.Height;
          rect1.Height = 0.0;
        }
        else
          overridesToIgnore |= LayoutOverrides.Height;
        designerForChild.SetChildRect(child, rect1, layoutOverrides, overridesToIgnore, LayoutOverrides.None, setRectMode);
        undo.Update();
        IViewVisual viewVisual = child.ViewObject as IViewVisual;
        if (viewVisual == null || !double.IsNaN(rect.Width) && !double.IsNaN(rect.Height))
          return;
        viewVisual.UpdateLayout();
        Rect childRect = designerForChild.GetChildRect(child);
        if (double.IsNaN(rect.Width) && !flag1)
        {
          if (viewVisual.RenderSize.Width < 5.0)
          {
            rect1.Width = 100.0;
            layoutOverrides &= ~LayoutOverrides.Width;
            overridesToIgnore |= LayoutOverrides.Width;
          }
          else
            rect1.Width = childRect.Width;
        }
        if (double.IsNaN(rect.Height) && !flag2)
        {
          if (viewVisual.RenderSize.Height < 5.0)
          {
            rect1.Height = 100.0;
            layoutOverrides &= ~LayoutOverrides.Height;
            overridesToIgnore |= LayoutOverrides.Height;
          }
          else
            rect1.Height = childRect.Height;
        }
        LayoutOverrides nonExplicitOverrides = LayoutOverrides.None;
        if (PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom((ITypeId) child.Type) || PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) child.Type))
          nonExplicitOverrides = LayoutOverrides.Width;
        designerForChild.SetChildRect(child, rect1, layoutOverrides, overridesToIgnore, nonExplicitOverrides, setRectMode);
      }
    }

    private static IEnumerable<FeatureProvider> GetDefaultInitializers(SceneNode node)
    {
      try
      {
        return Enumerable.Reverse<FeatureProvider>(node.ViewModel.ExtensibilityManager.FeatureManager.CreateFeatureProviders(typeof (DefaultInitializer), node.TargetType));
      }
      catch (Exception ex)
      {
      }
      return Enumerable.Empty<FeatureProvider>();
    }

    private bool ApplyDefaultInitializers(SceneNode node)
    {
      bool flag = false;
      if (this.ShouldUseDefaultInitializer)
      {
        foreach (DefaultInitializer defaultInitializer in DefaultTypeInstantiator.GetDefaultInitializers(node))
        {
          flag = true;
          defaultInitializer.InitializeDefaults((ModelItem) node.ModelItem, node.ModelItem.Context);
        }
      }
      return flag;
    }

    internal virtual void ApplyBeforeInsertionDefaultsToElements(IList<SceneNode> nodes, SceneNode rootNode, DefaultTypeInstantiator.SceneElementNamingCallback callback)
    {
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) nodes)
      {
        if (ProjectNeutralTypes.DockPanel.IsAssignableFrom((ITypeId) sceneNode.Type))
          sceneNode.SetValue(DockPanelElement.LastChildFillProperty, (object) false);
        SceneElement sceneElement = sceneNode as SceneElement;
        if (sceneElement != null)
          sceneElement.Name = callback(sceneElement);
      }
      foreach (SceneNode node in (IEnumerable<SceneNode>) nodes)
      {
        StyleAsset relatedUserThemeAsset = this.GetRelatedUserThemeAsset(node, rootNode);
        if (relatedUserThemeAsset != null)
          relatedUserThemeAsset.ApplyStyle(node);
      }
    }

    public virtual IList<SceneNode> CreateNodeTreeOnInsertion(SceneNode node)
    {
      List<SceneNode> list = new List<SceneNode>();
      list.Add(node);
      if (ProjectNeutralTypes.TabControl.IsAssignableFrom((ITypeId) node.Type))
      {
        ItemsControlElement itemsControlElement = (ItemsControlElement) node;
        SceneElement sceneElement1 = (SceneElement) node.ViewModel.CreateSceneNode(ProjectNeutralTypes.TabItem);
        itemsControlElement.Items.Add((SceneNode) sceneElement1);
        list.AddRange((IEnumerable<SceneNode>) this.CreateNodeTreeOnInsertion((SceneNode) sceneElement1));
        SceneElement sceneElement2 = (SceneElement) node.ViewModel.CreateSceneNode(ProjectNeutralTypes.TabItem);
        itemsControlElement.Items.Add((SceneNode) sceneElement2);
        list.AddRange((IEnumerable<SceneNode>) this.CreateNodeTreeOnInsertion((SceneNode) sceneElement2));
      }
      else if (PlatformTypes.Popup.IsAssignableFrom((ITypeId) node.Type) || ProjectNeutralTypes.Expander.IsAssignableFrom((ITypeId) node.Type) || ProjectNeutralTypes.TabItem.IsAssignableFrom((ITypeId) node.Type))
      {
        GridElement gridElement = (GridElement) node.ViewModel.CreateSceneNode(PlatformTypes.Grid);
        SolidColorBrushNode solidColorBrushNode = (SolidColorBrushNode) node.ViewModel.CreateSceneNode(PlatformTypes.SolidColorBrush);
        solidColorBrushNode.SetValueAsWpf(SolidColorBrushNode.ColorProperty, (object) Color.FromArgb(byte.MaxValue, (byte) 229, (byte) 229, (byte) 229));
        gridElement.SetValueAsSceneNode(PanelElement.BackgroundProperty, (SceneNode) solidColorBrushNode);
        SceneElement sceneElement = (SceneElement) node;
        if (sceneElement.DefaultInsertionPoint != null)
        {
          sceneElement.DefaultInsertionPoint.Insert((SceneNode) gridElement);
          list.Add((SceneNode) gridElement);
          IExpandable expandable = node as IExpandable;
          if (expandable != null)
          {
            expandable.EnsureExpandable();
            if (sceneElement.ProjectContext.ResolveProperty(expandable.DesignTimeExpansionProperty) != null)
              node.SetLocalValue(expandable.DesignTimeExpansionProperty, (object) true);
          }
        }
      }
      return (IList<SceneNode>) list;
    }

    public static SceneNode GetLayoutTarget(SceneNode node)
    {
      if (node != null && PlatformTypes.Popup.IsAssignableFrom((ITypeId) node.Type))
        return node.DefaultContent[0];
      return node;
    }

    public static SceneElement GetSelectionTarget(SceneNode node)
    {
      SceneElement sceneElement = node as SceneElement;
      if ((PlatformTypes.Popup.IsAssignableFrom((ITypeId) node.Type) || ProjectNeutralTypes.Expander.IsAssignableFrom((ITypeId) node.Type) || ProjectNeutralTypes.TabItem.IsAssignableFrom((ITypeId) node.Type)) && sceneElement.DefaultContent.Count == 1)
        return sceneElement.DefaultContent[0] as SceneElement;
      return sceneElement;
    }

    protected virtual StyleAsset GetRelatedUserThemeAsset(SceneNode node, SceneNode rootNode)
    {
      return (StyleAsset) null;
    }

    internal void ApplyAfterInsertionDefaultsToElements(IList<SceneNode> nodes, SceneNode rootNode)
    {
      foreach (SceneNode node in (IEnumerable<SceneNode>) nodes)
      {
        SceneElement element = node as SceneElement;
        if (element != null)
        {
          string name = element.Name;
          if (name == null)
          {
            StyleAsset relatedUserThemeAsset = this.GetRelatedUserThemeAsset(node, rootNode);
            if (relatedUserThemeAsset != null)
            {
              DocumentCompositeNode documentCompositeNode = relatedUserThemeAsset.ResourceModel.ValueNode as DocumentCompositeNode;
              if (documentCompositeNode != null)
              {
                name = documentCompositeNode.GetValue<string>(DesignTimeProperties.StyleDefaultContentProperty);
                double num1 = documentCompositeNode.GetValue<double>(DesignTimeProperties.ExplicitWidthProperty);
                if (num1 > 0.0)
                  DefaultTypeInstantiator.SetIfUnset(node, BaseFrameworkElement.WidthProperty, (object) num1);
                double num2 = documentCompositeNode.GetValue<double>(DesignTimeProperties.ExplicitHeightProperty);
                if (num2 > 0.0)
                  DefaultTypeInstantiator.SetIfUnset(node, BaseFrameworkElement.HeightProperty, (object) num2);
              }
            }
          }
          if (name == null)
            name = element.TargetType.Name;
          if (element.Name == null && this.ViewModel.DesignerContext.ProjectManager.OptionsModel.NameInteractiveElementsByDefault && Enumerable.FirstOrDefault<ITypeId>((IEnumerable<ITypeId>) DefaultTypeInstantiator.InteractiveElementTypes, (Func<ITypeId, bool>) (i =>
          {
            IType type = this.ViewModel.ProjectContext.ResolveType(i);
            if (type != null)
              return type.IsAssignableFrom((ITypeId) element.Type);
            return false;
          })) != null)
            element.EnsureNamed();
          if (ProjectNeutralTypes.HeaderedContentControl.IsAssignableFrom((ITypeId) node.Type))
            DefaultTypeInstantiator.SetIfUnset(node, HeaderedControlProperties.HeaderedContentHeaderProperty, (object) name);
          else if (ProjectNeutralTypes.HeaderedItemsControl.IsAssignableFrom((ITypeId) node.Type) && !PlatformTypes.ToolBar.IsAssignableFrom((ITypeId) node.Type))
            DefaultTypeInstantiator.SetIfUnset(node, HeaderedControlProperties.HeaderedItemsHeaderProperty, (object) name);
          else if (PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) node.Type) && !PlatformTypes.ScrollViewer.IsAssignableFrom((ITypeId) node.Type) && !PlatformTypes.UserControl.IsAssignableFrom((ITypeId) node.Type))
          {
            if (ProjectNeutralTypes.TabItem.IsAssignableFrom((ITypeId) node.Type) && this.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.UseHeaderOnTabItem))
            {
              IPropertyId property = (IPropertyId) ProjectNeutralTypes.TabItem.GetMember(MemberType.LocalProperty, "Header", MemberAccessTypes.Public);
              DefaultTypeInstantiator.SetIfUnset(node, property, (object) name);
            }
            else
              DefaultTypeInstantiator.SetIfUnset(node, ContentControlElement.ContentProperty, (object) name);
          }
          else
          {
            BaseTextElement baseTextElement;
            if ((baseTextElement = node as BaseTextElement) != null)
            {
              if (string.IsNullOrEmpty(baseTextElement.Text.Trim()))
                baseTextElement.Text = name;
              if (PlatformTypes.TextBox.IsAssignableFrom((ITypeId) node.Type))
                DefaultTypeInstantiator.SetAsWpfIfUnset(node, TextBoxElement.TextWrappingProperty, (object) TextWrapping.Wrap);
              if (!node.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) node.Type))
                DefaultTypeInstantiator.SetAsWpfIfUnset(node, RichTextBoxElement.TextWrappingProperty, (object) TextWrapping.Wrap);
            }
            else if (PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) node.Type))
            {
              DefaultTypeInstantiator.SetIfUnset(node, TextBlockElement.TextProperty, (object) name);
              DefaultTypeInstantiator.SetAsWpfIfUnset(node, TextBlockElement.TextWrappingProperty, (object) TextWrapping.Wrap);
            }
            else if (PlatformTypes.ListView.IsAssignableFrom((ITypeId) node.Type))
            {
              GridViewElement gridViewElement = (GridViewElement) node.ViewModel.CreateSceneNode(PlatformTypes.GridView);
              SceneNode sceneNode = node.ViewModel.CreateSceneNode(PlatformTypes.GridViewColumn);
              gridViewElement.Columns.Add(sceneNode);
              node.SetValueAsSceneNode(DefaultTypeInstantiator.ListViewViewProperty, (SceneNode) gridViewElement);
            }
            else if (PlatformTypes.Border.IsAssignableFrom((ITypeId) node.Type))
            {
              DefaultTypeInstantiator.SetAsWpfIfUnset(node, BorderElement.BorderBrushProperty, (object) Brushes.Black);
              DefaultTypeInstantiator.SetAsWpfIfUnset(node, BorderElement.BorderThicknessProperty, (object) new Thickness(1.0));
            }
            else if (typeof (FlowDocumentScrollViewer).IsAssignableFrom(node.TargetType))
              DefaultTypeInstantiator.SetIfUnset(node, FlowDocumentScrollViewerElement.DocumentProperty, (object) new FlowDocument((Block) new Paragraph((Inline) new Run(name))));
            else if (typeof (Glyphs).IsAssignableFrom(node.TargetType))
            {
              DefaultTypeInstantiator.SetIfUnset(node, DefaultTypeInstantiator.GlyphsUnicodeStringProperty, (object) name);
              DefaultTypeInstantiator.SetIfUnset(node, DefaultTypeInstantiator.GlyphsFillProperty, (object) Brushes.Black);
              DefaultTypeInstantiator.SetIfUnset(node, DefaultTypeInstantiator.GlyphsFontRenderingSizeEmProperty, (object) 12.0);
            }
            else if (typeof (Viewport3D).IsAssignableFrom(node.TargetType))
            {
              Viewport3DElement viewport3Delement = node as Viewport3DElement;
              if (viewport3Delement != null)
              {
                Camera camera1 = (Camera) Viewport3D.CameraProperty.DefaultMetadata.DefaultValue;
                Camera camera2 = (Camera) viewport3Delement.GetComputedValue(Viewport3DElement.CameraProperty);
                if (camera2 == null || PropertyUtilities.Compare((object) camera1, (object) camera2, this.sceneView))
                {
                  Camera perspectiveCamera = Helper3D.CreateEnclosingPerspectiveCamera(45.0, 1.0, new Rect3D(-1.0, -1.0, -1.0, 2.0, 2.0, 2.0), 1.0);
                  DefaultTypeInstantiator.SetIfUnset((SceneNode) viewport3Delement, Viewport3DElement.CameraProperty, (object) perspectiveCamera);
                }
              }
            }
          }
        }
      }
    }

    private static void SetIfUnset(SceneNode node, IPropertyId property, object value)
    {
      if (!DefaultTypeInstantiator.ShouldSet(node, property))
        return;
      node.SetValue(property, value);
    }

    private static void SetAsWpfIfUnset(SceneNode node, IPropertyId property, object value)
    {
      if (!DefaultTypeInstantiator.ShouldSet(node, property))
        return;
      node.SetValueAsWpf(property, value);
    }

    private static bool ShouldSet(SceneNode node, IPropertyId property)
    {
      ReferenceStep referenceStep = node.ProjectContext.ResolveProperty(property) as ReferenceStep;
      if (referenceStep == null)
        return false;
      IViewObject viewObject = node.IsViewObjectValid ? node.ViewObject : (IViewObject) null;
      object objToInspect = viewObject != null ? viewObject.PlatformSpecificObject : (object) null;
      if (objToInspect == null || referenceStep.IsSet(objToInspect))
        return false;
      return object.Equals(referenceStep.GetDefaultValue(objToInspect.GetType()), referenceStep.GetValue(objToInspect));
    }

    protected virtual SceneNode InternalCreateRawInstance(ITypeId instanceType)
    {
      return this.ViewModel.CreateSceneNode(instanceType);
    }

    private SceneNode CreateRawInstance(ITypeId instanceType)
    {
      try
      {
        return this.InternalCreateRawInstance(instanceType);
      }
      catch (Exception ex)
      {
        this.messageManager.ShowError(new ErrorArgs()
        {
          Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.LibraryPaneCreationFailed, new object[1]
          {
            (object) instanceType.FullName
          }),
          Exception = ex
        });
        return (SceneNode) null;
      }
    }

    internal delegate string SceneElementNamingCallback(SceneElement sceneElement);
  }
}
