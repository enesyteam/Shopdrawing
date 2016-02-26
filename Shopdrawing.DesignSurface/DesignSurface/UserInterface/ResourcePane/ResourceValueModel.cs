// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceValueModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class ResourceValueModel : NotifyingObject, IPropertyInspector
  {
    private ResourceEntryItem resourceItem;
    private SceneNodeProperty valueProperty;
    private PropertyEditingHelper helper;
    private ResourceValueObjectSet objectSet;
    private bool exceptionOccurred;

    public bool ExceptionOccurred
    {
      get
      {
        return this.exceptionOccurred;
      }
      private set
      {
        if (this.exceptionOccurred == value)
          return;
        this.exceptionOccurred = value;
        this.OnPropertyChanged("ExceptionOccurred");
      }
    }

    public PropertyEntry EditingProperty
    {
      get
      {
        return (PropertyEntry) this.valueProperty;
      }
    }

    public Microsoft.Windows.Design.PropertyEditing.PropertyValue EditorValue
    {
      get
      {
        return this.valueProperty.PropertyValue;
      }
    }

    public ResourceEntryItem Resource
    {
      get
      {
        return this.resourceItem;
      }
    }

    public string Key
    {
      get
      {
        return this.resourceItem.Key;
      }
    }

    internal PropertyEditingHelper PropertyEditingHelper
    {
      get
      {
        return this.helper;
      }
    }

    public ResourceValueModel(ResourceEntryItem resourceItem, DesignerContext designerContext)
    {
      this.resourceItem = resourceItem;
      this.helper = new PropertyEditingHelper(this.resourceItem.Container.Document, (UIElement) null);
      this.objectSet = new ResourceValueObjectSet(resourceItem, designerContext, (IPropertyInspector) this);
      this.valueProperty = this.objectSet.CreateSceneNodeProperty(new PropertyReference((ReferenceStep) resourceItem.Container.ProjectContext.ResolveProperty(DictionaryEntryNode.ValueProperty)), TypeUtilities.GetAttributes(resourceItem.Resource.TargetType));
      this.valueProperty.PropertyValue.PropertyValueException += new EventHandler<PropertyValueExceptionEventArgs>(this.OnPropertyValueException);
    }

    public bool IsCategoryExpanded(string categoryName)
    {
      return true;
    }

    public void UpdateTransaction()
    {
      this.helper.UpdateTransaction();
    }

    public void Recache()
    {
      this.valueProperty.Recache();
    }

    public void OnRemoved()
    {
      this.valueProperty.OnRemoveFromCategory();
      if (this.objectSet != null)
      {
        this.objectSet.Dispose();
        this.objectSet = (ResourceValueObjectSet) null;
      }
      if (this.helper.ActiveDocument.IsEditable)
        return;
      this.helper.CancelOutstandingTransactions();
    }

    private void OnPropertyValueException(object sender, PropertyValueExceptionEventArgs e)
    {
      int num = (int) this.resourceItem.Container.ViewModel.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.InvalidPropertyValueErrorMessage, new object[1]
        {
          (object) e.Exception.Message
        }),
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Hand
      });
      this.ExceptionOccurred = true;
    }
  }
}
