// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.AnnotationManagerSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class AnnotationManagerSceneNode : SceneNode
  {
    public static readonly IPropertyId AnnotationsProperty = (IPropertyId) PlatformTypes.AnnotationManager.GetMember(MemberType.AttachedProperty, "Annotations", MemberAccessTypes.Public);
    public static readonly AnnotationManagerSceneNode.ConcreteAnnotationManagerSceneNodeFactory Factory = new AnnotationManagerSceneNode.ConcreteAnnotationManagerSceneNodeFactory();

    public static IList<AnnotationSceneNode> GetAnnotations(SceneNode ownerNode)
    {
      return (IList<AnnotationSceneNode>) new SceneNode.SceneNodeCollection<AnnotationSceneNode>(ownerNode, AnnotationManagerSceneNode.AnnotationsProperty);
    }

    public static void DeleteAnnotation(AnnotationSceneNode nodeToRemove)
    {
      SceneNode parent = nodeToRemove.Parent;
      IList<AnnotationSceneNode> annotations = AnnotationManagerSceneNode.GetAnnotations(parent);
      annotations.Remove(nodeToRemove);
      if (annotations.Count != 0)
        return;
      parent.ClearLocalValue(AnnotationManagerSceneNode.AnnotationsProperty);
    }

    public static AnnotationSceneNode CreateAnnotation(SceneNode ownerNode)
    {
      if (ownerNode == null)
        throw new ArgumentNullException("ownerNode");
      AnnotationSceneNode annotationNode = AnnotationSceneNode.Factory.Instantiate(ownerNode.ViewModel);
      annotationNode.Timestamp = DateTime.UtcNow;
      AnnotationManagerSceneNode.SetAnnotationParent(annotationNode, ownerNode);
      return annotationNode;
    }

    public static void SetAnnotationParent(AnnotationSceneNode annotationNode, SceneNode newParent)
    {
      if (annotationNode.Parent != null)
        AnnotationManagerSceneNode.DeleteAnnotation(annotationNode);
      AnnotationManagerSceneNode.GetAnnotations(newParent).Add(annotationNode);
    }

    public static void CloneAnnotation(AnnotationSceneNode annotationNode, SceneNode newParent)
    {
      DocumentNode node = annotationNode.DocumentNode.Clone(newParent.DocumentContext);
      AnnotationSceneNode annotationSceneNode = (AnnotationSceneNode) newParent.ViewModel.GetSceneNode(node);
      AnnotationManagerSceneNode.GetAnnotations(newParent).Add(annotationSceneNode);
    }

    public class ConcreteAnnotationManagerSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new AnnotationManagerSceneNode();
      }

      public AnnotationManagerSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (AnnotationManagerSceneNode) this.Instantiate(viewModel, PlatformTypes.AnnotationManager);
      }
    }
  }
}
