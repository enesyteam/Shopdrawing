// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.ContentEditProxy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public class ContentEditProxy : TextBoxEditProxyBase
  {
    public static readonly IPropertyId PasswordBoxCaretBrushPropertyId = (IPropertyId) PlatformTypes.PasswordBox.GetMember(MemberType.LocalProperty, "CaretBrush", MemberAccessTypes.Public);
    private PropertyReference targetProperty;

    public ContentEditProxy(BaseFrameworkElement textSource, PropertyReference targetProperty)
      : base(textSource)
    {
      this.targetProperty = targetProperty;
    }

    public ContentEditProxy(BaseFrameworkElement textSource, IPropertyId targetProperty)
      : this(textSource, new PropertyReference((ReferenceStep) textSource.ProjectContext.ResolveProperty(targetProperty)))
    {
    }

    public override void Instantiate()
    {
      base.Instantiate();
      this.TextBox.TextAlignment = this.ConvertContentAlignment((HorizontalAlignment) this.TextSource.GetComputedValue(ControlElement.HorizontalContentAlignmentProperty));
      object obj = (object) null;
      if (this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        object computedValueAsWpf = this.TextSource.GetComputedValueAsWpf(ControlElement.ForegroundProperty);
        this.TextBox.SetValue(this.ProxyPlatform.Metadata.DefaultTypeResolver, ControlElement.ForegroundProperty, computedValueAsWpf);
        obj = computedValueAsWpf;
      }
      if (PlatformTypes.PasswordBox.IsAssignableFrom((ITypeId) this.TextSource.Type) && this.TextSource.ProjectContext.ResolveProperty(ContentEditProxy.PasswordBoxCaretBrushPropertyId) != null)
        obj = this.TextSource.GetComputedValueAsWpf(ContentEditProxy.PasswordBoxCaretBrushPropertyId);
      if (obj != null)
        this.TextBox.SetValue(this.ProxyPlatform.Metadata.DefaultTypeResolver, TextBoxElement.CaretBrushProperty, obj);
      string str = this.TextSource.GetComputedValue(this.targetProperty) as string;
      if (str != null)
        this.TextBox.Text = str;
      if (ProjectNeutralTypes.Callout.IsAssignableFrom((ITypeId) this.TextSource.Type))
      {
        this.TextBox.TextWrapping = TextWrapping.Wrap;
        this.TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
      }
      if (this.TextSource.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return;
      Thickness padding = this.TextBox.Padding;
      --padding.Left;
      padding.Top -= 2.0;
      padding.Right -= 3.0;
      padding.Bottom -= 2.0;
      this.TextBox.Padding = padding;
    }

    public override void UpdateDocumentModel()
    {
      using (this.TextSource.ViewModel.AnimationEditor.DeferKeyFraming())
      {
        using (SceneEditTransaction editTransaction = this.TextSource.ViewModel.CreateEditTransaction(StringTable.TextEditUndo))
        {
          this.TextSource.SetValue(this.targetProperty, (object) this.Text);
          editTransaction.Commit();
        }
      }
    }

    public override bool IsTextChange(DocumentNodeChange change)
    {
      if (change.PropertyKey != this.targetProperty.LastStep)
        return base.IsTextChange(change);
      return true;
    }

    private TextAlignment ConvertContentAlignment(HorizontalAlignment contentAlignment)
    {
      TextAlignment textAlignment = TextAlignment.Left;
      switch (contentAlignment)
      {
        case HorizontalAlignment.Left:
          textAlignment = TextAlignment.Left;
          break;
        case HorizontalAlignment.Center:
          textAlignment = TextAlignment.Center;
          break;
        case HorizontalAlignment.Right:
          textAlignment = TextAlignment.Right;
          break;
        case HorizontalAlignment.Stretch:
          textAlignment = TextAlignment.Justify;
          break;
      }
      return textAlignment;
    }
  }
}
