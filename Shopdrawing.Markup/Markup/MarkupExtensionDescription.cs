// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.MarkupExtensionDescription
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class MarkupExtensionDescription
  {
    private string name;
    private List<string> positionalArguments;
    private List<KeyValuePair<string, string>> namedArguments;

    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }

    public IList<string> PositionalArguments
    {
      get
      {
        return (IList<string>) this.positionalArguments;
      }
    }

    public IList<KeyValuePair<string, string>> NamedArguments
    {
      get
      {
        return (IList<KeyValuePair<string, string>>) this.namedArguments;
      }
    }

    public MarkupExtensionDescription()
    {
      this.positionalArguments = new List<string>();
      this.namedArguments = new List<KeyValuePair<string, string>>();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("{");
      if (this.name != null)
      {
        int num = 0;
        stringBuilder.Append(this.name);
        foreach (string str in this.positionalArguments)
        {
          stringBuilder.Append(num > 0 ? ", " : " ");
          stringBuilder.Append(str);
          ++num;
        }
        foreach (KeyValuePair<string, string> keyValuePair in this.namedArguments)
        {
          stringBuilder.Append(num > 0 ? ", " : " ");
          stringBuilder.Append(keyValuePair.Key);
          stringBuilder.Append("=");
          stringBuilder.Append(keyValuePair.Value);
          ++num;
        }
      }
      else
        stringBuilder.Append("<unnamed>");
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
