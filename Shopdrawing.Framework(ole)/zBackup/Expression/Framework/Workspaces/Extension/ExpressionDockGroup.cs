// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionDockGroup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class ExpressionDockGroup : DockGroup, ICustomXmlSerializable
  {
    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new ExpressionDockGroupCustomSerializer(this);
    }

    public override bool IsChildAllowed(ViewElement element)
    {
      if (!(element is NakedView))
        return base.IsChildAllowed(element);
      return true;
    }
  }
}
