// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneElementHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface
{
  public static class SceneElementHelper
  {
    public static SceneNode FindNode(SceneNode node, string name)
    {
      if (node != null)
      {
        DocumentNode byName = SceneElementHelper.FindByName(node.DocumentNode, name);
        if (byName != null)
          return node.ViewModel.GetSceneNode(byName);
      }
      return (SceneNode) null;
    }

    public static DocumentNode FindByName(DocumentNode node, string name)
    {
      return (node.NameScope != null ? node.NameScope : node.FindContainingNameScope()).FindNode(name);
    }

    public static SceneElement FindLowestAncestorOfCollection(ICollection<SceneElement> collection)
    {
      SceneElement sceneElement1 = (SceneElement) null;
      foreach (SceneElement sceneElement2 in (IEnumerable<SceneElement>) collection)
      {
        if (sceneElement1 == null)
        {
          sceneElement1 = sceneElement2;
        }
        else
        {
          sceneElement1 = (SceneElement) sceneElement1.GetCommonAncestor((SceneNode) sceneElement2);
          if (sceneElement1 == null)
            break;
        }
      }
      return sceneElement1;
    }

    public static IEnumerable<SceneElement> GetElementTree(SceneElement rootElement)
    {
      return (IEnumerable<SceneElement>) new SceneElementHelper.DepthFirstSceneElementCollection(rootElement);
    }

    public static IEnumerable<SceneElement> GetElementTreeBreadthFirst(SceneElement rootElement)
    {
      return (IEnumerable<SceneElement>) new SceneElementHelper.BreadthFirstSceneElementCollection(rootElement);
    }

    public static IEnumerable<SceneElement> GetLogicalTree(SceneElement rootElement)
    {
      Stack<IEnumerator<DocumentNode>> nodes = new Stack<IEnumerator<DocumentNode>>();
      nodes.Push((IEnumerator<DocumentNode>) new List<DocumentNode>()
      {
        rootElement.DocumentNode
      }.GetEnumerator());
      while (nodes.Count != 0)
      {
        IEnumerator<DocumentNode> currentEnumerator = nodes.Peek();
        if (currentEnumerator.MoveNext())
        {
          SceneElement currentElement = rootElement.ViewModel.GetSceneNode(currentEnumerator.Current) as SceneElement;
          if (currentElement != null)
            yield return currentElement;
          nodes.Push(currentEnumerator.Current.ChildNodes.GetEnumerator());
        }
        else
          nodes.Pop();
      }
    }

    public static List<FrameworkElement> ConvertToFrameworkElementList(ICollection<SceneElement> collection)
    {
      List<FrameworkElement> list = new List<FrameworkElement>();
      foreach (SceneElement sceneElement in (IEnumerable<SceneElement>) collection)
      {
        Base3DElement base3Delement = sceneElement as Base3DElement;
        if (base3Delement != null)
          list.Add(base3Delement.Viewport.ViewTargetElement.PlatformSpecificObject as FrameworkElement);
        else
          list.Add(sceneElement.ViewTargetElement.PlatformSpecificObject as FrameworkElement);
      }
      return list;
    }

    public static Dictionary<IPropertyId, SceneNode> StoreProperties(SceneNode sourceNode)
    {
      return SceneElementHelper.StoreProperties(sourceNode, (IEnumerable<IPropertyId>) null, true);
    }

    public static Dictionary<IPropertyId, SceneNode> StoreProperties(SceneNode sourceNode, bool detachProperties)
    {
      return SceneElementHelper.StoreProperties(sourceNode, (IEnumerable<IPropertyId>) null, detachProperties);
    }

    public static Dictionary<IPropertyId, SceneNode> StoreProperties(SceneNode sourceNode, IEnumerable<IPropertyId> propertyFilter, bool detachProperties)
    {
      Dictionary<IPropertyId, SceneNode> dictionary = new Dictionary<IPropertyId, SceneNode>();
      List<IPropertyId> list = new List<IPropertyId>();
      ReadOnlyCollection<IPropertyId> contentProperties = sourceNode.ContentProperties;
      DocumentCompositeNode documentCompositeNode = sourceNode.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        if (propertyFilter == null)
        {
          foreach (IPropertyId propertyId in (IEnumerable<IProperty>) documentCompositeNode.Properties.Keys)
            list.Add(propertyId);
        }
        else
        {
          foreach (IPropertyId propertyId in propertyFilter)
          {
            IProperty property = sourceNode.ProjectContext.ResolveProperty(propertyId);
            if (property != null && documentCompositeNode.Properties.Keys.Contains(property))
              list.Add((IPropertyId) property);
          }
        }
      }
      foreach (IPropertyId propertyId in list)
      {
        if (!contentProperties.Contains(propertyId))
        {
          DocumentNode node;
          using (sourceNode.ViewModel.ForceBaseValue())
            node = sourceNode.GetLocalValueAsDocumentNode(propertyId).Node;
          if (node != null)
          {
            if (detachProperties)
            {
              DocumentNodeHelper.PreserveFormatting(node);
              sourceNode.ClearLocalValue(propertyId);
            }
            else
              node = node.Clone(sourceNode.DocumentContext);
            dictionary.Add(propertyId, sourceNode.ViewModel.GetSceneNode(node));
          }
        }
      }
      return dictionary;
    }

    public static void FixElementNameBindingsInStoredProperties(SceneNode source, SceneNode target, Dictionary<IPropertyId, SceneNode> properties)
    {
      foreach (KeyValuePair<IPropertyId, SceneNode> keyValuePair in properties)
      {
        BindingSceneNode bindingSceneNode = keyValuePair.Value as BindingSceneNode;
        if (bindingSceneNode != null && bindingSceneNode.SupportsElementName && bindingSceneNode.ElementName == source.Name)
        {
          target.EnsureNamed();
          bindingSceneNode.ElementName = target.Name;
        }
      }
    }

    public static bool ApplyProperties(SceneNode propertyTarget, Dictionary<IPropertyId, SceneNode> properties)
    {
      bool flag1 = true;
      foreach (KeyValuePair<IPropertyId, SceneNode> keyValuePair in properties)
      {
        IProperty propertyKey = (IProperty) keyValuePair.Key;
        SceneNode valueNode = keyValuePair.Value;
        ReferenceStep referenceStep = propertyKey as ReferenceStep;
        if (referenceStep != null)
        {
          ReferenceStep appliedReferenceStep;
          if (SceneElementHelper.DoesPropertyApply(propertyTarget, referenceStep, valueNode, out appliedReferenceStep))
          {
            if (propertyTarget.IsSet((IPropertyId) appliedReferenceStep) == PropertyState.Unset)
              propertyTarget.SetValueAsSceneNode((IPropertyId) appliedReferenceStep, valueNode);
            else
              flag1 = false;
          }
        }
        else
        {
          bool flag2 = false;
          bool flag3 = true;
          DocumentCompositeNode documentCompositeNode = propertyTarget.DocumentNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            flag2 = documentCompositeNode.Properties.Contains(propertyKey);
            ITypeId typeId = (ITypeId) documentCompositeNode.TypeResolver.GetType(propertyKey.TargetType);
            flag3 = typeId != null && typeId.IsAssignableFrom((ITypeId) documentCompositeNode.Type);
          }
          if (!flag2 && flag3)
            propertyTarget.SetValueAsSceneNode((IPropertyId) propertyKey, valueNode);
          else
            flag1 = false;
        }
      }
      return flag1;
    }

    public static bool DoesPropertyApply(SceneNode sceneNode, ReferenceStep referenceStep, SceneNode valueNode, out ReferenceStep appliedReferenceStep)
    {
      appliedReferenceStep = sceneNode.DesignerContext.PropertyManager.FilterProperty(sceneNode, referenceStep);
      if (appliedReferenceStep == null)
        return false;
      if (DocumentNodeUtilities.IsStyleOrTemplate(appliedReferenceStep.PropertyType))
      {
        DocumentNode node = valueNode.DocumentNode;
        if (node.Type.IsExpression)
          node = new ExpressionEvaluator(sceneNode.ViewModel.DocumentRootResolver).EvaluateExpression(sceneNode.DocumentNodePath, valueNode.DocumentNode);
        if (node == null || !DocumentNodeUtilities.IsStyleOrTemplate(node.Type) || !appliedReferenceStep.PropertyType.IsAssignableFrom((ITypeId) node.Type))
          return false;
        IType templateTargetType = DocumentNodeUtilities.GetStyleOrTemplateTargetType(node);
        Type propertyTargetType = sceneNode.Metadata.GetStylePropertyTargetType((IPropertyId) appliedReferenceStep);
        if (templateTargetType != null && propertyTargetType != (Type) null && !templateTargetType.RuntimeType.IsAssignableFrom(propertyTargetType))
          return false;
      }
      return true;
    }

    private sealed class DepthFirstSceneElementCollection : IEnumerable<SceneElement>, IEnumerable
    {
      private SceneElement root;

      public DepthFirstSceneElementCollection(SceneElement root)
      {
        this.root = root;
      }

      public IEnumerator<SceneElement> GetEnumerator()
      {
        List<SceneNode> initialElementList = new List<SceneNode>();
        initialElementList.Add((SceneNode) this.root);
        List<IPropertyId> initialPropertyList = new List<IPropertyId>();
        List<SceneElementHelper.DepthFirstSceneElementCollection.EnumeratorEntry> nodeStack = new List<SceneElementHelper.DepthFirstSceneElementCollection.EnumeratorEntry>();
        nodeStack.Add(new SceneElementHelper.DepthFirstSceneElementCollection.EnumeratorEntry((SceneNode) null, (IEnumerator<SceneNode>) initialElementList.GetEnumerator(), (IEnumerator<IPropertyId>) initialPropertyList.GetEnumerator()));
        while (nodeStack.Count > 0)
        {
          SceneElementHelper.DepthFirstSceneElementCollection.EnumeratorEntry currentEntry = nodeStack[nodeStack.Count - 1];
          if (currentEntry.MoveNext())
          {
            SceneElement currentElement = currentEntry.Current as SceneElement;
            if (currentElement != null && currentElement.DocumentNode is DocumentCompositeNode)
            {
              nodeStack.Add(new SceneElementHelper.DepthFirstSceneElementCollection.EnumeratorEntry(currentElement));
              yield return currentElement;
            }
          }
          else
            nodeStack.RemoveAt(nodeStack.Count - 1);
        }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private class EnumeratorEntry
      {
        private SceneNode parentNode;
        private IEnumerator<SceneNode> childCollectionEnumerator;
        private IEnumerator<IPropertyId> propertyEnumerator;

        public SceneNode Current
        {
          get
          {
            return this.childCollectionEnumerator.Current;
          }
        }

        public EnumeratorEntry(SceneNode parentNode, IEnumerator<SceneNode> childCollectionEnumerator, IEnumerator<IPropertyId> propertyEnumerator)
        {
          this.parentNode = parentNode;
          this.childCollectionEnumerator = childCollectionEnumerator;
          this.propertyEnumerator = propertyEnumerator;
        }

        public EnumeratorEntry(SceneElement parentElement)
        {
          this.parentNode = (SceneNode) parentElement;
          this.propertyEnumerator = parentElement.ContentProperties.GetEnumerator();
        }

        public bool MoveNext()
        {
          if (this.childCollectionEnumerator != null && this.childCollectionEnumerator.MoveNext())
            return true;
          while (this.propertyEnumerator.MoveNext())
          {
            ISceneNodeCollection<SceneNode> collectionForProperty = this.parentNode.GetCollectionForProperty(this.propertyEnumerator.Current);
            if (collectionForProperty.Count > 0)
            {
              this.childCollectionEnumerator = collectionForProperty.GetEnumerator();
              return this.childCollectionEnumerator.MoveNext();
            }
          }
          return false;
        }
      }
    }

    private sealed class BreadthFirstSceneElementCollection : IEnumerable<SceneElement>, IEnumerable
    {
      private SceneElement root;

      public BreadthFirstSceneElementCollection(SceneElement root)
      {
        this.root = root;
      }

      public IEnumerator<SceneElement> GetEnumerator()
      {
        return (IEnumerator<SceneElement>) new SceneElementHelper.BreadthFirstSceneElementCollection.BreadthFirstEnumerator(this.root);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private class BreadthFirstEnumerator : IEnumerator<SceneElement>, IDisposable, IEnumerator
      {
        private SceneElement parentNode;
        private Queue<SceneElement> childQueue;
        private SceneElement currentItem;

        public SceneElement Current
        {
          get
          {
            return this.currentItem;
          }
        }

        object IEnumerator.Current
        {
          get
          {
            return (object) this.Current;
          }
        }

        public BreadthFirstEnumerator(SceneElement parentNode)
        {
          this.parentNode = parentNode;
          this.Reset();
        }

        void IDisposable.Dispose()
        {
          this.parentNode = (SceneElement) null;
          this.childQueue = (Queue<SceneElement>) null;
          this.currentItem = (SceneElement) null;
        }

        public void Reset()
        {
          this.childQueue = new Queue<SceneElement>();
          this.currentItem = (SceneElement) null;
        }

        public bool MoveNext()
        {
          if (this.Current == null)
          {
            if (this.parentNode == null)
              return false;
            this.currentItem = this.parentNode;
            return true;
          }
          if (this.currentItem.DocumentNode is DocumentCompositeNode)
          {
            IEnumerator<IPropertyId> enumerator = this.currentItem.ContentProperties.GetEnumerator();
            while (enumerator.MoveNext())
            {
              foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.currentItem.GetCollectionForProperty(enumerator.Current))
              {
                SceneElement sceneElement = sceneNode as SceneElement;
                if (sceneElement != null)
                  this.childQueue.Enqueue(sceneElement);
              }
            }
          }
          if (this.childQueue.Count == 0)
            return false;
          this.currentItem = this.childQueue.Dequeue();
          return true;
        }
      }
    }
  }
}
