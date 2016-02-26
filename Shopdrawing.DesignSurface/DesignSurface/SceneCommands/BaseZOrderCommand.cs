// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.BaseZOrderCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class BaseZOrderCommand : SceneCommandBase
  {
    private bool requiresSortedList;

    public override sealed bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && !this.SceneViewModel.ElementSelectionSet.IsEmpty)
        {
          SceneElement sceneElement1 = this.SceneViewModel.RootNode as SceneElement;
          if (sceneElement1 == null || !this.SceneViewModel.ElementSelectionSet.Selection.Contains(sceneElement1))
          {
            foreach (SceneElement sceneElement2 in this.SceneViewModel.ElementSelectionSet.Selection)
            {
              if (!(sceneElement2 is BaseFrameworkElement))
                return false;
            }
            return true;
          }
        }
        return false;
      }
    }

    protected abstract string UndoDescription { get; }

    public BaseZOrderCommand(SceneViewModel viewModel, bool requiresSortedList)
      : base(viewModel)
    {
      this.requiresSortedList = requiresSortedList;
    }

    public override sealed void Execute()
    {
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.UndoDescription))
        {
          ICollection<SceneElement> selection = (ICollection<SceneElement>) this.SceneViewModel.ElementSelectionSet.Selection;
          SceneElement primarySelection = this.SceneViewModel.ElementSelectionSet.PrimarySelection;
          this.SceneViewModel.ElementSelectionSet.Clear();
          List<SceneElement> list = new List<SceneElement>((IEnumerable<SceneElement>) selection);
          foreach (KeyValuePair<KeyValuePair<SceneElement, IPropertyId>, SceneElementCollection> keyValuePair in this.SortSelectionByParentAndZOrder(selection))
            this.ReorderElements(keyValuePair.Key.Key.GetCollectionForProperty(keyValuePair.Key.Value), keyValuePair.Value);
          this.SceneViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list, primarySelection);
          editTransaction.Commit();
        }
      }
    }

    protected abstract void ReorderElements(ISceneNodeCollection<SceneNode> childCollection, SceneElementCollection selectedChildren);

    private Dictionary<KeyValuePair<SceneElement, IPropertyId>, SceneElementCollection> SortSelectionByParentAndZOrder(ICollection<SceneElement> selection)
    {
      Dictionary<KeyValuePair<SceneElement, IPropertyId>, SceneElementCollection> dictionary = new Dictionary<KeyValuePair<SceneElement, IPropertyId>, SceneElementCollection>();
      foreach (SceneElement sceneElement in (IEnumerable<SceneElement>) selection)
      {
        SceneElement parentElement = sceneElement.ParentElement;
        IPropertyId propertyId = (IPropertyId) parentElement.GetPropertyForChild((SceneNode) sceneElement);
        KeyValuePair<SceneElement, IPropertyId> key = new KeyValuePair<SceneElement, IPropertyId>(parentElement, propertyId);
        SceneElementCollection elementCollection;
        if (!dictionary.TryGetValue(key, out elementCollection))
        {
          elementCollection = new SceneElementCollection();
          dictionary.Add(key, elementCollection);
        }
        elementCollection.Add(sceneElement);
      }
      if (this.requiresSortedList)
      {
        ZOrderComparer<SceneElement> zorderComparer = new ZOrderComparer<SceneElement>(this.SceneViewModel.RootNode);
        foreach (KeyValuePair<KeyValuePair<SceneElement, IPropertyId>, SceneElementCollection> keyValuePair in dictionary)
          keyValuePair.Value.Sort((IComparer<SceneElement>) zorderComparer);
      }
      return dictionary;
    }
  }
}
