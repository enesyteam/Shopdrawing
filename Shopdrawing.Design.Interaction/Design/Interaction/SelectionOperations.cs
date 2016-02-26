// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.SelectionOperations
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Interaction
{
  public static class SelectionOperations
  {
    public static Selection Select(EditingContext context, ModelItem itemToSelect)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (itemToSelect == null)
        throw new ArgumentNullException("itemToSelect");
      Selection selection1 = context.Items.GetValue<Selection>();
      if (selection1.PrimarySelection == itemToSelect)
        return selection1;
      Selection selection2 = (Selection) null;
      foreach (ModelItem modelItem in selection1.SelectedObjects)
      {
        if (modelItem == itemToSelect)
        {
          List<ModelItem> list = new List<ModelItem>(selection1.SelectedObjects);
          list.Remove(itemToSelect);
          list.Insert(0, itemToSelect);
          selection2 = new Selection((IEnumerable<ModelItem>) list);
        }
      }
      if (selection2 == null)
        selection2 = new Selection(new ModelItem[1]
        {
          itemToSelect
        });
      context.Items.SetValue((ContextItem) selection2);
      return selection2;
    }

    public static Selection SelectOnly(EditingContext context, ModelItem itemToSelect)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (itemToSelect == null)
        throw new ArgumentNullException("itemToSelect");
      Selection selection1 = context.Items.GetValue<Selection>();
      if (selection1.PrimarySelection == itemToSelect)
      {
        IEnumerator<ModelItem> enumerator = selection1.SelectedObjects.GetEnumerator();
        enumerator.MoveNext();
        if (!enumerator.MoveNext())
          return selection1;
      }
      Selection selection2 = new Selection(new ModelItem[1]
      {
        itemToSelect
      });
      context.Items.SetValue((ContextItem) selection2);
      return selection2;
    }

    public static void Subscribe(EditingContext context, SubscribeContextCallback<Selection> handler)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (handler == null)
        throw new ArgumentNullException("handler");
      context.Items.Subscribe<Selection>(handler);
    }

    public static Selection Toggle(EditingContext context, ModelItem itemToToggle)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (itemToToggle == null)
        throw new ArgumentNullException("itemToToggle");
      List<ModelItem> list = new List<ModelItem>(context.Items.GetValue<Selection>().SelectedObjects);
      if (list.Contains(itemToToggle))
        list.Remove(itemToToggle);
      else
        list.Insert(0, itemToToggle);
      if (list.Count == 0)
      {
        ModelService service = context.Services.GetService<ModelService>();
        if (service != null)
          list.Add(service.Root);
      }
      Selection selection = new Selection((IEnumerable<ModelItem>) list);
      context.Items.SetValue((ContextItem) selection);
      return selection;
    }

    public static Selection Union(EditingContext context, ModelItem itemToAdd)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (itemToAdd == null)
        throw new ArgumentNullException("itemToAdd");
      Selection selection1 = context.Items.GetValue<Selection>();
      if (selection1.PrimarySelection == itemToAdd)
        return selection1;
      List<ModelItem> list = new List<ModelItem>(selection1.SelectedObjects);
      if (list.Contains(itemToAdd))
        list.Remove(itemToAdd);
      list.Insert(0, itemToAdd);
      Selection selection2 = new Selection((IEnumerable<ModelItem>) list);
      context.Items.SetValue((ContextItem) selection2);
      return selection2;
    }

    public static void Unsubscribe(EditingContext context, SubscribeContextCallback<Selection> handler)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (handler == null)
        throw new ArgumentNullException("handler");
      context.Items.Unsubscribe<Selection>(handler);
    }
  }
}
