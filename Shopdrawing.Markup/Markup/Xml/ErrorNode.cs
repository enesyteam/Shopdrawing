// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.ErrorNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class ErrorNode
  {
    public SourceContext SourceContext;
    private string errorCode;
    private string[] errorParameters;

    public string ErrorCode
    {
      get
      {
        return this.errorCode;
      }
    }

    public string[] ErrorParameters
    {
      get
      {
        return this.errorParameters;
      }
    }

    public ErrorNode(string code, string[] parms)
    {
      this.errorCode = code;
      this.errorParameters = parms;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      ErrorNode errorNode = obj as ErrorNode;
      if (errorNode == null || !(this.errorCode == errorNode.errorCode) || (!this.SourceContext.Equals((object) errorNode.SourceContext) || this.errorParameters.Length != errorNode.errorParameters.Length))
        return false;
      for (int index = 0; index < this.errorParameters.Length; ++index)
      {
        if (this.errorParameters[index] != errorNode.errorParameters[index])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return this.errorCode.GetHashCode() ^ this.SourceContext.GetHashCode() ^ this.errorParameters.GetHashCode();
    }
  }
}
