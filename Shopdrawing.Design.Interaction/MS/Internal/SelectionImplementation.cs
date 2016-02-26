// Decompiled with JetBrains decompiler
// Type: MS.Internal.SelectionImplementation
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MS.Internal
{
  internal static class SelectionImplementation
  {
    internal static Selection SelectParent(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      ModelService requiredService = context.Services.GetRequiredService<ModelService>();
      Selection s = context.Items.GetValue<Selection>();
      Selection selection = new Selection();
      ModelItem primarySelection = s.PrimarySelection;
      if (SelectionImplementation.AreSiblings(s))
        selection = new Selection(new ModelItem[1]
        {
          primarySelection.Parent
        });
      if (selection.PrimarySelection == null)
        selection = new Selection(new ModelItem[1]
        {
          requiredService.Root
        });
      context.Items.SetValue((ContextItem) selection);
      return selection;
    }

    private static IEnumerable<ModelItem> EnumerateContents(ModelItem start)
    {
      if (SelectionImplementation.IsSelectable(start))
      {
        yield return start;
        ModelProperty content = start.Content;
        if (content != (ModelProperty) null && content.IsSet)
        {
          if (content.IsCollection)
          {
            foreach (ModelItem start1 in content.Collection)
            {
              foreach (ModelItem modelItem in SelectionImplementation.EnumerateContents(start1))
                yield return modelItem;
            }
          }
          else
          {
            ModelItem value = content.Value;
            if (value != null)
            {
              foreach (ModelItem modelItem in SelectionImplementation.EnumerateContents(value))
                yield return modelItem;
            }
          }
        }
      }
    }

    internal static Selection SelectAll(EditingContext context, bool local)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      ModelService requiredService = context.Services.GetRequiredService<ModelService>();
      Selection s = context.Items.GetValue<Selection>();
      Selection selection = new Selection();
      ModelItem primarySelection = s.PrimarySelection;
      if (local && primarySelection != null)
      {
        if (!SelectionImplementation.IsMultiSelection(s) && primarySelection.Content != (ModelProperty) null)
        {
          selection = SelectionImplementation.SelectContent(primarySelection);
          if (selection.PrimarySelection == null && primarySelection.Parent != null)
            selection = SelectionImplementation.SelectContent(primarySelection.Parent);
        }
        else if (SelectionImplementation.AreSiblings(s))
          selection = SelectionImplementation.SelectContent(primarySelection.Parent);
      }
      if (selection.PrimarySelection == null)
        selection = new Selection(SelectionImplementation.EnumerateContents(requiredService.Root));
      context.Items.SetValue((ContextItem) selection);
      return selection;
    }

    internal static void SelectNext(EditingContext context)
    {
      Selection s = context.Items.GetValue<Selection>();
      if (SelectionImplementation.IsMultiSelection(s))
      {
        SelectionImplementation.SelectParent(context);
      }
      else
      {
        if (s.PrimarySelection == null)
          return;
        ModelItem modelItem1 = s.PrimarySelection;
        ViewService service = context.Services.GetService<ViewService>();
        ModelItem modelItem2;
        for (modelItem2 = SelectionImplementation.GetFirstChild(s.PrimarySelection, service); modelItem2 == null && modelItem1.Parent != null; modelItem1 = modelItem1.Parent)
          modelItem2 = SelectionImplementation.GetNextSibling(modelItem1, service);
        if (modelItem2 == null)
          modelItem2 = modelItem1;
        if (modelItem2 == null)
          return;
        context.Items.SetValue((ContextItem) new Selection(new ModelItem[1]
        {
          modelItem2
        }));
      }
    }

    internal static void SelectPrevious(EditingContext context)
    {
      Selection s = context.Items.GetValue<Selection>();
      if (SelectionImplementation.IsMultiSelection(s))
      {
        SelectionImplementation.SelectParent(context);
      }
      else
      {
        if (s.PrimarySelection == null)
          return;
        ModelItem primarySelection = s.PrimarySelection;
        ViewService service = context.Services.GetService<ViewService>();
        ModelItem previousSibling = SelectionImplementation.GetPreviousSibling(primarySelection, service);
        ModelItem modelItem = previousSibling != null ? SelectionImplementation.GetLastChild(previousSibling, service) : (primarySelection.Parent != null ? primarySelection.Parent : SelectionImplementation.GetLastChild(primarySelection, service));
        if (modelItem == null)
          return;
        context.Items.SetValue((ContextItem) new Selection(new ModelItem[1]
        {
          modelItem
        }));
      }
    }

    private static ModelItem GetFirstChild(ModelItem modelItem, ViewService viewService)
    {
      ModelProperty content = modelItem.Content;
      if (content == (ModelProperty) null)
        return (ModelItem) null;
      ModelItem modelItem1 = (ModelItem) null;
      if (content.IsCollection)
      {
        if (content.Collection.Count == 0)
          return (ModelItem) null;
        modelItem1 = content.Collection[0];
      }
      else if (content.IsSet)
        modelItem1 = content.Value;
      if (modelItem1 == null || !SelectionImplementation.IsSelectable(modelItem1) || modelItem1.View == (ViewItem) null)
        return (ModelItem) null;
      return modelItem1;
    }

    private static ModelItem GetLastChild(ModelItem modelItem, ViewService viewService)
    {
      ModelItem modelItem1 = modelItem;
      bool flag = false;
      while (modelItem != null && modelItem.Content != (ModelProperty) null && !flag)
      {
        if (modelItem.Content.IsCollection)
        {
          if (modelItem.Content.Collection.Count > 0)
            modelItem = modelItem.Content.Collection[modelItem.Content.Collection.Count - 1];
          else
            flag = true;
        }
        else if (modelItem.Content.IsSet)
          modelItem = modelItem.Content.Value;
        else
          flag = true;
        if (modelItem != null && SelectionImplementation.IsSelectable(modelItem) && modelItem.View != (ViewItem) null)
          modelItem1 = modelItem;
      }
      return modelItem1;
    }

    private static ModelItem GetNextSibling(ModelItem modelItem, ViewService viewService)
    {
      ModelItem parent = modelItem.Parent;
      if (parent == null || parent.Content == (ModelProperty) null || !parent.Content.IsCollection)
        return (ModelItem) null;
      int num = parent.Content.Collection.IndexOf(modelItem);
      if (num >= parent.Content.Collection.Count - 1)
        return (ModelItem) null;
      ModelItem modelItem1 = parent.Content.Collection[num + 1];
      if (modelItem1.View == (ViewItem) null)
        return (ModelItem) null;
      if (!SelectionImplementation.IsSelectable(modelItem1))
        return (ModelItem) null;
      return modelItem1;
    }

    private static ModelItem GetPreviousSibling(ModelItem modelItem, ViewService viewService)
    {
      ModelItem parent = modelItem.Parent;
      if (parent == null || parent.Content == (ModelProperty) null || !parent.Content.IsCollection)
        return (ModelItem) null;
      int num = parent.Content.Collection.IndexOf(modelItem);
      if (num <= 0)
        return (ModelItem) null;
      ModelItem modelItem1 = parent.Content.Collection[num - 1];
      if (modelItem1.View == (ViewItem) null)
        return (ModelItem) null;
      if (!SelectionImplementation.IsSelectable(modelItem1))
        return (ModelItem) null;
      return modelItem1;
    }

    private static bool AreSiblings(Selection s)
    {
      bool flag = false;
      if (s != null && s.PrimarySelection != null)
      {
        ModelItem primarySelection = s.PrimarySelection;
        if (primarySelection != null && primarySelection.Parent != null)
        {
          ModelItem parent = primarySelection.Parent;
          flag = true;
          foreach (ModelItem modelItem in s.SelectedObjects)
          {
            if (modelItem.Parent != parent)
            {
              flag = false;
              break;
            }
          }
        }
      }
      return flag;
    }

    private static bool IsMultiSelection(Selection s)
    {
      bool flag = false;
      if (s != null && s.SelectionCount > 1)
        flag = true;
      return flag;
    }

    private static Selection SelectContent(ModelItem parent)
    {
      Selection selection = new Selection();
      if (parent.Content.IsCollection)
        selection = new Selection((IEnumerable) parent.Content.Collection);
      else if (parent.Content.IsDictionary)
        selection = new Selection((IEnumerable) parent.Content.Dictionary);
      else if (parent.Content.IsSet && SelectionImplementation.IsSelectable(parent) && SelectionImplementation.IsSelectable(parent.Content.Value))
        selection = new Selection(new ModelItem[1]
        {
          parent.Content.Value
        });
      return selection;
    }

    private static bool IsSelectable(ModelItem item)
    {
      if (item != null && !item.ItemType.IsPrimitive)
        return !typeof (string).Equals(item.ItemType);
      return false;
    }

    internal static void ShowDefaultEvent(EditingContext context)
    {
      Selection selection = context.Items.GetValue<Selection>();
      if (selection == null)
        return;
      ModelItem primarySelection = selection.PrimarySelection;
      if (primarySelection == null)
        return;
      DefaultEventAttribute defaultEventAttribute = (DefaultEventAttribute) null;
      using (IEnumerator<object> enumerator = primarySelection.GetAttributes(typeof (DefaultEventAttribute)).GetEnumerator())
      {
        if (enumerator.MoveNext())
          defaultEventAttribute = (DefaultEventAttribute) enumerator.Current;
      }
      if (defaultEventAttribute == null || defaultEventAttribute.Name == null || primarySelection.Events == null)
        return;
      ModelEvent modelEvent = primarySelection.Events.Find(defaultEventAttribute.Name);
      if (modelEvent == null)
        return;
      EventBindingService service = context.Services.GetService<EventBindingService>();
      if (service == null)
        return;
      string methodName = (string) null;
      while (modelEvent.Handlers.Count > 0)
      {
        using (IEnumerator<string> enumerator = modelEvent.Handlers.GetEnumerator())
        {
          if (enumerator.MoveNext())
            methodName = enumerator.Current;
        }
        try
        {
          service.ValidateMethodName(modelEvent, methodName);
          break;
        }
        catch (NotSupportedException ex)
        {
        }
        modelEvent.Handlers.Remove(methodName);
        methodName = (string) null;
      }
      bool flag = false;
      foreach (string str in service.GetMethodHandlers(modelEvent))
      {
        if (methodName == null)
        {
          methodName = str;
          flag = !service.AllowClassNameForMethodName();
          break;
        }
        if (methodName == str)
        {
          flag = true;
          break;
        }
      }
      if (methodName == null || !flag)
      {
        if (methodName == null)
        {
          methodName = service.CreateUniqueMethodName(modelEvent);
          if (methodName == null)
            return;
        }
        else
          flag = true;
        if (!service.IsExistingMethodName(modelEvent, methodName) && !service.CreateMethod(modelEvent, methodName))
          return;
        if (!flag && !service.AddEventHandler(modelEvent, methodName))
          modelEvent.Handlers.Add(methodName);
      }
      service.ShowMethod(modelEvent, methodName);
    }
  }
}
