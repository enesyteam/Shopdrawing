// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DragDropInsertBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DragDropInsertBehavior : ToolBehavior
  {
    private InsertionPointHighlighter previewHighlighter;

    protected ISceneInsertionPoint PreviewInsertionPoint
    {
      get
      {
        return this.previewHighlighter.InsertionPointPreview;
      }
      private set
      {
        this.previewHighlighter.InsertionPointPreview = value;
      }
    }

    internal DragDropInsertBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.previewHighlighter = new InsertionPointHighlighter(toolContext);
    }

    private void UpdatePreviewElement(Point pointerPosition)
    {
      this.PreviewInsertionPoint = ElementCreateBehavior.GetInsertionPointToPreview(this.ActiveSceneViewModel, new InsertionPointContext(pointerPosition));
    }

    protected override bool OnDragOver(DragEventArgs args)
    {
      this.UpdatePreviewElement(args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer));
      return false;
    }

    protected override bool OnDragLeave(DragEventArgs args)
    {
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      return base.OnDragLeave(args);
    }

    protected override void OnDetach()
    {
      this.PreviewInsertionPoint = (ISceneInsertionPoint) null;
      base.OnDetach();
    }
  }
}
