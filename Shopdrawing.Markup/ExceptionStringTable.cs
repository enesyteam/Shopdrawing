// Decompiled with JetBrains decompiler
// Type: ExceptionStringTable
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[DebuggerNonUserCode]
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

  internal static string DocumentNodeCannotReparentNode
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("DocumentNodeCannotReparentNode", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string DocumentNodeValueTypeValueIsNull
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("DocumentNodeValueTypeValueIsNull", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string InvalidIndentString
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("InvalidIndentString", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string InvalidMarkupExtensionAttributeQuote
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("InvalidMarkupExtensionAttributeQuote", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string InvalidSpacesPerTabStop
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("InvalidSpacesPerTabStop", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string InvalidXmlAttributeQuote
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("InvalidXmlAttributeQuote", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string InvalidXmlnsPrefix
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("InvalidXmlnsPrefix", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string MemberTypeValueShouldContainExactlyOneType
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("MemberTypeValueShouldContainExactlyOneType", ExceptionStringTable.resourceCulture);
    }
  }

  internal static string UnsitedValueCannotBeConstructed
  {
    get
    {
      return ExceptionStringTable.ResourceManager.GetString("UnsitedValueCannotBeConstructed", ExceptionStringTable.resourceCulture);
    }
  }

  internal ExceptionStringTable()
  {
  }
}
