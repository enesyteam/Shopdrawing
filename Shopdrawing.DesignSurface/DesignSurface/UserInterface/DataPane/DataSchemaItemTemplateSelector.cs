// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSchemaItemTemplateSelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class DataSchemaItemTemplateSelector : DataTemplateSelector
  {
    public SampleDataTemplateSet SampleData { get; set; }

    public SchemaItemBasicTemplateSet DesignData { get; set; }

    public SchemaItemBasicTemplateSet LiveData { get; set; }

    public DataSchemaItemTemplateSelector()
    {
      this.SampleData = new SampleDataTemplateSet();
      this.DesignData = new SchemaItemBasicTemplateSet();
      this.LiveData = new SchemaItemBasicTemplateSet();
    }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      DataSchemaItem dataSchemaItem = item as DataSchemaItem;
      if (dataSchemaItem == null)
        return (DataTemplate) null;
      DataSourceNode dataSourceNode = dataSchemaItem.DataSourceNode;
      if (dataSourceNode == null)
        return (DataTemplate) null;
      DataTemplate dataTemplate = (DataTemplate) null;
      if (dataSourceNode.IsDesignData)
        dataTemplate = this.DesignData.GetTemplate(dataSchemaItem);
      else if (dataSourceNode.IsSampleDataSource)
        dataTemplate = this.SampleData.GetTemplate(dataSchemaItem);
      if (dataTemplate == null)
        dataTemplate = this.LiveData.GetTemplate(dataSchemaItem);
      return dataTemplate;
    }
  }
}
