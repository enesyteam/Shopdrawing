// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrObjectDataSourceDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class ClrObjectDataSourceDialog : ObjectDataSourceDialog
  {
    public static string DefaultClrObjectDataSourceName = "ObjectDataSource";

    public override string DataSourceNameSuffix
    {
      get
      {
        return "DataSource";
      }
    }

    private ClrObjectDataSourceDialog(DataPanelModel model)
      : base(model, ClrObjectDataSourceDialog.DefaultClrObjectDataSourceName)
    {
      this.Title = StringTable.AddClrObjectDataSourceDialogTitle;
    }

    protected override AssemblyItem CreateAssemblyModel(Assembly runtimeAssembly, Assembly referenceAssembly)
    {
      return (AssemblyItem) new ClrAssemblyDataSourceModel(this.SelectionContext, this.Model, runtimeAssembly, referenceAssembly, true, true, false);
    }

    public static SceneNode CreateClrObjectDataSource(out string dataSourceName, out Type dataSourceType, DataPanelModel model)
    {
      SceneNode sceneNode = (SceneNode) null;
      dataSourceName = (string) null;
      dataSourceType = (Type) null;
      ClrObjectDataSourceDialog dataSourceDialog = new ClrObjectDataSourceDialog(model);
      bool? nullable = dataSourceDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0)
      {
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.AddClrDataSource);
        dataSourceType = dataSourceDialog.ObjectType;
        dataSourceName = dataSourceDialog.DataSourceName;
        if (dataSourceDialog.ObjectType != (Type) null)
        {
          dataSourceName = dataSourceName.Trim();
          SceneViewModel activeSceneViewModel = model.DesignerContext.ActiveSceneViewModel;
          DocumentNode node = (DocumentNode) activeSceneViewModel.Document.DocumentContext.CreateNode(dataSourceDialog.ObjectType);
          sceneNode = activeSceneViewModel.GetSceneNode(node);
        }
      }
      return sceneNode;
    }
  }
}
