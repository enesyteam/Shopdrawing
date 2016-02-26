// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ClrNewObjectDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal sealed class ClrNewObjectDialog : ClrObjectDialogBase
  {
    private Type propertyType;
    private SceneViewModel viewModel;

    private ClrNewObjectDialog(SceneViewModel viewModel, Type propertyType)
    {
      this.viewModel = viewModel;
      this.propertyType = propertyType;
      FrameworkElement element = FileTable.GetElement("Resources\\PropertyInspector\\NewClrObjectDialog.xaml");
      element.DataContext = (object) this;
      this.DialogContent = (UIElement) element;
      this.ResizeMode = ResizeMode.CanResizeWithGrip;
      this.SizeToContent = SizeToContent.Manual;
      this.Width = 400.0;
      this.Height = 600.0;
      this.Title = StringTable.AddClrObjectDialogTitle;
      this.Initialize(viewModel.DesignerContext);
    }

    protected override AssemblyItem CreateAssemblyModel(Assembly runtimeAssembly, Assembly referenceAssembly)
    {
      return (AssemblyItem) new ClrAssemblyNewObjectModel(this.SelectionContext, this.viewModel, runtimeAssembly, referenceAssembly, this.propertyType);
    }

    public static Type CreateNewClrObject(SceneViewModel viewModel, Type propertyType)
    {
      return ClrNewObjectDialog.CreateNewClrObject(viewModel, propertyType, false);
    }

    public static Type CreateNewClrObject(SceneViewModel viewModel, Type propertyType, bool showSystemTypesByDefault)
    {
      ClrNewObjectDialog clrNewObjectDialog = new ClrNewObjectDialog(viewModel, propertyType);
      IList<Type> types = clrNewObjectDialog.Types;
      if (types.Count == 0)
      {
        viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.CollectionEditorCannotAddObjectMessage);
        return (Type) null;
      }
      if (types.Count == 1)
      {
        clrNewObjectDialog.Close();
        return types[0];
      }
      if (showSystemTypesByDefault || clrNewObjectDialog.SystemTypes.Count > 0 && clrNewObjectDialog.CustomTypes.Count == 0)
        clrNewObjectDialog.ShowAllAssemblies = true;
      bool? nullable = clrNewObjectDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0 && clrNewObjectDialog.SelectionContext.PrimarySelection != null)
        return clrNewObjectDialog.SelectionContext.PrimarySelection.Type;
      return (Type) null;
    }
  }
}
