// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ConvertElementCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class ConvertElementCommand : SceneCommandBase
  {
    private bool showUI = true;

    public bool ShowUI
    {
      get
      {
        return this.showUI;
      }
      set
      {
        this.showUI = value;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
        {
          SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
          if (elementSelectionSet.Count == 1 && elementSelectionSet.PrimarySelection is BaseFrameworkElement && elementSelectionSet.PrimarySelection != this.SceneViewModel.RootNode)
            return true;
        }
        return false;
      }
    }

    protected virtual bool ShouldReplaceOriginal
    {
      get
      {
        return false;
      }
    }

    protected abstract string UndoUnitName { get; }

    protected abstract bool CreateResource { get; }

    protected virtual CreateResourceModel.ContextFlags ResourceContextFlags
    {
      get
      {
        return CreateResourceModel.ContextFlags.None;
      }
    }

    protected abstract ITypeId Type { get; }

    protected ConvertElementCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected virtual BaseFrameworkElement CreateElement(BaseFrameworkElement originalElement)
    {
      return (BaseFrameworkElement) null;
    }

    protected abstract DocumentNode CreateValue(BaseFrameworkElement source);

    public override void Execute()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.Make3D);
      try
      {
        using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
        {
          BaseFrameworkElement frameworkElement = (BaseFrameworkElement) this.SceneViewModel.ElementSelectionSet.PrimarySelection;
          using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.UndoUnitName, false))
          {
            ILayoutDesigner designerForChild = this.SceneViewModel.GetLayoutDesignerForChild((SceneElement) frameworkElement, true);
            Rect childRect = designerForChild.GetChildRect(frameworkElement);
            DocumentNode newValue = this.CreateValue(frameworkElement);
            if (newValue == null)
              editTransaction.Cancel();
            else if (this.CreateResource && this.ProcessAsResource(frameworkElement, newValue) == null)
            {
              editTransaction.Cancel();
            }
            else
            {
              if (this.ShouldReplaceOriginal)
              {
                this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree((SceneElement) frameworkElement);
                Dictionary<IPropertyId, SceneNode> properties = SceneElementHelper.StoreProperties((SceneNode) frameworkElement, true);
                this.SceneViewModel.ElementSelectionSet.Clear();
                BaseFrameworkElement element = this.CreateElement(frameworkElement);
                using (this.SceneViewModel.ForceBaseValue())
                {
                  element.Name = frameworkElement.Name;
                  ISceneNodeCollection<SceneNode> collectionForChild = frameworkElement.ParentElement.GetCollectionForChild((SceneNode) frameworkElement);
                  int index = collectionForChild.IndexOf((SceneNode) frameworkElement);
                  frameworkElement.Remove();
                  this.Postprocess(frameworkElement, element, properties, childRect);
                  collectionForChild.Insert(index, (SceneNode) element);
                  SceneElementHelper.ApplyProperties((SceneNode) element, properties);
                  editTransaction.Update();
                  designerForChild.SetChildRect(element, childRect);
                }
                this.SceneViewModel.ElementSelectionSet.SetSelection((SceneElement) element);
              }
              editTransaction.Commit();
            }
          }
        }
      }
      finally
      {
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.Make3D);
      }
    }

    protected virtual DocumentNode ProcessAsResource(BaseFrameworkElement originalElement, DocumentNode newValue)
    {
      if (!this.CreateResource)
        return (DocumentNode) null;
      CreateResourceModel model = new CreateResourceModel(this.SceneViewModel, this.DesignerContext.ResourceManager, this.SceneViewModel.ProjectContext.ResolveType(this.Type).RuntimeType, (System.Type) null, (string) null, (SceneElement) null, (SceneNode) originalElement, this.ResourceContextFlags);
      if (originalElement.IsNamed)
      {
        model.KeyString = model.CurrentResourceSite.GetUniqueResourceKey(originalElement.Name);
        if (model.KeyStringHasIssues)
          model.ResetResourceKey();
      }
      if (this.ShowUI)
      {
        bool? nullable = new CreateResourceDialog(this.DesignerContext, model).ShowDialog();
        if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
          return (DocumentNode) null;
      }
      return (DocumentNode) model.CreateResource(newValue, (IPropertyId) null, -1);
    }

    protected virtual void Postprocess(BaseFrameworkElement originalElement, BaseFrameworkElement newElement, Dictionary<IPropertyId, SceneNode> properties, Rect layoutRect)
    {
    }
  }
}
