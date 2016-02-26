// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SchemaItemBasicTemplateSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class SchemaItemBasicTemplateSet
  {
    public DataTemplate CompositeTemplate { get; set; }

    public DataTemplate CollectionTemplate { get; set; }

    public DataTemplate BasicTemplate { get; set; }

    public virtual DataTemplate GetTemplate(DataSchemaItem dataSchemaItem)
    {
      return !dataSchemaItem.DataSchemaNode.IsCollection ? (!dataSchemaItem.HasChildren ? (!(SampleDataSet.SampleDataTypeFromType(dataSchemaItem.DataSchemaNode.Type) is SampleCompositeType) ? this.BasicTemplate : this.CompositeTemplate) : this.CompositeTemplate) : this.CollectionTemplate;
    }
  }
}
