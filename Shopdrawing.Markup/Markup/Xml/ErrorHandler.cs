// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.ErrorHandler
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class ErrorHandler
  {
    private ErrorNodeList errors;

    public ErrorHandler(ErrorNodeList errors)
    {
      this.errors = errors;
    }

    public void HandleError(Node offendingNode, string code, params string[] args)
    {
      if (this.errors == null)
        return;
      ErrorNode errorNode1 = new ErrorNode(code, args);
      Document document = offendingNode.SourceContext.Document;
      int length = document.Text.Length;
      int startCol = Math.Min(offendingNode.SourceContext.StartCol, length);
      int endCol = Math.Min(offendingNode.SourceContext.EndCol, length);
      errorNode1.SourceContext = new SourceContext(document, startCol, endCol);
      int index1 = 0;
      for (int index2 = this.errors.Length - 1; index2 >= 0; --index2)
      {
        ErrorNode errorNode2 = this.errors[index2];
        if (errorNode2.Equals((object) errorNode1))
          return;
        if (errorNode2.SourceContext.StartCol <= errorNode1.SourceContext.StartCol)
        {
          index1 = index2 + 1;
          break;
        }
      }
      this.errors.Insert(index1, errorNode1);
    }
  }
}
