// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.FlowDocumentScrollViewerEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class FlowDocumentScrollViewerEditProxy : FlowDocumentEditProxy
  {
    public FlowDocumentScrollViewerEditProxy(BaseFrameworkElement textSource)
      : base(textSource)
    {
    }

    public override void Instantiate()
    {
      base.Instantiate();
      this.CopyProperty(ControlElement.PaddingProperty);
      if (this.ForceLoadOnInstantiate)
      {
        this.EditingElement.UpdateLayout();
        this.AdjustPadding();
      }
      else
        this.RichTextBox.Loaded += new RoutedEventHandler(this.RichTextBox_Loaded);
    }

    private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
    {
      this.RichTextBox.Loaded -= new RoutedEventHandler(this.RichTextBox_Loaded);
      this.AdjustPadding();
    }

    private void AdjustPadding()
    {
      Thickness thickness = this.ComputePageMargin((FlowDocumentScrollViewerElement) this.TextSource);
      thickness = new Thickness(RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Left, -1.0), RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Top, -1.0), RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Right, -1.0), RichTextBoxEditProxyBase.AdjustPaddingByOffset(thickness.Bottom, -1.0));
      this.RichTextBox.Document.PagePadding = thickness;
    }

    private Thickness ComputePageMargin(FlowDocumentScrollViewerElement textSource)
    {
      FlowDocumentElement flowDocumentElement = textSource.FlowDocumentElement;
      Thickness thickness = flowDocumentElement != null ? (Thickness) flowDocumentElement.GetComputedValue(FlowDocumentElement.PagePaddingProperty) : (Thickness) FlowDocument.PagePaddingProperty.DefaultMetadata.DefaultValue;
      double d = flowDocumentElement != null ? (double) flowDocumentElement.GetComputedValue(FlowDocumentElement.LineHeightProperty) : (double) FlowDocument.LineHeightProperty.DefaultMetadata.DefaultValue;
      if (double.IsNaN(d))
        d = (flowDocumentElement != null ? (FontFamily) flowDocumentElement.GetComputedValue(FlowDocumentElement.FontFamilyProperty) : (FontFamily) FlowDocument.FontFamilyProperty.DefaultMetadata.DefaultValue).LineSpacing * (flowDocumentElement != null ? (double) flowDocumentElement.GetComputedValue(FlowDocumentElement.FontSizeProperty) : (double) FlowDocument.FontSizeProperty.DefaultMetadata.DefaultValue);
      if (double.IsNaN(thickness.Left))
        thickness.Left = d;
      if (double.IsNaN(thickness.Right))
        thickness.Right = d;
      if (double.IsNaN(thickness.Top))
        thickness.Top = d;
      if (double.IsNaN(thickness.Bottom))
        thickness.Bottom = d;
      return thickness;
    }
  }
}
