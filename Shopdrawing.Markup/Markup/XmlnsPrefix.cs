// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlnsPrefix
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class XmlnsPrefix
  {
    private static XmlnsPrefix emptyPrefix = new XmlnsPrefix(string.Empty);
    private string value;

    public static XmlnsPrefix EmptyPrefix
    {
      get
      {
        return XmlnsPrefix.emptyPrefix;
      }
    }

    public string Value
    {
      get
      {
        return this.value;
      }
    }

    private XmlnsPrefix(string value)
    {
      this.value = value;
    }

    public static XmlnsPrefix ToPrefix(string value)
    {
      if (value == null || value.Length == 0)
        return XmlnsPrefix.EmptyPrefix;
      return new XmlnsPrefix(value);
    }

    public override string ToString()
    {
      return this.value;
    }

    public override bool Equals(object obj)
    {
      XmlnsPrefix xmlnsPrefix = obj as XmlnsPrefix;
      if (xmlnsPrefix != null)
        return xmlnsPrefix.value == this.value;
      return false;
    }

    public override int GetHashCode()
    {
      return this.value.GetHashCode();
    }
  }
}
