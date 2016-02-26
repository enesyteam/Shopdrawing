// Decompiled with JetBrains decompiler
// Type: ExceptionStringTable
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

[DebuggerNonUserCode]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[CompilerGenerated]
internal class ExceptionStringTable
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (object.ReferenceEquals((object) ExceptionStringTable.resourceMan, (object) null))
        ExceptionStringTable.resourceMan = new ResourceManager("ExceptionStringTable", typeof (ExceptionStringTable).Assembly);
      return ExceptionStringTable.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get
    {
      return ExceptionStringTable.resourceCulture;
    }
    set
    {
      ExceptionStringTable.resourceCulture = value;
    }
  }

  internal static string DocumentFileNotFound
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("DocumentFileNotFound", ExceptionStringTable.resourceCulture);
    }
  }

  internal ExceptionStringTable()
  {
  }
}
