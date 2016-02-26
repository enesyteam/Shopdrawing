// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TextElementSceneElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class TextElementSceneElement : SceneElement, ITextFlowSceneNode
  {
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.TextElement.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly IPropertyId FontSizeProperty = (IPropertyId) PlatformTypes.TextElement.GetMember(MemberType.LocalProperty, "FontSize", MemberAccessTypes.Public);
    public static readonly IPropertyId LanguageProperty = (IPropertyId) PlatformTypes.TextElement.GetMember(MemberType.LocalProperty, "Language", MemberAccessTypes.Public);
    public static readonly IPropertyId ResourcesProperty = (IPropertyId) PlatformTypes.TextElement.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
    public static readonly TextElementSceneElement.ConcreteTextElementSceneElementFactory Factory = new TextElementSceneElement.ConcreteTextElementSceneElementFactory();

    private IViewTextElement TextElement
    {
      get
      {
        return this.ViewObject as IViewTextElement;
      }
    }

    internal BaseFrameworkElement HostElement
    {
      get
      {
        SceneNode parent = this.Parent;
        BaseFrameworkElement frameworkElement;
        for (frameworkElement = parent as BaseFrameworkElement; frameworkElement == null && parent != null; frameworkElement = parent as BaseFrameworkElement)
          parent = parent.Parent;
        return frameworkElement;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        return false;
      }
    }

    public IViewTextPointer ContentStart
    {
      get
      {
        return this.TextElement.ContentStart;
      }
    }

    public IViewTextPointer ContentEnd
    {
      get
      {
        return this.TextElement.ContentEnd;
      }
    }

    public IPropertyId TextChildProperty
    {
      get
      {
        return ParagraphElement.InlinesProperty;
      }
    }

    public ISceneNodeCollection<SceneNode> TextFlowCollectionForTextChildProperty
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.TextFlowSceneNodeCollection(this.HostElement, (SceneElement) this, (SceneElement) this, this.TextChildProperty);
      }
    }

    public ISceneNodeCollection<SceneNode> CollectionForTextChildProperty
    {
      get
      {
        return base.GetCollectionForProperty(this.TextChildProperty);
      }
    }

    public override ISceneNodeCollection<SceneNode> GetCollectionForProperty(IPropertyId childProperty)
    {
      BaseFrameworkElement hostElement = this.HostElement;
      if (this.TextChildProperty.Equals((object) childProperty) && hostElement is ITextFlowSceneNode)
        return this.TextFlowCollectionForTextChildProperty;
      return base.GetCollectionForProperty(childProperty);
    }

    public IViewTextPointer GetPositionFromPoint(Point point)
    {
      ITextFlowSceneNode textFlowSceneNode = this.HostElement as ITextFlowSceneNode;
      if (textFlowSceneNode != null)
        return textFlowSceneNode.GetPositionFromPoint(point);
      return this.ContentEnd;
    }

    public virtual void AddInlineTextChild(DocumentNode child)
    {
      ITextFlowSceneNode textFlowSceneNode = this.HostElement as ITextFlowSceneNode;
      if (textFlowSceneNode == null)
        return;
      textFlowSceneNode.AddInlineTextChild(child);
    }

    public virtual void InsertInlineTextChild(int index, DocumentNode child)
    {
      ITextFlowSceneNode textFlowSceneNode = this.HostElement as ITextFlowSceneNode;
      if (textFlowSceneNode == null)
        return;
      textFlowSceneNode.InsertInlineTextChild(index, child);
    }

    public virtual SceneElement EnsureTextParent()
    {
      return (SceneElement) this;
    }

    public class ConcreteTextElementSceneElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TextElementSceneElement();
      }
    }
  }
}
