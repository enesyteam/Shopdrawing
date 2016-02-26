// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSchemaNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSchemaNode : INotifyPropertyChanged
  {
    public static readonly string IndexNodePath = "[0]";
    private string pathName;
    private string displayName;
    private bool isReadOnly;
    private DataSchemaNode parent;
    private DataSchemaNode collectionItem;
    private List<DataSchemaNode> children;
    private Type type;
    private IDataSchemaNodeDelayLoader loader;
    private string typeName;
    private string rawDisplayName;

    public SchemaNodeTypes NodeType { get; private set; }

    public string DisplayName
    {
      get
      {
        if (this.displayName == null)
          this.displayName = this.FormatDisplayName(this.rawDisplayName, this.typeName);
        return this.displayName;
      }
    }

    public string PathName
    {
      get
      {
        return this.pathName;
      }
    }

    public Type Type
    {
      get
      {
        return this.type;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return this.isReadOnly;
      }
      set
      {
        this.isReadOnly = value;
        this.OnPropertyChanged("IsReadOnly");
      }
    }

    public bool IsCollection
    {
      get
      {
        return this.NodeType == SchemaNodeTypes.Collection;
      }
    }

    public bool IsCollectionItem
    {
      get
      {
        return this.NodeType == SchemaNodeTypes.CollectionItem;
      }
    }

    public bool IsProperty
    {
      get
      {
        return this.NodeType == SchemaNodeTypes.Property;
      }
    }

    public bool IsMethod
    {
      get
      {
        return this.NodeType == SchemaNodeTypes.Method;
      }
    }

    public DataSchemaNode Parent
    {
      get
      {
        return this.parent;
      }
    }

    public List<DataSchemaNode> Children
    {
      get
      {
        this.EnsureChildren();
        return this.children;
      }
    }

    public DataSchemaNode CollectionItem
    {
      get
      {
        this.EnsureChildren();
        return this.collectionItem;
      }
      set
      {
        if (this.collectionItem != null)
          this.collectionItem.parent = (DataSchemaNode) null;
        this.collectionItem = value;
        if (this.collectionItem == null)
          return;
        this.collectionItem.parent = this;
      }
    }

    public bool HasChildren
    {
      get
      {
        return this.Children.Count != 0;
      }
    }

    public SampleType SampleType
    {
      get
      {
        SampleNonBasicType sampleNonBasicType = SampleDataSet.SampleDataTypeFromType(this.Type);
        if (sampleNonBasicType != null)
          return (SampleType) sampleNonBasicType;
        SampleCompositeType effectiveParentType = this.EffectiveParentType;
        if (effectiveParentType == null)
          return (SampleType) null;
        SampleProperty sampleProperty = effectiveParentType.GetSampleProperty(this.PathName);
        if (sampleProperty != null)
          return sampleProperty.PropertySampleType;
        return (SampleType) null;
      }
    }

    public SampleCompositeType EffectiveParentType
    {
      get
      {
        if (this.Parent == null)
          return (SampleCompositeType) null;
        return DataSchemaNode.CalculateEffectiveType(this.Parent.Type);
      }
    }

    public SampleCompositeType EffectiveType
    {
      get
      {
        return DataSchemaNode.CalculateEffectiveType(this.Type);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public DataSchemaNode(string displayName, string pathName, SchemaNodeTypes nodeType, string typeName, Type type, IDataSchemaNodeDelayLoader loader)
    {
      this.pathName = pathName;
      this.NodeType = nodeType;
      this.type = type;
      this.loader = loader;
      this.rawDisplayName = displayName;
      this.typeName = typeName;
    }

    public bool IsHierarchicalCollection(ITypeResolver typeResolver)
    {
      if (!this.IsCollection || this.type == (Type) null)
        return false;
      IType collectionType = typeResolver.GetType(this.type);
      IType typeId = collectionType != null ? collectionType.ItemType : (IType) null;
      if (typeId == null)
        return false;
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, typeId);
      return Enumerable.FirstOrDefault<IProperty>(typeId.GetProperties(allowableMemberAccess), (Func<IProperty, bool>) (prop => prop.PropertyType == collectionType)) != null;
    }

    public IType ResolveType(ITypeResolver typeResolver)
    {
      IType type = (IType) null;
      if (this.type != (Type) null)
        type = typeResolver.GetType(this.type);
      else if (this.IsCollection)
        type = typeResolver.ResolveType(PlatformTypes.Array);
      else if (!this.HasChildren)
        type = typeResolver.ResolveType(PlatformTypes.String);
      return DesignDataHelper.GetSourceType(type, typeResolver);
    }

    private static SampleCompositeType CalculateEffectiveType(Type type)
    {
      SampleNonBasicType sampleNonBasicType = SampleDataSet.SampleDataTypeFromType(type);
      if (sampleNonBasicType != null)
      {
        SampleCollectionType sampleCollectionType;
        while ((sampleCollectionType = sampleNonBasicType as SampleCollectionType) != null)
          sampleNonBasicType = sampleCollectionType.ItemType as SampleNonBasicType;
      }
      return sampleNonBasicType as SampleCompositeType;
    }

    public void AddChild(DataSchemaNode child)
    {
      this.Children.Add(child);
      child.parent = this;
      this.OnPropertyChanged("Children");
      this.OnPropertyChanged("HasChildren");
    }

    public void RemoveChild(DataSchemaNode child)
    {
      this.Children.Remove(child);
      child.parent = (DataSchemaNode) null;
      this.OnPropertyChanged("Children");
      this.OnPropertyChanged("HasChildren");
    }

    public bool IsAncestorOf(DataSchemaNode descendant)
    {
      for (; descendant != null; descendant = descendant.Parent)
      {
        if (this == descendant)
          return true;
      }
      return false;
    }

    public DataSchemaNode FindChildByPathName(string pathName)
    {
      DataSchemaNode dataSchemaNode1 = (DataSchemaNode) null;
      if (pathName != DataSchemaNode.IndexNodePath)
      {
        foreach (DataSchemaNode dataSchemaNode2 in this.Children)
        {
          if (dataSchemaNode2.PathName == pathName)
          {
            dataSchemaNode1 = dataSchemaNode2;
            break;
          }
        }
      }
      else
        dataSchemaNode1 = this.CollectionItem;
      return dataSchemaNode1;
    }

    public override string ToString()
    {
      return this.DisplayName;
    }

    private void EnsureChildren()
    {
      if (this.children != null)
        return;
      this.children = new List<DataSchemaNode>();
      if (this.loader == null)
        return;
      this.loader.ProcessChildren(this);
    }

    private string FormatDisplayName(string displayName, string typeName)
    {
      if (this.IsMethod)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DatabindingSchemaMethod, new object[1]
        {
          (object) displayName
        });
      if (typeName != null)
      {
        Type type = this.Type;
        if (this.IsCollection && this.CollectionItem != null)
          type = this.CollectionItem.Type;
        if (type != (Type) null)
          typeName = this.FormatTypeName(type);
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DatabindingSchemaPropertyFormatWithType, new object[2]
        {
          (object) displayName,
          (object) typeName
        });
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DatabindingSchemaPropertyFormatWithoutType, new object[1]
      {
        (object) displayName
      });
    }

    private string FormatTypeName(Type type)
    {
      string str1 = type.Name;
      int length = str1.IndexOf('`');
      if (length != -1)
      {
        string str2 = str1.Substring(0, length);
        Type[] genericArguments = type.GetGenericArguments();
        string str3 = str2 + (object) '<' + this.FormatTypeName(genericArguments[0]);
        for (int index = 1; index < genericArguments.Length; ++index)
          str3 = str3 + (object) ',' + this.FormatTypeName(genericArguments[index]);
        str1 = str3 + (object) '>';
      }
      return str1;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public class PathNameComparer : System.Collections.Generic.Comparer<DataSchemaNode>
    {
      public override int Compare(DataSchemaNode x, DataSchemaNode y)
      {
        return x.PathName.CompareTo(y.PathName);
      }
    }
  }
}
