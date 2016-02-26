// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.DocumentGroupContainer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class DocumentGroupContainer : DockGroup
  {
    public DocumentGroupContainer()
    {
      this.IsVisible = true;
      AutoHideRoot.SetIsAutoHideCenter((ViewElement) this, true);
    }

    public override bool IsChildAllowed(ViewElement element)
    {
      return element is DocumentGroup;
    }

    protected internal override void OnChildVisibilityChanged()
    {
      this.RebuildVisibleChildren();
    }

    public static DocumentGroupContainer Create()
    {
      return ViewElementFactory.Current.CreateDocumentGroupContainer();
    }
  }
}
