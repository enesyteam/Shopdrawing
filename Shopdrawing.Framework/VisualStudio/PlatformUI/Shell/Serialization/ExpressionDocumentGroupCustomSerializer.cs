// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.ExpressionDocumentGroupCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  internal class ExpressionDocumentGroupCustomSerializer : ViewElementCustomSerializer
  {
    public override object Content
    {
      get
      {
        return (object) null;
      }
    }

    public ExpressionDocumentGroupCustomSerializer(ExpressionDocumentGroup group)
      : base((ViewElement) group)
    {
    }
  }
}
