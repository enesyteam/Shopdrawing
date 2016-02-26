// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrAssemblyValueConverterModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ClrAssemblyValueConverterModel : AssemblyItem
  {
    private SelectionContext<TypeItem> selectionContext;

    public ClrAssemblyValueConverterModel(SelectionContext<TypeItem> selectionContext, SceneViewModel viewModel, Assembly runtimeAssembly, Assembly referenceAssembly)
      : base(viewModel, runtimeAssembly, referenceAssembly)
    {
      this.selectionContext = selectionContext;
      this.LoadAssembly();
    }

    protected override void ProcessType(Type type)
    {
      ITypeId type1 = (ITypeId) this.ViewModel.ProjectContext.GetType(type);
      if (!PlatformTypes.IValueConverter.IsAssignableFrom(type1) || type.IsNested)
        return;
      this.AddType(type, this.selectionContext);
    }

    protected override void OnAssemblyExpanded()
    {
    }
  }
}
