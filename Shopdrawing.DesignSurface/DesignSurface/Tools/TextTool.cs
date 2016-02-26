// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.TextTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Text;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class TextTool : GenericControlTool
  {
    public static readonly IEnumerable<ITypeId> TextToolTypes = (IEnumerable<ITypeId>) new ITypeId[6]
    {
      PlatformTypes.TextBlock,
      PlatformTypes.TextBox,
      PlatformTypes.RichTextBox,
      PlatformTypes.PasswordBox,
      ProjectNeutralTypes.Label,
      PlatformTypes.FlowDocumentScrollViewer
    };

    public override Key Key
    {
      get
      {
        return Key.None;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return true;
      }
    }

    public TextTool(ToolContext toolContext, ITypeId textBoxType)
      : base(toolContext, textBoxType, ToolCategory.Text)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new TextToolBehavior(this.GetActiveViewContext(), (ToolBehavior) new TextCreateBehavior(this.GetActiveViewContext(), this.ShapeType));
    }

    protected override void OnDoubleClick()
    {
      if (!this.CreateDefaultElement(this.ShapeType))
        return;
      this.ActiveView.TryEnterTextEditMode(false);
    }
  }
}
