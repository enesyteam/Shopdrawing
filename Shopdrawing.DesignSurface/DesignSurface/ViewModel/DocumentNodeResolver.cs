// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DocumentNodeResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class DocumentNodeResolver
  {
    public static DocumentNodePath ResolveValue(DocumentNodePath source, IList<IProperty> propertyPath, SceneNode referenceSource, int numberOfStepsToResolve, bool evaluateExpressions, bool visualTriggerOnly, BaseTriggerNode trigger, DocumentNodeResolver.ShouldUseTrigger shouldUseTrigger, out int stepsResolved)
    {
      if (source.Node is DocumentPrimitiveNode)
      {
        stepsResolved = 0;
        return source;
      }
      List<DocumentNode> list1 = (List<DocumentNode>) null;
      List<IProperty> list2 = (List<IProperty>) null;
      List<DocumentNode> list3 = (List<DocumentNode>) null;
      DocumentNode childNode = source.Node;
      int index;
      for (index = 0; index < numberOfStepsToResolve; ++index)
      {
        IProperty property1 = propertyPath[index];
        DocumentCompositeNode documentCompositeNode1 = childNode as DocumentCompositeNode;
        if (documentCompositeNode1 != null)
        {
          DocumentNode documentNode1 = childNode;
          IProperty property2 = property1;
          IndexedClrPropertyReferenceStep propertyReferenceStep = property1 as IndexedClrPropertyReferenceStep;
          DocumentNode documentNode2;
          if (propertyReferenceStep != null)
          {
            if (documentCompositeNode1.SupportsChildren && propertyReferenceStep.Index >= 0 && propertyReferenceStep.Index < documentCompositeNode1.Children.Count)
              documentNode2 = documentCompositeNode1.Children[propertyReferenceStep.Index];
            else
              break;
          }
          else if (property1.TargetType != (Type) null && property1.TargetType.IsAssignableFrom(childNode.TargetType))
          {
            if (trigger != null && (shouldUseTrigger == null || shouldUseTrigger(trigger, (IPropertyId) property1)))
            {
              documentNode2 = trigger.GetDocumentNodeValue(documentCompositeNode1, (IPropertyId) property1, visualTriggerOnly);
              if (documentNode2 != null)
              {
                documentNode1 = (DocumentNode) documentNode2.Parent;
                property2 = documentNode2.SitePropertyKey;
              }
            }
            else
              documentNode2 = documentCompositeNode1.Properties[(IPropertyId) property1];
          }
          else
          {
            DocumentNodeReference documentNodeReference = (DocumentNodeReference) DocumentNodeResolver.CreateCompositeOrCollectionNodePropertyReference(referenceSource, documentCompositeNode1, (IPropertyId) property1, trigger, shouldUseTrigger, visualTriggerOnly);
            documentNode2 = documentNodeReference != null ? documentNodeReference.Node : (DocumentNode) null;
            if (documentNode2 != null)
            {
              documentNode1 = (DocumentNode) documentNode2.Parent;
              property2 = documentNode2.SitePropertyKey;
            }
          }
          bool flag = false;
          if (documentNode2 != null && documentNode2.Type.IsExpression && evaluateExpressions)
          {
            DocumentNodePath context = DocumentNodeResolver.RecreatePath(source, (IList<DocumentNode>) list1, (IList<DocumentNode>) list3, (IList<IProperty>) list2, documentNode2);
            DocumentNode documentNode3 = new ExpressionEvaluator((IDocumentRootResolver) documentNode2.Context).EvaluateExpression(context, documentNode2);
            if (documentNode3 != null && documentNode3 != documentNode2)
            {
              flag = true;
              documentNode2 = documentNode3;
            }
          }
          if (documentNode2 != null)
          {
            DocumentCompositeNode documentCompositeNode2 = documentNode2 as DocumentCompositeNode;
            if ((flag || PlatformTypes.Style.IsAssignableFrom((ITypeId) property1.PropertyType) || PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) property1.PropertyType)) && documentCompositeNode2 != null)
            {
              if (list1 == null)
              {
                list1 = new List<DocumentNode>();
                list2 = new List<IProperty>();
                list3 = new List<DocumentNode>();
              }
              list1.Add(documentNode2);
              list3.Add(documentNode1);
              list2.Add(property2);
            }
            childNode = documentNode2;
          }
          else
            break;
        }
        else
          break;
      }
      stepsResolved = index;
      if (stepsResolved == 0)
        return source;
      return DocumentNodeResolver.RecreatePath(source, (IList<DocumentNode>) list1, (IList<DocumentNode>) list3, (IList<IProperty>) list2, childNode);
    }

    public static DocumentPropertyNodeReferenceBase CreateCompositeOrCollectionNodePropertyReference(SceneNode referenceSource, DocumentCompositeNode parent, IPropertyId propertyKey, BaseTriggerNode trigger, DocumentNodeResolver.ShouldUseTrigger shouldUseTrigger, bool visualTriggerOnly)
    {
      IndexedClrPropertyReferenceStep referenceStep = propertyKey as IndexedClrPropertyReferenceStep;
      return referenceStep == null ? (trigger == null || shouldUseTrigger == null || !shouldUseTrigger(trigger, propertyKey) ? (referenceSource == null ? (DocumentPropertyNodeReferenceBase) new DocumentPropertyNodeReference(parent, propertyKey) : referenceSource.CreateLocalDocumentPropertyNodeReference(parent, propertyKey)) : (DocumentPropertyNodeReferenceBase) new TriggerSetterNodeReference(trigger, parent, propertyKey, visualTriggerOnly)) : (!parent.SupportsChildren || referenceStep.Index < 0 || referenceStep.Index >= parent.Children.Count ? (DocumentPropertyNodeReferenceBase) null : (DocumentPropertyNodeReferenceBase) new DocumentIndexedPropertyNodeReference(parent, referenceStep));
    }

    private static DocumentNodePath RecreatePath(DocumentNodePath path, IList<DocumentNode> containers, IList<DocumentNode> containerOwners, IList<IProperty> containerProperties, DocumentNode childNode)
    {
      DocumentNodePath documentNodePath = path;
      if (containers != null)
      {
        for (int index = 0; index < containers.Count; ++index)
          documentNodePath = documentNodePath.GetPathInContainer(containerOwners[index]).GetPathInSubContainer(containerProperties[index], containers[index]);
      }
      return documentNodePath.GetPathInContainer(childNode);
    }

    public delegate bool ShouldUseTrigger(BaseTriggerNode trigger, IPropertyId propertyKey);
  }
}
