// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.AddDataStorePropertyDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Design.UserInterface.Dialogs;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class AddDataStorePropertyDialogModel : NotifyingObject, IGenericDialogModel
  {
    private StringEditor propertyNameEditor;
    private string selectedDataStore;
    private Button acceptButton;
    private MessageBubbleHelper propertyNameMessageBubble;
    private MessageBubbleValidator<string, TextChangedEventArgs> propertyNameValidator;

    private UIElement DialogContent { get; set; }

    public string PropertyName { get; set; }

    public IEnumerable<DataStoreReferenceEntry> DataStores { get; private set; }

    public string SelectedDataStore
    {
      get
      {
        return this.selectedDataStore;
      }
      set
      {
        this.selectedDataStore = value;
        this.propertyNameValidator.Validate();
      }
    }

    public IEnumerable<string> DataStoreNames
    {
      get
      {
        return Enumerable.Select<DataStoreReferenceEntry, string>(this.DataStores, (Func<DataStoreReferenceEntry, string>) (item => item.DataStore.Name));
      }
    }

    public AddDataStorePropertyDialogModel(IEnumerable<DataStoreReferenceEntry> dataStores)
    {
      this.DataStores = dataStores;
      this.selectedDataStore = Enumerable.FirstOrDefault<string>(this.DataStoreNames);
    }

    public void Initialize(UIElement dialogContent)
    {
      this.DialogContent = dialogContent;
      this.propertyNameEditor = LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "PropertyName") as StringEditor;
      this.acceptButton = LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "AcceptButton") as Button;
      this.propertyNameValidator = new MessageBubbleValidator<string, TextChangedEventArgs>((Func<string>) (() => this.propertyNameEditor.Text), new Func<string, string>(this.ValidatePropertyName));
      this.propertyNameMessageBubble = new MessageBubbleHelper((UIElement) this.propertyNameEditor, (IMessageBubbleValidator) this.propertyNameValidator);
      this.propertyNameEditor.TextChanged += new TextChangedEventHandler(this.propertyNameValidator.EventHook);
      this.acceptButton.IsEnabled = false;
    }

    private string ValidatePropertyName(string value)
    {
      if (Enumerable.FirstOrDefault<DataStoreReferenceEntry>(Enumerable.Where<DataStoreReferenceEntry>(this.DataStores, (Func<DataStoreReferenceEntry, bool>) (item => item.DataStore.Name == this.SelectedDataStore && Enumerable.FirstOrDefault<IProperty>(Enumerable.Where<IProperty>(item.DataStore.RootType.GetProperties(MemberAccessTypes.All), (Func<IProperty, bool>) (property => string.Compare(property.Name, value, StringComparison.OrdinalIgnoreCase) == 0))) != null))) != null)
      {
        this.acceptButton.IsEnabled = false;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ConflictingDataStorePropertyName, new object[1]
        {
          (object) value
        });
      }
      value = value.Trim();
      this.acceptButton.IsEnabled = !string.IsNullOrEmpty(value);
      return (string) null;
    }
  }
}
