// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ContentPresenterElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ContentPresenterElement : BaseFrameworkElement
  {
    public static readonly IPropertyId ContentProperty = (IPropertyId) PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
    public static readonly IPropertyId ContentTemplateProperty = (IPropertyId) PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "ContentTemplate", MemberAccessTypes.Public);
    public static readonly IPropertyId RecognizesAccessKeyProperty = (IPropertyId) PlatformTypes.ContentPresenter.GetMember(MemberType.LocalProperty, "RecognizesAccessKey", MemberAccessTypes.Public);
    public static readonly ContentPresenterElement.ConcreteContentPresenterElementFactory Factory = new ContentPresenterElement.ConcreteContentPresenterElementFactory();

    public bool IsRecognizesAccessKeySupported
    {
      get
      {
        return this.ProjectContext.ResolveProperty(ContentPresenterElement.RecognizesAccessKeyProperty) != null;
      }
    }

    public static void PrepareContentPresenter(ContentPresenterElement element)
    {
      IDocumentContext documentContext = element.DocumentContext;
      element.SetLocalValue(BaseFrameworkElement.HorizontalAlignmentProperty, (DocumentNode) DocumentNodeUtilities.NewTemplateBindingNode(element.DocumentNode, ControlElement.HorizontalContentAlignmentProperty));
      element.SetLocalValue(BaseFrameworkElement.VerticalAlignmentProperty, (DocumentNode) DocumentNodeUtilities.NewTemplateBindingNode(element.DocumentNode, ControlElement.VerticalContentAlignmentProperty));
      if (element.IsSnapsToDevicePixelsSupported)
        element.SetLocalValue(Base2DElement.SnapsToDevicePixelsProperty, (DocumentNode) DocumentNodeUtilities.NewTemplateBindingNode(element.DocumentNode, Base2DElement.SnapsToDevicePixelsProperty));
      if (!element.IsRecognizesAccessKeySupported)
        return;
      element.SetLocalValue(ContentPresenterElement.RecognizesAccessKeyProperty, (object) true);
    }

    public class ConcreteContentPresenterElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ContentPresenterElement();
      }
    }
  }
}
