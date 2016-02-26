// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AddDataGridColumnCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class AddDataGridColumnCommand : ICommand
  {
    public IType ColumnType { get; private set; }

    public DataGridElement GridElement { get; private set; }

    public string DisplayName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AddDataGridColumnCommandText, new object[1]
        {
          (object) this.ColumnType.Name
        });
      }
    }

    public event EventHandler CanExecuteChanged;

    public AddDataGridColumnCommand(DataGridElement gridElement, IType columnType)
    {
      this.GridElement = gridElement;
      this.ColumnType = columnType;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      using (SceneEditTransaction editTransaction = this.GridElement.ViewModel.CreateEditTransaction(this.DisplayName, false))
      {
        SceneNode sceneNode = this.GridElement.ViewModel.GetSceneNode((DocumentNode) this.GridElement.DocumentContext.CreateNode((ITypeId) this.ColumnType));
        this.GridElement.ColumnCollection.Add(sceneNode);
        this.GridElement.ViewModel.ElementSelectionSet.Clear();
        this.GridElement.ViewModel.SelectNodes((ICollection<SceneNode>) new SceneNode[1]
        {
          sceneNode
        });
        editTransaction.Commit();
      }
    }

    protected void OnCanExecuteChanged()
    {
      if (this.CanExecuteChanged == null)
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }
  }
}
