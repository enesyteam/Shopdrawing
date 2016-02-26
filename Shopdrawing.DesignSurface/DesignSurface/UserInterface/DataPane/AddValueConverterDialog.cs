// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.AddValueConverterDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class AddValueConverterDialog : ClrObjectDialogBase
  {
    private string valueConverterResourceKey;
    private SceneViewModel viewModel;

    public string ValueConverterResourceKey
    {
      get
      {
        return this.valueConverterResourceKey;
      }
      set
      {
        this.valueConverterResourceKey = value;
        this.OnPropertyChanged("ValueConverterResourceKey");
      }
    }

    private AddValueConverterDialog(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.SelectionContext.PropertyChanged += new PropertyChangedEventHandler(((ClrObjectDialogBase) this).SelectionContextPropertyChanged);
      FrameworkElement element = FileTable.GetElement("Resources\\DataPane\\AddValueConverterDialog.xaml");
      element.DataContext = (object) this;
      this.DialogContent = (UIElement) element;
      this.ResizeMode = ResizeMode.CanResizeWithGrip;
      this.SizeToContent = SizeToContent.Manual;
      this.Width = 400.0;
      this.Height = 600.0;
      this.Title = StringTable.AddValueConverterDialogTitle;
      this.Initialize(viewModel.DesignerContext);
    }

    protected override AssemblyItem CreateAssemblyModel(Assembly runtimeAssembly, Assembly referenceAssembly)
    {
      return (AssemblyItem) new ClrAssemblyValueConverterModel(this.SelectionContext, this.viewModel, runtimeAssembly, referenceAssembly);
    }

    protected override void SelectionContextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "PrimarySelection"))
        return;
      this.ValueConverterResourceKey = this.ObjectType.Name;
      this.OnPropertyChanged("ObjectType");
    }

    public static SceneNode CreateValueConverter(SceneViewModel viewModel)
    {
      AddValueConverterDialog valueConverterDialog = new AddValueConverterDialog(viewModel);
      bool? nullable = valueConverterDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0)
      {
        Type objectType = valueConverterDialog.ObjectType;
        string name = valueConverterDialog.ValueConverterResourceKey;
        if (objectType != (Type) null)
        {
          viewModel.ProjectContext.GetType(objectType);
          SceneNode sceneNode = viewModel.CreateSceneNode(objectType);
          SceneNode rootNode = viewModel.RootNode;
          if (rootNode != null)
          {
            Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureResourceDictionaryNode(rootNode);
            ResourceDictionaryNode resourceDictionaryNode = ResourceManager.ProvideResourcesForElement(rootNode);
            if (resourceDictionaryNode != null)
            {
              if (string.IsNullOrEmpty(name))
                name = objectType.Name;
              string uniqueResourceKey = resourceDictionaryNode.GetUniqueResourceKey(name);
              DictionaryEntryNode dictionaryEntryNode = DictionaryEntryNode.Factory.Instantiate((object) uniqueResourceKey, sceneNode);
              resourceDictionaryNode.Insert(0, dictionaryEntryNode);
              return (SceneNode) dictionaryEntryNode;
            }
          }
        }
      }
      return (SceneNode) null;
    }
  }
}
