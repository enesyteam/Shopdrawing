// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlSchema
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class XmlSchema : ISchema, INotifyPropertyChanged
  {
    private DataSourceNode dataSource;
    private DataSchemaNode root;
    private bool invalidRootXPath;
    private XmlNamespaceMappingCollection namespaceManager;
    private Dictionary<string, string> prefixToNamespace;
    private Dictionary<string, string> namespaceToPrefix;
    private string targetNamespace;

    public XmlNamespaceMappingCollection NamespaceManager
    {
      get
      {
        return this.namespaceManager;
      }
    }

    public DataSchemaNode Root
    {
      get
      {
        return this.root;
      }
    }

    public DataSchemaNode AbsoluteRoot
    {
      get
      {
        DataSchemaNode dataSchemaNode1 = this.root;
        for (DataSchemaNode dataSchemaNode2 = this.root; dataSchemaNode2 != null; dataSchemaNode2 = dataSchemaNode2.Parent)
          dataSchemaNode1 = dataSchemaNode2;
        return dataSchemaNode1;
      }
    }

    public string PathDescription
    {
      get
      {
        return StringTable.UseCustomXPathDescription;
      }
    }

    public DataSourceNode DataSource
    {
      get
      {
        return this.dataSource;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public XmlSchema(XmlSchemaSet schemaSet, DocumentNode dataSource)
    {
      this.Initialize(schemaSet);
      this.dataSource = (DataSourceNode) new XmlDataSourceNode(this, dataSource);
      this.ValidateRootXPath();
    }

    public XmlSchema(DocumentNode dataSource)
    {
      this.dataSource = (DataSourceNode) new XmlDataSourceNode(this, dataSource);
      this.ValidateRootXPath();
    }

    private XmlSchema(XmlSchema source, DataSchemaNode root)
    {
      this.root = root;
      this.dataSource = (DataSourceNode) new XmlDataSourceNode(this, source.dataSource.DocumentNode);
      this.invalidRootXPath = source.invalidRootXPath;
      this.namespaceManager = source.namespaceManager;
      this.prefixToNamespace = source.prefixToNamespace;
      this.namespaceToPrefix = source.namespaceToPrefix;
      this.targetNamespace = source.targetNamespace;
    }

    public DataSchemaNode GetEffectiveCollectionNode(DataSchemaNode endNode)
    {
      if (endNode.IsCollection)
        return endNode;
      if (endNode == this.Root)
        return (DataSchemaNode) null;
      for (DataSchemaNode parent = endNode.Parent; parent != this.Root; parent = parent.Parent)
      {
        if (parent == null)
          return (DataSchemaNode) null;
        if (parent.IsCollection)
          return parent;
      }
      return (DataSchemaNode) null;
    }

    public int GetCollectionDepth(DataSchemaNode endNode)
    {
      if (this.root == null)
        return 0;
      int num = endNode.IsCollection ? true : false;
      if (endNode == this.root)
        return num;
      for (DataSchemaNode parent = endNode.Parent; parent != this.root; parent = parent.Parent)
      {
        if (parent == null)
          return -1;
        if (parent.IsCollection)
          ++num;
      }
      return num;
    }

    public ISchema MakeRelativeToNode(DataSchemaNode node)
    {
      XmlSchema xmlSchema;
      if (node == this.Root)
        xmlSchema = this;
      else if (node != null)
      {
        xmlSchema = new XmlSchema(this, node);
      }
      else
      {
        DataSchemaNode absoluteRoot = this.AbsoluteRoot;
        xmlSchema = this.Root != absoluteRoot ? new XmlSchema(this, absoluteRoot) : this;
      }
      return (ISchema) xmlSchema;
    }

    public DataSchemaNodePath CreateNodePath()
    {
      return new DataSchemaNodePath((ISchema) this, this.root);
    }

    public DataSchemaNodePath GetNodePathFromPath(string path)
    {
      DataSchemaNode nodeFromPath = this.GetNodeFromPath(path);
      if (nodeFromPath != null)
        return new DataSchemaNodePath((ISchema) this, nodeFromPath);
      return (DataSchemaNodePath) null;
    }

    private DataSchemaNode GetNodeFromPath(string path)
    {
      if (this.root == null)
        return (DataSchemaNode) null;
      bool flag = !string.IsNullOrEmpty(path) && (int) path[0] == 47;
      if (this.invalidRootXPath && !flag)
        return (DataSchemaNode) null;
      if (string.IsNullOrEmpty(path))
        return this.root;
      DataSchemaNode dataSchemaNode1 = flag ? this.AbsoluteRoot : this.root;
      string[] strArray = path.Split('/');
      int num = 0;
      if (flag)
      {
        if (string.IsNullOrEmpty(strArray[1]) && strArray.Length == 2)
          return dataSchemaNode1;
        if (dataSchemaNode1.PathName != strArray[1])
          return (DataSchemaNode) null;
        num = 2;
      }
      DataSchemaNode dataSchemaNode2 = dataSchemaNode1;
      for (int index = num; index < strArray.Length && dataSchemaNode2 != null; ++index)
      {
        string pathName = strArray[index];
        if (pathName == "..")
          dataSchemaNode2 = dataSchemaNode2.Parent;
        else if (pathName != ".")
          dataSchemaNode2 = dataSchemaNode2.FindChildByPathName(pathName);
      }
      return dataSchemaNode2;
    }

    public string GetPath(DataSchemaNodePath nodePath)
    {
      return this.GetPathInternal(this.root, nodePath.Node);
    }

    private string GetPathInternal(DataSchemaNode rootNode, DataSchemaNode node)
    {
      if (this.root == null)
        return (string) null;
      string localXPath = string.Empty;
      for (; node != rootNode; node = node.Parent)
        localXPath = localXPath.Length != 0 ? node.PathName + "/" + localXPath : node.PathName;
      if (rootNode.Parent == null)
        localXPath = XmlSchema.CombineXPaths("/" + rootNode.PathName, localXPath);
      return localXPath;
    }

    public static string CombineXPaths(string inheritedXPath, string localXPath)
    {
      if (string.IsNullOrEmpty(localXPath))
        return inheritedXPath;
      if ((int) localXPath[0] == 47 || string.IsNullOrEmpty(inheritedXPath))
        return localXPath;
      return !(inheritedXPath == "/") ? inheritedXPath + "/" + localXPath : inheritedXPath + localXPath;
    }

    public static string NormalizeXPath(string xPath)
    {
      if (string.IsNullOrEmpty(xPath) || xPath.IndexOf('.') < 0)
        return xPath;
      string[] strArray = xPath.Split('/');
      int index1 = 0;
      for (int index2 = 0; index2 < strArray.Length; ++index2)
      {
        string str = strArray[index2];
        if (str == "..")
        {
          --index1;
          if (index1 < 0)
            return string.Empty;
        }
        else if (str != ".")
        {
          if (index2 != index1)
            strArray[index1] = strArray[index2];
          ++index1;
        }
      }
      StringBuilder stringBuilder = new StringBuilder(xPath.Length);
      for (int index2 = 0; index2 < index1; ++index2)
      {
        if (index2 > 0)
          stringBuilder.Append('/');
        stringBuilder.Append(strArray[index2]);
      }
      return stringBuilder.ToString();
    }

    private void ValidateRootXPath()
    {
      DocumentNode node = ((DocumentCompositeNode) this.DataSource.DocumentNode).Properties[XmlDataProviderSceneNode.XPathProperty];
      if (node == null)
        return;
      string valueAsString = DocumentPrimitiveNode.GetValueAsString(node);
      if (string.IsNullOrEmpty(valueAsString))
        return;
      DataSchemaNode dataSchemaNode = (DataSchemaNode) null;
      if ((int) valueAsString[0] == 47)
        dataSchemaNode = this.GetNodeFromPath(valueAsString);
      if (dataSchemaNode != null)
        return;
      this.invalidRootXPath = true;
    }

    private void Initialize(XmlSchemaSet schemaSet)
    {
      IEnumerator enumerator1 = schemaSet.Schemas().GetEnumerator();
      System.Xml.Schema.XmlSchema xmlSchema = (System.Xml.Schema.XmlSchema) null;
      while (enumerator1.MoveNext() && xmlSchema == null)
      {
        xmlSchema = (System.Xml.Schema.XmlSchema) enumerator1.Current;
        if (xmlSchema.Elements.Count == 0)
          xmlSchema = (System.Xml.Schema.XmlSchema) null;
      }
      if (xmlSchema == null)
        return;
      IEnumerator enumerator2 = (IEnumerator) xmlSchema.Elements.GetEnumerator();
      enumerator2.MoveNext();
      XmlSchemaElement element = (XmlSchemaElement) ((DictionaryEntry) enumerator2.Current).Value;
      this.prefixToNamespace = new Dictionary<string, string>();
      this.namespaceToPrefix = new Dictionary<string, string>();
      this.targetNamespace = xmlSchema.TargetNamespace;
      foreach (XmlQualifiedName xmlQualifiedName in xmlSchema.Namespaces.ToArray())
      {
        if (!xmlQualifiedName.IsEmpty)
          this.namespaceToPrefix[xmlQualifiedName.Namespace] = xmlQualifiedName.Name;
      }
      if (!string.IsNullOrEmpty(xmlSchema.TargetNamespace))
      {
        string str1 = this.GetNicePrefix(xmlSchema.TargetNamespace);
        int num = 0;
        bool flag;
        do
        {
          string str2 = str1;
          if (num != 0)
            str2 += (string) (object) num;
          flag = true;
          foreach (KeyValuePair<string, string> keyValuePair in this.namespaceToPrefix)
          {
            if (keyValuePair.Value == str2)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            str1 = str2;
          ++num;
        }
        while (!flag);
        this.namespaceToPrefix[xmlSchema.TargetNamespace] = str1;
      }
      this.root = this.MakeSchemaElement(element);
      this.namespaceManager = new XmlNamespaceMappingCollection();
      foreach (KeyValuePair<string, string> keyValuePair in this.prefixToNamespace)
      {
        Uri result;
        if (Uri.TryCreate(keyValuePair.Value, UriKind.RelativeOrAbsolute, out result))
          this.namespaceManager.Add(new XmlNamespaceMapping(keyValuePair.Key, result));
      }
    }

    private string GetNicePrefix(string elementNamespace)
    {
      int index = elementNamespace.Length - 1;
      while (index >= 0 && char.IsLetterOrDigit(elementNamespace[index]))
        --index;
      if (index != elementNamespace.Length - 1)
        return elementNamespace.Substring(index + 1);
      return "a";
    }

    private DataSchemaNode MakeSchemaAttribute(XmlSchemaAttribute attribute)
    {
      string typeName = (string) null;
      Type type = (Type) null;
      if (attribute.AttributeSchemaType != null && attribute.AttributeSchemaType.Datatype != null)
        type = attribute.AttributeSchemaType.Datatype.ValueType;
      else if (attribute.SchemaTypeName != (XmlQualifiedName) null && attribute.SchemaTypeName.Name != string.Empty)
        typeName = attribute.SchemaTypeName.Name;
      if (type != (Type) null)
      {
        type = this.ConvertType(type);
        typeName = type.Name;
      }
      string str = this.ProcessQualifiedName(attribute.QualifiedName);
      return new DataSchemaNode("@ " + str, "@" + str, SchemaNodeTypes.Property, typeName, type, (IDataSchemaNodeDelayLoader) null);
    }

    private string ProcessQualifiedName(XmlQualifiedName name)
    {
      string str1 = string.Empty;
      string index = string.Empty;
      string key = string.Empty;
      if (!name.IsEmpty)
        key = name.Namespace;
      else if (!string.IsNullOrEmpty(this.targetNamespace))
        key = this.targetNamespace;
      string str2;
      if (!string.IsNullOrEmpty(key) && this.namespaceToPrefix.TryGetValue(key, out index))
      {
        str2 = index + ":" + name.Name;
        this.prefixToNamespace[index] = key;
      }
      else
        str2 = name.Name;
      return str2;
    }

    private Type ConvertType(Type type)
    {
      if (typeof (Decimal).IsAssignableFrom(type))
        return typeof (double);
      if (typeof (byte).IsAssignableFrom(type))
        return typeof (int);
      return type;
    }

    private DataSchemaNode MakeSchemaElement(XmlSchemaElement element)
    {
      string typeName = (string) null;
      Type type = (Type) null;
      if (element.ElementSchemaType != null && element.ElementSchemaType.Datatype != null)
        type = element.ElementSchemaType.Datatype.ValueType;
      else if (element.SchemaTypeName != (XmlQualifiedName) null && element.SchemaTypeName.Name != string.Empty)
        typeName = element.SchemaTypeName.Name;
      if (type != (Type) null)
      {
        type = this.ConvertType(type);
        typeName = type.Name;
      }
      string str = this.ProcessQualifiedName(element.QualifiedName);
      SchemaNodeTypes nodeType = element.MaxOccurs > new Decimal(1) ? SchemaNodeTypes.Collection : SchemaNodeTypes.Property;
      DataSchemaNode schemaNode = new DataSchemaNode(str, str, nodeType, typeName, type, (IDataSchemaNodeDelayLoader) null);
      XmlSchemaComplexType schemaComplexType = element.ElementSchemaType as XmlSchemaComplexType;
      if (schemaComplexType != null)
      {
        XmlSchemaObjectCollection objectCollection = (XmlSchemaObjectCollection) null;
        if (schemaComplexType.Attributes.Count > 0)
        {
          objectCollection = schemaComplexType.Attributes;
        }
        else
        {
          XmlSchemaSimpleContent schemaSimpleContent = schemaComplexType.ContentModel as XmlSchemaSimpleContent;
          if (schemaSimpleContent != null)
          {
            XmlSchemaSimpleContentExtension contentExtension = schemaSimpleContent.Content as XmlSchemaSimpleContentExtension;
            if (contentExtension != null)
              objectCollection = contentExtension.Attributes;
          }
        }
        if (objectCollection != null)
        {
          foreach (XmlSchemaAttribute attribute in objectCollection)
          {
            DataSchemaNode child = this.MakeSchemaAttribute(attribute);
            schemaNode.AddChild(child);
          }
        }
        XmlSchemaGroupBase xmlSchemaGroupBase = schemaComplexType.Particle as XmlSchemaGroupBase;
        if (xmlSchemaGroupBase != null)
          this.ProcessSchemaItems(schemaNode, xmlSchemaGroupBase.Items);
      }
      return schemaNode;
    }

    private void ProcessSchemaItems(DataSchemaNode schemaNode, XmlSchemaObjectCollection items)
    {
      foreach (XmlSchemaObject xmlSchemaObject in items)
      {
        XmlSchemaElement element;
        if ((element = xmlSchemaObject as XmlSchemaElement) != null)
        {
          DataSchemaNode child = this.MakeSchemaElement(element);
          schemaNode.AddChild(child);
        }
        else
        {
          XmlSchemaChoice xmlSchemaChoice;
          if ((xmlSchemaChoice = xmlSchemaObject as XmlSchemaChoice) != null)
            this.ProcessSchemaItems(schemaNode, xmlSchemaChoice.Items);
        }
      }
    }

    public override string ToString()
    {
      string name = this.DataSource.Name;
      string pathInternal = this.GetPathInternal(this.AbsoluteRoot, this.root);
      if (string.IsNullOrEmpty(name))
        return pathInternal;
      if (string.IsNullOrEmpty(pathInternal))
        return name;
      return name + ": " + pathInternal;
    }
  }
}
