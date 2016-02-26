// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.XmlDataProviderSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class XmlDataProviderSceneNode : DataSourceProviderSceneNode
  {
    public static readonly IPropertyId DocumentProperty = (IPropertyId) PlatformTypes.XmlDataProvider.GetMember(MemberType.LocalProperty, "Document", MemberAccessTypes.Public);
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.XmlDataProvider.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly IPropertyId XmlNamespaceManagerProperty = (IPropertyId) PlatformTypes.XmlDataProvider.GetMember(MemberType.LocalProperty, "XmlNamespaceManager", MemberAccessTypes.Public);
    public static readonly IPropertyId XmlSerializerProperty = (IPropertyId) PlatformTypes.XmlDataProvider.GetMember(MemberType.LocalProperty, "XmlSerializer", MemberAccessTypes.Public);
    public static readonly IPropertyId XPathProperty = (IPropertyId) PlatformTypes.XmlDataProvider.GetMember(MemberType.LocalProperty, "XPath", MemberAccessTypes.Public);
    public static readonly XmlDataProviderSceneNode.ConcreteXmlDataProviderSceneNodeFactory Factory = new XmlDataProviderSceneNode.ConcreteXmlDataProviderSceneNodeFactory();

    public XmlNamespaceManager XmlNamespaceManager
    {
      get
      {
        return (XmlNamespaceManager) this.GetLocalValue(XmlDataProviderSceneNode.XmlNamespaceManagerProperty);
      }
      set
      {
        this.SetLocalValue(XmlDataProviderSceneNode.XmlNamespaceManagerProperty, (object) value);
      }
    }

    public XmlDocument Document
    {
      get
      {
        return (XmlDocument) this.GetLocalValue(XmlDataProviderSceneNode.DocumentProperty);
      }
      set
      {
        this.SetLocalValue(XmlDataProviderSceneNode.DocumentProperty, (object) value);
      }
    }

    public string InlineXmlData
    {
      get
      {
        SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(XmlDataProviderSceneNode.XmlSerializerProperty);
        if (valueAsSceneNode != null)
          return (string) valueAsSceneNode.GetLocalValue(DesignTimeProperties.InlineXmlProperty);
        return (string) null;
      }
    }

    public string XPath
    {
      get
      {
        return (string) this.GetLocalValue(XmlDataProviderSceneNode.XPathProperty);
      }
      set
      {
        this.SetLocalValue(XmlDataProviderSceneNode.XPathProperty, (object) value);
      }
    }

    public class ConcreteXmlDataProviderSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new XmlDataProviderSceneNode();
      }
    }
  }
}
