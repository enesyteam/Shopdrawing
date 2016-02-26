// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.PastePackage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Clipboard;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public class PastePackage
  {
    private List<DictionaryEntryNode> resources = new List<DictionaryEntryNode>();
    private List<SceneElement> elementsToCopy = new List<SceneElement>();
    private List<SceneNode> childPropertyToCopy = new List<SceneNode>();
    private IDictionary<SceneNode, SceneNode> elementDictionary = (IDictionary<SceneNode, SceneNode>) new Dictionary<SceneNode, SceneNode>();
    private List<StoryboardTimelineSceneNode> storyboardsToCopy = new List<StoryboardTimelineSceneNode>();
    private Dictionary<string, KeyValuePair<Uri, string>> imageReferences = new Dictionary<string, KeyValuePair<Uri, string>>();
    private Dictionary<string, CopyItem> imageStreams = new Dictionary<string, CopyItem>();
    private List<DocumentNode> cachedResources = new List<DocumentNode>();
    private SceneViewModel viewModel;
    private Microsoft.Expression.Framework.UserInterface.IWindowService windowService;
    private bool copyStoryboardsReferencingElements;
    private static DataObject currentDataObject;
    private static PastePackage pastePackageWithCopyTokenHandlers;
    private static DesignerContext designerContextWithCopyTokenHandlers;

    public static bool PasteSelectionChangePending { get; set; }

    public static string GlobalCopyElementToken { get; private set; }

    public string ClipboardCopyElementToken
    {
      get
      {
        if (this.Elements.Count > 0)
          return this.Elements[0].GetLocalValue(DesignTimeProperties.CopyTokenProperty) as string;
        return (string) null;
      }
    }

    public ReadOnlyCollection<SceneElement> Elements
    {
      get
      {
        return new ReadOnlyCollection<SceneElement>((IList<SceneElement>) this.elementsToCopy);
      }
    }

    public ReadOnlyCollection<SceneNode> ChildPropertyNodes
    {
      get
      {
        return new ReadOnlyCollection<SceneNode>((IList<SceneNode>) this.childPropertyToCopy);
      }
    }

    public Dictionary<string, CopyItem> ImageStreams
    {
      get
      {
        return this.imageStreams;
      }
    }

    public Dictionary<string, KeyValuePair<Uri, string>> ImageReferences
    {
      get
      {
        return new Dictionary<string, KeyValuePair<Uri, string>>((IDictionary<string, KeyValuePair<Uri, string>>) this.imageReferences);
      }
    }

    public ReadOnlyCollection<StoryboardTimelineSceneNode> Storyboards
    {
      get
      {
        return new ReadOnlyCollection<StoryboardTimelineSceneNode>((IList<StoryboardTimelineSceneNode>) this.storyboardsToCopy);
      }
    }

    public ReadOnlyCollection<DictionaryEntryNode> Resources
    {
      get
      {
        return new ReadOnlyCollection<DictionaryEntryNode>((IList<DictionaryEntryNode>) this.resources);
      }
    }

    public bool CopyStoryboardsReferencingElements
    {
      get
      {
        return this.copyStoryboardsReferencingElements;
      }
      set
      {
        this.copyStoryboardsReferencingElements = value;
      }
    }

    private IDocumentContext DocumentContext
    {
      get
      {
        return this.viewModel.Document.DocumentContext;
      }
    }

    public PastePackage(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public void SetGlobalCopyElementToken(string guid)
    {
      PastePackage.GlobalCopyElementToken = guid;
      if (PastePackage.pastePackageWithCopyTokenHandlers != null)
      {
        PastePackage.designerContextWithCopyTokenHandlers.ViewService.ViewClosed -= new ViewEventHandler(PastePackage.pastePackageWithCopyTokenHandlers.ViewService_ViewClosed);
        PastePackage.designerContextWithCopyTokenHandlers.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(PastePackage.pastePackageWithCopyTokenHandlers.SelectionManager_LateActiveSceneUpdatePhase);
        PastePackage.pastePackageWithCopyTokenHandlers = (PastePackage) null;
        PastePackage.designerContextWithCopyTokenHandlers = (DesignerContext) null;
      }
      if (string.IsNullOrEmpty(guid))
        return;
      this.viewModel.DesignerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.viewModel.DesignerContext.ViewService.ViewClosed += new ViewEventHandler(this.ViewService_ViewClosed);
      PastePackage.designerContextWithCopyTokenHandlers = this.viewModel.DesignerContext;
      PastePackage.pastePackageWithCopyTokenHandlers = this;
    }

    public void AddChildPropertyNodes(IList<SceneNode> childPropertyNodes)
    {
      for (int index = 0; index < childPropertyNodes.Count; ++index)
        this.AddChildPropertyNode(childPropertyNodes[index]);
    }

    public void AddElements(List<SceneElement> elements)
    {
      this.AddElements(elements, false);
    }

    public void AddElements(List<SceneElement> elements, bool includeCopyToken)
    {
      for (int index = 0; index < elements.Count; ++index)
      {
        SceneElement sceneElement = elements[index];
        this.AddElement(elements[index], false, includeCopyToken && index == 0);
      }
      if (!this.copyStoryboardsReferencingElements)
        return;
      this.CollectAllAnimationsForElements((IEnumerable<SceneElement>) elements);
    }

    public void AddChildPropertyNode(SceneNode childPropertyNode)
    {
      this.childPropertyToCopy.Add(this.viewModel.GetSceneNode(childPropertyNode.DocumentNode.Clone(this.DocumentContext)));
    }

    private void AddElement(SceneElement element, bool collectAnimations, bool includeCopyToken)
    {
      SceneElement rootElement = (SceneElement) this.viewModel.GetSceneNode(element.DocumentNode.Clone(this.DocumentContext));
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) rootElement.DocumentNode;
      BaseFrameworkElement element1 = rootElement as BaseFrameworkElement;
      if (element1 != null)
      {
        BaseFrameworkElement element2 = (BaseFrameworkElement) element;
        ILayoutDesigner designerForChild = element.ViewModel.GetLayoutDesignerForChild((SceneElement) element2, true);
        LayoutCacheRecord layoutCacheRecord = designerForChild.CacheLayout(element2);
        element1.SetValueAsWpf(DesignTimeProperties.LayoutRectProperty, (object) layoutCacheRecord.Rect);
        if (!layoutCacheRecord.Overlapping)
          element1.SetValueAsWpf(DesignTimeProperties.SlotOriginProperty, (object) layoutCacheRecord.SlotOrigin);
        LayoutUtilities.SetLayoutOverrides((SceneElement) element1, layoutCacheRecord.Overrides);
        designerForChild.ClearUnusedLayoutProperties(element1);
        if (includeCopyToken)
        {
          string guid = Guid.NewGuid().ToString();
          this.SetGlobalCopyElementToken(guid);
          element1.SetValueAsWpf(DesignTimeProperties.CopyTokenProperty, (object) guid);
        }
        documentCompositeNode.ClearValue(DesignTimeProperties.ClassProperty);
        documentCompositeNode.ClearValue(DesignTimeProperties.SubclassProperty);
        documentCompositeNode.ClearValue(DesignTimeProperties.ClassModifierProperty);
      }
      this.elementsToCopy.Add(rootElement);
      IEnumerator<SceneElement> enumerator = SceneElementHelper.GetLogicalTree(rootElement).GetEnumerator();
      foreach (SceneElement sceneElement1 in SceneElementHelper.GetLogicalTree(element))
      {
        SceneElement sceneElement2 = (SceneElement) null;
        if (enumerator.MoveNext())
          sceneElement2 = enumerator.Current;
        if (sceneElement2 != null)
          this.elementDictionary.Add((SceneNode) sceneElement1, (SceneNode) sceneElement2);
        else
          break;
      }
      if (collectAnimations && this.copyStoryboardsReferencingElements)
        this.CollectAllAnimationsForElements((IEnumerable<SceneElement>) new List<SceneElement>()
        {
          element
        });
      this.CacheImages(rootElement.DocumentNode);
      this.CacheReferencedResources(element.DocumentNode);
    }

    public void AddKeyframes(ICollection<KeyFrameSceneNode> keyframes)
    {
      IDocumentContext documentContext = this.viewModel.Document.DocumentContext;
      StoryboardTimelineSceneNode timelineSceneNode = StoryboardTimelineSceneNode.Factory.Instantiate(this.viewModel);
      Dictionary<KeyFrameAnimationSceneNode, KeyFrameAnimationSceneNode> dictionary = new Dictionary<KeyFrameAnimationSceneNode, KeyFrameAnimationSceneNode>();
      foreach (KeyFrameSceneNode keyFrameSceneNode1 in (IEnumerable<KeyFrameSceneNode>) keyframes)
      {
        KeyFrameAnimationSceneNode keyFrameAnimation = keyFrameSceneNode1.KeyFrameAnimation;
        KeyFrameAnimationSceneNode animationSceneNode = (KeyFrameAnimationSceneNode) null;
        if (!dictionary.TryGetValue(keyFrameAnimation, out animationSceneNode))
        {
          animationSceneNode = (KeyFrameAnimationSceneNode) this.viewModel.GetSceneNode(keyFrameAnimation.DocumentNode.Clone(documentContext));
          animationSceneNode.ClearValue(StoryboardTimelineSceneNode.TargetNameProperty);
          animationSceneNode.ClearKeyFrames();
          animationSceneNode.ClearValue(DesignTimeProperties.ShouldSerializeProperty);
          animationSceneNode.ClearValue(DesignTimeProperties.IsAnimationProxyProperty);
          dictionary[keyFrameAnimation] = animationSceneNode;
          timelineSceneNode.Children.Add((TimelineSceneNode) animationSceneNode);
        }
        KeyFrameSceneNode keyFrameSceneNode2 = (KeyFrameSceneNode) this.viewModel.GetSceneNode(keyFrameSceneNode1.DocumentNode.Clone(documentContext));
        keyFrameSceneNode2.ClearValue(DesignTimeProperties.ShouldSerializeProperty);
        animationSceneNode.AddKeyFrame(keyFrameSceneNode2);
      }
      this.storyboardsToCopy.Add(timelineSceneNode);
      foreach (SceneNode sceneNode in dictionary.Keys)
        this.CacheReferencedResources(sceneNode.DocumentNode);
    }

    public void AddResource(DictionaryEntryNode resource)
    {
      this.cachedResources.Add(resource.DocumentNode);
      this.CacheReferencedResources(resource.DocumentNode);
      DocumentNode node = resource.DocumentNode.Clone(this.DocumentContext);
      this.resources.Add((DictionaryEntryNode) resource.ViewModel.GetSceneNode(node));
    }

    private void CollectAllAnimationsForElements(IEnumerable<SceneElement> elements)
    {
      List<SceneElement> list1 = new List<SceneElement>();
      AnimationEditor animationEditor = this.viewModel.AnimationEditor;
      foreach (SceneElement rootElement in elements)
      {
        foreach (SceneElement sceneElement in SceneElementHelper.GetLogicalTree(rootElement))
          list1.Add(sceneElement);
      }
      List<SceneElement> list2 = new List<SceneElement>((IEnumerable<SceneElement>) list1);
      while (list2.Count > 0)
      {
        IStoryboardContainer storyboardContainer = list2[0].StoryboardContainer;
        HybridDictionary hybridDictionary = new HybridDictionary();
        for (int index = list2.Count - 1; index >= 0; --index)
        {
          SceneElement sceneElement = list2[index];
          if (sceneElement.StoryboardContainer == storyboardContainer)
          {
            list2.RemoveAt(index);
            hybridDictionary[(object) sceneElement] = (object) true;
          }
        }
        foreach (StoryboardTimelineSceneNode timelineSceneNode1 in animationEditor.EnumerateStoryboardsForContainer(storyboardContainer))
        {
          if (timelineSceneNode1.IsInResourceDictionary)
          {
            bool flag1 = false;
            for (int index = timelineSceneNode1.Children.Count - 1; index >= 0; --index)
            {
              AnimationSceneNode animationSceneNode = timelineSceneNode1.Children[index] as AnimationSceneNode;
              MediaTimelineSceneNode timelineSceneNode2 = timelineSceneNode1.Children[index] as MediaTimelineSceneNode;
              if (animationSceneNode != null)
              {
                TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
                if (elementAndProperty.SceneNode != null && elementAndProperty.PropertyReference != null && hybridDictionary.Contains((object) elementAndProperty.SceneNode))
                {
                  flag1 = true;
                  break;
                }
              }
              else if (timelineSceneNode2 != null && timelineSceneNode2.TargetElement != null && hybridDictionary.Contains((object) timelineSceneNode2.TargetElement))
              {
                flag1 = true;
                break;
              }
            }
            if (flag1)
            {
              DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) this.viewModel.GetSceneNode(timelineSceneNode1.Parent.DocumentNode.Clone(this.DocumentContext));
              StoryboardTimelineSceneNode timelineSceneNode2 = dictionaryEntryNode.Value as StoryboardTimelineSceneNode;
              for (int index = timelineSceneNode1.Children.Count - 1; index >= 0; --index)
              {
                AnimationSceneNode animationSceneNode = timelineSceneNode1.Children[index] as AnimationSceneNode;
                MediaTimelineSceneNode timelineSceneNode3 = timelineSceneNode1.Children[index] as MediaTimelineSceneNode;
                bool flag2 = false;
                if (animationSceneNode != null)
                {
                  TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode.TargetElementAndProperty;
                  if (elementAndProperty.SceneNode == null || elementAndProperty.PropertyReference == null || !hybridDictionary.Contains((object) elementAndProperty.SceneNode))
                  {
                    flag2 = true;
                  }
                  else
                  {
                    SceneNode sceneNode;
                    if ((PathElement.DataProperty.Equals((object) elementAndProperty.PropertyReference.FirstStep) || Base2DElement.ClipProperty.Equals((object) elementAndProperty.PropertyReference.FirstStep)) && this.elementDictionary.TryGetValue(elementAndProperty.SceneNode, out sceneNode))
                      sceneNode.GetLocalValueAsSceneNode((IPropertyId) elementAndProperty.PropertyReference.FirstStep).SetLocalValue(DesignTimeProperties.IsAnimatedProperty, (object) true);
                  }
                }
                else if (timelineSceneNode3 == null || timelineSceneNode3.TargetElement == null || !hybridDictionary.Contains((object) timelineSceneNode3.TargetElement))
                  flag2 = true;
                if (flag2)
                  timelineSceneNode2.Children.RemoveAt(index);
              }
              this.resources.Add(dictionaryEntryNode);
              this.CacheReferencedResources(timelineSceneNode1.DocumentNode);
            }
          }
        }
      }
    }

    private static void SerializeItemToXAML(CopyBuffer copyBuffer, CopyItem copyItem, byte[] resourceDictionaryMarkup)
    {
      copyItem.ContentType = "xaml";
      copyItem.FilenameExtension = ".xaml";
      copyBuffer.AddSelectedItem(copyItem);
      if (resourceDictionaryMarkup == null)
        return;
      CopyItem copyItem1 = (CopyItem) new MemoryCopyItem(resourceDictionaryMarkup);
      copyItem1.ContentType = "xaml";
      copyItem1.FilenameExtension = ".xaml";
      copyBuffer.AddReferencedItem(copyItem1);
    }

    private Stream CreateClipboardStream()
    {
      XamlDocument document = (XamlDocument) this.viewModel.XamlDocument;
      CopyBuffer copyBuffer = new CopyBuffer();
      byte[] numArray = (byte[]) null;
      bool flag = false;
      if (this.resources.Count > 0)
      {
        ResourceDictionaryNode resourceDictionaryNode = ResourceDictionaryNode.Factory.Instantiate(this.viewModel);
        foreach (DictionaryEntryNode dictionaryEntryNode in this.resources)
          resourceDictionaryNode.Add(dictionaryEntryNode);
        numArray = PastePackage.Serialize(document, resourceDictionaryNode.DocumentNode);
        flag = true;
      }
      if (this.childPropertyToCopy.Count > 0)
      {
        DocumentNode node;
        if (this.childPropertyToCopy.Count == 1)
        {
          node = this.childPropertyToCopy[0].DocumentNode;
        }
        else
        {
          DocumentCompositeNode arrayListNode = this.CreateArrayListNode();
          for (int index = 0; index < this.childPropertyToCopy.Count; ++index)
            arrayListNode.Children.Add(this.childPropertyToCopy[index].DocumentNode);
          node = (DocumentNode) arrayListNode;
        }
        PastePackage.SerializeItemToXAML(copyBuffer, (CopyItem) new MemoryCopyItem(PastePackage.Serialize(document, node)), numArray);
        flag = false;
      }
      if (this.elementsToCopy.Count > 0)
      {
        DocumentNode node;
        if (this.elementsToCopy.Count == 1)
        {
          node = this.elementsToCopy[0].DocumentNode;
        }
        else
        {
          DocumentCompositeNode arrayListNode = this.CreateArrayListNode();
          for (int index = 0; index < this.elementsToCopy.Count; ++index)
            arrayListNode.Children.Add(this.elementsToCopy[index].DocumentNode);
          node = (DocumentNode) arrayListNode;
        }
        PastePackage.SerializeItemToXAML(copyBuffer, (CopyItem) new MemoryCopyItem(PastePackage.Serialize(document, node)), numArray);
        flag = false;
      }
      foreach (StoryboardTimelineSceneNode timelineSceneNode in this.storyboardsToCopy)
      {
        PastePackage.SerializeItemToXAML(copyBuffer, (CopyItem) new MemoryCopyItem(PastePackage.Serialize(document, timelineSceneNode.DocumentNode)), numArray);
        flag = false;
      }
      if (flag && numArray != null)
      {
        CopyItem copyItem = (CopyItem) new MemoryCopyItem(numArray);
        copyItem.ContentType = "xaml";
        copyItem.FilenameExtension = ".xaml";
        copyBuffer.AddReferencedItem(copyItem);
      }
      List<string> list = new List<string>();
      foreach (KeyValuePair<string, KeyValuePair<Uri, string>> keyValuePair in this.imageReferences)
      {
        CopyItem copyItem1 = (CopyItem) new MetadataCopyItem();
        copyItem1.ContentType = "text";
        copyItem1.Key = keyValuePair.Key;
        copyItem1.LocalPath = keyValuePair.Value.Value;
        copyItem1.OriginalUri = keyValuePair.Value.Key;
        copyBuffer.AddReferencedItem(copyItem1);
        if (!list.Contains(copyItem1.LocalPath))
        {
          try
          {
            using (FileStream fileStream = File.Open(copyItem1.LocalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
              CopyItem copyItem2 = (CopyItem) new MemoryCopyItem(Microsoft.Expression.Framework.Clipboard.Container.GetBytes((Stream) fileStream));
              copyItem2.ContentType = "image";
              copyItem2.FilenameExtension = Path.GetExtension(keyValuePair.Value.Value);
              copyItem2.Key = copyItem1.LocalPath;
              copyItem2.LocalPath = copyItem1.LocalPath;
              copyItem2.OriginalUri = copyItem1.OriginalUri;
              copyBuffer.AddReferencedItem(copyItem2);
              list.Add(copyItem2.LocalPath);
            }
          }
          catch
          {
          }
        }
      }
      return (Stream) Microsoft.Expression.Framework.Clipboard.Container.CreateClipboardStream(copyBuffer);
    }

    private DocumentCompositeNode CreateArrayListNode()
    {
      ITypeResolver typeResolver = this.viewModel.DocumentRoot.DocumentContext.TypeResolver;
      IType type = typeResolver.ResolveType(PlatformTypes.ArrayList);
      if (typeResolver.PlatformMetadata.IsNullType((ITypeId) type))
        return new DocumentCompositeNode(this.viewModel.DocumentRoot.DocumentContext, (IType) new PastePackage.ArrayListWorkaroundType(typeResolver, this.viewModel.DesignerContext.DesignerDefaultPlatformService), PlatformTypes.Object);
      return this.viewModel.DocumentRoot.DocumentContext.CreateNode(PlatformTypes.ArrayList);
    }

    public DataObject GetPasteDataObject()
    {
      return new DataObject((object) new PastePackage.PastePackageDataObject(this));
    }

    public void SendToClipboard()
    {
      bool flag = PastePackage.currentDataObject == null;
      try
      {
        DataObject pasteDataObject = this.GetPasteDataObject();
        PastePackage.currentDataObject = pasteDataObject;
        ClipboardService.SetDataObject((object) pasteDataObject, false);
        if (!flag)
          return;
        try
        {
          this.windowService = this.viewModel.DesignerContext.WindowService;
          if (this.windowService == null)
            return;
          this.windowService.Closing += new CancelEventHandler(this.WindowService_Closing);
        }
        catch
        {
        }
      }
      catch (ExternalException ex)
      {
        if (this.viewModel.DesignerContext.MessageDisplayService != null)
        {
          if (ex.ErrorCode == -2147221040)
            this.viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.OpenClipboardFailureDialogMessage);
          else
            this.viewModel.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CopyFailedDialogMessage, new object[1]
            {
              (object) ex.Message
            }));
        }
        else
          throw;
      }
    }

    private void WindowService_Closing(object sender, CancelEventArgs e)
    {
      PastePackage.FlushClipboard();
      if (this.windowService == null)
        return;
      this.windowService.Closing -= new CancelEventHandler(this.WindowService_Closing);
      this.windowService = (Microsoft.Expression.Framework.UserInterface.IWindowService) null;
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        return;
      if (!PastePackage.PasteSelectionChangePending)
        this.SetGlobalCopyElementToken((string) null);
      PastePackage.PasteSelectionChangePending = false;
    }

    private void ViewService_ViewClosed(object sender, ViewEventArgs args)
    {
      this.SetGlobalCopyElementToken((string) null);
    }

    [DllImport("ole32.dll")]
    private static extern int OleFlushClipboard();

    public static void FlushClipboard()
    {
      if (!ClipboardService.IsCurrent((IDataObject) PastePackage.currentDataObject))
        return;
      PastePackage.OleFlushClipboard();
    }

    public static PastePackage FromData(SceneViewModel viewModel, SafeDataObject dataObject)
    {
      PastePackage pastePackage = (PastePackage) null;
      CopyBuffer copyBuffer = (CopyBuffer) null;
      if (dataObject != null && dataObject.GetDataPresent(Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name))
      {
        object data = dataObject.GetData(Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name);
        Stream clipboardStream = data as Stream;
        if (clipboardStream == null)
        {
          if (data is byte[])
          {
            byte[] buffer;
            try
            {
              buffer = (byte[]) dataObject.GetData(Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name);
            }
            catch (OutOfMemoryException ex)
            {
              LowMemoryMessage.Show();
              return (PastePackage) null;
            }
            clipboardStream = (Stream) new MemoryStream(buffer);
          }
        }
        if (clipboardStream != null)
        {
          try
          {
            copyBuffer = Microsoft.Expression.Framework.Clipboard.Container.ExtractCopyBufferFromClipboardStream(clipboardStream);
          }
          catch (FileFormatException ex)
          {
          }
        }
      }
      if (copyBuffer != null)
      {
        PerformanceUtility.MarkInterimStep(PerformanceEvent.PasteElements, "Begin Deserialization");
        pastePackage = new PastePackage(viewModel);
        IDocumentContext documentContext = viewModel.Document.DocumentContext;
        bool displayErrors = true;
        for (int index = 0; index < copyBuffer.ReferencedItemCount; ++index)
        {
          CopyItem copyItem = copyBuffer.ReferencedItem(index);
          if (copyItem.ContentType == "xaml" || string.IsNullOrEmpty(copyItem.ContentType))
            PastePackage.ProcessPasteItem(viewModel, pastePackage, documentContext, ref displayErrors, copyItem);
          else if (copyItem.ContentType == "image" || copyItem.ContentType == "text")
            PastePackage.ProcessImagePasteItem(pastePackage, copyItem);
        }
        for (int index = 0; index < copyBuffer.SelectedItemCount; ++index)
        {
          CopyItem copyItem = copyBuffer.SelectedItem(index);
          PastePackage.ProcessPasteItem(viewModel, pastePackage, documentContext, ref displayErrors, copyItem);
        }
        PerformanceUtility.MarkInterimStep(PerformanceEvent.PasteElements, "Complete Deserialization");
      }
      return pastePackage;
    }

    private static void ProcessPasteItem(SceneViewModel viewModel, PastePackage pastePackage, IDocumentContext documentContext, ref bool displayErrors, CopyItem item)
    {
      DocumentNode documentNode1 = PastePackage.TryParseXamlBuffer(viewModel, item, ref displayErrors);
      if (documentNode1 == null)
        return;
      SceneNode sceneNode1 = viewModel.GetSceneNode(documentNode1.Clone(documentContext));
      DocumentCompositeNode documentCompositeNode = documentNode1 as DocumentCompositeNode;
      SceneElement sceneElement1 = sceneNode1 as SceneElement;
      StoryboardTimelineSceneNode timelineSceneNode = sceneNode1 as StoryboardTimelineSceneNode;
      ResourceDictionaryNode resourceDictionaryNode = sceneNode1 as ResourceDictionaryNode;
      if (sceneElement1 != null)
        pastePackage.elementsToCopy.Add(sceneElement1);
      else if (timelineSceneNode != null)
        pastePackage.storyboardsToCopy.Add(timelineSceneNode);
      else if (resourceDictionaryNode != null)
      {
        foreach (DictionaryEntryNode dictionaryEntryNode in resourceDictionaryNode)
          pastePackage.resources.Add(dictionaryEntryNode);
        resourceDictionaryNode.Clear();
      }
      else if (documentCompositeNode != null && documentCompositeNode.SupportsChildren)
      {
        for (int index = 0; index < documentCompositeNode.Children.Count; ++index)
        {
          DocumentNode documentNode2 = documentCompositeNode.Children[index];
          SceneNode sceneNode2 = viewModel.GetSceneNode(documentNode2.Clone(documentContext));
          SceneElement sceneElement2 = sceneNode2 as SceneElement;
          if (sceneElement2 != null)
            pastePackage.elementsToCopy.Add(sceneElement2);
          else if (PasteCommand.ChildSceneNodeToPropertyId(sceneNode2) != null)
            pastePackage.childPropertyToCopy.Add(sceneNode2);
        }
      }
      else
      {
        if (PasteCommand.ChildSceneNodeToPropertyId(sceneNode1) == null)
          return;
        pastePackage.childPropertyToCopy.Add(sceneNode1);
      }
    }

    private static void ProcessImagePasteItem(PastePackage pastePackage, CopyItem item)
    {
      if (item.ContentType == "text")
      {
        pastePackage.imageReferences.Add(item.Key, new KeyValuePair<Uri, string>(item.OriginalUri, item.LocalPath));
      }
      else
      {
        if (!(item.ContentType == "image"))
          return;
        pastePackage.imageStreams.Add(item.Key, item);
      }
    }

    private void CacheReferencedResources(DocumentNode node)
    {
      SceneViewModel nodeViewModel = this.viewModel.GetViewModel(node.DocumentRoot, false);
      Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindAllReferencedResources(node, this.cachedResources, (Microsoft.Expression.DesignSurface.Utility.ResourceHelper.PostOrderOperation) (evaluatedResource =>
      {
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) evaluatedResource.Parent.Clone(this.DocumentContext);
        this.CacheImages(documentCompositeNode.Properties[DictionaryEntryNode.ValueProperty]);
        if (node.IsAncestorOf((DocumentNode) evaluatedResource.Parent))
          return;
        this.resources.Add((DictionaryEntryNode) nodeViewModel.GetSceneNode((DocumentNode) documentCompositeNode));
      }));
    }

    private void CacheImages(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null && documentPrimitiveNode.Value != null && documentPrimitiveNode.SitePropertyKey != null)
      {
        if (!PlatformTypes.ImageSource.IsAssignableFrom((ITypeId) documentPrimitiveNode.SitePropertyKey.PropertyType))
          return;
        Uri uriValue = DocumentNodeHelper.GetUriValue((DocumentNode) documentPrimitiveNode);
        if (!(uriValue != (Uri) null))
          return;
        Uri uri = documentPrimitiveNode.Context.MakeDesignTimeUri(uriValue);
        KeyValuePair<Uri, string> keyValuePair = new KeyValuePair<Uri, string>(uriValue, uri.OriginalString);
        string index = Guid.NewGuid().ToString((string) null, (IFormatProvider) CultureInfo.CurrentCulture);
        this.imageReferences[index] = keyValuePair;
        DocumentNode documentNode = (DocumentNode) documentPrimitiveNode.Context.CreateNode((ITypeId) node.Type, (IDocumentNodeValue) new DocumentNodeStringValue(index));
        if (documentPrimitiveNode.IsProperty)
          documentPrimitiveNode.Parent.Properties[(IPropertyId) documentPrimitiveNode.SitePropertyKey] = documentNode;
        else
          documentPrimitiveNode.Parent.Children[documentPrimitiveNode.SiteChildIndex] = documentNode;
      }
      else
      {
        foreach (DocumentNode node1 in node.ChildNodes)
          this.CacheImages(node1);
      }
    }

    public static bool FilterResources(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      return documentCompositeNode != null && documentCompositeNode.Type.IsResource;
    }

    private static DocumentNode TryParseBuffer(SceneViewModel viewModel, ITextBuffer textBuffer, ref bool displayErrors)
    {
      XamlParserResults xamlParserResults = new XamlParser(viewModel.Document.DocumentContext, (IReadableSelectableTextBuffer) textBuffer, PlatformTypes.Object).Parse(viewModel.XamlDocument.RootClassAttributes);
      DocumentNode documentNode = xamlParserResults.Errors.Count != 0 || xamlParserResults.RootNode == null ? (DocumentNode) null : xamlParserResults.RootNode;
      if (documentNode == null && displayErrors)
      {
        string str = xamlParserResults.Errors.Count > 0 ? xamlParserResults.Errors[0].Message : "Unknown";
        viewModel.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PasteElementsParseFailedDialogMessage, new object[1]
        {
          (object) str
        }));
        displayErrors = false;
      }
      return documentNode;
    }

    private static DocumentNode TryParseXamlBuffer(SceneViewModel viewModel, CopyItem item, ref bool displayErrors)
    {
      ITextBuffer textBuffer = viewModel.DesignerContext.TextBufferService.CreateTextBuffer();
      string text = PastePackage.ReadXamlTextFromStream(item, viewModel.DocumentRoot.DocumentContext.TypeResolver);
      textBuffer.SetText(0, textBuffer.Length, text);
      return PastePackage.TryParseBuffer(viewModel, textBuffer, ref displayErrors);
    }

    private static string ReadXamlTextFromStream(CopyItem item, ITypeResolver typeResolver)
    {
      string str;
      using (Stream stream = item.GetStream())
      {
        using (StreamReader streamReader = new StreamReader(stream))
        {
          str = streamReader.ReadToEnd();
          if (!string.IsNullOrEmpty(str))
            str = str.Trim();
          if (string.IsNullOrEmpty(str))
            return str;
        }
      }
      IType type = typeResolver.ResolveType(PlatformTypes.ArrayList);
      if (typeResolver.PlatformMetadata.IsNullType((ITypeId) type))
      {
        try
        {
          XDocument xdocument = XDocument.Parse(str);
          if (string.Equals(xdocument.Root.Name.LocalName, "ArrayList", StringComparison.OrdinalIgnoreCase))
          {
            if (xdocument.Root.Name.NamespaceName != null)
            {
              if (!xdocument.Root.Name.NamespaceName.StartsWith("clr-namespace:System.Collections;assembly=mscorlib", StringComparison.OrdinalIgnoreCase))
              {
                if (!xdocument.Root.Name.NamespaceName.StartsWith("clr-namespace:System.Collections;assembly=mscorlib,", StringComparison.OrdinalIgnoreCase))
                  goto label_19;
              }
              str = PastePackage.CoerceArrayListBuffer(str, xdocument.Root.GetPrefixOfNamespace(xdocument.Root.Name.Namespace));
            }
          }
        }
        catch (XmlException ex)
        {
        }
      }
label_19:
      return str;
    }

    private static string CoerceArrayListBuffer(string bufferText, string prefixName)
    {
      string str1 = "<" + prefixName + ":ArrayList ";
      string str2 = "</" + prefixName + ":ArrayList>";
      if (bufferText.StartsWith(str1, StringComparison.OrdinalIgnoreCase) && bufferText.EndsWith(str2, StringComparison.OrdinalIgnoreCase))
      {
        int length = bufferText.Length - str1.Length - str2.Length;
        bufferText = "<ItemCollection " + bufferText.Substring(str1.Length, length) + "</ItemCollection>";
      }
      return bufferText;
    }

    private static byte[] Serialize(XamlDocument document, DocumentNode node)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, Encoding.UTF8))
          new XamlSerializer((IDocumentRoot) document, (IXamlSerializerFilter) new PastePackage.PasteXamlSerializer()).Serialize(node, (TextWriter) streamWriter);
        return memoryStream.ToArray();
      }
    }

    private class PasteXamlSerializer : DefaultXamlSerializerFilter
    {
      public override IXmlNamespace GetReplacementNamespace(IXmlNamespace xmlNamespace)
      {
        if (XmlNamespace.JoltXmlNamespace.Equals((object) xmlNamespace) || XmlNamespace.NetFx2007Namespace.Equals((object) xmlNamespace))
          return (IXmlNamespace) XmlNamespace.AvalonXmlNamespace;
        return xmlNamespace;
      }
    }

    private class PastePackageDataObject : IDataObject
    {
      private Stream dataStream;

      public PastePackageDataObject(PastePackage pastePackage)
      {
        this.dataStream = pastePackage.CreateClipboardStream();
      }

      public bool GetDataPresent(Type format)
      {
        return false;
      }

      public bool GetDataPresent(string format)
      {
        return format == Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name;
      }

      public object GetData(Type format)
      {
        return (object) null;
      }

      public object GetData(string format)
      {
        if (format == Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name)
          return (object) this.dataStream;
        return (object) null;
      }

      public string[] GetFormats()
      {
        return new string[1]
        {
          Microsoft.Expression.Framework.Clipboard.Container.DataFormat.Name
        };
      }

      object IDataObject.GetData(string format, bool autoConvert)
      {
        return this.GetData(format);
      }

      bool IDataObject.GetDataPresent(string format, bool autoConvert)
      {
        return this.GetDataPresent(format);
      }

      string[] IDataObject.GetFormats(bool autoConvert)
      {
        return this.GetFormats();
      }

      void IDataObject.SetData(string format, object data, bool autoConvert)
      {
        throw new InvalidOperationException();
      }

      void IDataObject.SetData(Type format, object data)
      {
        throw new InvalidOperationException();
      }

      void IDataObject.SetData(string format, object data)
      {
        throw new InvalidOperationException();
      }

      void IDataObject.SetData(object data)
      {
        throw new InvalidOperationException();
      }
    }

    private class ArrayListWorkaroundType : IType, IMember, ITypeId, IMemberId
    {
      private IType objectType;
      private IType realArrayListType;

      public IPlatformMetadata PlatformMetadata { get; private set; }

      public IAssembly RuntimeAssembly
      {
        get
        {
          return this.realArrayListType.RuntimeAssembly;
        }
      }

      public TypeConverter TypeConverter
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public IType BaseType
      {
        get
        {
          return this.objectType;
        }
      }

      public bool IsArray
      {
        get
        {
          return this.realArrayListType.IsArray;
        }
      }

      public bool IsInterface
      {
        get
        {
          return this.realArrayListType.IsInterface;
        }
      }

      public bool IsAbstract
      {
        get
        {
          return this.realArrayListType.IsAbstract;
        }
      }

      public bool IsGenericType
      {
        get
        {
          return this.realArrayListType.IsGenericType;
        }
      }

      public bool IsBinding
      {
        get
        {
          return this.realArrayListType.IsBinding;
        }
      }

      public bool IsResource
      {
        get
        {
          return this.realArrayListType.IsResource;
        }
      }

      public bool IsExpression
      {
        get
        {
          return this.realArrayListType.IsExpression;
        }
      }

      public string XamlSourcePath
      {
        get
        {
          return this.realArrayListType.XamlSourcePath;
        }
      }

      public IType ItemType
      {
        get
        {
          return this.objectType;
        }
      }

      public IType NearestResolvedType
      {
        get
        {
          return (IType) this;
        }
      }

      public ITypeMetadata Metadata { get; private set; }

      public IType NullableType
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public bool SupportsNullValues
      {
        get
        {
          return this.realArrayListType.SupportsNullValues;
        }
      }

      public Exception InitializationException
      {
        get
        {
          return this.realArrayListType.InitializationException;
        }
      }

      public Type RuntimeType
      {
        get
        {
          return typeof (ArrayList);
        }
      }

      public ITypeId MemberTypeId
      {
        get
        {
          return this.realArrayListType.MemberTypeId;
        }
      }

      public IType DeclaringType
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public MemberAccessType Access
      {
        get
        {
          return this.realArrayListType.Access;
        }
      }

      public ITypeId DeclaringTypeId
      {
        get
        {
          return this.realArrayListType.DeclaringTypeId;
        }
      }

      public string FullName
      {
        get
        {
          return this.realArrayListType.FullName;
        }
      }

      public string Name
      {
        get
        {
          return this.realArrayListType.Name;
        }
      }

      public bool IsResolvable
      {
        get
        {
          return true;
        }
      }

      public MemberType MemberType
      {
        get
        {
          return this.realArrayListType.MemberType;
        }
      }

      public string UniqueName
      {
        get
        {
          return this.realArrayListType.UniqueName;
        }
      }

      public IXmlNamespace XmlNamespace
      {
        get
        {
          return this.realArrayListType.XmlNamespace;
        }
      }

      public string Namespace
      {
        get
        {
          return this.realArrayListType.Namespace;
        }
      }

      public bool IsBuilt
      {
        get
        {
          return this.realArrayListType.IsBuilt;
        }
      }

      public ArrayListWorkaroundType(ITypeResolver typeResolver, IDesignerDefaultPlatformService designerDefaultPlatformService)
      {
        this.PlatformMetadata = typeResolver.PlatformMetadata;
        this.objectType = this.PlatformMetadata.ResolveType(PlatformTypes.Object);
        this.Metadata = (ITypeMetadata) new ClrObjectMetadata(this.RuntimeType);
        this.Metadata.TypeResolver = typeResolver;
        this.realArrayListType = designerDefaultPlatformService.DefaultPlatform.Metadata.ResolveType(PlatformTypes.ArrayList);
      }

      public bool HasDefaultConstructor(bool supportInternal)
      {
        return this.realArrayListType.HasDefaultConstructor(supportInternal);
      }

      public IConstructorArgumentProperties GetConstructorArgumentProperties()
      {
        throw new NotSupportedException();
      }

      public IList<IType> GetGenericTypeArguments()
      {
        throw new NotSupportedException();
      }

      public IList<IConstructor> GetConstructors()
      {
        throw new NotSupportedException();
      }

      public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
      {
        throw new NotSupportedException();
      }

      public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
      {
        throw new NotSupportedException();
      }

      public bool IsInProject(ITypeResolver typeResolver)
      {
        return true;
      }

      public void InitializeClass()
      {
        throw new NotSupportedException();
      }

      public IMember Clone(ITypeResolver typeResolver)
      {
        throw new NotSupportedException();
      }

      public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
      {
        throw new NotSupportedException();
      }

      public bool IsAssignableFrom(ITypeId type)
      {
        throw new NotSupportedException();
      }
    }
  }
}
