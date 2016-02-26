// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrAssemblyDataSourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ClrAssemblyDataSourceModel : AssemblyItem
  {
    private DataPanelModel model;
    private SelectionContext<TypeItem> selectionContext;
    private bool hideAbstractSystemTypes;

    public ClrAssemblyDataSourceModel(SelectionContext<TypeItem> selectionContext, DataPanelModel model, Assembly runtimeAssembly, Assembly referenceAssembly, bool hideSystemTypesWithoutDefaultConstructor, bool hideCustomTypesWithoutDefaultConstructor, bool hideAbstractSystemTypes)
      : base(model.ViewModel, runtimeAssembly, referenceAssembly)
    {
      this.model = model;
      this.selectionContext = selectionContext;
      this.HideSystemTypesWithoutDefaultConstructor = hideSystemTypesWithoutDefaultConstructor;
      this.HideCustomTypesWithoutDefaultConstructor = hideCustomTypesWithoutDefaultConstructor;
      this.hideAbstractSystemTypes = hideAbstractSystemTypes;
      this.LoadAssembly();
    }

    protected override void ProcessType(Type type)
    {
      if (type.IsGenericType || type.IsGenericTypeDefinition || (type.IsNested || type.IsEnum) || (typeof (Application).IsAssignableFrom(type) || this.model.ViewModel.RootNode == null || !(this.model.ViewModel.RootNode.TrueTargetType != type)) || type.IsAbstract && AssemblyHelper.IsSystemAssembly(this.Assembly) && this.hideAbstractSystemTypes)
        return;
      this.AddType(type, this.selectionContext);
    }

    protected override void OnAssemblyExpanded()
    {
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, (Delegate) new DispatcherOperationCallback(this.RestoreCursor), (object) Mouse.OverrideCursor);
      Mouse.OverrideCursor = Cursors.Wait;
    }

    private object RestoreCursor(object cursor)
    {
      Mouse.OverrideCursor = (Cursor) cursor;
      return (object) null;
    }
  }
}
