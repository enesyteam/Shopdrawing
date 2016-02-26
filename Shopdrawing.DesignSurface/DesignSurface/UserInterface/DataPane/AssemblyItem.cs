// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.AssemblyItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class AssemblyItem : FilteredTreeItem, INotifyPropertyChanged
  {
    private Dictionary<string, NamespaceItem> namespaces = new Dictionary<string, NamespaceItem>();
    private bool hideSystemTypesWithoutDefaultConstructor = true;
    private bool hideCustomTypesWithoutDefaultConstructor = true;
    private const int MaxLoaderExceptions = 5;
    private const int MaxTypesForAutoExpand = 9;
    private Assembly runtimeAssembly;
    private Assembly referenceAssembly;
    private List<Type> types;
    private string name;
    private SceneViewModel viewModel;

    protected bool HideSystemTypesWithoutDefaultConstructor
    {
      get
      {
        return this.hideSystemTypesWithoutDefaultConstructor;
      }
      set
      {
        this.hideSystemTypesWithoutDefaultConstructor = value;
      }
    }

    protected bool HideCustomTypesWithoutDefaultConstructor
    {
      get
      {
        return this.hideCustomTypesWithoutDefaultConstructor;
      }
      set
      {
        this.hideCustomTypesWithoutDefaultConstructor = value;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.name;
      }
      set
      {
      }
    }

    public List<Type> Types
    {
      get
      {
        return this.types;
      }
    }

    public override bool MatchesFilter
    {
      get
      {
        if (this.Filter != null)
          return this.Filter((FilteredTreeItem) this);
        return false;
      }
    }

    public override bool IsVisible
    {
      get
      {
        if (this.namespaces.Values.Count == 0)
          return false;
        foreach (VirtualizingTreeItem<FilteredTreeItem> virtualizingTreeItem in this.namespaces.Values)
        {
          if (virtualizingTreeItem.IsVisible)
            return true;
        }
        return this.Filter != null && this.MatchesFilter;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string UniqueId
    {
      get
      {
        return this.name;
      }
    }

    public bool IsProjectAssembly { get; private set; }

    public Assembly Assembly
    {
      get
      {
        return this.runtimeAssembly;
      }
    }

    protected SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    protected AssemblyItem(SceneViewModel viewModel, Assembly runtimeAssembly, Assembly referenceAssembly)
    {
      this.viewModel = viewModel;
      this.runtimeAssembly = runtimeAssembly;
      this.referenceAssembly = referenceAssembly;
      this.name = AssemblyHelper.GetAssemblyName(runtimeAssembly).Name;
      this.types = new List<Type>();
      this.IsProjectAssembly = this.IsAssemblyProjectOrReferencedProject();
    }

    protected abstract void ProcessType(Type type);

    protected void AddType(Type type, SelectionContext<TypeItem> selectionContext)
    {
      this.Types.Add(type);
      NamespaceItem owningNamespace = (NamespaceItem) null;
      string index = type.Namespace ?? StringTable.ClrObjectDialogDefaultNamespace;
      if (!this.namespaces.TryGetValue(index, out owningNamespace))
      {
        owningNamespace = new NamespaceItem(AssemblyHelper.GetAssemblyName(type.Assembly).Name, index, this);
        this.namespaces[index] = owningNamespace;
        this.AddChild((FilteredTreeItem) owningNamespace);
      }
      owningNamespace.AddChild((FilteredTreeItem) new TypeItem(type, selectionContext, owningNamespace));
    }

    protected void SetExpandedState()
    {
      int num = 0;
      bool flag = false;
      foreach (NamespaceItem namespaceItem in this.namespaces.Values)
        num += namespaceItem.VisibleChildrenCount;
      foreach (NamespaceItem namespaceItem in this.namespaces.Values)
      {
        if (namespaceItem.IsForcedVisible)
          namespaceItem.IsExpanded = true;
        else if (num <= 9)
          namespaceItem.IsExpanded = namespaceItem.VisibleChildrenCount <= 6 && namespaceItem.VisibleChildrenCount > 0;
        else
          namespaceItem.IsExpanded = false;
        flag = flag || namespaceItem.IsExpanded;
      }
      this.IsExpanded = flag;
    }

    public override int CompareTo(FilteredTreeItem treeItem)
    {
      return this.DisplayName.CompareTo(treeItem.DisplayName);
    }

    protected abstract void OnAssemblyExpanded();

    protected void LoadAssembly()
    {
      IDictionary<string, Type> dictionary = (IDictionary<string, Type>) null;
      Type[] typeArray;
      try
      {
        if (this.referenceAssembly != (Assembly) null)
        {
          dictionary = (IDictionary<string, Type>) new Dictionary<string, Type>();
          foreach (Type type in this.referenceAssembly.GetTypes())
            dictionary.Add(type.FullName, type);
        }
        typeArray = this.runtimeAssembly.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ClrDataSourceAssemblyLoadError, new object[1]
        {
          (object) AssemblyHelper.GetAssemblyName(this.runtimeAssembly).Name
        });
        for (int index = 0; index < Math.Min(5, ex.LoaderExceptions.Length); ++index)
          message = message + Environment.NewLine + ex.LoaderExceptions[index].Message;
        if (ex.LoaderExceptions.Length > 5)
          message = message + Environment.NewLine + "...";
        this.viewModel.DesignerContext.MessageDisplayService.ShowError(message);
        typeArray = Type.EmptyTypes;
      }
      bool supportInternal = this.viewModel.ProjectContext.ProjectAssembly != null && this.viewModel.ProjectContext.ProjectAssembly.CompareTo(this.runtimeAssembly);
      string message1 = StringTable.ClrDataSourceTypeLoadError;
      int num = 0;
      foreach (Type type in typeArray)
      {
        if (dictionary != null)
        {
          if (!dictionary.ContainsKey(type.FullName))
            continue;
        }
        try
        {
          if (!TypeUtilities.HasDefaultConstructor(type, supportInternal) && !(type == typeof (string)) && (this.hideSystemTypesWithoutDefaultConstructor || !AssemblyHelper.IsSystemAssembly(this.runtimeAssembly)))
          {
            if (!this.hideCustomTypesWithoutDefaultConstructor)
            {
              if (AssemblyHelper.IsSystemAssembly(this.runtimeAssembly))
                continue;
            }
            else
              continue;
          }
          this.ProcessType(type);
        }
        catch (FileNotFoundException ex)
        {
          if (num < 5)
            message1 = message1 + Environment.NewLine + string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ClrDataSourceTypeLoadErrorMessage, new object[2]
            {
              (object) type.Name,
              (object) ex.Message
            });
          else if (num == 5)
            message1 = message1 + Environment.NewLine + "...";
          ++num;
        }
      }
      if (num <= 0)
        return;
      this.viewModel.DesignerContext.MessageDisplayService.ShowError(message1);
    }

    public void ApplyFilterString(string filter)
    {
      if (filter == null)
        filter = "";
      string upperFilterString = filter.ToUpper(CultureInfo.CurrentCulture);
      Predicate<FilteredTreeItem> partialMatchPredicate = (Predicate<FilteredTreeItem>) (item => item.FullName.ToUpper(CultureInfo.CurrentCulture).Contains(upperFilterString));
      Predicate<FilteredTreeItem> predicate = (Predicate<FilteredTreeItem>) (item => item.DisplayName.ToUpper(CultureInfo.CurrentCulture).Equals(upperFilterString));
      foreach (NamespaceItem namespaceItem in this.namespaces.Values)
      {
        namespaceItem.Filter = partialMatchPredicate;
        foreach (FilteredTreeItem filteredTreeItem in (Collection<FilteredTreeItem>) namespaceItem.Children)
        {
          TypeItem typeItem = filteredTreeItem as TypeItem;
          if (typeItem != null)
          {
            typeItem.ForceVisibleFilter = predicate;
            typeItem.Filter = partialMatchPredicate;
          }
        }
      }
      this.Filter = (Predicate<FilteredTreeItem>) (treeItem =>
      {
        if (filter.Length > 0)
          return partialMatchPredicate(treeItem);
        return false;
      });
    }

    public override void OnFilterChanged()
    {
      this.SetExpandedState();
    }

    private bool IsAssemblyProjectOrReferencedProject()
    {
      IProject project1 = (IProject) this.viewModel.ProjectContext.GetService(typeof (IProject));
      if (project1.TargetAssembly != null && this.runtimeAssembly == project1.TargetAssembly.RuntimeAssembly)
        return true;
      foreach (IProject project2 in project1.ReferencedProjects)
      {
        if (project2.TargetAssembly != null && this.runtimeAssembly == project2.TargetAssembly.RuntimeAssembly)
          return true;
      }
      return false;
    }
  }
}
