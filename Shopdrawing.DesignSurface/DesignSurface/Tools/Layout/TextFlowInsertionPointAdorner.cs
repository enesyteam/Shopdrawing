// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.TextFlowInsertionPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal class TextFlowInsertionPointAdorner : BaseFlowInsertionPointAdorner
  {
    public TextFlowInsertionPointAdorner(AdornerSet adornerSet, BaseFlowInsertionPoint baseFlowInsertionPoint)
      : base(adornerSet, baseFlowInsertionPoint)
    {
    }

    protected override bool GetInsertionInfo(SceneElement container, int insertionIndex, bool isCursorAtEnd, out Point position, out double length, out Orientation orientation)
    {
      orientation = Orientation.Horizontal;
      IViewTextPointer pointerFromIndex = this.GetTextPointerFromIndex(insertionIndex, isCursorAtEnd);
      if (pointerFromIndex != null && pointerFromIndex.HasValidLayout)
      {
        Rect characterRect = pointerFromIndex.GetCharacterRect(LogicalDirection.Forward);
        position = characterRect.TopLeft;
        length = characterRect.Height;
        return true;
      }
      position = new Point();
      length = 0.0;
      return false;
    }

    private IViewTextPointer GetTextPointerFromIndex(int insertionIndex, bool isCursorAtEnd)
    {
      if (!this.Element.IsViewObjectValid)
        return (IViewTextPointer) null;
      ITextFlowSceneNode textFlowSceneNode = this.Element as ITextFlowSceneNode;
      if (textFlowSceneNode == null)
        return (IViewTextPointer) null;
      if (isCursorAtEnd)
        return textFlowSceneNode.ContentEnd.GetInsertionPosition(LogicalDirection.Backward);
      return textFlowSceneNode.ContentStart.GetPositionAtOffset(insertionIndex, LogicalDirection.Forward);
    }
  }
}
