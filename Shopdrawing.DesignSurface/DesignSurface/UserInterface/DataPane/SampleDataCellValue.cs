// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataCellValue
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataCellValue : NotifyingObject
  {
    public SampleDataRow Row { get; private set; }

    public DocumentCompositeNode RowNode
    {
      get
      {
        return this.Row.RowNode;
      }
    }

    public SampleDataProperty Property { get; private set; }

    public object Value
    {
      get
      {
        object obj = (object) null;
        DocumentPrimitiveNode documentPrimitiveNode = this.RowNode.Properties[(IPropertyId) this.Property.SampleProperty] as DocumentPrimitiveNode;
        SampleBasicType sampleType = this.Property.SampleType;
        if (sampleType == SampleBasicType.Boolean)
          obj = (object) (bool) (documentPrimitiveNode != null ? (documentPrimitiveNode.GetValue<bool>() ? true : false) : 0);
        else if (sampleType == SampleBasicType.Number)
          obj = (object) (documentPrimitiveNode != null ? documentPrimitiveNode.GetValue<double>() : double.NaN);
        else if (sampleType == SampleBasicType.Image)
        {
          string fromRelativePath = this.Property.SampleDataSet.GetAbsoluteAssetFilePathFromRelativePath(documentPrimitiveNode != null ? documentPrimitiveNode.GetValue<string>() : string.Empty);
          if (!string.IsNullOrEmpty(fromRelativePath))
            obj = (object) this.Property.Model.GetImage(fromRelativePath);
        }
        else
          obj = documentPrimitiveNode != null ? (object) documentPrimitiveNode.GetValue<string>() : (object) string.Empty;
        return obj;
      }
      set
      {
        DocumentNode other = this.RowNode.Properties[(IPropertyId) this.Property.SampleProperty];
        DocumentNode documentNode = (DocumentNode) null;
        if (value != null)
        {
          string str = value as string;
          if (this.Property.SampleType == SampleBasicType.Image)
          {
            DocumentPrimitiveNode documentPrimitiveNode = other as DocumentPrimitiveNode;
            this.Property.SampleDataSet.UnuseAssetFile(documentPrimitiveNode != null ? documentPrimitiveNode.GetValue<string>() : (string) null);
          }
          else if (str == null)
            str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
          else if (this.Property.SampleType != SampleBasicType.String)
          {
            try
            {
              TypeConverter typeConverter = MetadataStore.GetTypeConverter(this.Property.SampleDataSet.ProjectContext.ResolveType(this.Property.SampleType.TypeId).RuntimeType);
              str = Convert.ToString(!(typeConverter.GetType() == typeof (DoubleConverter)) ? typeConverter.ConvertFrom((ITypeDescriptorContext) null, CultureInfo.CurrentCulture, (object) str) : (object) Convert.ToDouble(str, (IFormatProvider) CultureInfo.CurrentCulture), (IFormatProvider) CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
              return;
            }
          }
          documentNode = (DocumentNode) this.Property.Model.ValueBuilder.CreateBasicNode(this.Property.SampleType, str);
        }
        if (documentNode != null && other != null && documentNode.Equals(other))
          return;
        this.Row.Model.SetModified();
        if (documentNode == null)
          this.RowNode.ClearValue((IPropertyId) this.Property.SampleProperty);
        else
          this.RowNode.Properties[(IPropertyId) this.Property.SampleProperty] = documentNode;
        this.OnPropertyChanged("Value");
      }
    }

    public SampleDataCellValue(SampleDataRow row, SampleDataProperty property)
    {
      this.Row = row;
      this.Property = property;
    }

    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
