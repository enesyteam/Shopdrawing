// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlDeclaration
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class XmlDeclaration : XmlElement
  {
    private static Identifier PIName = Identifier.For("?xml");
    public string Encoding;

    public XmlDeclaration(XmlDocument doc)
      : base(doc)
    {
      this.Name = XmlDeclaration.PIName;
      this.NodeType = NodeType.XmlDeclaration;
    }

    public static ArrayList GetEncodings()
    {
      ArrayList arrayList = new ArrayList();
      foreach (EncodingInfo encodingInfo in System.Text.Encoding.GetEncodings())
        arrayList.Add((object) encodingInfo.GetEncoding().WebName);
      return arrayList;
    }
  }
}
