// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AddDataGridColumnsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class AddDataGridColumnsCommand : SingleTargetDynamicMenuCommandBase
  {
    private static List<List<WeakReference>> resolvedColumnTypes = new List<List<WeakReference>>();

    public override IEnumerable Items
    {
      get
      {
        DataGridElement gridElement = this.TargetElement as DataGridElement;
        if (gridElement == null)
          return (IEnumerable) new List<MenuItem>();
        IEnumerable<IType> gridColumnTypes = AddDataGridColumnsCommand.GetGridColumnTypes(gridElement);
        List<MenuItem> list = new List<MenuItem>();
        foreach (IType columnType in gridColumnTypes)
        {
          AddDataGridColumnCommand gridColumnCommand = new AddDataGridColumnCommand(gridElement, columnType);
          MenuItem menuItem = this.CreateMenuItem(gridColumnCommand.DisplayName, "Add_" + columnType.Name, (ICommand) gridColumnCommand);
          list.Add(menuItem);
        }
        return (IEnumerable) list;
      }
    }

    public AddDataGridColumnsCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    private static IEnumerable<IType> GetGridColumnTypes(DataGridElement gridElement)
    {
      IType columnBaseType = gridElement.ColumnBaseType;
      AddDataGridColumnsCommand.resolvedColumnTypes.RemoveAll((Predicate<List<WeakReference>>) (reaOnlyList => !reaOnlyList[0].IsAlive));
      int index = AddDataGridColumnsCommand.resolvedColumnTypes.FindIndex((Predicate<List<WeakReference>>) (reaOnlyList => reaOnlyList[0].Target == columnBaseType));
      List<WeakReference> list = (List<WeakReference>) null;
      if (index >= 0)
      {
        list = AddDataGridColumnsCommand.resolvedColumnTypes[index];
        if (list.Find((Predicate<WeakReference>) (r => !r.IsAlive)) != null)
        {
          AddDataGridColumnsCommand.resolvedColumnTypes.RemoveAt(index);
          list = (List<WeakReference>) null;
        }
      }
      if (list == null)
      {
        list = new List<WeakReference>();
        foreach (Type type1 in AssemblyHelper.GetTypes(columnBaseType.RuntimeAssembly))
        {
          if (columnBaseType.RuntimeType.IsAssignableFrom(type1) && !type1.IsAbstract && (!type1.IsGenericType && !type1.IsNotPublic))
          {
            IType type2 = gridElement.DocumentNode.TypeResolver.GetType(type1);
            list.Add(new WeakReference((object) type2));
          }
        }
        list.Sort((Comparison<WeakReference>) ((a, b) =>
        {
          string name1 = ((IMemberId) a.Target).Name;
          string name2 = ((IMemberId) b.Target).Name;
          return StringLogicalComparer.Instance.Compare(name1, name2);
        }));
        list.Insert(0, new WeakReference((object) columnBaseType));
        AddDataGridColumnsCommand.resolvedColumnTypes.Add(list);
      }
      return Enumerable.Select<WeakReference, IType>(Enumerable.Skip<WeakReference>((IEnumerable<WeakReference>) list, 1), (Func<WeakReference, IType>) (r => (IType) r.Target));
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "Text")
        return (object) StringTable.AddDataGridColumn;
      return base.GetProperty(propertyName);
    }
  }
}
