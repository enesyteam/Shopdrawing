// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelProperty : ModelProperty
  {
    private SceneNodeModelItem parent;
    private IProperty property;

    public override Type AttachedOwnerType
    {
      get
      {
        return this.property.DeclaringType.RuntimeType;
      }
    }

    public override ModelItemCollection Collection
    {
      get
      {
        ITextFlowSceneNode textFlowSceneNode = this.parent.SceneNode as ITextFlowSceneNode;
        return (ModelItemCollection) new SceneNodeModelItemCollection(textFlowSceneNode == null || !textFlowSceneNode.TextChildProperty.Equals((object) this.property) ? this.parent.SceneNode.GetCollectionForProperty((IPropertyId) this.property) : textFlowSceneNode.CollectionForTextChildProperty, this.parent.SceneNode.ViewModel, this.parent.SceneNode, this.property);
      }
    }

    public override object ComputedValue
    {
      get
      {
        return this.parent.SceneNode.GetComputedValue((IPropertyId) this.property);
      }
      set
      {
        using (SceneEditTransaction editTransaction = this.CreateEditTransaction())
        {
          this.parent.SceneNode.SetValue((IPropertyId) this.property, value);
          editTransaction.Commit();
        }
      }
    }

    public override object DefaultValue
    {
      get
      {
        return this.property.GetDefaultValue(this.PropertyType);
      }
    }

    public override ModelItemDictionary Dictionary
    {
      get
      {
        return this.Value as ModelItemDictionary;
      }
    }

    public override bool IsAttached
    {
      get
      {
        return this.property.IsAttachable;
      }
    }

    public override bool IsBrowsable
    {
      get
      {
        ReferenceStep referenceStep = this.property as ReferenceStep;
        if (referenceStep == null)
          return false;
        return referenceStep.IsBrowsable;
      }
    }

    public override bool IsCollection
    {
      get
      {
        return typeof (ICollection).IsAssignableFrom(this.PropertyType);
      }
    }

    public override bool IsDictionary
    {
      get
      {
        return typeof (IDictionary).IsAssignableFrom(this.PropertyType);
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return this.property.WriteAccess != MemberAccessType.Public;
      }
    }

    public override bool IsSet
    {
      get
      {
        return this.parent.SceneNode.IsSet((IPropertyId) this.property) != PropertyState.Unset;
      }
    }

    public override string Name
    {
      get
      {
        return this.property.Name;
      }
    }

    public override ModelItem Parent
    {
      get
      {
        return (ModelItem) this.parent;
      }
    }

    public override Type PropertyType
    {
      get
      {
        return this.property.PropertyType.RuntimeType;
      }
    }

    public override ModelItem Value
    {
      get
      {
        SceneNode valueAsSceneNode = this.parent.SceneNode.GetLocalValueAsSceneNode((IPropertyId) this.property);
        if (valueAsSceneNode != null)
          return (ModelItem) valueAsSceneNode.ModelItem;
        return (ModelItem) null;
      }
    }

    public SceneNodeModelProperty(SceneNodeModelItem parent, IProperty property)
    {
      this.parent = parent;
      this.property = property;
    }

    public override IEnumerable<object> GetAttributes(Type attributeType)
    {
      DependencyPropertyReferenceStep step = this.property as DependencyPropertyReferenceStep;
      if (step != null)
      {
        foreach (System.Attribute attribute in step.Attributes)
        {
          if (attributeType.IsAssignableFrom(attribute.GetType()))
            yield return (object) attribute;
        }
      }
    }

    public override void ClearValue()
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction())
      {
        this.parent.SceneNode.ClearValue((IPropertyId) this.property);
        editTransaction.Commit();
      }
    }

    public override ModelItem SetValue(object value)
    {
      using (SceneEditTransaction editTransaction = this.CreateEditTransaction())
      {
        ModelItem modelItem = value as ModelItem;
        if (modelItem != null)
        {
          ISceneNodeModelItem sceneNodeModelItem = value as ISceneNodeModelItem;
          value = sceneNodeModelItem == null ? modelItem.GetCurrentValue() : (object) sceneNodeModelItem.SceneNode.DocumentNode;
        }
        string text = value as string;
        if (text != null)
        {
          bool flag = true;
          if (text.StartsWith("{", StringComparison.Ordinal))
          {
            if (text.StartsWith("{}", StringComparison.Ordinal))
            {
              text = text.Substring(2);
            }
            else
            {
              XamlDocument document = (XamlDocument) this.parent.SceneNode.ViewModel.XamlDocument;
              DocumentNode parentNode = this.parent.SceneNode.DocumentNode;
              if (parentNode.DocumentRoot == null)
                parentNode = document.RootNode;
              IList<XamlParseError> errors;
              DocumentNode expressionFromString = XamlExpressionSerializer.GetExpressionFromString(text, document, parentNode, this.PropertyType, out errors);
              if (errors == null || errors.Count == 0)
              {
                flag = false;
                value = (object) expressionFromString;
              }
            }
          }
          if (flag)
            value = (object) this.parent.SceneNode.DocumentContext.CreateNode((ITypeId) this.property.PropertyType, (IDocumentNodeValue) new DocumentNodeStringValue(text));
        }
        if (value != null && !(value is DocumentNode) && this.parent.SceneNode.ProjectContext.GetType(value.GetType()) == null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.TypeIsNotResolveableWithinProject, new object[1]
          {
            (object) value.GetType().FullName
          }));
        this.parent.SceneNode.SetValue((IPropertyId) this.property, value);
        editTransaction.Commit();
      }
      return this.Value;
    }

    private SceneEditTransaction CreateEditTransaction()
    {
      return this.parent.SceneNode.ViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) this.property.Name
      }));
    }
  }
}
