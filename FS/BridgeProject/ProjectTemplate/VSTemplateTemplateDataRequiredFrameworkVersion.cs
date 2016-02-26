using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.Templates
{
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
    public enum VSTemplateTemplateDataRequiredFrameworkVersion
    {
        [XmlEnum("2.0")]
        Item20,
        [XmlEnum("3.0")]
        Item30,
        [XmlEnum("3.5")]
        Item35,
        [XmlEnum("4.0")]
        Item40
    }
}