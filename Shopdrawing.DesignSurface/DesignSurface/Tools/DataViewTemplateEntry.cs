// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataViewTemplateEntry
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class DataViewTemplateEntry
  {
    public ITypeId DataType { get; set; }

    public DocumentCompositeNode LabelNode { get; set; }

    public IProperty LabelValueProperty { get; set; }

    public DocumentCompositeNode FieldNode { get; set; }

    public IProperty FieldValueProperty { get; set; }

    public DataViewTemplateEntry(ITypeId dataType)
    {
      this.DataType = dataType;
    }
  }
}
