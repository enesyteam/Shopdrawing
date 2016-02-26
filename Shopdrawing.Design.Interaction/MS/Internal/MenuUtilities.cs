// Decompiled with JetBrains decompiler
// Type: MS.Internal.MenuUtilities
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MS.Internal
{
  internal static class MenuUtilities
  {
    public static int CompareProviders<T>(T provider1, T provider2) where T : FeatureProvider
    {
      return string.Compare(provider1.GetType().FullName, provider2.GetType().FullName, StringComparison.Ordinal);
    }

    public static int CompareContextMenuProviders(ContextMenuProvider provider1, ContextMenuProvider provider2)
    {
      string strA = provider1.Items.Count > 0 ? provider1.Items[0].Name : string.Empty;
      string strB = provider2.Items.Count > 0 ? provider2.Items[0].Name : string.Empty;
      bool flag1 = !string.IsNullOrEmpty(strA);
      bool flag2 = !string.IsNullOrEmpty(strB);
      if (flag1 & !flag2)
        return -1;
      if (!flag1 & flag2)
        return 1;
      int num = 0;
      if (flag1 & flag2)
        num = string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase);
      if (num == 0)
        num = string.Compare(provider1.GetType().FullName, provider2.GetType().FullName, StringComparison.OrdinalIgnoreCase);
      return num;
    }

    public static IEnumerable<MenuBase> ExpandMenuGroups(IEnumerable<MenuBase> items, MenuUtilities.MenuBaseExpander groupExpander)
    {
      if (items == null)
        throw new ArgumentNullException("items");
      if (groupExpander == null)
        throw new ArgumentNullException("groupExpander");
      foreach (MenuBase menuBase1 in items)
      {
        MenuGroup group = menuBase1 as MenuGroup;
        if (group != null)
        {
          foreach (MenuBase menuBase2 in MenuUtilities.ExpandMenuGroups(groupExpander(group), groupExpander))
            yield return menuBase2;
        }
        else
          yield return menuBase1;
      }
    }

    public static IEnumerable<MenuBase> MergeMenuGroups(MenuUtilities.MenuBaseEnumerator rootItemEnumerator)
    {
      List<MenuBase> list = new List<MenuBase>();
      Dictionary<string, MenuGroup> dictionary = new Dictionary<string, MenuGroup>();
      foreach (MenuBase menuBase in rootItemEnumerator())
      {
        MenuGroup groupToMergeIn = menuBase as MenuGroup;
        if (groupToMergeIn != null)
        {
          string key = groupToMergeIn.Name + groupToMergeIn.HasDropDown.ToString();
          if (dictionary.ContainsKey(key))
          {
            MenuGroup existingGroup = dictionary[key];
            int index = list.IndexOf((MenuBase) existingGroup);
            if (index >= 0)
              list.RemoveAt(index);
            if (index < list.Count)
              list.Insert(index, (MenuBase) MenuUtilities.MergeGroups(existingGroup, groupToMergeIn));
            else
              list.Add((MenuBase) MenuUtilities.MergeGroups(existingGroup, groupToMergeIn));
          }
          else
          {
            dictionary.Add(key, groupToMergeIn);
            list.Add((MenuBase) groupToMergeIn);
          }
        }
        else
          list.Add(menuBase);
      }
      return (IEnumerable<MenuBase>) list;
    }

    private static MenuGroup MergeGroups(MenuGroup existingGroup, MenuGroup groupToMergeIn)
    {
      MenuGroup menuGroup1 = new MenuGroup(existingGroup.Name, existingGroup.DisplayName);
      menuGroup1.HasDropDown = existingGroup.HasDropDown;
      Dictionary<string, MenuGroup> dictionary = new Dictionary<string, MenuGroup>();
      foreach (MenuBase menuBase in (Collection<MenuBase>) existingGroup.Items)
      {
        MenuGroup menuGroup2 = menuBase as MenuGroup;
        if (menuGroup2 != null)
        {
          string key = menuGroup2.Name + menuGroup2.HasDropDown.ToString();
          dictionary.Add(key, menuGroup2);
        }
        menuGroup1.Items.Add(menuBase);
      }
      foreach (MenuBase menuBase in (Collection<MenuBase>) groupToMergeIn.Items)
      {
        MenuGroup groupToMergeIn1 = menuBase as MenuGroup;
        if (groupToMergeIn1 != null)
        {
          string key = groupToMergeIn1.Name + groupToMergeIn1.HasDropDown.ToString();
          if (dictionary.ContainsKey(key))
          {
            menuGroup1.Items.Remove((MenuBase) dictionary[key]);
            menuGroup1.Items.Add((MenuBase) MenuUtilities.MergeGroups(dictionary[key], groupToMergeIn1));
          }
          else
            menuGroup1.Items.Add(menuBase);
        }
        else
          menuGroup1.Items.Add(menuBase);
      }
      return menuGroup1;
    }

    public delegate IEnumerable<MenuBase> MenuBaseEnumerator();

    public delegate IEnumerable<MenuBase> MenuBaseExpander(MenuGroup group);
  }
}
