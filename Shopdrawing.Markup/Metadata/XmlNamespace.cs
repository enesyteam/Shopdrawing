// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.XmlNamespace
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Metadata
{
  public sealed class XmlNamespace : IXmlNamespace
  {
    public static readonly XmlNamespace AvalonXmlNamespace = new XmlNamespace("http://schemas.microsoft.com/winfx/2006/xaml/presentation");
    public static readonly XmlNamespace NetFx2007Namespace = new XmlNamespace("http://schemas.microsoft.com/netfx/2007/xaml/presentation");
    public static readonly XmlNamespace JoltXmlNamespace = new XmlNamespace("http://schemas.microsoft.com/client/2007");
    public static readonly XmlNamespace XamlXmlNamespace = new XmlNamespace("http://schemas.microsoft.com/winfx/2006/xaml");
    public static readonly XmlNamespace PresentationOptionsXmlNamespace = new XmlNamespace("http://schemas.microsoft.com/winfx/2006/xaml/presentation/options");
    public static readonly XmlNamespace CompatibilityXmlNamespace = new XmlNamespace("http://schemas.openxmlformats.org/markup-compatibility/2006");
    public static readonly XmlNamespace DesignTimeXmlNamespace = new XmlNamespace("http://schemas.microsoft.com/expression/blend/2008");
    public static readonly XmlNamespace AnnotationsXmlNamespace = new XmlNamespace("http://schemas.microsoft.com/expression/blend/extensions/annotations/2008");
    public const string XmlStandardXmlnsUri = "http://www.w3.org/2000/xmlns/";
    public const string XamlUri = "http://schemas.microsoft.com/winfx/2006/xaml";
    public const string AvalonUri = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    public const string NetFx2007Uri = "http://schemas.microsoft.com/netfx/2007/xaml/presentation";
    public const string JoltUri = "http://schemas.microsoft.com/client/2007";
    public const string PresentationOptionsUri = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options";
    public const string CompatibilityUri = "http://schemas.openxmlformats.org/markup-compatibility/2006";
    public const string DesignTimeUri2006 = "http://schemas.microsoft.com/expression/interactivedesigner/2006";
    public const string DesignTimeUriBlend2006 = "http://schemas.microsoft.com/expression/blend/2006";
    public const string DesignTimeUri = "http://schemas.microsoft.com/expression/blend/2008";
    public const string AnnotationsUri = "http://schemas.microsoft.com/expression/blend/extensions/annotations/2008";
    private string value;

    public string Value
    {
      get
      {
        return this.value;
      }
    }

    private XmlNamespace(string value)
    {
      this.value = value;
    }

    public static XmlNamespace ToNamespace(string value, XmlNamespaceCanonicalization canonicalization)
    {
      switch (value)
      {
        case "http://schemas.microsoft.com/winfx/2006/xaml/presentation":
          if (canonicalization == XmlNamespaceCanonicalization.ToJolt)
            return XmlNamespace.JoltXmlNamespace;
          return XmlNamespace.AvalonXmlNamespace;
        case "http://schemas.microsoft.com/client/2007":
          if (canonicalization == XmlNamespaceCanonicalization.ToAvalon)
            return XmlNamespace.AvalonXmlNamespace;
          return XmlNamespace.JoltXmlNamespace;
        case "http://schemas.microsoft.com/netfx/2007/xaml/presentation":
          return XmlNamespace.NetFx2007Namespace;
        case "http://schemas.microsoft.com/winfx/2006/xaml":
          return XmlNamespace.XamlXmlNamespace;
        case "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options":
          return XmlNamespace.PresentationOptionsXmlNamespace;
        case "http://schemas.openxmlformats.org/markup-compatibility/2006":
          return XmlNamespace.CompatibilityXmlNamespace;
        case "http://schemas.microsoft.com/expression/blend/2008":
        case "http://schemas.microsoft.com/expression/interactivedesigner/2006":
        case "http://schemas.microsoft.com/expression/blend/2006":
          return XmlNamespace.DesignTimeXmlNamespace;
        case "http://schemas.microsoft.com/expression/blend/extensions/annotations/2008":
          return XmlNamespace.AnnotationsXmlNamespace;
        default:
          return new XmlNamespace(value);
      }
    }

    public override bool Equals(object obj)
    {
      XmlNamespace xmlNamespace = obj as XmlNamespace;
      if (xmlNamespace != null)
        return xmlNamespace.value == this.value;
      return false;
    }

    public override int GetHashCode()
    {
      return this.value.GetHashCode();
    }

    public override string ToString()
    {
      return this.value;
    }

    public static XmlNamespaceCanonicalization GetNamespaceCanonicalization(ITypeResolver typeResolver)
    {
      return typeResolver.IsCapabilitySet(PlatformCapability.CanonicalizeNamespaceToSilverlight) ? XmlNamespaceCanonicalization.ToJolt : XmlNamespaceCanonicalization.None;
    }
  }
}
