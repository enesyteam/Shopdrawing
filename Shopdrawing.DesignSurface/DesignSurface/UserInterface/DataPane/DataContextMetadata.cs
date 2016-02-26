// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextMetadata
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Windows.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public static class DataContextMetadata
  {
    public static DataContextProperty GetDataContextProperty(DocumentCompositeNode documentNode, IProperty property)
    {
      DataContextProperty dataContextProperty = DataContextMetadata.GetDataContextPropertyInternal(documentNode, property);
      if (dataContextProperty != null && dataContextProperty.IsValid && documentNode.Properties[(IPropertyId) property] != null)
      {
        DualDataContextAttribute contextAttribute = DataContextMetadata.GetDataContextAttribute<DualDataContextAttribute>(property);
        if (contextAttribute != null && contextAttribute.UseDefaultDataContextWhenValueSet)
          dataContextProperty = (DataContextProperty) null;
      }
      return dataContextProperty;
    }

    public static DataContextPropertyPathExtension GetDataContextPropertyPathExtension(DocumentCompositeNode documentNode, IProperty property)
    {
      DataContextPropertyPathExtension extensionInternal = DataContextMetadata.GetDataContextPropertyPathExtensionInternal(documentNode, property);
      if (extensionInternal != null && !extensionInternal.IsValid)
        DataContextMetadata.GetDataContextAttribute<DataContextPathExtensionAttribute>(property);
      return extensionInternal;
    }

    private static DataContextPropertyPathExtension GetDataContextPropertyPathExtensionInternal(DocumentCompositeNode documentNode, IProperty property)
    {
      DataContextPathExtensionAttribute contextAttribute = DataContextMetadata.GetDataContextAttribute<DataContextPathExtensionAttribute>(property);
      if (contextAttribute == null)
        return (DataContextPropertyPathExtension) null;
      IProperty property1 = DataContextMetadata.GetProperty((DocumentNode) documentNode, contextAttribute.Property);
      if (property1 == null || property1 == property)
        return DataContextPropertyPathExtension.Invalid;
      return new DataContextPropertyPathExtension(property1, contextAttribute.IsCollectionItem);
    }

    private static DataContextProperty GetDataContextPropertyInternal(DocumentCompositeNode documentNode, IProperty property)
    {
      DataContextValueSourceAttribute contextAttribute = DataContextMetadata.GetDataContextAttribute<DataContextValueSourceAttribute>(property);
      if (contextAttribute == null)
        return (DataContextProperty) null;
      if (string.IsNullOrEmpty(contextAttribute.DataContextValueSourceProperty))
        return DataContextProperty.Invalid;
      PropertyReference ancestorPath = (PropertyReference) null;
      DocumentCompositeNode sourceNode;
      if (!string.IsNullOrEmpty(contextAttribute.AncestorPath))
      {
        DataContextMetadata.AncestorPropertyPath ancestorPropertyPath = DataContextMetadata.AncestorPropertyPathParser.Parse(documentNode, contextAttribute.AncestorPath);
        if (ancestorPropertyPath == null)
          return DataContextProperty.Invalid;
        sourceNode = ancestorPropertyPath.AncestorNode;
        ancestorPath = ancestorPropertyPath.PropertyPath;
      }
      else
        sourceNode = documentNode;
      if (sourceNode == null)
        return DataContextProperty.Invalid;
      IProperty property1 = DataContextMetadata.GetProperty((DocumentNode) sourceNode, contextAttribute.DataContextValueSourceProperty);
      if (property1 == null || property1 == property)
        return DataContextProperty.Invalid;
      return new DataContextProperty(sourceNode, property1, contextAttribute.IsCollectionItem, ancestorPath);
    }

    public static bool HasDataContextAttributes(IProperty property)
    {
      ReferenceStep referenceStep = property as ReferenceStep;
      if (referenceStep == null)
        return false;
      AttributeCollection attributes = referenceStep.Attributes;
      return attributes != null && attributes.Count != 0 && (attributes[typeof (DataContextValueSourceAttribute)] != null || attributes[typeof (DataContextPathExtensionAttribute)] != null);
    }

    public static T GetDataContextAttribute<T>(IProperty property) where T : Attribute
    {
      ReferenceStep referenceStep = property as ReferenceStep;
      if (referenceStep == null)
        return default (T);
      AttributeCollection attributes = referenceStep.Attributes;
      if (attributes == null)
        return default (T);
      return (T) attributes[typeof (T)];
    }

    private static IProperty GetProperty(DocumentNode documentNode, string propertyName)
    {
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(documentNode.TypeResolver, documentNode.Type);
      return documentNode.Type.GetMember(MemberType.Property, propertyName, allowableMemberAccess) as IProperty;
    }

    private class AncestorPropertyPathParser
    {
      private string ancestorPath;
      private DocumentCompositeNode ancestorNode;
      private int parsePosition;

      private AncestorPropertyPathParser(DocumentCompositeNode documentNode, string ancestorPath)
      {
        this.ancestorPath = ancestorPath;
        this.ancestorNode = documentNode;
      }

      public static DataContextMetadata.AncestorPropertyPath Parse(DocumentCompositeNode documentNode, string ancestorPath)
      {
        return new DataContextMetadata.AncestorPropertyPathParser(documentNode, ancestorPath).ParseInternal();
      }

      private DataContextMetadata.AncestorPropertyPath ParseInternal()
      {
        List<string> list = new List<string>();
        while (this.parsePosition < this.ancestorPath.Length)
        {
          string str = this.ParseNextStep();
          if (str == null)
            return (DataContextMetadata.AncestorPropertyPath) null;
          list.Add(str);
        }
        List<ReferenceStep> steps = new List<ReferenceStep>(list.Count);
        for (int index = list.Count - 1; index >= 0; --index)
        {
          this.ancestorNode = this.ancestorNode.Parent;
          if (this.ancestorNode == null)
            return (DataContextMetadata.AncestorPropertyPath) null;
          ReferenceStep property = this.GetProperty(list[index]);
          if (property == null)
            return (DataContextMetadata.AncestorPropertyPath) null;
          steps.Add(property);
        }
        steps.Reverse();
        return new DataContextMetadata.AncestorPropertyPath(this.ancestorNode, new PropertyReference(steps));
      }

      private string ParseNextStep()
      {
        string str;
        if ((int) this.ancestorPath[this.parsePosition] == 92)
        {
          str = "\\";
          ++this.parsePosition;
        }
        else
        {
          int index = this.ancestorPath.IndexOfAny(".\\".ToCharArray(), this.parsePosition + 1);
          if (index < 0)
          {
            str = this.ancestorPath.Substring(this.parsePosition);
            this.parsePosition = this.ancestorPath.Length;
          }
          else
          {
            str = this.ancestorPath.Substring(this.parsePosition, index - this.parsePosition);
            char ch = this.ancestorPath[index];
            this.parsePosition = index + ((int) ch == 46 ? true : false);
          }
        }
        return str;
      }

      private ReferenceStep GetProperty(string propertyName)
      {
        if (string.IsNullOrEmpty(propertyName))
          return (ReferenceStep) null;
        if (propertyName == "\\")
          return (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep(this.ancestorNode.TypeResolver, this.ancestorNode.TargetType, 0, false);
        MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(this.ancestorNode.TypeResolver, this.ancestorNode.Type);
        return this.ancestorNode.Type.GetMember(MemberType.Property, propertyName, allowableMemberAccess) as ReferenceStep;
      }
    }

    private class AncestorPropertyPath
    {
      public DocumentCompositeNode AncestorNode { get; private set; }

      public PropertyReference PropertyPath { get; private set; }

      public AncestorPropertyPath(DocumentCompositeNode ancestorNode, PropertyReference propertyPath)
      {
        this.AncestorNode = ancestorNode;
        this.PropertyPath = propertyPath;
      }
    }
  }
}
