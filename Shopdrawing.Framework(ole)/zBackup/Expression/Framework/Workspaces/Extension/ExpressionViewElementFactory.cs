// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionViewElementFactory
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  internal class ExpressionViewElementFactory : ViewElementFactory
  {
    protected override View CreateViewCore(Type viewType)
    {
      if (viewType.Equals(typeof (View)))
        return (View) new ExpressionView();
      if (typeof (ExpressionView).IsAssignableFrom(viewType))
        return (View) Activator.CreateInstance(viewType);
      return base.CreateViewCore(viewType);
    }

    protected override DockGroup CreateDockGroupCore()
    {
      return (DockGroup) new ExpressionDockGroup();
    }

    protected override DockRoot CreateDockRootCore()
    {
      return (DockRoot) new ExpressionDockRoot();
    }

    protected override DocumentGroup CreateDocumentGroupCore()
    {
      return (DocumentGroup) new ExpressionDocumentGroup();
    }
  }
}
