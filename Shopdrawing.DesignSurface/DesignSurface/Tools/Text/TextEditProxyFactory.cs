// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Text.TextEditProxyFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Text
{
  public static class TextEditProxyFactory
  {
    public static readonly IPropertyId PasswordBoxPasswordProperty = (IPropertyId) PlatformTypes.PasswordBox.GetMember(MemberType.LocalProperty, "Password", MemberAccessTypes.Public);
    public static readonly IPropertyId HeaderedContentControlHeaderProperty = (IPropertyId) ProjectNeutralTypes.HeaderedContentControl.GetMember(MemberType.LocalProperty, "Header", MemberAccessTypes.Public);
    public static readonly IPropertyId HeaderedItemsControlHeaderProperty = (IPropertyId) ProjectNeutralTypes.HeaderedItemsControl.GetMember(MemberType.LocalProperty, "Header", MemberAccessTypes.Public);

    public static bool IsEditableElement(SceneElement sceneElement)
    {
      return TextEditProxyFactory.IsEditableElement(sceneElement, false);
    }

    public static bool IsEditableElement(SceneElement sceneElement, bool textElementOnly)
    {
      bool flag = false;
      if (PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) sceneElement.Type) || PlatformTypes.TextBox.IsAssignableFrom((ITypeId) sceneElement.Type) || (PlatformTypes.PasswordBox.IsAssignableFrom((ITypeId) sceneElement.Type) || PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) sceneElement.Type)) || (PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom((ITypeId) sceneElement.Type) || PlatformTypes.AccessText.IsAssignableFrom((ITypeId) sceneElement.Type)))
        flag = true;
      else if (ProjectNeutralTypes.Label.IsAssignableFrom((ITypeId) sceneElement.Type) && TextEditProxyFactory.HasEditableContent(sceneElement, ContentControlElement.ContentProperty))
        flag = true;
      else if (!textElementOnly)
      {
        if (ProjectNeutralTypes.HeaderedContentControl.IsAssignableFrom((ITypeId) sceneElement.Type) && TextEditProxyFactory.HasEditableContent(sceneElement, TextEditProxyFactory.HeaderedContentControlHeaderProperty))
          flag = true;
        else if (PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) sceneElement.Type) && !PlatformTypes.Window.IsAssignableFrom((ITypeId) sceneElement.Type) && (!PlatformTypes.ScrollViewer.IsAssignableFrom((ITypeId) sceneElement.Type) && TextEditProxyFactory.HasEditableContent(sceneElement, ContentControlElement.ContentProperty)))
          flag = true;
        else if (ProjectNeutralTypes.HeaderedItemsControl.IsAssignableFrom((ITypeId) sceneElement.Type) && TextEditProxyFactory.HasEditableContent(sceneElement, TextEditProxyFactory.HeaderedItemsControlHeaderProperty))
          flag = true;
      }
      if (sceneElement.Type.XamlSourcePath != null)
        flag = false;
      return flag;
    }

    public static TextEditProxy CreateEditProxy(BaseFrameworkElement textElement)
    {
      TextEditProxy textEditProxy = (TextEditProxy) null;
      if (PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = !textElement.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) ? (TextEditProxy) new SilverlightRichTextBoxEditProxy(textElement) : (TextEditProxy) new FlowDocumentEditProxy(textElement);
      else if (PlatformTypes.TextBox.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new TextBoxEditProxy(textElement);
      else if (ProjectNeutralTypes.HeaderedContentControl.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new ContentEditProxy(textElement, TextEditProxyFactory.HeaderedContentControlHeaderProperty);
      else if (PlatformTypes.ContentControl.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new ContentEditProxy(textElement, ContentControlElement.ContentProperty);
      else if (ProjectNeutralTypes.HeaderedItemsControl.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new ContentEditProxy(textElement, TextEditProxyFactory.HeaderedItemsControlHeaderProperty);
      else if (PlatformTypes.PasswordBox.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new ContentEditProxy(textElement, TextEditProxyFactory.PasswordBoxPasswordProperty);
      else if (PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new TextBlockEditProxy(textElement);
      else if (PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new FlowDocumentScrollViewerEditProxy(textElement);
      else if (PlatformTypes.AccessText.IsAssignableFrom((ITypeId) textElement.Type))
        textEditProxy = (TextEditProxy) new AccessTextEditProxy(textElement);
      return textEditProxy;
    }

    private static bool HasEditableContent(SceneElement sceneElement, IPropertyId propertyKey)
    {
      return !(sceneElement.GetLocalValueAsSceneNode(propertyKey) is BaseFrameworkElement);
    }
  }
}
