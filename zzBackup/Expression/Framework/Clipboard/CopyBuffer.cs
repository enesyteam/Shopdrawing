// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Clipboard.CopyBuffer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Clipboard
{
  public class CopyBuffer
  {
    private List<CopyItem> selectedItems = new List<CopyItem>();
    private List<CopyItem> referencedItems = new List<CopyItem>();

    public string Creator { get; set; }

    public double Version { get; set; }

    public int SelectedItemCount
    {
      get
      {
        return this.selectedItems.Count;
      }
    }

    public int ReferencedItemCount
    {
      get
      {
        return this.referencedItems.Count;
      }
    }

    public void AddSelectedItem(CopyItem itemToPaste)
    {
      this.selectedItems.Add(itemToPaste);
    }

    public void AddReferencedItem(CopyItem item)
    {
      this.referencedItems.Add(item);
    }

    public CopyItem SelectedItem(int index)
    {
      return this.selectedItems[index];
    }

    public CopyItem ReferencedItem(int index)
    {
      return this.referencedItems[index];
    }
  }
}
