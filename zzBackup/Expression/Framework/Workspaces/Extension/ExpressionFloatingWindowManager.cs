// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionFloatingWindowManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class ExpressionFloatingWindowManager : FloatingWindowManager
  {
    protected override FloatingWindow CreateFloatingWindow(FloatSite floatSite)
    {
      bool isSizable = (bool) ExpressionView.IsSizableWhenFloatingProperty.DefaultMetadata.DefaultValue;
      ExpressionView expressionView = floatSite.Child as ExpressionView;
      if (expressionView != null)
        isSizable = expressionView.IsSizableWhenFloating;
      return (FloatingWindow) new ExpressionFloatingWindow(isSizable);
    }
  }
}
