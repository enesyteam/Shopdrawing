// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.EditResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class EditResourceModel : INotifyPropertyChanged, IDisposable
  {
    private DesignerContext designerContext;
    private DictionaryEntryNode resourceEntryNode;
    private PropertyReferenceProperty standInProperty;
    private ResourceValueObjectSet resourceObjectSet;
    private DocumentNode parsedKeyString;
    private bool exceptionOccurred;
    private bool keyStringIsValid;
    private string keyString;
    private bool isDisposed;

    public bool ExceptionOccurred
    {
      get
      {
        return this.exceptionOccurred;
      }
    }

    public string KeyString
    {
      get
      {
        return this.keyString;
      }
      set
      {
        if (!(this.keyString != value))
          return;
        this.keyString = value;
        this.OnPropertyChanged("KeyString");
        this.ParseKeyString();
      }
    }

    public PropertyEntry EditingProperty
    {
      get
      {
        return (PropertyEntry) this.standInProperty;
      }
    }

    public Microsoft.Windows.Design.PropertyEditing.PropertyValue EditorValue
    {
      get
      {
        return this.standInProperty.PropertyValue;
      }
    }

    public SceneDocument Document
    {
      get
      {
        return this.resourceEntryNode.ViewModel.Document;
      }
    }

    public bool KeyStringIsValid
    {
      get
      {
        return this.keyStringIsValid;
      }
      set
      {
        if (this.keyStringIsValid == value)
          return;
        this.keyStringIsValid = value;
        this.OnPropertyChanged("KeyStringIsValid");
        this.OnPropertyChanged("ResourceIsValid");
      }
    }

    public bool ResourceIsValid
    {
      get
      {
        if (this.KeyStringIsValid)
          return !string.IsNullOrEmpty(this.EditorValue.StringValue);
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public EditResourceModel(DesignerContext designerContext, DictionaryEntryNode resourceEntryNode, IPropertyInspector transactionContext)
    {
      this.designerContext = designerContext;
      this.resourceEntryNode = resourceEntryNode;
      object key = resourceEntryNode.Key;
      DocumentNode expression;
      if ((expression = key as DocumentNode) != null)
        this.keyString = XamlExpressionSerializer.GetStringFromExpression(expression, resourceEntryNode.DocumentNode);
      else if ((this.keyString = key as string) == null)
        this.keyString = key.ToString();
      this.keyStringIsValid = true;
      ResourceEntryItem resource = (ResourceEntryItem) this.designerContext.ResourceManager.GetResourceItem((DocumentCompositeNode) resourceEntryNode.DocumentNode);
      this.resourceObjectSet = new ResourceValueObjectSet(resource, designerContext, transactionContext);
      this.standInProperty = this.resourceObjectSet.CreateProperty(new PropertyReference((ReferenceStep) this.resourceObjectSet.ProjectContext.ResolveProperty(DictionaryEntryNode.ValueProperty)), TypeUtilities.GetAttributes(resource.EffectiveType));
      this.standInProperty.PropertyValue.PropertyValueException += new EventHandler<PropertyValueExceptionEventArgs>(this.OnPropertyValueException);
      this.standInProperty.PropertyValue.PropertyChanged += new PropertyChangedEventHandler(this.OnValuePropertyChanged);
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
      {
        if (this.standInProperty != null && this.standInProperty.PropertyValue != null)
        {
          this.standInProperty.PropertyValue.PropertyChanged -= new PropertyChangedEventHandler(this.OnValuePropertyChanged);
          this.standInProperty.PropertyValue.PropertyValueException -= new EventHandler<PropertyValueExceptionEventArgs>(this.OnPropertyValueException);
        }
        this.resourceObjectSet.Dispose();
      }
      this.isDisposed = true;
    }

    public void ClearExceptionOccurred()
    {
      this.exceptionOccurred = false;
    }

    public void Update()
    {
      this.Document.OnUpdatedEditTransaction();
    }

    private void OnValuePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "StringValue"))
        return;
      this.OnPropertyChanged("ResourceIsValid");
    }

    private void OnPropertyValueException(object sender, PropertyValueExceptionEventArgs e)
    {
      int num = (int) this.resourceEntryNode.ViewModel.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.InvalidPropertyValueErrorMessage, new object[1]
        {
          (object) e.Exception.Message
        }),
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Hand
      });
      this.exceptionOccurred = true;
    }

    private void ParseKeyString()
    {
      if (this.keyString.Length == 0)
      {
        this.parsedKeyString = (DocumentNode) null;
        this.KeyStringIsValid = false;
      }
      else if ((int) this.keyString[0] != 123)
      {
        this.KeyStringIsValid = true;
        this.parsedKeyString = (DocumentNode) this.resourceEntryNode.DocumentContext.CreateNode(this.keyString);
      }
      else
      {
        IList<XamlParseError> errors;
        DocumentNode expressionFromString = XamlExpressionSerializer.GetExpressionFromString(this.keyString, this.resourceEntryNode.DocumentNode, typeof (string), out errors);
        this.parsedKeyString = expressionFromString == null ? (DocumentNode) null : (!(expressionFromString is DocumentPrimitiveNode) || !(expressionFromString.TargetType == typeof (Type)) ? (!DocumentNodeUtilities.IsStaticExtension(expressionFromString) ? (DocumentNode) null : expressionFromString) : expressionFromString);
        this.KeyStringIsValid = this.parsedKeyString != null;
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
