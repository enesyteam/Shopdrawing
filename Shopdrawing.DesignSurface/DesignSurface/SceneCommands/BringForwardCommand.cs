// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.BringForwardCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class BringForwardCommand : BaseZOrderCommand
  {
    protected override string UndoDescription
    {
      get
      {
        return StringTable.UndoUnitBringForward;
      }
    }

    public BringForwardCommand(SceneViewModel viewModel)
      : base(viewModel, true)
    {
    }

    protected override void ReorderElements(ISceneNodeCollection<SceneNode> childCollection, SceneElementCollection selectedChildren)
    {
      if (childCollection.Count == selectedChildren.Count)
        return;
      int num1 = childCollection.Count - 1;
      for (int index = selectedChildren.Count - 1; index >= 0; --index)
      {
        SceneElement sceneElement = selectedChildren[index];
        int num2 = childCollection.IndexOf((SceneNode) sceneElement);
        if (num2 < num1 && !selectedChildren.Contains(childCollection[num2 + 1] as SceneElement))
        {
          sceneElement.Remove();
          childCollection.Insert(num2 + 1, (SceneNode) sceneElement);
        }
      }
    }
  }
}
