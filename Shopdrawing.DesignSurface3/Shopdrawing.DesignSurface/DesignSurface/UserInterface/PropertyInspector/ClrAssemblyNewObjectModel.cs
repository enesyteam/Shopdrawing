// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ClrAssemblyNewObjectModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ClrAssemblyNewObjectModel : AssemblyItem
  {
    private Type propertyType;
    private SelectionContext<TypeItem> selectionContext;

    public ClrAssemblyNewObjectModel(SelectionContext<TypeItem> selectionContext, SceneViewModel viewModel, Assembly runtimeAssembly, Assembly referenceAssembly, Type propertyType)
      : base(viewModel, runtimeAssembly, referenceAssembly)
    {
      this.selectionContext = selectionContext;
      this.propertyType = propertyType;
      this.LoadAssembly();
    }

    protected override void ProcessType(Type type)
    {
      if (!this.propertyType.IsAssignableFrom(type) || type.IsNested || (typeof (Application).IsAssignableFrom(type) || this.ViewModel.RootNode == null) || !(this.ViewModel.RootNode.TrueTargetType != type))
        return;
      this.AddType(type, this.selectionContext);
    }

    protected override void OnAssemblyExpanded()
    {
    }
  }
}
