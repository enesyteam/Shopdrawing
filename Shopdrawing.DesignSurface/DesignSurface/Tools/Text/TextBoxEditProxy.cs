// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.TextBoxEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class TextBoxEditProxy : TextBoxEditProxyBase
  {
    public TextBoxEditProxy(BaseFrameworkElement textSource)
      : base(textSource)
    {
    }

    public override void Instantiate()
    {
      base.Instantiate();
      TextBoxElement textBoxElement = (TextBoxElement) this.TextSource;
      this.TextBox.Text = textBoxElement.Text;
      IPropertyId propertyKey = (IPropertyId) textBoxElement.ProjectContext.ResolveProperty(TextBoxElement.TextWrappingProperty);
      if (propertyKey != null)
        this.TextBox.TextWrapping = (TextWrapping) textBoxElement.GetComputedValue(propertyKey);
      if (this.TextSource.ProjectContext.ResolveProperty(TextBoxElement.CaretBrushProperty) != null)
        this.CopyProperty(TextBoxElement.CaretBrushProperty);
      if (this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return;
      Thickness padding = this.TextBox.Padding;
      padding.Left -= 2.0;
      padding.Right -= 2.0;
      padding.Bottom -= 3.0;
      this.TextBox.Padding = padding;
    }

    public override void UpdateDocumentModel()
    {
      using (this.TextSource.ViewModel.AnimationEditor.DeferKeyFraming())
      {
        using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
        {
          this.TextSource.SetValue(TextBoxElement.TextProperty, (object) this.Text);
          editTransaction.Commit();
        }
      }
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      if (change.PropertyKey == null || !change.PropertyKey.Equals((object) TextBoxElement.TextProperty))
        return base.IsTextChange(change);
      return true;
    }
  }
}
