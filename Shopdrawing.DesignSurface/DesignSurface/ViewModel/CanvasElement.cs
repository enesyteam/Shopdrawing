// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.CanvasElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class CanvasElement : PanelElement
  {
    public static readonly IPropertyId BottomProperty = (IPropertyId) PlatformTypes.Canvas.GetMember(MemberType.AttachedProperty, "Bottom", MemberAccessTypes.Public);
    public static readonly IPropertyId LeftProperty = (IPropertyId) PlatformTypes.Canvas.GetMember(MemberType.AttachedProperty, "Left", MemberAccessTypes.Public);
    public static readonly IPropertyId RightProperty = (IPropertyId) PlatformTypes.Canvas.GetMember(MemberType.AttachedProperty, "Right", MemberAccessTypes.Public);
    public static readonly IPropertyId TopProperty = (IPropertyId) PlatformTypes.Canvas.GetMember(MemberType.AttachedProperty, "Top", MemberAccessTypes.Public);
    public static readonly IPropertyId CanvasZIndexProperty = (IPropertyId) PlatformTypes.Canvas.GetMember(MemberType.AttachedProperty, "ZIndex", MemberAccessTypes.Public);
    public static readonly CanvasElement.ConcreteCanvasElementFactory Factory = new CanvasElement.ConcreteCanvasElementFactory();

    public static double GetLeft(SceneElement element)
    {
      return (double) element.GetLocalOrDefaultValue(CanvasElement.LeftProperty);
    }

    public static void SetLeft(SceneElement element, double length)
    {
      element.SetValue(CanvasElement.LeftProperty, (object) length);
    }

    public static double GetTop(SceneElement element)
    {
      return (double) element.GetLocalOrDefaultValue(CanvasElement.TopProperty);
    }

    public static void SetTop(SceneElement element, double length)
    {
      element.SetValue(CanvasElement.TopProperty, (object) length);
    }

    public static double GetRight(SceneElement element)
    {
      return (double) element.GetLocalOrDefaultValue(CanvasElement.RightProperty);
    }

    public static void SetRight(SceneElement element, double length)
    {
      element.SetValue(CanvasElement.RightProperty, (object) length);
    }

    public static double GetBottom(SceneElement element)
    {
      return (double) element.GetLocalOrDefaultValue(CanvasElement.BottomProperty);
    }

    public static void SetBottom(SceneElement element, double length)
    {
      element.SetValue(CanvasElement.BottomProperty, (object) length);
    }

    public class ConcreteCanvasElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new CanvasElement();
      }
    }
  }
}
