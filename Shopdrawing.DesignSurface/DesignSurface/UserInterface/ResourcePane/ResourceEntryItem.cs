// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceEntryItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ResourceEntryItem : ResourceItem
  {
    private ResourceModel resource;
    private string currentName;

    public override Type EffectiveType
    {
      get
      {
        return this.EffectiveTypeId.RuntimeType;
      }
    }

    public virtual IType EffectiveTypeId
    {
      get
      {
        return this.resource.ValueNode.Type;
      }
    }

    public ResourceModel Resource
    {
      get
      {
        return this.resource;
      }
    }

    public override object ToolTip
    {
      get
      {
        return (object) new DocumentationEntry((string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ResourceItemFormat, new object[1]
        {
          (object) this.Resource.TargetType.Name
        }), (Type) null, this.Key);
      }
    }

    public virtual string Key
    {
      get
      {
        if (this.currentName != null)
          return this.currentName;
        string name = this.resource.Name;
        if (!string.IsNullOrEmpty(name))
          return name;
        return "[" + this.EffectiveType.Name + "]";
      }
      set
      {
        if (this.currentName == null)
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) new ResourceEntryItem.AsyncAction(this.InteractiveRenameResource));
        if (value != null)
          value = value.Trim();
        this.currentName = value;
      }
    }

    public override DocumentNode DocumentNode
    {
      get
      {
        return (DocumentNode) this.resource.ResourceNode;
      }
    }

    public override DocumentNodeMarker Marker
    {
      get
      {
        return this.resource.Marker;
      }
    }

    public SolidColorBrush DragOverColor
    {
      get
      {
        return new SolidColorBrush(Colors.Goldenrod);
      }
    }

    protected ResourceEntryItem(ResourceManager resourceManager, ResourceContainer resourceContainer, ResourceModel resource)
      : base(resourceManager, resourceContainer)
    {
      this.resource = resource;
    }

    public override void InteractiveDelete()
    {
      ReferencesFoundModel referencingResources = this.InteractiveGetReferencingResources(ReferencesFoundModel.UseScenario.DeleteResource);
      if (referencingResources == null || referencingResources.ReferenceNames.Count > 0 && !this.ShowReferencesFoundDialog(referencingResources).GetValueOrDefault(false))
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.DeleteResourceItem);
      SceneViewModel viewModel = this.Container.ViewModel;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoUnitDeleteResource))
      {
        this.Container.ResourceDictionaryNode.Remove((DictionaryEntryNode) viewModel.GetSceneNode((DocumentNode) this.resource.ResourceNode));
        editTransaction.Commit();
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.DeleteResourceItem);
    }

    private bool? ShowReferencesFoundDialog(ReferencesFoundModel model)
    {
      bool? nullable1 = new bool?(true);
      bool? nullable2 = new ReferencesFoundDialog(model).ShowDialog();
      if (nullable2.GetValueOrDefault(false) && model.SelectedUpdateMethod != ReferencesFoundModel.UpdateMethod.DontFix)
        nullable2 = new AsyncProcessDialog(model.FixReferencesAsync(), this.DesignerContext.ExpressionInformationService).ShowDialog();
      return nullable2;
    }

    internal ReferencesFoundModel InteractiveGetReferencingResources(ReferencesFoundModel.UseScenario useScenario)
    {
      SceneViewModel viewModel = this.Container.ViewModel;
      List<SceneNode> list = new List<SceneNode>();
      viewModel.FindInternalResourceReferences(this.resource.ResourceNode, (ICollection<SceneNode>) list);
      ITypeId type = (ITypeId) viewModel.RootNode.Type;
      bool flag = PlatformTypes.ResourceDictionary.IsAssignableFrom(type) || PlatformTypes.Application.IsAssignableFrom(type);
      ReferencesFoundModel model = new ReferencesFoundModel(viewModel.GetSceneNode((DocumentNode) this.resource.ResourceNode), (ICollection<SceneNode>) list, useScenario);
      if (list.Count > 0 || flag)
      {
        if (!new AsyncProcessDialog((AsyncProcess) new SerialAsyncProcess((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), new AsyncProcess[2]
        {
          (AsyncProcess) new ExternalOpenSceneResourceReferenceAnalyzer(model),
          (AsyncProcess) new ExternalClosedSceneResourceReferenceAnalyzer(model)
        }), this.DesignerContext.ExpressionInformationService).ShowDialog().GetValueOrDefault(false))
          model = (ReferencesFoundModel) null;
      }
      return model;
    }

    private void InteractiveRenameResource()
    {
      if (this.DocumentNode == null || this.DocumentNode.DocumentRoot == null || this.DocumentNode.DocumentRoot.DocumentContext == null)
        return;
      string key = this.currentName;
      this.currentName = (string) null;
      if (key == null || key.Equals(this.resource.Name))
      {
        this.OnKeyChanged();
      }
      else
      {
        SceneViewModel viewModel = this.Container.ViewModel;
        if (new ResourceSite(viewModel.Document.DocumentContext, this.Container.ResourcesCollection).FindResource((IDocumentRootResolver) null, key, (ICollection<DocumentCompositeNode>) null, (ICollection<IDocumentRoot>) null) != null)
        {
          viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.CreateResourceKeyStringIssueDirectConflict);
          this.OnKeyChanged();
        }
        else
        {
          List<SceneNode> list = new List<SceneNode>();
          ReferencesFoundModel model = (ReferencesFoundModel) null;
          viewModel.FindInternalResourceReferences(this.resource.ResourceNode, (ICollection<SceneNode>) list);
          ITypeId type = (ITypeId) viewModel.RootNode.Type;
          bool flag = PlatformTypes.ResourceDictionary.IsAssignableFrom(type) || PlatformTypes.Application.IsAssignableFrom(type);
          DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) viewModel.GetSceneNode((DocumentNode) this.resource.ResourceNode);
          if (list.Count > 0 || flag)
          {
            model = new ReferencesFoundModel((SceneNode) dictionaryEntryNode, (ICollection<SceneNode>) list, ReferencesFoundModel.UseScenario.RenameResource);
            bool valueOrDefault = new AsyncProcessDialog((AsyncProcess) new SerialAsyncProcess((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), new AsyncProcess[2]
            {
              (AsyncProcess) new ExternalOpenSceneResourceReferenceAnalyzer(model),
              (AsyncProcess) new ExternalClosedSceneResourceReferenceAnalyzer(model)
            }), this.DesignerContext.ExpressionInformationService).ShowDialog().GetValueOrDefault(false);
            if (valueOrDefault && model.ReferenceNames.Count > 0)
              valueOrDefault = new ReferencesFoundDialog(model).ShowDialog().GetValueOrDefault(false);
            if (!valueOrDefault)
            {
              this.OnKeyChanged();
              return;
            }
          }
          using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoUnitRenameResource))
          {
            if (model != null && model.SelectedUpdateMethod != ReferencesFoundModel.UpdateMethod.DontFix && model.ReferenceNames.Count > 0)
            {
              model.NewKey = (object) key;
              new AsyncProcessDialog(model.FixReferencesAsync(), this.DesignerContext.ExpressionInformationService).ShowDialog();
            }
            dictionaryEntryNode.Key = (object) key;
            editTransaction.Commit();
          }
          this.OnKeyChanged();
        }
      }
    }

    internal void OnKeyChanged()
    {
      this.OnPropertyChanged((string) null);
    }

    public static ResourceItem GetTypedItem(ResourceManager resourceManager, ResourceContainer resourceContainer, ResourceModel model)
    {
      ITypeId type = (ITypeId) model.ValueNode.Type;
      if (PlatformTypes.Brush.IsAssignableFrom(type))
        return (ResourceItem) new BrushResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.DrawingImage.IsAssignableFrom(type))
        return (ResourceItem) new DrawingImageResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.Transform.IsAssignableFrom(type) || PlatformTypes.Transform3D.IsAssignableFrom(type))
        return (ResourceItem) new TransformResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.Style.IsAssignableFrom(type))
        return (ResourceItem) new StyleResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.ControlTemplate.IsAssignableFrom(type))
        return (ResourceItem) new ControlTemplateResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.DataTemplate.IsAssignableFrom(type))
        return (ResourceItem) new DataTemplateResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.ItemsPanelTemplate.IsAssignableFrom(type))
        return (ResourceItem) new ItemsPanelTemplateResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.Timeline.IsAssignableFrom(type))
        return (ResourceItem) new TimelineResourceItem(resourceManager, resourceContainer, model);
      if (PlatformTypes.XmlDataProvider.IsAssignableFrom(type))
        return (ResourceItem) new DataSourceResourceItem(resourceManager, resourceContainer, model);
      return (ResourceItem) new ResourceEntryItem(resourceManager, resourceContainer, model);
    }

    public override void OnDragBegin(DragBeginEventArgs e)
    {
      if (this.Container.Document == null || !this.Container.Document.IsEditable || (Mouse.Captured != null || this.IsEditing))
        return;
      DataObject dataObject = new DataObject();
      dataObject.SetData("ResourceEntryItem", (object) this, true);
      int num = (int) DragDrop.DoDragDrop((DependencyObject) e.DragSource, (object) dataObject, DragDropEffects.Copy | DragDropEffects.Move);
    }

    public override void OnDrop(DragEventArgs e)
    {
      int destinationIndex = this.Container.ResourceDictionaryNode.IndexOf((DictionaryEntryNode) this.Container.ViewModel.GetSceneNode((DocumentNode) this.resource.ResourceNode)) + 1;
      this.DoDrop(e, destinationIndex);
      base.OnDrop(e);
    }

    private delegate void AsyncAction();
  }
}
