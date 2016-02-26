// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.AccessTextEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class AccessTextEditProxy : TextBoxEditProxyBase
  {
    public AccessTextEditProxy(BaseFrameworkElement textSource)
      : base(textSource)
    {
    }

    public override void Instantiate()
    {
      base.Instantiate();
      object computedValue = this.TextSource.GetComputedValue(AccessTextElement.ForegroundProperty);
      this.TextBox.SetValue(this.ProxyPlatform.Metadata.DefaultTypeResolver, ControlElement.ForegroundProperty, computedValue);
      this.TextBox.SetValue(this.ProxyPlatform.Metadata.DefaultTypeResolver, TextBoxElement.CaretBrushProperty, computedValue);
      this.TextBox.SetValue(this.ProxyPlatform.Metadata.DefaultTypeResolver, ControlElement.BackgroundProperty, this.TextSource.GetComputedValue(AccessTextElement.BackgroundProperty));
      this.TextBox.Text = (string) this.TextSource.GetComputedValue(AccessTextElement.TextProperty);
    }

    public override void UpdateDocumentModel()
    {
      using (this.TextSource.ViewModel.AnimationEditor.DeferKeyFraming())
      {
        using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
        {
          this.TextSource.SetValue(AccessTextElement.TextProperty, (object) this.Text);
          editTransaction.Commit();
        }
      }
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      if (change.PropertyKey == null || !change.PropertyKey.Equals((object) AccessTextElement.TextProperty))
        return base.IsTextChange(change);
      return true;
    }
  }
}
