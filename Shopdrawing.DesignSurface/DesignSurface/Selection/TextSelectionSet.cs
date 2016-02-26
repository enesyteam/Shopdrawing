// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.TextSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class TextSelectionSet : SelectionSet<DesignTimeTextSelection, OrderedList<DesignTimeTextSelection>>
  {
    private bool isActive;
    private TextEditProxy textEditProxy;

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
      set
      {
        this.isActive = value;
        this.Rebroadcast();
      }
    }

    public override bool IsExclusive
    {
      get
      {
        return false;
      }
    }

    public bool CanCopy
    {
      get
      {
        if (this.PrimarySelection != null)
          return this.PrimarySelection.CanCopy;
        return false;
      }
    }

    public bool CanDelete
    {
      get
      {
        if (this.PrimarySelection != null)
          return this.PrimarySelection.CanDelete;
        return false;
      }
    }

    public TextEditProxy TextEditProxy
    {
      get
      {
        return this.textEditProxy;
      }
      set
      {
        if (this.textEditProxy != null)
          this.textEditProxy.EditingElement.SelectionChanged -= new RoutedEventHandler(this.editingElement_SelectionChanged);
        this.textEditProxy = value;
        if (this.textEditProxy != null)
        {
          this.SetSelection(this.textEditProxy.TextSelection);
          this.textEditProxy.EditingElement.SelectionChanged += new RoutedEventHandler(this.editingElement_SelectionChanged);
        }
        else
          this.Clear();
      }
    }

    public TextSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new TextSelectionSet.TextSelectionNamingHelper(), (SelectionSet<DesignTimeTextSelection, OrderedList<DesignTimeTextSelection>>.IStorageProvider) new BasicSelectionSetStorageProvider<DesignTimeTextSelection>((IComparer<DesignTimeTextSelection>) new TextSelectionComparer()))
    {
    }

    private void editingElement_SelectionChanged(object sender, RoutedEventArgs e)
    {
      this.Rebroadcast();
    }

    private class TextSelectionNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitText;
        }
      }

      public string GetUndoString(object obj)
      {
        return ((DesignTimeTextSelection) obj).ToString();
      }
    }
  }
}
