// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.DocumentGroup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class DocumentGroup : NestedGroup
  {
    public override bool IsChildAllowed(ViewElement element)
    {
      if (!(element is View))
        return element is ViewBookmark;
      return true;
    }

    public static bool IsTabbedDocument(ViewElement element)
    {
      return Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<DocumentGroup, ViewElement>(element, (Func<ViewElement, ViewElement>) (e => (ViewElement) e.Parent)) != null;
    }

    public static DocumentGroup Create()
    {
      return ViewElementFactory.Current.CreateDocumentGroup();
    }
  }
}
