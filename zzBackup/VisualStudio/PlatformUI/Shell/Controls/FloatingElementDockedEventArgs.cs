// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.FloatingElementDockedEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class FloatingElementDockedEventArgs : FloatingOperationFinishedEventArgs
  {
    public DockDirection DockDirection { get; private set; }

    public bool CreateDocumentGroup { get; private set; }

    public FloatingElementDockedEventArgs(RoutedEvent routedEvent, ViewElement content, DockDirection dockDirection, bool createDocumentGroup)
      : base(routedEvent, content)
    {
      this.DockDirection = dockDirection;
      this.CreateDocumentGroup = createDocumentGroup;
    }
  }
}
