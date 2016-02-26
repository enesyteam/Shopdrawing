// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DesignDataGenerator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class DesignDataGenerator
  {
    private static Random random = new Random((int) DateTime.Now.Ticks);
    private static readonly int maxComplexDepth = 3;
    private static ReadOnlyCollection<int> DefaultCollectionCounts = new ReadOnlyCollection<int>((IList<int>) new List<int>()
    {
      10,
      5,
      0
    });
    private List<DesignDataGenerator.NodeType> stack = new List<DesignDataGenerator.NodeType>();
    private Dictionary<IType, bool> xamlFriendlyTypes = new Dictionary<IType, bool>();
    private IType rootType;
    private IDocumentContext documentContext;
    private int complexDepth;
    private int collectionDepth;
    private DesignDataGenerator.SimpleTypeHandlerFactory valueBuilderFactory;

    private bool IsCollectionRecent
    {
      get
      {
        if (this.collectionDepth > 0 && this.stack.Count > 2)
        {
          for (int index = this.stack.Count - 3; index < this.stack.Count; ++index)
          {
            if (this.stack[index] == DesignDataGenerator.NodeType.Collection)
              return true;
          }
        }
        return false;
      }
    }

    public DesignDataGenerator(IType rootType, IDocumentContext context)
    {
      this.rootType = rootType;
      this.documentContext = context;
      this.valueBuilderFactory = new DesignDataGenerator.SimpleTypeHandlerFactory((IMetadataResolver) context.TypeResolver);
    }

    public DocumentNode Build()
    {
      this.complexDepth = 0;
      this.collectionDepth = 0;
      return this.BuildNode(this.rootType, false, false);
    }

    public static bool UpdateDesignValues(IProjectItem designDataFile, IProperty property, ISampleTypeConfiguration valueGenerator)
    {
      designDataFile.OpenView(false);
      SceneDocument sceneDocument = designDataFile.Document as SceneDocument;
      if (sceneDocument == null || sceneDocument.DocumentRoot == null || sceneDocument.DocumentRoot.RootNode == null)
        return false;
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(StringTable.UpdateDesignValuesUndo))
      {
        DesignDataGenerator.UpdateDesignValues(sceneDocument.DocumentRoot.RootNode, property, valueGenerator);
        editTransaction.Commit();
      }
      return true;
    }

    public static bool IsPropertyWritable(IProperty property, ITypeResolver typeResolver)
    {
      if (property == null)
        return false;
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, property.DeclaringType);
      return DesignDataGenerator.IsPropertyWritable(property, allowableMemberAccess);
    }

    public static bool IsPropertyWritable(IProperty property, MemberAccessTypes allowaedAccess)
    {
      return property != null && (property.WriteAccess & (MemberAccessType) allowaedAccess) != MemberAccessType.None;
    }

    public static void UpdateDesignValues(DocumentNode node, IProperty property, ISampleTypeConfiguration valueGenerator)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode == null)
        return;
      if (property.DeclaringType.IsAssignableFrom((ITypeId) documentCompositeNode.Type))
        documentCompositeNode.Properties[(IPropertyId) property] = documentCompositeNode.Context.CreateNode(property.PropertyType.RuntimeType, valueGenerator.Value);
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode.Properties)
        DesignDataGenerator.UpdateDesignValues(keyValuePair.Value, property, valueGenerator);
      if (!documentCompositeNode.SupportsChildren)
        return;
      for (int index = 0; index < documentCompositeNode.Children.Count; ++index)
        DesignDataGenerator.UpdateDesignValues(documentCompositeNode.Children[index], property, valueGenerator);
    }

    private int GetCollectionLength(int depth)
    {
      int index = Math.Min(depth, DesignDataGenerator.DefaultCollectionCounts.Count) - 1;
      return DesignDataGenerator.DefaultCollectionCounts[index];
    }

    private DocumentCompositeNode BuildCompositeNode(IType type)
    {
      DocumentCompositeNode node = this.documentContext.CreateNode((ITypeId) type);
      ++this.complexDepth;
      bool recursionLimitReached = this.complexDepth > DesignDataGenerator.maxComplexDepth && !this.IsCollectionRecent;
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(this.documentContext.TypeResolver, type);
      SortedSet<string> sortedSet = new SortedSet<string>();
      foreach (IProperty property in ITypeExtensions.GetProperties(type, allowableMemberAccess, true))
      {
        if (!sortedSet.Contains(property.Name))
        {
          sortedSet.Add(property.Name);
          if (DesignDataGenerator.IsPropertyWritable(property, this.documentContext.TypeResolver))
          {
            DocumentNode documentNode = this.BuildNode(property.PropertyType, true, recursionLimitReached);
            if (documentNode != null)
              node.Properties[(IPropertyId) property] = documentNode;
          }
        }
      }
      if (type.ItemType != null && (this.IsTypeXamlFriendly(type.ItemType) || this.CanBuildSimpleNode(type.ItemType)))
      {
        this.stack.Add(DesignDataGenerator.NodeType.Collection);
        this.BuildCollectionNode(type, node);
        this.stack.RemoveAt(this.stack.Count - 1);
      }
      --this.complexDepth;
      return node;
    }

    private bool CanBuildSimpleNode(IType type)
    {
      return this.valueBuilderFactory.GetValueBuilder(type) != null;
    }

    private DocumentNode BuildSimpleNode(IType type)
    {
      DesignDataGenerator.ISimpleValueGenerator valueBuilder = this.valueBuilderFactory.GetValueBuilder(type);
      if (valueBuilder == null)
        return (DocumentNode) null;
      DesignDataGenerator.EnumValueGenerator enumValueGenerator = valueBuilder as DesignDataGenerator.EnumValueGenerator;
      if (enumValueGenerator != null)
        enumValueGenerator.SetEnumType(type);
      return valueBuilder.MakeNode(this.documentContext);
    }

    private DocumentNode BuildNode(IType type, bool isProperty, bool recursionLimitReached)
    {
      DocumentNode documentNode = this.BuildSimpleNode(type);
      if (documentNode == null && !recursionLimitReached)
      {
        if (this.IsTypeXamlFriendly(type))
          documentNode = (DocumentNode) this.BuildCompositeNode(type);
        else if (isProperty && type.ItemType != null && (this.IsTypeXamlFriendly(type.ItemType) || this.CanBuildSimpleNode(type.ItemType)))
        {
          this.stack.Add(DesignDataGenerator.NodeType.Collection);
          documentNode = (DocumentNode) this.BuildCollectionNode(type, (DocumentCompositeNode) null);
          this.stack.RemoveAt(this.stack.Count - 1);
        }
      }
      return documentNode;
    }

    private bool IsTypeXamlFriendly(IType type)
    {
      bool flag1;
      if (this.xamlFriendlyTypes.TryGetValue(type, out flag1))
        return flag1;
      bool flag2 = true;
      if (type.RuntimeType == typeof (object))
        flag2 = false;
      else if (type.IsAbstract)
        flag2 = false;
      else if (type.IsGenericType || type.Name.IndexOf('`') >= 0)
        flag2 = false;
      else if (!type.HasDefaultConstructor(false))
        flag2 = type.IsArray && type.PlatformMetadata.IsNullType((ITypeId) type.PlatformMetadata.ResolveType(PlatformTypes.ArrayExtension));
      this.xamlFriendlyTypes[type] = flag2;
      return flag2;
    }

    private DocumentCompositeNode BuildCollectionNode(IType type, DocumentCompositeNode node)
    {
      ++this.collectionDepth;
      int collectionLength = this.GetCollectionLength(this.collectionDepth);
      if (collectionLength > 0)
      {
        if (node == null)
          node = this.documentContext.CreateNode((ITypeId) type);
        if (node.Children != null)
        {
          IType itemType = type.ItemType;
          if (itemType != null)
          {
            for (int index = 0; index < collectionLength; ++index)
            {
              DocumentNode documentNode = this.BuildNode(itemType, false, false);
              if (documentNode != null)
                node.Children.Add(documentNode);
            }
          }
        }
      }
      --this.collectionDepth;
      return node;
    }

    private enum NodeType
    {
      Complex,
      Collection,
    }

    private class SimpleTypeHandlerFactory : TypeIdHandlerFactory<DesignDataGenerator.ISimpleValueGenerator>
    {
      private IMetadataResolver typeResolver;

      public SimpleTypeHandlerFactory(IMetadataResolver typeResolver)
      {
        this.typeResolver = typeResolver;
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.IntegerValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.DoubleValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.StringValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.BooleanValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.BrushValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.ColorValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.DateTimeValueGenerator());
        this.RegisterHandler((DesignDataGenerator.ISimpleValueGenerator) new DesignDataGenerator.EnumValueGenerator());
      }

      protected override ITypeId GetBaseType(DesignDataGenerator.ISimpleValueGenerator handler)
      {
        return handler.ValueType;
      }

      public DesignDataGenerator.ISimpleValueGenerator GetValueBuilder(IType type)
      {
        return this.GetHandler(this.typeResolver, type);
      }
    }

    private interface ISimpleValueGenerator
    {
      ITypeId ValueType { get; }

      DocumentNode MakeNode(IDocumentContext documentContext);
    }

    private class IntegerValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private SampleNumberConfiguration valueGenerator;

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.Int32;
        }
      }

      public IntegerValueGenerator()
      {
        this.valueGenerator = new SampleNumberConfiguration(string.Empty);
        this.valueGenerator.SetConfigurationValue(ConfigurationPlaceholder.NumberLength, (object) 2);
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        int num = (int) this.valueGenerator.Value;
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.Int32, (IDocumentNodeValue) new DocumentNodeStringValue(num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    private class DoubleValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private SampleNumberConfiguration valueGenerator;

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.Double;
        }
      }

      public DoubleValueGenerator()
      {
        this.valueGenerator = new SampleNumberConfiguration(string.Empty);
        this.valueGenerator.SetConfigurationValue(ConfigurationPlaceholder.NumberLength, (object) "5");
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        double num1;
        do
        {
          num1 = (double) (int) this.valueGenerator.Value;
        }
        while (num1 % 100.0 == 0.0);
        double num2 = num1 / 100.0;
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.Double, (IDocumentNodeValue) new DocumentNodeStringValue(num2.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    private class StringValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private SampleStringConfiguration valueGenerator;

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.String;
        }
      }

      public StringValueGenerator()
      {
        this.valueGenerator = new SampleStringConfiguration(SampleDataConfigurationOption.StringFormatRandomLatin.StringValue, string.Empty);
        this.valueGenerator.SetConfigurationValue(ConfigurationPlaceholder.RandomLatinWordLength, (object) 10);
        this.valueGenerator.SetConfigurationValue(ConfigurationPlaceholder.RandomLatinWordCount, (object) 5);
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.String, (IDocumentNodeValue) new DocumentNodeStringValue(this.valueGenerator.Value.ToString()));
      }
    }

    private class ColorValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private static readonly char[] hexDigits = new char[16]
      {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'A',
        'B',
        'C',
        'D',
        'E',
        'F'
      };

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.Color;
        }
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.Color, (IDocumentNodeValue) new DocumentNodeStringValue(this.GetRandomColorValue()));
      }

      public string GetRandomColorValue()
      {
        StringBuilder stringBuilder = new StringBuilder("#FF");
        for (int index = 0; index < 6; ++index)
          stringBuilder.Append(this.GetRandomHexDigit());
        return stringBuilder.ToString();
      }

      public char GetRandomHexDigit()
      {
        return DesignDataGenerator.ColorValueGenerator.hexDigits[DesignDataGenerator.random.Next(16)];
      }
    }

    private class EnumValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private IType enumType;

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.Enum;
        }
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        string[] names = Enum.GetNames(this.enumType.RuntimeType);
        string str = names[DesignDataGenerator.random.Next(names.Length)];
        return (DocumentNode) documentContext.CreateNode((ITypeId) this.enumType, (IDocumentNodeValue) new DocumentNodeStringValue(str));
      }

      public void SetEnumType(IType enumType)
      {
        this.enumType = enumType;
      }
    }

    private class BrushValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private DesignDataGenerator.ColorValueGenerator colorGenerator = new DesignDataGenerator.ColorValueGenerator();

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.Brush;
        }
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.SolidColorBrush, (IDocumentNodeValue) new DocumentNodeStringValue(this.colorGenerator.GetRandomColorValue()));
      }
    }

    private class DateTimeValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.DateTime;
        }
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.DateTime, (IDocumentNodeValue) new DocumentNodeStringValue(DateTime.Now.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    private class BooleanValueGenerator : DesignDataGenerator.ISimpleValueGenerator
    {
      private SampleBooleanConfiguration valueGenerator = new SampleBooleanConfiguration();

      public ITypeId ValueType
      {
        get
        {
          return PlatformTypes.Boolean;
        }
      }

      public DocumentNode MakeNode(IDocumentContext documentContext)
      {
        bool flag = (bool) this.valueGenerator.Value;
        return (DocumentNode) documentContext.CreateNode(PlatformTypes.Boolean, (IDocumentNodeValue) new DocumentNodeStringValue(flag.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }
  }
}
