// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.PaletteRegistry
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Workspaces.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class PaletteRegistry
  {
    public static readonly DependencyProperty PaletteHeaderContentProperty = DependencyProperty.RegisterAttached("PaletteHeaderContent", typeof (object), typeof (PaletteRegistry));
    private List<PaletteRegistryEntry> paletteRegistryEntries = new List<PaletteRegistryEntry>();

    public IWindowService WindowService { get; private set; }

    public IWorkspaceService WorkspaceService { get; private set; }

    public IList<PaletteRegistryEntry> PaletteRegistryEntries
    {
      get
      {
        return (IList<PaletteRegistryEntry>) new ReadOnlyCollection<PaletteRegistryEntry>((IList<PaletteRegistryEntry>) this.paletteRegistryEntries);
      }
    }

    public PaletteRegistryEntry this[string name]
    {
      get
      {
        for (int index = 0; index < this.paletteRegistryEntries.Count; ++index)
        {
          if (name == this.paletteRegistryEntries[index].Name)
            return this.paletteRegistryEntries[index];
        }
        return (PaletteRegistryEntry) null;
      }
    }

    public PaletteRegistry(IWorkspaceService workspaceService)
    {
      this.WorkspaceService = workspaceService;
    }

    public PaletteRegistry(IWindowService windowService)
    {
      this.WindowService = windowService;
    }

    public static object GetPaletteHeaderContent(DependencyObject dependencyObject)
    {
      return dependencyObject.GetValue(PaletteRegistry.PaletteHeaderContentProperty);
    }

    public static void SetPaletteHeaderContent(DependencyObject dependencyObject, object value)
    {
      dependencyObject.SetValue(PaletteRegistry.PaletteHeaderContentProperty, value);
    }

    public PaletteRegistryEntry Add(string name, FrameworkElement content, string caption, KeyBinding keyBinding)
    {
      return this.Add(name, content, caption, keyBinding, new ExpressionViewProperties(true));
    }

    public PaletteRegistryEntry Add(string name, FrameworkElement content, string caption, KeyBinding keyBinding, ExpressionViewProperties viewProperties)
    {
      PaletteRegistryEntry paletteRegistryEntry = new PaletteRegistryEntry(this, name, content, caption, keyBinding, viewProperties);
      this.paletteRegistryEntries.Add(paletteRegistryEntry);
      if (this.WorkspaceService != null)
        this.WorkspaceService.AddCommand(paletteRegistryEntry.CommandName, (Microsoft.Expression.Framework.Commands.ICommand) new PaletteRegistry.ShowPaletteCommand(paletteRegistryEntry));
      this.paletteRegistryEntries.Sort(new Comparison<PaletteRegistryEntry>(this.ComparePaletteRegistryEntries));
      return paletteRegistryEntry;
    }

    public void Remove(string name)
    {
      PaletteRegistryEntry paletteRegistryEntry = this[name];
      if (paletteRegistryEntry == null)
        return;
      this.paletteRegistryEntries.Remove(paletteRegistryEntry);
      this.WorkspaceService.RemoveCommand(paletteRegistryEntry.CommandName);
    }

    public string[] GetCommandNames()
    {
      string[] strArray = new string[this.paletteRegistryEntries.Count];
      for (int index = 0; index < this.paletteRegistryEntries.Count; ++index)
        strArray[index] = this.paletteRegistryEntries[index].CommandName;
      return strArray;
    }

    private int ComparePaletteRegistryEntries(PaletteRegistryEntry x, PaletteRegistryEntry y)
    {
      return string.Compare(x.Caption, y.Caption, StringComparison.CurrentCultureIgnoreCase);
    }

    private class ShowPaletteCommand : CheckCommandBase
    {
      private PaletteRegistryEntry paletteRegistryEntry;

      public ShowPaletteCommand(PaletteRegistryEntry paletteRegistryEntry)
      {
        this.paletteRegistryEntry = paletteRegistryEntry;
      }

      protected override void OnCheckedChanged(bool isChecked)
      {
        this.paletteRegistryEntry.IsVisibleAndSelected = isChecked;
      }

      protected override bool IsChecked()
      {
        return this.paletteRegistryEntry.IsVisible;
      }

      public override object GetProperty(string propertyName)
      {
          string str = propertyName;
          string str1 = str;
          if (str != null)
          {
              if (str1 == "Text")
              {
                  return this.paletteRegistryEntry.MenuText;
              }
              if (str1 == "IsVisible")
              {
                  return !this.paletteRegistryEntry.IsForcedInvisible;
              }
          }
          return base.GetProperty(propertyName);
      }
    }
  }
}
