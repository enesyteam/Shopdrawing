// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PaintBucketTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class PaintBucketTool : Tool
  {
    public override string Identifier
    {
      get
      {
        return "PaintBucket";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.PaintBucketToolCaption;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.F;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "bucket";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.PaintBucket;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    public override bool KeepTextSelectionSet
    {
      get
      {
        return true;
      }
    }

    public PaintBucketTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new PropertyToolBehavior(this.GetActiveViewContext(), PropertyToolAction.PaintBucket);
    }
  }
}
