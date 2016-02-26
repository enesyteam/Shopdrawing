// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeCompoundPathCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeCompoundPathCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count < 2)
          return false;
        bool flag = true;
        SceneElement parentElement = elementSelectionSet.PrimarySelection.ParentElement;
        foreach (SceneElement element in elementSelectionSet.Selection)
        {
          if (!PathConversionHelper.CanConvert(element) || element.Parent != parentElement)
          {
            flag = false;
            break;
          }
        }
        return flag;
      }
    }

    public MakeCompoundPathCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitMakeCompoundPath))
      {
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        List<PathElement> otherElements = new List<PathElement>();
        foreach (SceneElement element in elementSelectionSet.Selection)
        {
          PathElement pathElement = element as PathElement;
          if (pathElement != null)
            otherElements.Add(pathElement);
          else if (PathConversionHelper.CanConvert(element))
            otherElements.Add(PathCommandHelper.ConvertToPath((BaseFrameworkElement) element));
        }
        editTransaction.Update();
        this.DesignerContext.ActiveView.UpdateLayout();
        otherElements.Sort((IComparer<PathElement>) new ZOrderComparer<PathElement>(this.SceneViewModel.RootNode));
        elementSelectionSet.Clear();
        PathElement mainElement = otherElements[0];
        BaseFrameworkElement frameworkElement = (BaseFrameworkElement) otherElements[otherElements.Count - 1];
        ISceneNodeCollection<SceneNode> collectionForChild = frameworkElement.ParentElement.GetCollectionForChild((SceneNode) frameworkElement);
        int num = collectionForChild.IndexOf((SceneNode) frameworkElement);
        SceneNode sceneNode = num + 1 < collectionForChild.Count ? collectionForChild[num + 1] : (SceneNode) null;
        otherElements.RemoveAt(0);
        PathCommandHelper.MakeCompoundPath(mainElement, otherElements, editTransaction);
        mainElement.Remove();
        if (sceneNode == null)
          collectionForChild.Add((SceneNode) mainElement);
        else
          collectionForChild.Insert(collectionForChild.IndexOf(sceneNode), (SceneNode) mainElement);
        elementSelectionSet.SetSelection((SceneElement) mainElement);
        editTransaction.Commit();
      }
    }
  }
}
