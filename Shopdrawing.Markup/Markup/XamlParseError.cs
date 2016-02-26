// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlParseError
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class XamlParseError
  {
    private XamlErrorSeverity severity;
    private readonly int errorCode;
    private readonly ITextLocation lineInformation;
    private readonly string messageFormat;
    private readonly List<string> parameters;

    public XamlErrorSeverity Severity
    {
      get
      {
        return this.severity;
      }
    }

    public int ErrorCode
    {
      get
      {
        return this.errorCode;
      }
    }

    public IList<string> Parameters
    {
      get
      {
        return (IList<string>) this.parameters;
      }
    }

    public int Line
    {
      get
      {
        return this.lineInformation.Line;
      }
    }

    public int Column
    {
      get
      {
        return this.lineInformation.Column;
      }
    }

    public string Message
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.messageFormat, (object[]) this.parameters.ToArray());
      }
    }

    public string FullMessage
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ParserErrorFullMessage, (object) this.Line, (object) this.Column, (object) this.Message);
      }
    }

    internal XamlParseError(XamlErrorSeverity severity, int errorCode, ITextLocation lineInformation, string messageFormat, params string[] parameters)
    {
      this.severity = severity;
      this.errorCode = errorCode;
      this.lineInformation = lineInformation != null ? lineInformation : (ITextLocation) new TextLocation(0, 0);
      this.messageFormat = messageFormat;
      this.parameters = new List<string>((IEnumerable<string>) parameters);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      XamlParseError xamlParseError = obj as XamlParseError;
      if (xamlParseError == null || this.errorCode != xamlParseError.errorCode || (!this.lineInformation.Equals((object) xamlParseError.lineInformation) || !(this.messageFormat == xamlParseError.messageFormat)) || this.parameters.Count != xamlParseError.parameters.Count)
        return false;
      for (int index = 0; index < this.parameters.Count; ++index)
      {
        if (this.parameters[index] != xamlParseError.parameters[index])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return this.errorCode.GetHashCode() ^ this.lineInformation.GetHashCode() ^ this.parameters.GetHashCode();
    }

    public override string ToString()
    {
      return this.FullMessage;
    }
  }
}
