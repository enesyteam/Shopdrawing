// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Utility.ResourceHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.DesignSurface.Utility
{
  internal static class ResourceHelper
  {
    internal static bool CheckEvaluationResult(ResourceEvaluation resourceEvaluation, params ResourceEvaluationResult[] results)
    {
      bool flag = false;
      foreach (ResourceEvaluationResult evaluationResult in results)
      {
        if (resourceEvaluation.ConflictType == evaluationResult)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    internal static bool CheckEvaluationResults(IList<ResourceEvaluation> resourceEvaluations, params ResourceEvaluationResult[] results)
    {
      bool flag = false;
      foreach (ResourceEvaluation resourceEvaluation in (IEnumerable<ResourceEvaluation>) resourceEvaluations)
      {
        if (ResourceHelper.CheckEvaluationResult(resourceEvaluation, results))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    internal static void AddPrimaryResource(ResourceEvaluation evaluatedPrimaryResource, IList<SceneNode> referringElements, ResourceConflictResolution conflictResolution, SceneNode destination, int insertionIndex)
    {
      SceneViewModel viewModel = destination.ViewModel;
      ResourceDictionaryNode destinationDictionary = ResourceHelper.EnsureResourceDictionaryNode(destination);
      DictionaryEntryNode resourceToAdd;
      switch (evaluatedPrimaryResource.ConflictType)
      {
        case ResourceEvaluationResult.NoExistingResource:
          resourceToAdd = evaluatedPrimaryResource.OriginalResource;
          break;
        case ResourceEvaluationResult.IdenticalResourceExists:
          resourceToAdd = ResourceHelper.ResolveResourceConflict(evaluatedPrimaryResource, referringElements, destination, conflictResolution);
          break;
        case ResourceEvaluationResult.IdenticalResourceIsMasked:
          resourceToAdd = evaluatedPrimaryResource.OriginalResource;
          break;
        case ResourceEvaluationResult.ConflictingResourceExists:
          resourceToAdd = ResourceHelper.ResolveResourceConflict(evaluatedPrimaryResource, referringElements, destination, conflictResolution);
          break;
        case ResourceEvaluationResult.ConflictingResourceIsMasked:
          resourceToAdd = evaluatedPrimaryResource.OriginalResource;
          break;
        default:
          throw new InvalidEnumArgumentException("ConflictType", (int) evaluatedPrimaryResource.ConflictType, typeof (ResourceEvaluationResult));
      }
      if (resourceToAdd == null)
        return;
      ResourceHelper.AddResource(resourceToAdd, destinationDictionary, insertionIndex);
    }

    internal static int AddResources(IList<ResourceEvaluation> referencedResources, IList<SceneNode> referringElements, ResourceConflictResolution conflictResolution, SceneNode destination, int insertionIndex)
    {
      SceneViewModel viewModel = destination.ViewModel;
      ResourceDictionaryNode destinationDictionary = ResourceHelper.EnsureResourceDictionaryNode(destination);
      int num = insertionIndex;
      foreach (ResourceEvaluation resourceEvaluation in (IEnumerable<ResourceEvaluation>) referencedResources)
      {
        DictionaryEntryNode resourceToAdd = (DictionaryEntryNode) null;
        switch (resourceEvaluation.ConflictType)
        {
          case ResourceEvaluationResult.NoExistingResource:
            resourceToAdd = resourceEvaluation.OriginalResource;
            goto case 1;
          case ResourceEvaluationResult.IdenticalResourceExists:
          case ResourceEvaluationResult.IdenticalResourceIsMasked:
            if (resourceToAdd != null)
            {
              ResourceHelper.AddResource(resourceToAdd, destinationDictionary, insertionIndex);
              ++insertionIndex;
              continue;
            }
            continue;
          case ResourceEvaluationResult.ConflictingResourceExists:
            resourceToAdd = ResourceHelper.ResolveResourceConflict(resourceEvaluation, referringElements, destination, conflictResolution);
            goto case 1;
          case ResourceEvaluationResult.ConflictingResourceIsMasked:
            resourceToAdd = ResourceHelper.ResolveResourceConflict(resourceEvaluation, referringElements, destination, conflictResolution);
            goto case 1;
          default:
            throw new InvalidEnumArgumentException("ConflictType", (int) resourceEvaluation.ConflictType, typeof (ResourceEvaluationResult));
        }
      }
      return insertionIndex - num;
    }

    internal static void AddResource(DictionaryEntryNode resourceToAdd, ResourceDictionaryNode destinationDictionary, int insertionIndex)
    {
      if (insertionIndex >= 0 && insertionIndex < destinationDictionary.Count)
        destinationDictionary.Insert(insertionIndex, resourceToAdd);
      else
        destinationDictionary.Add(resourceToAdd);
    }

    internal static DocumentCompositeNode LookupResource(SceneViewModel viewModel, DocumentCompositeNode findMe)
    {
      DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(findMe);
      if (resourceKey != null)
      {
        DocumentNode documentNode = new ExpressionEvaluator(viewModel.DocumentRootResolver).EvaluateResource(viewModel.GetSceneNode((DocumentNode) findMe).DocumentNodePath, DocumentNodeUtilities.IsDynamicResource((DocumentNode) findMe) ? ResourceReferenceType.Dynamic : ResourceReferenceType.Static, resourceKey);
        if (documentNode != null)
          return documentNode.Parent;
      }
      return (DocumentCompositeNode) null;
    }

    internal static bool FilterResources(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      return documentCompositeNode != null && documentCompositeNode.Type.IsResource;
    }

    internal static ResourceConflictResolution PromptForResourceConflictResolution(ResourceConflictResolution itemsToDisplay)
    {
      ResourceConflictResolution conflictResolution = ResourceConflictResolution.Undetermined;
      ResourceConflictResolutionDialog resolutionDialog = new ResourceConflictResolutionDialog(itemsToDisplay);
      if (resolutionDialog.ShowDialog().GetValueOrDefault(false))
        conflictResolution = resolutionDialog.Resolution;
      return conflictResolution;
    }

    internal static ResourceEvaluation EvaluateResource(DictionaryEntryNode primaryResource, SceneNode destination)
    {
      return ResourceHelper.EvaluateResourcesInternal((IList<DictionaryEntryNode>) new List<DictionaryEntryNode>()
      {
        primaryResource
      }, destination)[0];
    }

    internal static IList<ResourceEvaluation> EvaluateResources(IList<DictionaryEntryNode> resources, SceneNode destination)
    {
      return ResourceHelper.EvaluateResourcesInternal(resources, destination);
    }

    internal static bool CopyResourcesToNewResourceSite(IList<DocumentCompositeNode> auxillaryResources, SceneViewModel resourcesHostViewModel, DocumentCompositeNode resourcesHostNode, DocumentCompositeNode insertedResourceNode, int indexInResourceSite)
    {
      if (auxillaryResources == null || auxillaryResources.Count <= 0)
        return true;
      bool flag = true;
      ResourceConflictResolution conflictResolution = ResourceConflictResolution.Undetermined;
      SceneNode sceneNode = resourcesHostViewModel.GetSceneNode((DocumentNode) resourcesHostNode);
      List<DictionaryEntryNode> list1 = new List<DictionaryEntryNode>();
      List<SceneNode> list2 = new List<SceneNode>();
      foreach (DocumentNode documentNode in (IEnumerable<DocumentCompositeNode>) auxillaryResources)
      {
        DocumentNode node = documentNode.Clone(resourcesHostViewModel.Document.DocumentContext);
        DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) resourcesHostViewModel.GetSceneNode(node);
        list1.Add(dictionaryEntryNode);
        if (dictionaryEntryNode.Value != null)
          list2.Add(dictionaryEntryNode.Value);
      }
      if (insertedResourceNode != null)
        list2.Add(resourcesHostViewModel.GetSceneNode((DocumentNode) insertedResourceNode));
      IList<ResourceEvaluation> list3 = ResourceHelper.EvaluateResources((IList<DictionaryEntryNode>) list1, sceneNode);
      if (ResourceHelper.CheckEvaluationResults(list3, ResourceEvaluationResult.ConflictingResourceExists, ResourceEvaluationResult.ConflictingResourceIsMasked))
      {
        conflictResolution = ResourceHelper.PromptForResourceConflictResolution(ResourceConflictResolution.RenameNew | ResourceConflictResolution.OverwriteOld);
        if (conflictResolution == ResourceConflictResolution.Undetermined)
          flag = false;
      }
      if (flag)
      {
        ResourceHelper.EnsureResourceDictionaryNode(sceneNode);
        ResourceHelper.AddResources(list3, (IList<SceneNode>) list2, conflictResolution, sceneNode, indexInResourceSite);
      }
      return flag;
    }

    private static IList<ResourceEvaluation> EvaluateResourcesInternal(IList<DictionaryEntryNode> resources, SceneNode destination)
    {
      SceneViewModel viewModel = destination.ViewModel;
      List<ResourceEvaluation> list = new List<ResourceEvaluation>();
      foreach (DictionaryEntryNode originalResource in (IEnumerable<DictionaryEntryNode>) resources)
      {
        DocumentNode documentNode = (DocumentNode) null;
        SceneNode keyNode = originalResource.KeyNode;
        if (keyNode != null)
          documentNode = new ExpressionEvaluator(viewModel.DocumentRootResolver).EvaluateResource(destination.DocumentNodePath, keyNode.DocumentNode);
        if (documentNode == null)
        {
          list.Add(new ResourceEvaluation(originalResource, (DocumentNode) null, ResourceEvaluationResult.NoExistingResource));
        }
        else
        {
          ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(destination.DocumentNode);
          bool flag = resourcesCollection != null && resourcesCollection.Resources == documentNode.Parent.Parent;
          if (ResourceHelper.NodeTreesAreEquivalent(viewModel, originalResource.Value.DocumentNode, documentNode))
            list.Add(new ResourceEvaluation(originalResource, documentNode, flag ? ResourceEvaluationResult.IdenticalResourceExists : ResourceEvaluationResult.IdenticalResourceIsMasked));
          else
            list.Add(new ResourceEvaluation(originalResource, documentNode, flag ? ResourceEvaluationResult.ConflictingResourceExists : ResourceEvaluationResult.ConflictingResourceIsMasked));
        }
      }
      return (IList<ResourceEvaluation>) list;
    }

    internal static bool PasteResources(PastePackage pastePackage, IDictionary<DocumentNode, string> imageMap, ResourceConflictResolution itemsToDisplay, SceneNode destination, int index, bool lastResourceIsPrimary)
    {
      SceneViewModel viewModel = destination.ViewModel;
      List<DictionaryEntryNode> list1 = new List<DictionaryEntryNode>((IEnumerable<DictionaryEntryNode>) pastePackage.Resources);
      if (list1.Count == 0)
        return true;
      bool flag = true;
      IList<ResourceEvaluation> list2 = ResourceHelper.EvaluateResources((IList<DictionaryEntryNode>) list1, destination);
      ResourceConflictResolution conflictResolution = ResourceConflictResolution.Undetermined;
      if (ResourceHelper.CheckEvaluationResults(list2, ResourceEvaluationResult.ConflictingResourceExists, ResourceEvaluationResult.ConflictingResourceIsMasked))
      {
        conflictResolution = ResourceHelper.PromptForResourceConflictResolution(itemsToDisplay);
        if (conflictResolution == ResourceConflictResolution.Undetermined)
          flag = false;
      }
      if (flag)
      {
        foreach (ResourceEvaluation resourceEvaluation in (IEnumerable<ResourceEvaluation>) list2)
          ResourceHelper.UpdateImageReferences(resourceEvaluation.OriginalResource.Value.DocumentNode, imageMap, pastePackage, viewModel);
        List<SceneNode> list3 = new List<SceneNode>(pastePackage.Elements.Count);
        foreach (SceneElement sceneElement in pastePackage.Elements)
          list3.Add((SceneNode) sceneElement);
        ResourceEvaluation resourceEvaluation1 = list2[list2.Count - 1];
        if (lastResourceIsPrimary)
        {
          list2.RemoveAt(list2.Count - 1);
          int num = ResourceHelper.AddResources(list2, (IList<SceneNode>) list3, conflictResolution, destination, index);
          if (ResourceHelper.CheckEvaluationResult(resourceEvaluation1, ResourceEvaluationResult.IdenticalResourceExists))
            conflictResolution = ResourceConflictResolution.RenameNew;
          ResourceHelper.AddPrimaryResource(resourceEvaluation1, (IList<SceneNode>) list3, conflictResolution, destination, index + num);
        }
        else
          ResourceHelper.AddResources(list2, (IList<SceneNode>) list3, conflictResolution, destination, index);
      }
      return flag;
    }

    internal static ResourceDictionaryNode EnsureResourceDictionaryNode(SceneNode targetNode)
    {
      ResourceDictionaryNode resourceDictionaryNode = targetNode as ResourceDictionaryNode;
      IPropertyId resourcesProperty = targetNode.Metadata.ResourcesProperty;
      if (resourcesProperty != null)
      {
        DocumentNodePath valueAsDocumentNode = targetNode.GetLocalValueAsDocumentNode(resourcesProperty);
        if (valueAsDocumentNode == null || !(valueAsDocumentNode.Node is DocumentCompositeNode) || !PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) valueAsDocumentNode.Node.Type))
        {
          resourceDictionaryNode = (ResourceDictionaryNode) targetNode.ViewModel.CreateSceneNode(PlatformTypes.ResourceDictionary);
          targetNode.SetLocalValue(resourcesProperty, resourceDictionaryNode.DocumentNode);
        }
        else
          resourceDictionaryNode = (ResourceDictionaryNode) targetNode.ViewModel.GetSceneNode(valueAsDocumentNode.Node);
      }
      return resourceDictionaryNode;
    }

    internal static IDictionary<DocumentNode, string> CreateImageReferenceMap(DocumentNode documentNode, PastePackage pastePackage, SceneViewModel viewModel)
    {
      Dictionary<DocumentNode, string> candidateNodes = new Dictionary<DocumentNode, string>();
      ResourceHelper.CreateImageReferenceMapHelper(pastePackage, viewModel, candidateNodes, documentNode);
      foreach (DocumentNode descendant in documentNode.DescendantNodes)
        ResourceHelper.CreateImageReferenceMapHelper(pastePackage, viewModel, candidateNodes, descendant);
      return (IDictionary<DocumentNode, string>) candidateNodes;
    }

    private static void CreateImageReferenceMapHelper(PastePackage pastePackage, SceneViewModel viewModel, Dictionary<DocumentNode, string> candidateNodes, DocumentNode descendant)
    {
      DocumentPrimitiveNode documentPrimitiveNode = descendant as DocumentPrimitiveNode;
      if (documentPrimitiveNode == null)
        return;
      Uri uriValue = documentPrimitiveNode.GetUriValue();
      if (uriValue == (Uri) null)
        return;
      foreach (KeyValuePair<string, KeyValuePair<Uri, string>> keyValuePair in pastePackage.ImageReferences)
      {
        if (keyValuePair.Key == uriValue.OriginalString)
        {
          Uri key1 = keyValuePair.Value.Key;
          DocumentNode key2 = (DocumentNode) viewModel.Document.DocumentContext.CreateNode((ITypeId) documentPrimitiveNode.Type, (IDocumentNodeValue) new DocumentNodeStringValue(key1.OriginalString));
          if (documentPrimitiveNode.IsProperty)
            documentPrimitiveNode.Parent.Properties[(IPropertyId) documentPrimitiveNode.SitePropertyKey] = key2;
          else
            documentPrimitiveNode.Parent.Children[documentPrimitiveNode.SiteChildIndex] = key2;
          candidateNodes.Add(key2, keyValuePair.Key);
        }
      }
    }

    internal static void FindAllReferencedResources(DocumentNode node, List<DocumentNode> foundResources, ResourceHelper.PostOrderOperation postOrderOperation)
    {
      ResourceHelper.FindAllReferencedResources(node, foundResources, new Stack<DocumentNodePath>(), postOrderOperation);
    }

    private static void FindAllReferencedResources(DocumentNode node, List<DocumentNode> foundResources, Stack<DocumentNodePath> resourceReferenceTree, ResourceHelper.PostOrderOperation postOrderOperation)
    {
      foreach (DocumentNode documentNode1 in node.SelectDescendantNodes(new Predicate<DocumentNode>(ResourceHelper.FilterResources)))
      {
        DocumentNodePath context1 = new DocumentNodePath(node.DocumentRoot.RootNode, documentNode1);
        resourceReferenceTree.Push(context1);
        DocumentNode documentNode2 = (DocumentNode) null;
        if (DocumentNodeUtilities.IsDynamicResource(documentNode1))
        {
          foreach (DocumentNodePath context2 in resourceReferenceTree)
          {
            documentNode2 = new ExpressionEvaluator((IDocumentRootResolver) context2.Node.Context).EvaluateExpression(context2, documentNode1);
            if (documentNode2 != null)
              break;
          }
        }
        else
          documentNode2 = new ExpressionEvaluator((IDocumentRootResolver) context1.Node.Context).EvaluateExpression(context1, documentNode1);
        if (documentNode2 != null && !ResourceHelper.IsResourceCached(documentNode2, foundResources))
        {
          foundResources.Add(documentNode2);
          ResourceHelper.FindAllReferencedResources(documentNode2, foundResources, resourceReferenceTree, postOrderOperation);
          if (postOrderOperation != null)
            postOrderOperation(documentNode2);
        }
        resourceReferenceTree.Pop();
      }
    }

    private static bool IsResourceCached(DocumentNode resource, List<DocumentNode> foundResources)
    {
      foreach (DocumentNode documentNode in foundResources)
      {
        if (documentNode == resource || documentNode.IsAncestorOf(resource))
          return true;
      }
      return false;
    }

    internal static IList<DocumentCompositeNode> FindReferencedResources(DocumentNode root)
    {
      IList<DocumentCompositeNode> resources = (IList<DocumentCompositeNode>) new List<DocumentCompositeNode>();
      ResourceHelper.FindReferencedResourcesInternal(root, new Dictionary<DocumentNode, bool>(), resources);
      return resources;
    }

    private static void FindReferencedResourcesInternal(DocumentNode root, Dictionary<DocumentNode, bool> visited, IList<DocumentCompositeNode> resources)
    {
      foreach (DocumentNode documentNode in root.SelectDescendantNodes(new Predicate<DocumentNode>(ResourceHelper.FilterResources)))
      {
        DocumentNode index = new ExpressionEvaluator((IDocumentRootResolver) null).EvaluateExpression(new DocumentNodePath(root.DocumentRoot.RootNode, documentNode), documentNode);
        if (index != null && !visited.ContainsKey(index) && !root.IsAncestorOf(index))
        {
          visited[index] = true;
          ResourceHelper.FindReferencedResourcesInternal(index, visited, resources);
          DocumentCompositeNode parent = index.Parent;
          resources.Add(parent);
        }
      }
    }

    internal static void EnsureReferencedResourcesAreReachable(DocumentNode resourceNode, DocumentNode resourceExtensionNode)
    {
      if (resourceNode == null || resourceExtensionNode == null || resourceExtensionNode.DocumentRoot != resourceNode.DocumentRoot)
        return;
      DocumentCompositeNode dictionaryNode = resourceNode.Parent.Parent;
      int index1 = CreateResourceModel.IndexInResourceSite(resourceExtensionNode, dictionaryNode);
      if (index1 == -1)
        return;
      List<DocumentCompositeNode> list1 = Enumerable.ToList<DocumentCompositeNode>(Enumerable.Where<DocumentCompositeNode>((IEnumerable<DocumentCompositeNode>) ResourceHelper.FindReferencedResources(resourceNode), (Func<DocumentCompositeNode, bool>) (node => node.Parent == dictionaryNode)));
      if (!list1.Contains(resourceNode.Parent))
        list1.Add(resourceNode.Parent);
      list1.Sort((Comparison<DocumentCompositeNode>) ((lhs, rhs) => lhs.SiteChildIndex.CompareTo(rhs.SiteChildIndex)));
      List<DocumentCompositeNode> list2 = new List<DocumentCompositeNode>();
      for (int index2 = list1.Count - 1; index2 >= 0; --index2)
      {
        if (list1[index2].SiteChildIndex > index1)
        {
          list2.Add(list1[index2]);
          dictionaryNode.Children.RemoveAt(list1[index2].SiteChildIndex);
        }
      }
      foreach (DocumentCompositeNode documentCompositeNode in list2)
        dictionaryNode.Children.Insert(index1, (DocumentNode) documentCompositeNode);
    }

    internal static void UpdateImageReferences(DocumentNode parentNode, IDictionary<DocumentNode, string> imageMap, PastePackage pastePackage, SceneViewModel viewModel)
    {
      foreach (DocumentNode key1 in parentNode.DescendantNodes)
      {
        if (imageMap.ContainsKey(key1))
        {
          string index = imageMap[key1];
          KeyValuePair<Uri, string> keyValuePair = pastePackage.ImageReferences[index];
          Uri key2 = keyValuePair.Key;
          DocumentReference documentReference = DocumentReference.Create(keyValuePair.Value);
          if (!ResourceHelper.IsResourceInSameProject(viewModel, documentReference, key2))
          {
            IProjectItem itemForImage = ResourceHelper.CreateItemForImage(viewModel, documentReference, pastePackage);
            if (itemForImage != null)
              documentReference = itemForImage.DocumentReference;
          }
          IDocumentContext referencingDocument = ResourceHelper.DocumentContextFromViewModel(viewModel);
          string str = (ResourceHelper.MakeResourceReference(referencingDocument, documentReference) ?? key2).OriginalString;
          if (viewModel.Document.ProjectContext.IsCapabilitySet(PlatformCapability.ShouldSanitizeResourceReferences))
            str = str.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
          DocumentNode documentNode = (DocumentNode) referencingDocument.CreateNode((ITypeId) key1.Type, (IDocumentNodeValue) new DocumentNodeStringValue(str));
          if (key1.IsProperty)
            key1.Parent.Properties[(IPropertyId) key1.SitePropertyKey] = documentNode;
          else
            key1.Parent.Children[key1.SiteChildIndex] = documentNode;
        }
      }
    }

    private static bool IsResourceInSameProject(SceneViewModel viewModel, DocumentReference localPath, Uri originalUri)
    {
      IProject project = ResourceHelper.ProjectFromViewModel(viewModel);
      Uri uri = ResourceHelper.DocumentContextFromViewModel(viewModel).MakeDesignTimeUri(originalUri);
      if (project.FindItem(localPath) != null)
        return true;
      if (uri != (Uri) null)
        return project.FindItem(DocumentReference.Create(uri.OriginalString)) != null;
      return false;
    }

    private static IProject ProjectFromViewModel(SceneViewModel viewModel)
    {
      return ProjectHelper.GetProject(viewModel.DesignerContext.ProjectManager, viewModel.Document.DocumentContext);
    }

    private static IDocumentContext DocumentContextFromViewModel(SceneViewModel viewModel)
    {
      return viewModel.Document.DocumentContext;
    }

    private static IProjectItem CreateItemForImage(SceneViewModel viewModel, DocumentReference localPath, PastePackage pastePackage)
    {
      try
      {
        IProject project = ResourceHelper.ProjectFromViewModel(viewModel);
        string directoryName = Path.GetDirectoryName(ResourceHelper.DocumentContextFromViewModel(viewModel).DocumentUrl.TrimEnd(Path.DirectorySeparatorChar));
        string availableFilePath = ProjectPathHelper.GetAvailableFilePath(Path.GetFileName(localPath.Path), directoryName, project, true);
        using (Stream stream = pastePackage.ImageStreams[localPath.Path].GetStream())
        {
          using (FileStream fileStream = File.Create(availableFilePath, (int) stream.Length, FileOptions.RandomAccess))
            Microsoft.Expression.Framework.Clipboard.Container.CopyStream(stream, (Stream) fileStream);
        }
        IDocumentType documentType = project.GetDocumentType(localPath.Path);
        IProjectItem projectItem = project.AddItem(new DocumentCreationInfo()
        {
          DocumentType = documentType,
          TargetPath = availableFilePath,
          TargetFolder = directoryName
        });
        if (projectItem != null)
          viewModel.DesignerContext.ProjectManager.ItemSelectionSet.SetSelection((IDocumentItem) projectItem);
        return projectItem;
      }
      catch (Exception ex)
      {
        viewModel.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PasteElementsParseFailedDialogMessage, new object[1]
        {
          (object) ex.Message
        }));
      }
      return (IProjectItem) null;
    }

    private static Uri MakeResourceReference(IDocumentContext referencingDocument, DocumentReference resourcePath)
    {
      string uriString = referencingDocument.MakeResourceReference(resourcePath.Path);
      if (!string.IsNullOrEmpty(uriString))
        return new Uri(uriString, UriKind.RelativeOrAbsolute);
      return (Uri) null;
    }

    private static DictionaryEntryNode ResolveResourceConflict(ResourceEvaluation resourceEvaluation, IList<SceneNode> elements, SceneNode destination, ResourceConflictResolution resolution)
    {
      DictionaryEntryNode originalResource = resourceEvaluation.OriginalResource;
      DocumentNode evaluatedResource = resourceEvaluation.EvaluatedResource;
      DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) null;
      if (resolution == ResourceConflictResolution.RenameNew)
      {
        DocumentNode newKey = ResourceHelper.GenerateUniqueResourceKey(originalResource.Key.ToString(), destination);
        ResourceHelper.ChangeKey(elements, originalResource, originalResource.KeyNode.DocumentNode, newKey);
        dictionaryEntryNode = originalResource;
      }
      else if (resolution == ResourceConflictResolution.OverwriteOld)
      {
        DocumentCompositeNode parent = evaluatedResource.Parent;
        if (parent != null && typeof (DictionaryEntry).IsAssignableFrom(parent.TargetType))
        {
          SceneViewModel viewModel = SceneViewModel.GetViewModel((ISceneViewHost) destination.ProjectContext.GetService(typeof (ISceneViewHost)), parent.DocumentRoot, false);
          if (viewModel != null)
          {
            using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.UndoResourceOverwrite))
            {
              ((DictionaryEntryNode) viewModel.GetSceneNode((DocumentNode) parent)).Value = viewModel.GetSceneNode(originalResource.Value.DocumentNode.Clone(viewModel.Document.DocumentContext));
              editTransaction.Commit();
            }
          }
        }
        dictionaryEntryNode = (DictionaryEntryNode) null;
      }
      else if (resolution == ResourceConflictResolution.UseExisting)
        dictionaryEntryNode = (DictionaryEntryNode) null;
      return dictionaryEntryNode;
    }

    private static void ChangeKey(IList<SceneNode> elements, DictionaryEntryNode resource, DocumentNode oldKey, DocumentNode newKey)
    {
      resource.KeyNode = resource.ViewModel.GetSceneNode(newKey);
      if (elements == null)
        return;
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) elements)
      {
        foreach (DocumentNode expression in sceneNode.DocumentNode.SelectDescendantNodes(new Predicate<DocumentNode>(ResourceHelper.FilterResources)))
        {
          DocumentCompositeNode node = expression as DocumentCompositeNode;
          if (node != null)
          {
            DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(node);
            if (resourceKey != null && resourceKey.Equals(oldKey) && new ExpressionEvaluator((IDocumentRootResolver) null).EvaluateExpression(sceneNode.DocumentNodePath, expression) == null)
              ResourceNodeHelper.SetResourceKey(node, newKey.Clone(sceneNode.DocumentContext));
          }
        }
      }
    }

    internal static DocumentNode GenerateUniqueResourceKey(string baseKey, SceneNode destination)
    {
      string name = baseKey;
      DocumentNode keyNode;
      do
      {
        name = ResourceHelper.GetNextLogicalName(name);
        keyNode = (DocumentNode) destination.DocumentContext.CreateNode(name);
      }
      while (new ExpressionEvaluator(destination.ViewModel.DocumentRootResolver).EvaluateResource(destination.DocumentNodePath, keyNode) != null);
      return keyNode;
    }

    private static string GetNextLogicalName(string name)
    {
      Match match = new Regex("[0-9]+$").Match(name);
      string str = name;
      int num = 2;
      if (match.Success && match.Groups.Count > 0)
      {
        int length = name.LastIndexOf(match.Groups[0].Value, StringComparison.Ordinal);
        str = name.Substring(0, length);
        num = int.Parse(match.Groups[0].Value, (IFormatProvider) CultureInfo.InvariantCulture) + 1;
      }
      return str + (object) num;
    }

    private static bool NodeTreesAreEquivalent(SceneViewModel viewModel, DocumentNode nodeA, DocumentNode nodeB)
    {
      nodeA = nodeA.Clone(viewModel.DocumentRoot.DocumentContext);
      ResourceHelper.StripOffFormatting(nodeA);
      nodeB = nodeB.Clone(viewModel.DocumentRoot.DocumentContext);
      ResourceHelper.StripOffFormatting(nodeB);
      DocumentCompositeNode node1 = nodeA.Context.CreateNode(PlatformTypes.DictionaryEntry);
      node1.Properties[node1.PlatformMetadata.KnownProperties.DictionaryEntryValue] = nodeA;
      DocumentCompositeNode node2 = nodeB.Context.CreateNode(PlatformTypes.DictionaryEntry);
      node2.Properties[node2.PlatformMetadata.KnownProperties.DictionaryEntryValue] = nodeB;
      XamlSerializer xamlSerializer = new XamlSerializer((IDocumentRoot) viewModel.XamlDocument, (IXamlSerializerFilter) new DefaultXamlSerializerFilter());
      return xamlSerializer.Serialize(nodeA).Equals(xamlSerializer.Serialize(nodeB));
    }

    private static void StripOffFormatting(DocumentNode node)
    {
      node.SourceContext = (INodeSourceContext) null;
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        foreach (IPropertyId propertyKey in (IEnumerable<IProperty>) documentCompositeNode.Properties.Keys)
          documentCompositeNode.ClearContainerContext(propertyKey);
      }
      foreach (DocumentNode node1 in node.ChildNodes)
        ResourceHelper.StripOffFormatting(node1);
    }

    internal delegate void PostOrderOperation(DocumentNode evaluatedResource);
  }
}
