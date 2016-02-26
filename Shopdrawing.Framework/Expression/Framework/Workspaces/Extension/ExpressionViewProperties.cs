// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionViewProperties
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class ExpressionViewProperties
  {
    public bool IsAutoHideable { get; private set; }

    public bool IsTitleHidden { get; private set; }

    public bool IsSizableWhenFloating { get; private set; }

    public Size? DefaultFloatSize { get; private set; }

    public virtual Type ViewType
    {
      get
      {
        return typeof (ExpressionView);
      }
    }

    public ExpressionViewProperties(bool isAutoHideable)
      : this(isAutoHideable, false)
    {
    }

    public ExpressionViewProperties(bool isAutoHideable, bool isTitleHidden)
      : this(isAutoHideable, isTitleHidden, true)
    {
    }

    public ExpressionViewProperties(bool isAutoHideable, bool isTitleHidden, bool isSizableWhenFloating)
      : this(isAutoHideable, isTitleHidden, isSizableWhenFloating, new Size?())
    {
    }

    public ExpressionViewProperties(bool isAutoHideable, bool isTitleHidden, bool isSizableWhenFloating, Size? defaultFloatSize)
    {
      this.IsAutoHideable = isAutoHideable;
      this.IsTitleHidden = isTitleHidden;
      this.IsSizableWhenFloating = isSizableWhenFloating;
      this.DefaultFloatSize = defaultFloatSize;
    }

    public virtual void Apply(ExpressionView viewObject)
    {
      viewObject.IsAutoHideable = this.IsAutoHideable;
      viewObject.IsTitleHidden = this.IsTitleHidden;
      viewObject.IsSizableWhenFloating = this.IsSizableWhenFloating;
      viewObject.DefaultFloatSize = this.DefaultFloatSize;
    }
  }
}
