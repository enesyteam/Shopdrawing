// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ViewElementFactory
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class ViewElementFactory
  {
    public static ViewElementFactory Current { get; set; }

    internal bool IsConstructionAllowed
    {
      get
      {
        return this.AllowConstructionReferences > 0;
      }
    }

    private int AllowConstructionReferences { get; set; }

    static ViewElementFactory()
    {
      ViewElementFactory.Current = new ViewElementFactory();
    }

    public TabGroup CreateTabGroup()
    {
      using (this.AllowConstruction())
        return this.CreateTabGroupCore();
    }

    protected virtual TabGroup CreateTabGroupCore()
    {
      return new TabGroup();
    }

    public DockGroup CreateDockGroup()
    {
      using (this.AllowConstruction())
        return this.CreateDockGroupCore();
    }

    protected virtual DockGroup CreateDockGroupCore()
    {
      return new DockGroup();
    }

    public DocumentGroup CreateDocumentGroup()
    {
      using (this.AllowConstruction())
        return this.CreateDocumentGroupCore();
    }

    protected virtual DocumentGroup CreateDocumentGroupCore()
    {
      return new DocumentGroup();
    }

    public DocumentGroupContainer CreateDocumentGroupContainer()
    {
      using (this.AllowConstruction())
        return this.CreateDocumentGroupContainerCore();
    }

    protected virtual DocumentGroupContainer CreateDocumentGroupContainerCore()
    {
      return new DocumentGroupContainer();
    }

    public AutoHideGroup CreateAutoHideGroup()
    {
      using (this.AllowConstruction())
        return this.CreateAutoHideGroupCore();
    }

    protected virtual AutoHideGroup CreateAutoHideGroupCore()
    {
      return new AutoHideGroup();
    }

    public AutoHideChannel CreateAutoHideChannel()
    {
      using (this.AllowConstruction())
        return this.CreateAutoHideChannelCore();
    }

    protected virtual AutoHideChannel CreateAutoHideChannelCore()
    {
      return new AutoHideChannel();
    }

    public AutoHideRoot CreateAutoHideRoot()
    {
      using (this.AllowConstruction())
        return this.CreateAutoHideRootCore();
    }

    protected virtual AutoHideRoot CreateAutoHideRootCore()
    {
      return new AutoHideRoot();
    }

    public DockRoot CreateDockRoot()
    {
      using (this.AllowConstruction())
        return this.CreateDockRootCore();
    }

    protected virtual DockRoot CreateDockRootCore()
    {
      return new DockRoot();
    }

    public FloatSite CreateFloatSite()
    {
      using (this.AllowConstruction())
        return this.CreateFloatSiteCore();
    }

    protected virtual FloatSite CreateFloatSiteCore()
    {
      return new FloatSite();
    }

    public MainSite CreateMainSite()
    {
      using (this.AllowConstruction())
        return this.CreateMainSiteCore();
    }

    protected virtual MainSite CreateMainSiteCore()
    {
      return new MainSite();
    }

    public View CreateView()
    {
      using (this.AllowConstruction())
        return this.CreateViewCore(typeof (View));
    }

    public View CreateView(Type viewType)
    {
      if (!typeof (View).IsAssignableFrom(viewType))
        throw new InvalidOperationException("viewType must derive from View");
      using (this.AllowConstruction())
        return this.CreateViewCore(viewType);
    }

    protected virtual View CreateViewCore(Type viewType)
    {
      return (View) Activator.CreateInstance(viewType);
    }

    public ViewBookmark CreateViewBookmark()
    {
      using (this.AllowConstruction())
        return this.CreateViewBookmarkCore();
    }

    protected virtual ViewBookmark CreateViewBookmarkCore()
    {
      return new ViewBookmark();
    }

    public IDisposable AllowConstruction()
    {
      return (IDisposable) new ViewElementFactory.AllowPublicConstructionScope(this);
    }

    private class AllowPublicConstructionScope : DisposableObject
    {
      private ViewElementFactory Factory { get; set; }

      public AllowPublicConstructionScope(ViewElementFactory factory)
      {
        this.Factory = factory;
        ++this.Factory.AllowConstructionReferences;
      }

      protected override void DisposeManagedResources()
      {
        --this.Factory.AllowConstructionReferences;
      }
    }
  }
}
