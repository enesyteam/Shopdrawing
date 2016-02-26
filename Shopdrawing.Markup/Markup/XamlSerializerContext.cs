// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlSerializerContext
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class XamlSerializerContext
  {
    private bool removeProjectRootFromUri = true;
    private Dictionary<ITypeId, XmlnsPrefix> prefixesForType = new Dictionary<ITypeId, XmlnsPrefix>();
    private PersistenceSettings persistenceSettings;
    private IDocumentContext documentContext;
    private CultureInfo cultureInfo;
    private IType targetType;
    private ITypeId styleOrTemplateType;
    private RootNode root;
    private ContainerNode container;
    private ClrNamespaceUriParseCache clrNamespaceUriParseCache;
    private XamlSerializerContext.ClrNamespaceUriSerializeCache clrNamespaceUriSerializeCache;

    internal XmlSpace XmlSpace
    {
      get
      {
        ElementNode elementNode = this.container as ElementNode;
        if (elementNode != null)
          return elementNode.XmlSpace;
        return XmlSpace.Default;
      }
    }

    internal PersistenceSettings Settings
    {
      get
      {
        return this.persistenceSettings;
      }
    }

    internal IDocumentContext DocumentContext
    {
      get
      {
        return this.documentContext;
      }
    }

    public ITypeResolver TypeResolver
    {
      get
      {
        return this.documentContext.TypeResolver;
      }
    }

    internal IType TargetType
    {
      get
      {
        return this.targetType;
      }
    }

    internal CultureInfo Culture
    {
      get
      {
        return this.cultureInfo;
      }
    }

    public bool InStyleOrTemplate
    {
      get
      {
        return this.styleOrTemplateType != null;
      }
    }

    internal bool RemoveProjectRootFromUri
    {
      get
      {
        return this.removeProjectRootFromUri;
      }
    }

    internal RootNode DocumentRoot
    {
      get
      {
        return this.root;
      }
    }

    internal ContainerNode Node
    {
      get
      {
        return this.container;
      }
    }

    internal XamlSerializerContext(PersistenceSettings persistenceSettings, IDocumentContext documentContext, CultureInfo cultureInfo)
      : this(persistenceSettings, documentContext, cultureInfo, documentContext.TypeResolver.ProjectAssembly, true)
    {
    }

    public XamlSerializerContext(PersistenceSettings persistenceSettings, IDocumentContext documentContext, CultureInfo cultureInfo, IAssembly projectAssembly, bool removeProjectRootFromUri)
    {
      this.persistenceSettings = persistenceSettings;
      this.documentContext = documentContext;
      this.cultureInfo = cultureInfo;
      this.root = new RootNode();
      this.container = (ContainerNode) this.root;
      this.clrNamespaceUriParseCache = new ClrNamespaceUriParseCache(this.TypeResolver);
      this.clrNamespaceUriSerializeCache = new XamlSerializerContext.ClrNamespaceUriSerializeCache(this.documentContext, projectAssembly);
      this.removeProjectRootFromUri = removeProjectRootFromUri;
    }

    internal IDisposable PushTargetType(IType targetType)
    {
      IDisposable disposable = (IDisposable) new XamlSerializerContext.DisposableTargetTypeContext(this, this.targetType);
      this.targetType = targetType;
      return disposable;
    }

    internal IDisposable PushStyleOrTemplate(ITypeId type)
    {
      IDisposable disposable = (IDisposable) new XamlSerializerContext.DisposableStyleOrTemplateContext(this, this.styleOrTemplateType);
      this.styleOrTemplateType = type;
      return disposable;
    }

    internal IDisposable PushNode(ElementNode node)
    {
      ContainerNode container = this.container;
      this.container = (ContainerNode) node;
      if (node.HasNamespaces)
        this.InvalidatePrefixCache();
      return (IDisposable) new XamlSerializerContext.DisposableNodeContext(this, container);
    }

    internal void InvalidatePrefixCache()
    {
      this.prefixesForType.Clear();
    }

    internal bool IsDefaultNamespace(IXmlNamespace xmlNamespace)
    {
      XmlNamespace xmlNamespace1 = (XmlNamespace) this.TypeResolver.GetCapabilityValue(PlatformCapability.DefaultXmlns);
      return xmlNamespace == xmlNamespace1;
    }

    internal XmlnsPrefix GetNearestXmlnsPrefix(ITypeId typeId)
    {
      ElementNode elementNode = (ElementNode) this.Node;
      if (elementNode.HasNamespaces)
        this.InvalidatePrefixCache();
      XmlnsPrefix key;
      if (!this.prefixesForType.TryGetValue(typeId, out key))
      {
        foreach (KeyValuePair<XmlnsPrefix, IXmlNamespace> keyValuePair in elementNode.GetNamespacePrefixPairs())
        {
          IXmlNamespace xmlNamespace = keyValuePair.Value;
          if ((!xmlNamespace.Value.StartsWith("clr-namespace:", StringComparison.Ordinal) ? (xmlNamespace != XmlNamespace.DesignTimeXmlNamespace ? (ITypeId) this.TypeResolver.ProjectNamespaces.GetType(xmlNamespace, typeId.Name) : (ITypeId) this.TypeResolver.GetType(xmlNamespace, typeId.Name)) : this.GetType(xmlNamespace, typeId.Name)) == typeId)
          {
            if (key == null)
              key = keyValuePair.Key;
            if (this.IsDefaultNamespace(xmlNamespace))
            {
              key = keyValuePair.Key;
              break;
            }
          }
        }
        this.prefixesForType.Add(typeId, key);
      }
      return key;
    }

    internal ElementNode GetRootElement()
    {
      ElementNode elementNode1 = this.container as ElementNode;
      if (elementNode1 != null)
      {
        ElementNode elementNode2;
        while ((elementNode2 = elementNode1.Parent as ElementNode) != null)
          elementNode1 = elementNode2;
      }
      return elementNode1;
    }

    internal ITypeId GetType(IXmlNamespace clrNamespace, string typeName)
    {
      return (ITypeId) this.clrNamespaceUriParseCache.GetType(clrNamespace, typeName);
    }

    public XmlNamespace CreateClrNamespaceUri(IAssembly assembly, string clrNamespace)
    {
      return this.clrNamespaceUriSerializeCache.CreateClrNamespaceUri(assembly, clrNamespace);
    }

    public static bool IsXmlElement(DocumentNode node)
    {
      if (!(node.SourceContext is XmlElementReference))
        return node.ContainerSourceContext is XmlElementReference;
      return true;
    }

    private sealed class DisposableTargetTypeContext : IDisposable
    {
      private XamlSerializerContext serializerContext;
      private IType targetType;

      public DisposableTargetTypeContext(XamlSerializerContext serializerContext, IType targetType)
      {
        this.serializerContext = serializerContext;
        this.targetType = targetType;
      }

      public void Dispose()
      {
        this.serializerContext.targetType = this.targetType;
      }
    }

    private sealed class DisposableStyleOrTemplateContext : IDisposable
    {
      private XamlSerializerContext serializerContext;
      private ITypeId styleOrTemplateType;

      public DisposableStyleOrTemplateContext(XamlSerializerContext serializerContext, ITypeId styleOrTemplateType)
      {
        this.serializerContext = serializerContext;
        this.styleOrTemplateType = styleOrTemplateType;
      }

      public void Dispose()
      {
        this.serializerContext.styleOrTemplateType = this.styleOrTemplateType;
      }
    }

    private sealed class DisposableNodeContext : IDisposable
    {
      private XamlSerializerContext serializerContext;
      private ContainerNode container;

      public DisposableNodeContext(XamlSerializerContext serializerContext, ContainerNode container)
      {
        this.serializerContext = serializerContext;
        this.container = container;
      }

      public void Dispose()
      {
        ElementNode elementNode = this.container as ElementNode;
        if (elementNode != null && elementNode.HasNamespaces)
          this.serializerContext.InvalidatePrefixCache();
        this.serializerContext.container = this.container;
      }
    }

    private sealed class ClrNamespaceUriSerializeCache
    {
      private IDocumentContext documentContext;
      private IAssembly targetAssembly;
      private Dictionary<string, Dictionary<string, XmlNamespace>> assemblies;

      public ClrNamespaceUriSerializeCache(IDocumentContext documentContext, IAssembly targetAssembly)
      {
        this.documentContext = documentContext;
        this.targetAssembly = targetAssembly;
        this.assemblies = new Dictionary<string, Dictionary<string, XmlNamespace>>();
      }

      public XmlNamespace CreateClrNamespaceUri(IAssembly assembly, string clrNamespace)
      {
        string name = assembly.Name;
        Dictionary<string, XmlNamespace> dictionary;
        XmlNamespace uri;
        if (!this.assemblies.TryGetValue(name, out dictionary))
        {
          dictionary = new Dictionary<string, XmlNamespace>();
          this.assemblies.Add(name, dictionary);
        }
        else if (dictionary.TryGetValue(clrNamespace, out uri))
          return uri;
        uri = this.CreateUri(assembly, clrNamespace);
        dictionary.Add(clrNamespace, uri);
        return uri;
      }

      private XmlNamespace CreateUri(IAssembly assembly, string clrNamespace)
      {
        StringBuilder stringBuilder = new StringBuilder("clr-namespace:");
        stringBuilder.Append(clrNamespace);
        string assemblyName = this.GetAssemblyName(assembly);
        if (!string.IsNullOrEmpty(assemblyName))
        {
          stringBuilder.Append(';');
          stringBuilder.Append("assembly=");
          stringBuilder.Append(assemblyName);
        }
        return XmlNamespace.ToNamespace(stringBuilder.ToString(), XmlNamespace.GetNamespaceCanonicalization(this.documentContext.TypeResolver));
      }

      private string GetAssemblyName(IAssembly assembly)
      {
        if (assembly == null || string.IsNullOrEmpty(assembly.Name))
          return (string) null;
        if (!this.documentContext.IsLooseXaml && this.targetAssembly != null && this.targetAssembly.Equals((object) assembly))
          return (string) null;
        return assembly.Name;
      }
    }
  }
}
