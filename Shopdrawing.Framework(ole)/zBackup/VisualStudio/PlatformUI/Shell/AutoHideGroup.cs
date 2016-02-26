// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.AutoHideGroup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class AutoHideGroup : ViewGroup
  {
    public ViewGroup OriginalGroup
    {
      get
      {
        if (this.Children.Count == 0)
          return (ViewGroup) null;
        View view = (View) this.Children[0];
        ViewBookmark bookmark = DockOperations.FindBookmark(view, view.WindowProfile);
        if (bookmark == null)
          return (ViewGroup) null;
        return bookmark.Parent;
      }
    }

    public override bool IsChildAllowed(ViewElement element)
    {
      return element is View;
    }

    public static AutoHideGroup Create()
    {
      return ViewElementFactory.Current.CreateAutoHideGroup();
    }

    public override bool IsChildOnScreen(int childIndex)
    {
      if (this.IsOnScreen)
        return this.Children[childIndex].IsSelected;
      return false;
    }
  }
}
