// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TextBoxElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class TextBoxElement : BaseTextElement
  {
    public static readonly IPropertyId TextProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "Text", MemberAccessTypes.Public);
    public static readonly IPropertyId TextAlignmentProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "TextAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId TextDecorationsProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "TextDecorations", MemberAccessTypes.Public);
    public static readonly IPropertyId TextWrappingProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "TextWrapping", MemberAccessTypes.Public);
    public static readonly IPropertyId CaretBrushProperty = (IPropertyId) PlatformTypes.TextBox.GetMember(MemberType.LocalProperty, "CaretBrush", MemberAccessTypes.Public);
    public static readonly TextBoxElement.ConcreteTextBoxElementFactory Factory = new TextBoxElement.ConcreteTextBoxElementFactory();

    public override bool IsContainer
    {
      get
      {
        return false;
      }
    }

    private IViewTextBox TextBox
    {
      get
      {
        return (IViewTextBox) this.ViewObject;
      }
    }

    public override string Text
    {
      get
      {
        return (string) this.GetComputedValue(TextBoxElement.TextProperty);
      }
      set
      {
        this.SetValue(TextBoxElement.TextProperty, (object) value);
      }
    }

    internal override object GetTextValueAtPoint(Point point, bool snapToText, PropertyReference propertyReference)
    {
      return this.GetComputedValue(propertyReference);
    }

    protected override void UpdateDocumentModelInternal()
    {
      if (this.TextBox == null)
        return;
      this.SetValue(TextBoxElement.TextProperty, (object) this.TextBox.Text);
    }

    public class ConcreteTextBoxElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TextBoxElement();
      }
    }
  }
}
