// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.TextBoxEditProxyBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public abstract class TextBoxEditProxyBase : TextEditProxy
  {
    private IViewTextBox textBox;
    private string text;

    public override bool SupportsRangeProperties
    {
      get
      {
        return false;
      }
    }

    public override bool SupportsParagraphProperties
    {
      get
      {
        return false;
      }
    }

    public override IViewTextBoxBase EditingElement
    {
      get
      {
        return (IViewTextBoxBase) this.textBox;
      }
    }

    protected IViewTextBox TextBox
    {
      get
      {
        return this.textBox;
      }
    }

    protected string Text
    {
      get
      {
        return this.text;
      }
    }

    public override DesignTimeTextSelection TextSelection
    {
      get
      {
        return new DesignTimeTextSelection((SceneElement) this.TextSource, this.textBox);
      }
    }

    protected TextBoxEditProxyBase(BaseFrameworkElement textSource)
      : base(textSource)
    {
      this.ProxyPlatform = textSource.DesignerContext.DesignerDefaultPlatformService.DefaultPlatform;
      this.textBox = this.ProxyPlatform.ViewTextObjectFactory.CreateTextBox();
    }

    public override void Serialize()
    {
      this.text = this.textBox.Text;
    }

    public override void DeleteSelection()
    {
      if (this.textBox.SelectionLength == 0)
        this.textBox.Select(this.textBox.SelectionStart, 1);
      this.textBox.SelectedText = string.Empty;
    }

    public override void SelectNone()
    {
      this.textBox.Select(this.textBox.SelectionStart, 0);
    }

    public override void Instantiate()
    {
      base.Instantiate();
      this.CopyProperty(TextBoxElement.TextDecorationsProperty);
      this.CopyProperty(TextBoxElement.TextAlignmentProperty);
      if (!this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        this.EditingElement.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
      else
        this.EditingElement.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    }
  }
}
