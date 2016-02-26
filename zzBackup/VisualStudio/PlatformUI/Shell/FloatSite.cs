// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.FloatSite
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class FloatSite : ViewSite
  {
    public static bool IsFloating(ViewElement element)
    {
      return ViewElement.FindRootElement(element) is FloatSite;
    }

    public override bool IsChildAllowed(ViewElement element)
    {
      if (!(element is View) && !(element is DockGroup))
        return element is TabGroup;
      return true;
    }

    public static FloatSite Create()
    {
      return ViewElementFactory.Current.CreateFloatSite();
    }
  }
}
