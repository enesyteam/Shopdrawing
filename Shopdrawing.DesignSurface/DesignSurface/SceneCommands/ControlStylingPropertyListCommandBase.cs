// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ControlStylingPropertyListCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class ControlStylingPropertyListCommandBase : SingleTargetDynamicMenuCommandBase
  {
    private static List<KeyValuePair<IPropertyId, string>> displayNames = new List<KeyValuePair<IPropertyId, string>>();

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
        {
          SceneElement primarySelection = this.ViewModel.ElementSelectionSet.PrimarySelection;
          if (primarySelection != null && this.ViewModel.ElementSelectionSet.Count == 1)
          {
            foreach (IPropertyId propertyId in primarySelection.GetProperties())
            {
              ReferenceStep referenceStep = propertyId as ReferenceStep;
              if (referenceStep != null && this.IsCommandProperty(referenceStep) && this.GeneratePropertyItems(primarySelection, referenceStep).Count > 0)
                return true;
            }
          }
        }
        return false;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        SceneElement primarySelection = this.ViewModel.ElementSelectionSet.PrimarySelection;
        ArrayList arrayList = new ArrayList();
        if (primarySelection != null)
        {
          List<ReferenceStep> list1 = new List<ReferenceStep>();
          List<ReferenceStep> list2 = new List<ReferenceStep>();
          foreach (IPropertyId propertyId in primarySelection.GetProperties())
          {
            ReferenceStep referenceStep = propertyId as ReferenceStep;
            if (referenceStep != null && this.IsCommandProperty(referenceStep))
              (this.GetDisplayName((IPropertyId) referenceStep) != null ? list1 : list2).Add(referenceStep);
          }
          list1.Sort((Comparison<ReferenceStep>) ((a, b) => ControlStylingPropertyListCommandBase.displayNames.FindIndex((Predicate<KeyValuePair<IPropertyId, string>>) (i => i.Key.Name.Equals(a.Name))).CompareTo(ControlStylingPropertyListCommandBase.displayNames.FindIndex((Predicate<KeyValuePair<IPropertyId, string>>) (i => i.Key.Name.Equals(b.Name))))));
          foreach (ReferenceStep referenceStep in list1)
          {
            MenuItem menuItem = this.BuildMenuItem(this.TargetElement, referenceStep);
            if (menuItem != null)
              arrayList.Add((object) menuItem);
          }
          if (list1.Count > 0 && list2.Count > 0)
            arrayList.Add((object) new Separator());
          list2.Sort((Comparison<ReferenceStep>) ((a, b) => StringLogicalComparer.Instance.Compare(a.Name, b.Name)));
          foreach (ReferenceStep referenceStep in list2)
          {
            MenuItem menuItem = this.BuildMenuItem(this.TargetElement, referenceStep);
            if (menuItem != null)
              arrayList.Add((object) menuItem);
          }
        }
        return (IEnumerable) arrayList;
      }
    }

    static ControlStylingPropertyListCommandBase()
    {
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(ControlElement.TemplateProperty, StringTable.MenuControlTemplateDisplayName));
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(ItemsControlElement.ItemTemplateProperty, StringTable.MenuItemTemplateDisplayName));
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(ItemsControlElement.ItemContainerStyleProperty, StringTable.MenuItemContainerStyleDisplayName));
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(ItemsControlElement.ItemsPanelProperty, StringTable.MenuItemsPanelTemplateDisplayName));
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(ContentControlElement.ContentTemplateProperty, StringTable.MenuContentTemplateDisplayName));
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(TextEditProxyFactory.HeaderedContentControlHeaderProperty, StringTable.MenuHeaderTemplateDisplayName));
      ControlStylingPropertyListCommandBase.displayNames.Add(new KeyValuePair<IPropertyId, string>(TextEditProxyFactory.HeaderedItemsControlHeaderProperty, StringTable.MenuHeaderTemplateDisplayName));
    }

    public ControlStylingPropertyListCommandBase(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected string GetDisplayName(IPropertyId referenceStep)
    {
      ItemsControlElement itemsControlElement = this.TargetElement as ItemsControlElement;
      if (itemsControlElement != null && referenceStep == ItemsControlElement.GetItemContainerStyleProperty(itemsControlElement.ProjectContext, (ITypeId) itemsControlElement.Type))
        referenceStep = ItemsControlElement.ItemContainerStyleProperty;
      foreach (KeyValuePair<IPropertyId, string> keyValuePair in ControlStylingPropertyListCommandBase.displayNames)
      {
        if (keyValuePair.Key.Equals((object) referenceStep))
          return keyValuePair.Value;
      }
      return (string) null;
    }

    private MenuItem BuildMenuItem(SceneElement element, ReferenceStep referenceStep)
    {
      MenuItem menuItem = (MenuItem) null;
      IList<Control> list = this.GeneratePropertyItems(element, referenceStep);
      if (list.Count > 0)
      {
        menuItem = new MenuItem();
        menuItem.SetValue(AutomationElement.IdProperty, (object) referenceStep.Name);
        foreach (Control control in (IEnumerable<Control>) list)
          menuItem.Items.Add((object) control);
        string displayName = this.GetDisplayName((IPropertyId) referenceStep);
        if (displayName != null)
          menuItem.Header = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.EditPropertyItemWithDisplayNameFormat, new object[2]
          {
            (object) displayName,
            (object) referenceStep.Name
          });
        else
          menuItem.Header = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.EditPropertyItemFormat, new object[1]
          {
            (object) referenceStep.Name
          });
      }
      return menuItem;
    }

    protected abstract IList<Control> GeneratePropertyItems(SceneElement targetElement, ReferenceStep targetProperty);

    protected virtual bool IsCommandProperty(ReferenceStep referenceStep)
    {
      DependencyPropertyReferenceStep propertyReferenceStep = referenceStep as DependencyPropertyReferenceStep;
      if (propertyReferenceStep != null)
        return propertyReferenceStep.ShouldSerialize;
      return true;
    }
  }
}
