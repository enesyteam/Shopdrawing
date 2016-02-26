// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.NodeType
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal enum NodeType
  {
    Error = 1,
    Identifier = 2,
    Literal = 3,
    Namespace = 4,
    WhitespaceLiteral = 4000,
    XmlDeclaration = 4001,
    XmlElement = 4002,
    XmlAttribute = 4003,
    XmlDocument = 4004,
    XmlEntityReference = 4005,
    XmlComment = 4006,
    XmlProcessingInstruction = 4007,
    XmlCDATA = 4008,
    XmlDocType = 4009,
    XmlError = 4010,
    Dtd = 4011,
    ElementDecl = 4012,
    AttList = 4013,
    AttDef = 4014,
  }
}
