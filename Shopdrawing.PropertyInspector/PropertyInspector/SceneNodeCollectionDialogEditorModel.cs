// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCollectionDialogEditorModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodeCollectionDialogEditorModel : CollectionDialogEditorModel
  {
    private PropertyEditingHelper transactionHelper;
    private SceneNodePropertyLookup sceneNodePropertyLookup;
    private SceneNodeObjectSet sceneNodeObjectSet;

    public override IMessageLoggingService MessageLoggingService
    {
      get
      {
        return this.SceneNodeObjectSet.DesignerContext.MessageLoggingService;
      }
    }

    private SceneNodeObjectSet SceneNodeObjectSet
    {
      get
      {
        if (this.sceneNodeObjectSet == null && this.Host != null && (this.Host.PropertyValue != null && this.Host.PropertyValue.get_ParentProperty() != null))
        {
          this.sceneNodeObjectSet = ((SceneNodeProperty) this.Host.PropertyValue.get_ParentProperty()).SceneNodeObjectSet;
          this.transactionHelper.ActiveDocument = this.sceneNodeObjectSet.Document;
        }
        return this.sceneNodeObjectSet;
      }
    }

    public override bool CanMoveItem
    {
      get
      {
        return !this.SceneNodeObjectSet.ViewModel.AnimationEditor.IsKeyFraming;
      }
    }

    public override bool CanRemoveItem
    {
      get
      {
        return true;
      }
    }

    public override CollectionDialogEditor Host
    {
      get
      {
        return base.Host;
      }
      set
      {
        base.Host = value;
        this.transactionHelper = new PropertyEditingHelper((UIElement) value);
      }
    }

    public SceneNodePropertyLookup SceneNodePropertyLookup
    {
      get
      {
        return this.sceneNodePropertyLookup;
      }
    }

    public override void PropertyValueChanged()
    {
    }

    public override void UpdateTransaction()
    {
      this.transactionHelper.UpdateTransaction();
    }

    public override void OnRebuildComplete(PropertyValue activePropertyValue)
    {
      if (this.transactionHelper != null)
        this.transactionHelper.CommitOutstandingTransactions(0);
      if (this.sceneNodePropertyLookup != null)
      {
        this.sceneNodePropertyLookup.ClearProperties();
        this.sceneNodePropertyLookup.Unhook();
        this.sceneNodePropertyLookup = (SceneNodePropertyLookup) null;
      }
      if (activePropertyValue == null)
        return;
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) activePropertyValue.get_ParentProperty();
      this.sceneNodePropertyLookup = new SceneNodePropertyLookup(sceneNodeProperty.SceneNodeObjectSet, sceneNodeProperty.Reference);
      using (IEnumerator<PropertyEntry> enumerator = activePropertyValue.get_SubProperties().GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          SceneNodeProperty property = enumerator.Current as SceneNodeProperty;
          if (property != null)
            this.sceneNodePropertyLookup.AddProperty(property.Reference, property);
        }
      }
      bool isMixed;
      DocumentNode valueAsDocumentNode = sceneNodeProperty.GetLocalValueAsDocumentNode(true, out isMixed);
      SceneNode[] selectedObjects;
      if (valueAsDocumentNode != null)
        selectedObjects = new SceneNode[1]
        {
          sceneNodeProperty.SceneNodeObjectSet.ViewModel.GetSceneNode(valueAsDocumentNode)
        };
      else
        selectedObjects = new SceneNode[0];
      foreach (CategoryBase categoryBase in (IEnumerable<CategoryBase>) this.Categories)
      {
        SceneNodeCategory sceneNodeCategory = categoryBase as SceneNodeCategory;
        if (sceneNodeCategory != null)
          sceneNodeCategory.OnSelectionChanged(selectedObjects);
      }
    }

    public override CategoryBase FindOrCreateCategory(string categoryName, Type selectedType)
    {
      CategoryLocalizationHelper.CategoryName canonicalCategoryName = CategoryLocalizationHelper.GetCanonicalCategoryName(categoryName);
      if (canonicalCategoryName != CategoryLocalizationHelper.CategoryName.Unknown)
        categoryName = CategoryLocalizationHelper.GetLocalizedCategoryName(canonicalCategoryName);
      int index = OrderedListExtensions.GenericBinarySearch<CategoryBase, CategoryLocalizationHelper.CategoryName>(this.Categories, canonicalCategoryName, (Func<CategoryLocalizationHelper.CategoryName, CategoryBase, int>) ((canonicalName, category) =>
      {
        SceneNodeCategory sceneNodeCategory = (SceneNodeCategory) category;
        return canonicalName.CompareTo((object) sceneNodeCategory.CanonicalName);
      }));
      if (index >= 0)
        return this.Categories[index];
      CategoryBase category1 = this.CreateCategory(categoryName, selectedType);
      this.Categories.Insert(~index, category1);
      return category1;
    }

    private CategoryBase CreateCategory(string name, Type selectedType)
    {
      CategoryLocalizationHelper.CategoryName canonicalCategoryName = CategoryLocalizationHelper.GetCanonicalCategoryName(name);
      CategoryBase categoryBase = (CategoryBase) CategoryFactory.GetCustomCategorySelector(canonicalCategoryName).CreateSceneNodeCategory(canonicalCategoryName, name, this.SceneNodeObjectSet.DesignerContext.MessageLoggingService);
      ITypeResolver typeResolver = (ITypeResolver) this.SceneNodeObjectSet.ProjectContext;
      CategoryEditorSet categoryEditorsList = new CategoryEditorSet();
      IType type = typeResolver.GetType(selectedType);
      PropertyInspectorModel.GetCategoryEditors(type, categoryEditorsList, this.MessageLoggingService);
      using (IEnumerator<CategoryEditor> enumerator = categoryEditorsList.Union(CategoryEditorInstanceFactory.GetEditors((ITypeId) type, categoryBase as SceneNodeCategory)).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          CategoryEditor current = enumerator.Current;
          if (current.get_TargetCategory() == categoryBase.get_CategoryName())
            categoryBase.CategoryEditors.Add(current);
        }
      }
      return categoryBase;
    }

    public override object CreateType(Type type)
    {
      if (type == (Type) null)
      {
        SceneNodeProperty sceneNodeProperty = this.Host.PropertyValue.get_ParentProperty() as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          SceneViewModel viewModel = sceneNodeProperty.SceneNodeObjectSet.ViewModel;
          ReferenceStep step = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep((ITypeResolver) viewModel.ProjectContext, ((PropertyEntry) sceneNodeProperty).get_PropertyType(), 0);
          Type propertyType = sceneNodeProperty.Reference.Append(step).ValueType;
          Type genericCollectionType = CollectionAdapterDescription.GetGenericCollectionType(((PropertyEntry) sceneNodeProperty).get_PropertyType());
          if (genericCollectionType != (Type) null && propertyType.IsAssignableFrom(genericCollectionType))
            propertyType = genericCollectionType;
          SceneNode typeForProperty = this.CreateTypeForProperty(viewModel, propertyType);
          if (typeForProperty != null)
            return (object) typeForProperty.DocumentNode;
        }
      }
      return null;
    }

    public override IList<NewItemTypesAttribute> GetNewItemTypesAttributes()
    {
      return ((PropertyReferenceProperty) this.Host.PropertyValue.get_ParentProperty()).GetAttributes<NewItemTypesAttribute>(true);
    }

    private SceneNode CreateTypeForProperty(SceneViewModel viewModel, Type propertyType)
    {
      Type newClrObject = ClrNewObjectDialog.CreateNewClrObject(viewModel, propertyType);
      if (newClrObject == (Type) null)
        return (SceneNode) null;
      Type type = newClrObject;
      if (typeof (string).Equals(type))
        return viewModel.CreateSceneNode((object) StringTable.DefaultCollectionEditorString);
      return viewModel.CreateSceneNode(type);
    }

    public void Unhook()
    {
      if (this.sceneNodePropertyLookup != null)
      {
        this.sceneNodePropertyLookup.ClearProperties();
        this.sceneNodePropertyLookup.Unhook();
        this.sceneNodePropertyLookup = (SceneNodePropertyLookup) null;
      }
      this.Categories.Clear();
    }
  }
}
