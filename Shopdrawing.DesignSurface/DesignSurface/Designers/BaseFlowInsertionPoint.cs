// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.BaseFlowInsertionPoint
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public abstract class BaseFlowInsertionPoint
  {
    private BaseFrameworkElement element;
    private int index;
    private bool isCursorAtEnd;

    public BaseFrameworkElement Element
    {
      get
      {
        return this.element;
      }
      set
      {
        this.SetInsertionPoint(value, this.index, this.isCursorAtEnd);
      }
    }

    public int Index
    {
      get
      {
        return this.index;
      }
      set
      {
        this.SetInsertionPoint(this.element, value, this.isCursorAtEnd);
      }
    }

    public bool IsCursorAtEnd
    {
      get
      {
        return this.isCursorAtEnd;
      }
      set
      {
        this.SetInsertionPoint(this.element, this.index, value);
      }
    }

    public bool IsEmpty
    {
      get
      {
        if (this.element != null)
          return this.index < 0;
        return true;
      }
    }

    public void Clear()
    {
      if (this.IsEmpty)
        return;
      this.SetInsertionPoint((BaseFrameworkElement) null, -1, true);
    }

    protected void SetInsertionPoint(BaseFrameworkElement element, int index, bool isCursorAtEnd)
    {
      if (this.Element == element && this.Index == index && this.IsCursorAtEnd == isCursorAtEnd)
        return;
      this.element = element;
      this.index = index;
      this.isCursorAtEnd = isCursorAtEnd;
    }

    public abstract void Insert(ISceneNodeCollection<SceneNode> children, SceneElement element);
  }
}
