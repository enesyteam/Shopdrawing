// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrObjectDialogBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal abstract class ClrObjectDialogBase : Dialog, INotifyPropertyChanged, ITreeViewItemProvider<FilteredTreeItem>
  {
    private static readonly string showAllAssembliesProperty = "ShowAllAssemblies";
    private bool showAllAssemblies = true;
    private List<AssemblyItem> hiddenAssemblies = new List<AssemblyItem>();
    private IConfigurationObject configuration;
    private Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<TypeItem> selectionContext;
    protected string filter;
    private List<AssemblyItem> assemblies;
    private FilteredTreeItem root;
    private VirtualizingTreeItemFlattener<FilteredTreeItem> flattener;

    protected Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<TypeItem> SelectionContext
    {
      get
      {
        return this.selectionContext;
      }
    }

    public FilteredTreeItem RootItem
    {
      get
      {
        return this.root;
      }
    }

    public ICommand ConfirmSelectionCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnConfirmSelectionCommand));
      }
    }

    public ICommand ClearFilterStringCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnClearFilterString));
      }
    }

    public IList<Type> Types
    {
      get
      {
        return this.GetTypesFromAssemblies((Func<AssemblyItem, bool>) (item => true));
      }
    }

    public IList<Type> CustomTypes
    {
      get
      {
        return this.GetTypesFromAssemblies((Func<AssemblyItem, bool>) (item => item.IsProjectAssembly));
      }
    }

    public IList<Type> SystemTypes
    {
      get
      {
        return this.GetTypesFromAssemblies((Func<AssemblyItem, bool>) (item => !item.IsProjectAssembly));
      }
    }

    public string FilterString
    {
      get
      {
        return this.filter;
      }
      set
      {
        if (!(this.filter != value))
          return;
        this.filter = value;
        this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (arg =>
        {
          foreach (AssemblyItem assemblyItem in this.assemblies)
            assemblyItem.ApplyFilterString(this.filter);
          this.flattener.RebuildList();
          return (object) null;
        }), (object) null);
        this.OnPropertyChanged("FilterString");
      }
    }

    public virtual string HeadingText
    {
      get
      {
        return StringTable.ClrObjectDialogDefaultHeading;
      }
    }

    public bool ShowAllAssemblies
    {
      get
      {
        return this.showAllAssemblies;
      }
      set
      {
        if (this.showAllAssemblies == value)
          return;
        this.showAllAssemblies = value;
        if (this.showAllAssemblies)
        {
          foreach (FilteredTreeItem child in this.hiddenAssemblies)
            this.root.AddChild(child);
          this.hiddenAssemblies.Clear();
        }
        else
        {
          foreach (AssemblyItem assemblyItem in (Collection<FilteredTreeItem>) this.root.Children)
          {
            if (assemblyItem.Assembly != (Assembly) null && !assemblyItem.IsProjectAssembly)
              this.hiddenAssemblies.Add(assemblyItem);
          }
          foreach (FilteredTreeItem child in this.hiddenAssemblies)
            this.root.RemoveChild(child);
        }
        this.OnPropertyChanged("ShowAllAssemblies");
      }
    }

    public ReadOnlyObservableCollection<FilteredTreeItem> ItemList
    {
      get
      {
        return this.flattener.ItemList;
      }
    }

    public Type ObjectType
    {
      get
      {
        if (this.SelectionContext.Count == 0)
          return (Type) null;
        return this.SelectionContext.PrimarySelection.Type;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected ClrObjectDialogBase()
    {
      this.root = (FilteredTreeItem) new ProjectAssembliesItem();
      this.root.IsExpanded = true;
      this.flattener = new VirtualizingTreeItemFlattener<FilteredTreeItem>((ITreeViewItemProvider<FilteredTreeItem>) this);
      this.selectionContext = (Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectionContext<TypeItem>) new SingleSelectionContext<TypeItem>();
      this.selectionContext.PropertyChanged += new PropertyChangedEventHandler(this.SelectionContextPropertyChanged);
      this.SizeToContent = SizeToContent.Width;
      this.MinHeight = 600.0;
      this.MinWidth = 400.0;
      this.Height = 600.0;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private IList<Type> GetTypesFromAssemblies(Func<AssemblyItem, bool> assemblyValidationTest)
    {
      List<Type> list = new List<Type>();
      foreach (AssemblyItem assemblyItem in this.assemblies)
      {
        if (assemblyValidationTest(assemblyItem))
          list.AddRange((IEnumerable<Type>) assemblyItem.Types);
      }
      return (IList<Type>) list;
    }

    private void OnConfirmSelectionCommand()
    {
      this.Close(new bool?(true));
    }

    private void OnClearFilterString()
    {
      this.FilterString = (string) null;
    }

    protected virtual void SelectionContextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "PrimarySelection"))
        return;
      this.OnPropertyChanged("ObjectType");
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      this.SaveSettings();
      base.OnClosing(e);
    }

    private void LoadSettings(DesignerContext designerContext)
    {
      this.configuration = designerContext.Services.GetService<IConfigurationService>()["NewObjectDialog"];
      this.ShowAllAssemblies = (bool) this.configuration.GetProperty(ClrObjectDialogBase.showAllAssembliesProperty, (object) false);
    }

    private void SaveSettings()
    {
      if (this.configuration == null)
        return;
      this.configuration.SetProperty(ClrObjectDialogBase.showAllAssembliesProperty, (object) (bool) (this.ShowAllAssemblies ? true : false));
    }

    protected void Initialize(DesignerContext designerContext)
    {
      this.assemblies = new List<AssemblyItem>();
      foreach (IAssembly assembly in (IEnumerable<IAssembly>) designerContext.ActiveDocument.AssemblyReferences)
      {
        if (assembly.IsLoaded && !assembly.IsResolvedImplicitAssembly)
        {
          IPlatformTypes metadata = designerContext.ActiveDocument.ProjectContext.Platform.Metadata;
          Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
          Assembly referenceAssembly = (Assembly) null;
          if (metadata.RuntimeContext != null && metadata.ReferenceContext != null && metadata.RuntimeContext.ResolveRuntimeAssembly(AssemblyHelper.GetAssemblyName(reflectionAssembly)) != (Assembly) null)
            referenceAssembly = metadata.ReferenceContext.ResolveReferenceAssembly(reflectionAssembly);
          AssemblyItem assemblyModel = this.CreateAssemblyModel(reflectionAssembly, referenceAssembly);
          assemblyModel.ApplyFilterString((string) null);
          this.root.AddChild((FilteredTreeItem) assemblyModel);
          this.assemblies.Add(assemblyModel);
        }
      }
      this.LoadSettings(designerContext);
    }

    protected abstract AssemblyItem CreateAssemblyModel(Assembly runtimeAssembly, Assembly referenceAssembly);
  }
}
