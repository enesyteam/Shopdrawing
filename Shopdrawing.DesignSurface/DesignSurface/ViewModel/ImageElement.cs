// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ImageElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ImageElement : BaseFrameworkElement
  {
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.Image.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly IPropertyId StretchProperty = (IPropertyId) PlatformTypes.Image.GetMember(MemberType.LocalProperty, "Stretch", MemberAccessTypes.Public);
    public static readonly ImageElement.ConcreteImageElementFactory Factory = new ImageElement.ConcreteImageElementFactory();

    public string Uri
    {
      get
      {
        System.Uri uriValue = DocumentNodeHelper.GetUriValue(((DocumentCompositeNode) this.DocumentNode).Properties[ImageElement.SourceProperty]);
        if (!(uriValue != (System.Uri) null))
          return (string) null;
        return uriValue.OriginalString;
      }
      set
      {
        if (!(value != this.Uri))
          return;
        if (string.IsNullOrEmpty(value))
        {
          this.ClearValue(ImageElement.SourceProperty);
        }
        else
        {
          DocumentPrimitiveNode node = this.DocumentNode.Context.CreateNode(PlatformTypes.ImageSource, (IDocumentNodeValue) new DocumentNodeStringValue(value));
          this.SetValue(ImageElement.SourceProperty, (object) node);
        }
      }
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (propertyReference.Count != 1 || !ImageElement.SourceProperty.Equals((object) propertyReference.FirstStep))
        return base.GetComputedValueInternal(propertyReference);
      SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(propertyReference);
      if (valueAsSceneNode != null && valueAsSceneNode.TargetType == typeof (BitmapImage))
        return (object) DocumentNodeHelper.GetUriValue(valueAsSceneNode.DocumentNode);
      return this.GetLocalOrDefaultValue(propertyReference);
    }

    public class ConcreteImageElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ImageElement();
      }
    }
  }
}
