// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelParent
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Windows.Design.Model
{
  public static class ModelParent
  {
    public static bool CanParent(EditingContext context, ModelItem parent, Type childType)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (childType == null)
        throw new ArgumentNullException("childType");
      ModelItem redirectedParent;
      return ModelParent.GetImplementation(context).CanParent(parent, childType, (ModelItem) null, out redirectedParent);
    }

    public static ModelItem FindParent(EditingContext context, ModelItem childItem, ModelItem startingItem)
    {
      if (childItem == null)
        throw new ArgumentNullException("childItem");
      return ModelParent.FindParent(context, childItem.ItemType, startingItem, childItem);
    }

    public static ModelItem FindParent(EditingContext context, Type childType, ModelItem startingItem)
    {
      return ModelParent.FindParent(context, childType, startingItem, (ModelItem) null);
    }

    internal static ModelItem FindParent(EditingContext context, Type childType, ModelItem startingItem, ModelItem childItem)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (childType == null)
        throw new ArgumentNullException("childType");
      if (startingItem == null)
        throw new ArgumentNullException("startingItem");
      return ModelParent.GetImplementation(context).FindParent(childType, startingItem, childItem);
    }

    public static ModelItem FindParent(Type childType, GestureData gestureData)
    {
      if (gestureData == null)
        throw new ArgumentNullException("gestureData");
      ModelItem targetModel = gestureData.TargetModel;
      if (targetModel == null)
        return (ModelItem) null;
      return ModelParent.FindParent(gestureData.Context, childType, targetModel);
    }

    internal static ModelParent.ItemParentImplementationService GetImplementation(EditingContext context)
    {
      ModelParent.ItemParentImplementationService serviceInstance = context.Services.GetService<ModelParent.ItemParentImplementationService>();
      if (serviceInstance == null)
      {
        serviceInstance = new ModelParent.ItemParentImplementationService();
        context.Services.Publish<ModelParent.ItemParentImplementationService>(serviceInstance);
      }
      return serviceInstance;
    }

    public static void Parent(EditingContext context, ModelItem parentItem, ModelItem childItem)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (parentItem == null)
        throw new ArgumentNullException("parentItem");
      if (childItem == null)
        throw new ArgumentNullException("childItem");
      ModelParent.GetImplementation(context).Parent(parentItem, childItem);
    }

    internal class ItemParentImplementationService
    {
      private FeatureManager _featureManager;

      internal FeatureManager FeatureManager
      {
        set
        {
          this._featureManager = value;
        }
      }

      internal ItemParentImplementationService()
      {
      }

      internal bool CanParent(ModelItem parent, Type childType, ModelItem childItem, out ModelItem redirectedParent)
      {
        redirectedParent = (ModelItem) null;
        if (this._featureManager == null)
          return false;
        using (IEnumerator<FeatureProvider> enumerator = FeatureExtensions.CreateFeatureProviders(this._featureManager, typeof (ParentAdapter), parent).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            ParentAdapter parentAdapter = (ParentAdapter) enumerator.Current;
            redirectedParent = parentAdapter.RedirectParent(parent, childType);
            if (redirectedParent == null)
              throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidRedirectParent, new object[1]
              {
                (object) parentAdapter.GetType().Name
              }));
            if (redirectedParent.Equals((object) childItem))
              return false;
            ModelItem modelItem = redirectedParent;
            ViewItem logicalParent = ModelParent.ItemParentImplementationService.GetLogicalParent(modelItem);
            if (childItem != null)
            {
              for (; logicalParent != (ViewItem) null; logicalParent = ModelParent.ItemParentImplementationService.GetLogicalParent(modelItem))
              {
                if (logicalParent.Equals((object) childItem.View))
                  return false;
                modelItem = modelItem.Parent;
              }
            }
            if (childItem != null && parentAdapter.IsParent(redirectedParent, childItem))
              return true;
            if (redirectedParent == parent)
              return parentAdapter.CanParent(parent, childType);
            parent = redirectedParent;
            return this.CanParent(parent, childType, childItem, out redirectedParent);
          }
        }
        return false;
      }

      internal ModelItem FindParent(Type childType, ModelItem startingItem, ModelItem childItem)
      {
        if (this._featureManager == null)
          return (ModelItem) null;
        for (ModelItem parent = startingItem; parent != null; parent = parent.Parent)
        {
          ModelItem redirectedParent;
          if (this.CanParent(parent, childType, childItem, out redirectedParent))
            return redirectedParent;
        }
        return (ModelItem) null;
      }

      private static ViewItem GetLogicalParent(ModelItem item)
      {
        if (item == null)
          return (ViewItem) null;
        if (item.View == (ViewItem) null)
          return (ViewItem) null;
        return item.View.LogicalParent;
      }

      internal void Parent(ModelItem parent, ModelItem child)
      {
        if (this._featureManager == null)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ParentNotSupported, new object[2]
          {
            (object) parent.ItemType.Name,
            (object) child.ItemType.Name
          }));
        ParentAdapter parentAdapter1 = (ParentAdapter) null;
        using (IEnumerator<FeatureProvider> enumerator = FeatureExtensions.CreateFeatureProviders(this._featureManager, typeof (ParentAdapter), parent).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            ParentAdapter parentAdapter2 = (ParentAdapter) enumerator.Current;
            ModelItem parent1 = parentAdapter2.RedirectParent(parent, child.ItemType);
            if (parent1 == null)
              throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidRedirectParent, new object[1]
              {
                (object) parentAdapter2.GetType().Name
              }));
            if (parent1 != parent)
            {
              this.Parent(parent1, child);
              return;
            }
            parentAdapter1 = parentAdapter2;
          }
        }
        if (parentAdapter1 == null || !parentAdapter1.CanParent(parent, child.ItemType))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ParentNotSupported, new object[2]
          {
            (object) parent.ItemType.Name,
            (object) child.ItemType.Name
          }));
        ModelItem parent2 = child.Parent;
        if (parent2 == parent)
          return;
        if (parent2 != null)
        {
          using (IEnumerator<FeatureProvider> enumerator = FeatureExtensions.CreateFeatureProviders(this._featureManager, typeof (ParentAdapter), parent2).GetEnumerator())
          {
            if (enumerator.MoveNext())
              ((ParentAdapter) enumerator.Current).RemoveParent(parent2, parent, child);
          }
        }
        parentAdapter1.Parent(parent, child);
      }
    }
  }
}
