// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.FlowDocumentScrollViewerElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class FlowDocumentScrollViewerElement : BaseFrameworkElement, ITextFlowSceneNode
  {
    public static readonly IPropertyId DocumentProperty = (IPropertyId) PlatformTypes.FlowDocumentScrollViewer.GetMember(MemberType.LocalProperty, "Document", MemberAccessTypes.Public);
    public static readonly FlowDocumentScrollViewerElement.ConcreteFlowDocumentScrollViewerElementFactory Factory = new FlowDocumentScrollViewerElement.ConcreteFlowDocumentScrollViewerElementFactory();

    public FlowDocumentScrollViewer FlowDocumentScrollViewer
    {
      get
      {
        return (FlowDocumentScrollViewer) this.ViewObject.PlatformSpecificObject;
      }
    }

    public IViewTextPointer ContentStart
    {
      get
      {
        return this.WrapTextPointer(this.FlowDocumentScrollViewer.Document.ContentStart);
      }
    }

    public IViewTextPointer ContentEnd
    {
      get
      {
        return this.WrapTextPointer(this.FlowDocumentScrollViewer.Document.ContentEnd);
      }
    }

    public IPropertyId TextChildProperty
    {
      get
      {
        return FlowDocumentScrollViewerElement.DocumentProperty;
      }
    }

    public ISceneNodeCollection<SceneNode> TextFlowCollectionForTextChildProperty
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.TextFlowSceneNodeCollection((BaseFrameworkElement) this, (SceneElement) this, (SceneElement) this, this.TextChildProperty);
      }
    }

    public ISceneNodeCollection<SceneNode> CollectionForTextChildProperty
    {
      get
      {
        return base.GetCollectionForProperty(this.TextChildProperty);
      }
    }

    public FlowDocumentElement FlowDocumentElement
    {
      get
      {
        return this.GetLocalValueAsSceneNode(FlowDocumentScrollViewerElement.DocumentProperty) as FlowDocumentElement;
      }
    }

    public override ISceneNodeCollection<SceneNode> GetCollectionForProperty(IPropertyId childProperty)
    {
      if (this.TextChildProperty.Equals((object) childProperty))
        return this.TextFlowCollectionForTextChildProperty;
      return base.GetCollectionForProperty(childProperty);
    }

    private IViewTextPointer WrapTextPointer(TextPointer platformPointer)
    {
      return this.Platform.ViewObjectFactory.Instantiate((object) platformPointer) as IViewTextPointer;
    }

    public IViewTextPointer GetPositionFromPoint(Point point)
    {
      TextPointer textPointer = this.FlowDocumentScrollViewer.Document.ContentStart;
      TextPointer platformPointer = (TextPointer) null;
      double num = double.MaxValue;
      for (; textPointer != null; textPointer = textPointer.GetPositionAtOffset(1))
      {
        Rect characterRect = textPointer.GetCharacterRect(LogicalDirection.Forward);
        double length = (new Point(characterRect.Left, (characterRect.Top + characterRect.Bottom) / 2.0) - point).Length;
        if (length < num)
        {
          num = length;
          platformPointer = textPointer;
        }
      }
      return this.WrapTextPointer(platformPointer);
    }

    public SceneElement EnsureTextParent()
    {
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) this.DocumentNode;
      Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode1 = documentCompositeNode.Properties[FlowDocumentScrollViewerElement.DocumentProperty];
      if (documentNode1 == null || !typeof (FlowDocument).IsAssignableFrom(documentNode1.TargetType))
      {
        IDocumentContext context = documentCompositeNode.Context;
        Microsoft.Expression.DesignModel.DocumentModel.DocumentNode documentNode2 = this.FlowDocumentScrollViewer.Document == null ? (Microsoft.Expression.DesignModel.DocumentModel.DocumentNode) context.CreateNode(PlatformTypes.FlowDocument) : context.CreateNode(typeof (FlowDocument), (object) this.FlowDocumentScrollViewer.Document);
        documentCompositeNode.Properties[FlowDocumentScrollViewerElement.DocumentProperty] = documentNode2;
      }
      return (SceneElement) this.FlowDocumentElement;
    }

    public void AddInlineTextChild(Microsoft.Expression.DesignModel.DocumentModel.DocumentNode child)
    {
      this.EnsureTextParent();
      this.FlowDocumentElement.AddInlineTextChild(child);
    }

    public void InsertInlineTextChild(int index, Microsoft.Expression.DesignModel.DocumentModel.DocumentNode child)
    {
      this.EnsureTextParent();
      this.FlowDocumentElement.InsertInlineTextChild(index, child);
    }

    public class ConcreteFlowDocumentScrollViewerElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new FlowDocumentScrollViewerElement();
      }
    }
  }
}
