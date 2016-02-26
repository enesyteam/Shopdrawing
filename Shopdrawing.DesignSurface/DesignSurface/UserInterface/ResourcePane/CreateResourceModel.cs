// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.CreateResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class CreateResourceModel : INotifyPropertyChanged
  {
    private ObservableCollection<Named<ResourceContainer>> thisDocumentResourceDictionaries = new ObservableCollection<Named<ResourceContainer>>();
    private ObservableCollection<ResourceContainer> externalResourceDictionaries = new ObservableCollection<ResourceContainer>();
    private bool createAsResource = true;
    private string createAsResourceCheckBoxDescription = string.Empty;
    private Type targetType;
    private TypeAsset targetTypeAsset;
    private Type resourceType;
    private object selectedLocation;
    private SceneDocument applicationDocument;
    private Named<ResourceContainer> explicitResourcesHost;
    private Named<ResourceContainer> selectedResourceDictionary;
    private ResourceContainer selectedExternalResourceDictionaryFile;
    private string keyString;
    private bool applyAutomatically;
    private bool userHasModifiedKeyString;
    private bool canPickResourceOrName;
    private bool validateKeyAsName;
    private bool currentResourceSiteCacheValid;
    private ResourceSite currentResourceSiteCached;
    private SceneViewModel viewModel;
    private string targetPropertyName;
    private CreateResourceModel.ContextFlags contextFlags;
    private SceneNode nearestResourceScopeElement;
    private DocumentNodeNameScope nameScope;
    private SceneNode namedElement;
    private CreateResourceModel.KeyStringDocumentStatus keyStringDocumentStatus;

    public ResourceSite CurrentResourceSite
    {
      get
      {
        if (!this.currentResourceSiteCacheValid)
        {
          if (this.selectedLocation == this.thisDocumentResourceDictionaries && this.selectedResourceDictionary != null)
          {
            ISupportsResources resourcesCollection = this.selectedResourceDictionary.Instance.ResourcesCollection;
            this.currentResourceSiteCached = resourcesCollection == null ? (ResourceSite) null : new ResourceSite(this.viewModel.Document.DocumentContext, resourcesCollection);
          }
          else
          {
            IDocumentRoot documentRoot = (IDocumentRoot) null;
            if (this.applicationDocument != null && this.selectedLocation == this.applicationDocument)
              documentRoot = this.applicationDocument.DocumentRoot;
            else if (this.selectedExternalResourceDictionaryFile != null)
              documentRoot = this.selectedExternalResourceDictionaryFile.Document.DocumentRoot;
            if (documentRoot == null || documentRoot.RootNode == null)
            {
              this.currentResourceSiteCached = (ResourceSite) null;
            }
            else
            {
              ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(documentRoot.RootNode);
              this.currentResourceSiteCached = resourcesCollection != null ? new ResourceSite(documentRoot.DocumentContext, resourcesCollection) : (documentRoot.RootNode != null ? new ResourceSite(documentRoot.RootNode) : (ResourceSite) null);
            }
          }
          this.currentResourceSiteCacheValid = true;
          this.OnPropertyChanged("ResourceIsValid");
        }
        return this.currentResourceSiteCached;
      }
    }

    public SceneDocument ExternalDocument
    {
      get
      {
        if (this.selectedLocation != this.thisDocumentResourceDictionaries)
        {
          if (this.selectedLocation == this.applicationDocument)
            return this.applicationDocument;
          if (this.selectedExternalResourceDictionaryFile != null)
            return this.selectedExternalResourceDictionaryFile.Document;
        }
        return (SceneDocument) null;
      }
    }

    public string TargetPropertyName
    {
      get
      {
        return this.targetPropertyName;
      }
      set
      {
        this.targetPropertyName = value;
        this.OnPropertyChanged("TargetPropertyName");
      }
    }

    public string KeyString
    {
      get
      {
        return this.keyString;
      }
      set
      {
        if (value != null)
          value = value.Trim();
        if (!(this.keyString != value))
          return;
        this.userHasModifiedKeyString = true;
        this.keyString = value;
        this.OnPropertyChanged("KeyString");
      }
    }

    public bool KeyStringHasIssues
    {
      get
      {
        return this.keyStringDocumentStatus != CreateResourceModel.KeyStringDocumentStatus.None;
      }
    }

    public bool ApplyAutomatically
    {
      get
      {
        return this.applyAutomatically;
      }
      set
      {
        if (this.applyAutomatically == value)
          return;
        this.applyAutomatically = value;
        this.OnPropertyChanged("ApplyAutomatically");
        this.OnPropertyChanged("ResourceIsValid");
      }
    }

    public bool CanApplyAutomatically
    {
      get
      {
        if (this.targetType != (Type) null)
          return this.TestFlag(CreateResourceModel.ContextFlags.CanApplyAutomatically);
        return false;
      }
    }

    public bool CanDefineInApplicationRoot
    {
      get
      {
        if (this.ApplicationDocument != null)
          return !this.TestFlag(CreateResourceModel.ContextFlags.CanOnlyUseCurrentDocument);
        return false;
      }
    }

    public bool CanDefineInThisDocument
    {
      get
      {
        if (this.CanPickScope)
          return this.ThisDocumentResourceDictionaries.Count > 0;
        return false;
      }
    }

    public bool CanDefineInOtherDocument
    {
      get
      {
        if (this.CanPickScope && this.OtherDocuments.Count > 0)
          return !this.TestFlag(CreateResourceModel.ContextFlags.CanOnlyUseCurrentDocument);
        return false;
      }
    }

    public bool CanCreateOtherDocument
    {
      get
      {
        if (this.CanPickScope)
          return !this.TestFlag(CreateResourceModel.ContextFlags.CanOnlyUseCurrentDocument);
        return false;
      }
    }

    public bool CanPickScope
    {
      get
      {
        return !this.TestFlag(CreateResourceModel.ContextFlags.CanOnlyDefineKey);
      }
    }

    public bool CanPickResourceOrName
    {
      get
      {
        return this.canPickResourceOrName;
      }
      set
      {
        this.canPickResourceOrName = value;
      }
    }

    public bool CreateAsResource
    {
      get
      {
        return this.createAsResource;
      }
      set
      {
        this.createAsResource = value;
      }
    }

    public string CreateAsResourceCheckBoxDescription
    {
      get
      {
        return this.createAsResourceCheckBoxDescription;
      }
      set
      {
        this.createAsResourceCheckBoxDescription = value;
      }
    }

    public bool HasCreateAsResourceCheckBoxDescription
    {
      get
      {
        return !string.IsNullOrEmpty(this.createAsResourceCheckBoxDescription);
      }
    }

    public bool ValidateKeyAsName
    {
      get
      {
        return this.validateKeyAsName;
      }
      set
      {
        this.validateKeyAsName = value;
      }
    }

    public DocumentNodeNameScope NameScope
    {
      get
      {
        return this.nameScope;
      }
      set
      {
        this.nameScope = value;
      }
    }

    public SceneNode NamedElement
    {
      get
      {
        return this.namedElement;
      }
      set
      {
        this.namedElement = value;
      }
    }

    public string CommitActionName
    {
      get
      {
        return StringTable.CreateResourceDialogOKCommitString;
      }
    }

    public string KeyStringWarningText
    {
      get
      {
        CreateResourceModel.KeyStringDocumentStatus status1 = this.keyStringDocumentStatus;
        bool flag = this.CheckStatus(status1, CreateResourceModel.KeyStringDocumentStatus.IncompatibleTypes);
        CreateResourceModel.KeyStringDocumentStatus status2 = status1 & ~CreateResourceModel.KeyStringDocumentStatus.IncompatibleTypes;
        foreach (CreateResourceModel.KeyStringDocumentStatus test in Enum.GetValues(typeof (CreateResourceModel.KeyStringDocumentStatus)))
        {
          if (test != CreateResourceModel.KeyStringDocumentStatus.None && this.CheckStatus(status2, test))
          {
            status2 = test;
            break;
          }
        }
        if (flag && (status2 == CreateResourceModel.KeyStringDocumentStatus.Direct || status2 == CreateResourceModel.KeyStringDocumentStatus.Masked || status2 == CreateResourceModel.KeyStringDocumentStatus.Masking))
          status2 |= CreateResourceModel.KeyStringDocumentStatus.IncompatibleTypes;
        switch (status2)
        {
          case CreateResourceModel.KeyStringDocumentStatus.MaskedIncompatibleTypes:
            return StringTable.CreateResourceKeyStringIssueMaskedIncompatibleTypes;
          case CreateResourceModel.KeyStringDocumentStatus.MaskingIncompatibleTypes:
            return StringTable.CreateResourceKeyStringIssueMaskingIncompatibleTypes;
          case CreateResourceModel.KeyStringDocumentStatus.InvalidName:
            return StringTable.CreateResourceKeyStringInvalidName;
          case CreateResourceModel.KeyStringDocumentStatus.None:
            return string.Empty;
          case CreateResourceModel.KeyStringDocumentStatus.Empty:
            return StringTable.CreateResourceKeyStringIssueEmpty;
          case CreateResourceModel.KeyStringDocumentStatus.InvalidChars:
            return StringTable.CreateResourceKeyStringIssueInvalidChars;
          case CreateResourceModel.KeyStringDocumentStatus.Direct:
            return StringTable.CreateResourceKeyStringIssueDirectConflict;
          case CreateResourceModel.KeyStringDocumentStatus.Masked:
            return StringTable.CreateResourceKeyStringIssueMasked;
          case CreateResourceModel.KeyStringDocumentStatus.Masking:
            return StringTable.CreateResourceKeyStringIssueMasking;
          case CreateResourceModel.KeyStringDocumentStatus.DirectIncompatibleTypes:
            return StringTable.CreateResourceKeyStringIssueDirectConflictIncompatibleTypes;
          default:
            return string.Empty;
        }
      }
    }

    public Type TargetType
    {
      get
      {
        return this.targetType;
      }
      set
      {
        this.targetType = value;
        this.OnPropertyChanged("TargetType");
      }
    }

    internal TypeAsset TargetTypeAsset
    {
      get
      {
        return this.targetTypeAsset;
      }
      set
      {
        this.targetTypeAsset = value;
      }
    }

    private string TargetTypeName
    {
      get
      {
        if (!(this.targetType != (Type) null))
          return string.Empty;
        if (this.targetTypeAsset != null)
          return this.targetTypeAsset.Name;
        return this.targetType.Name;
      }
    }

    public Type ResourceType
    {
      get
      {
        return this.resourceType;
      }
      set
      {
        this.resourceType = value;
        this.OnPropertyChanged("ResourceType");
      }
    }

    public object SelectedLocation
    {
      get
      {
        return this.selectedLocation;
      }
      set
      {
        this.selectedLocation = value;
        this.OnPropertyChanged("SelectedLocation");
      }
    }

    public SceneDocument ApplicationDocument
    {
      get
      {
        return this.applicationDocument;
      }
      set
      {
        this.applicationDocument = value;
        this.OnPropertyChanged("ApplicationDocument");
        this.OnPropertyChanged("CanDefineInApplicationRoot");
      }
    }

    public ObservableCollection<Named<ResourceContainer>> ThisDocumentResourceDictionaries
    {
      get
      {
        return this.thisDocumentResourceDictionaries;
      }
    }

    public ObservableCollection<ResourceContainer> OtherDocuments
    {
      get
      {
        return this.externalResourceDictionaries;
      }
    }

    public Named<ResourceContainer> SelectedResourceDictionary
    {
      get
      {
        return this.selectedResourceDictionary;
      }
      set
      {
        if (this.selectedResourceDictionary == value)
          return;
        this.selectedResourceDictionary = value;
        this.OnPropertyChanged("SelectedResourceDictionary");
      }
    }

    public ResourceContainer SelectedExternalResourceDictionaryFile
    {
      get
      {
        return this.selectedExternalResourceDictionaryFile;
      }
      set
      {
        this.selectedExternalResourceDictionaryFile = value;
        this.OnPropertyChanged("SelectedExternalResourceDictionaryFile");
      }
    }

    public bool ResourceIsValid
    {
      get
      {
        if (this.keyStringDocumentStatus != CreateResourceModel.KeyStringDocumentStatus.None)
          return this.keyStringDocumentStatus == CreateResourceModel.KeyStringDocumentStatus.Masking;
        return true;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public CreateResourceModel(SceneViewModel viewModel, ResourceManager resourceManager, Type resourceType, Type targetType, string targetPropertyName, SceneElement explicitResourcesHost, SceneNode nearestResourceScopeElement, CreateResourceModel.ContextFlags contextFlags)
    {
      this.viewModel = viewModel;
      this.targetPropertyName = targetPropertyName;
      this.nearestResourceScopeElement = nearestResourceScopeElement;
      this.contextFlags = contextFlags;
      this.resourceType = resourceType;
      this.targetType = targetType;
      this.applicationDocument = this.viewModel.Document.ApplicationSceneDocument;
      this.selectedLocation = (object) this.thisDocumentResourceDictionaries;
      this.UpdateResourceList(resourceManager);
      if (explicitResourcesHost != null)
      {
        bool flag = false;
        foreach (Named<ResourceContainer> named in (Collection<Named<ResourceContainer>>) this.thisDocumentResourceDictionaries)
        {
          if (named.Instance.DocumentNode == explicitResourcesHost.DocumentNode)
          {
            this.SelectedResourceDictionary = named;
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this.explicitResourcesHost = this.CreateNamedResourceContainer(new NodeResourceContainer(resourceManager, (SceneNode) explicitResourcesHost));
          this.thisDocumentResourceDictionaries.Add(this.explicitResourcesHost);
          this.SelectedLocation = (object) this.ThisDocumentResourceDictionaries;
          this.SelectedResourceDictionary = this.explicitResourcesHost;
        }
      }
      if (this.thisDocumentResourceDictionaries.Count == 0)
      {
        bool flag = false;
        foreach (ResourceContainer resourceContainer in (Collection<ResourceContainer>) this.externalResourceDictionaries)
        {
          if (resourceContainer.Document == this.viewModel.Document)
          {
            this.SelectedLocation = (object) this.OtherDocuments;
            flag = true;
            break;
          }
        }
        if (!flag)
          this.SelectedLocation = (object) this.applicationDocument;
      }
      if (this.keyString != null)
        return;
      this.RegenerateUniqueResourceKey();
    }

    internal void UpdateResourceList(ResourceManager resourceManager)
    {
      ResourceContainer resourceContainer1 = this.selectedExternalResourceDictionaryFile;
      ResourceContainer resourceContainer2 = this.selectedResourceDictionary != null ? this.selectedResourceDictionary.Instance : resourceManager.ActiveRootContainer;
      this.externalResourceDictionaries.Clear();
      this.thisDocumentResourceDictionaries.Clear();
      this.selectedExternalResourceDictionaryFile = (ResourceContainer) null;
      this.selectedResourceDictionary = (Named<ResourceContainer>) null;
      ResourceContainer resourceContainer3 = resourceManager.TopLevelResourceContainer;
      if (resourceManager.ActiveSceneViewModel == this.viewModel && resourceManager.ActiveRootContainer is NodeResourceContainer)
      {
        foreach (ResourceContainer resourceContainer4 in resourceManager.ActiveResourceContainers)
        {
          if (resourceContainer4.IsInScope)
          {
            if (resourceContainer4 == resourceContainer3)
              resourceContainer3 = (ResourceContainer) null;
            NodeResourceContainer nodeResourceContainer;
            this.thisDocumentResourceDictionaries.Add((nodeResourceContainer = resourceContainer4 as NodeResourceContainer) == null ? new Named<ResourceContainer>(resourceContainer4.Name, resourceContainer4, resourceContainer4.DocumentReference.Path) : this.CreateNamedResourceContainer(nodeResourceContainer));
            this.AddExternalDictionaries(resourceManager, resourceContainer4);
          }
        }
      }
      IProject currentProject = ProjectHelper.GetProject(this.viewModel.DesignerContext.ProjectManager, this.viewModel.Document.DocumentContext);
      foreach (DocumentResourceContainer resourceContainer4 in Enumerable.Where<DocumentResourceContainer>((IEnumerable<DocumentResourceContainer>) resourceManager.DocumentResourceContainers, (Func<DocumentResourceContainer, bool>) (documentContainer =>
      {
        if (!documentContainer.ProjectItem.ContainsDesignTimeResources)
          return false;
        if (documentContainer.ProjectItem.Project != currentProject)
          return ProjectHelper.DoesProjectReferencesContainTarget(currentProject, documentContainer.ProjectContext);
        return true;
      })))
        this.AddExternalDictionaries(resourceManager, (ResourceContainer) resourceContainer4);
      if (resourceContainer3 != null)
      {
        if (resourceContainer3 is DocumentResourceContainer && resourceContainer3.Document != this.applicationDocument)
          this.externalResourceDictionaries.Add(resourceContainer3);
        this.AddExternalDictionaries(resourceManager, resourceContainer3);
      }
      if (this.explicitResourcesHost != null)
        this.thisDocumentResourceDictionaries.Add(this.explicitResourcesHost);
      foreach (Named<ResourceContainer> named in (Collection<Named<ResourceContainer>>) this.thisDocumentResourceDictionaries)
      {
        if (named.Instance == resourceContainer2)
        {
          this.SelectedResourceDictionary = named;
          break;
        }
      }
      if (this.externalResourceDictionaries.Contains(resourceContainer1))
        this.SelectedExternalResourceDictionaryFile = resourceContainer1;
      this.OnPropertyChanged("CanDefineInOtherDocument");
    }

    private Named<ResourceContainer> CreateNamedResourceContainer(NodeResourceContainer nodeResourceContainer)
    {
      string str = nodeResourceContainer.Node.Name ?? StringTable.CreateResourceResourceDictionaryNoNameName;
      return new Named<ResourceContainer>(nodeResourceContainer.Node.TargetType.Name + ": " + str, (ResourceContainer) nodeResourceContainer, nodeResourceContainer.Node.UniqueID);
    }

    private void AddExternalDictionaries(ResourceManager resourceManager, ResourceContainer resourceContainer)
    {
      List<ResourceDictionaryItem> list = new List<ResourceDictionaryItem>();
      resourceManager.FindAllReachableDictionaries(resourceContainer, (ICollection<ResourceDictionaryItem>) list);
      foreach (ResourceDictionaryItem resourceDictionaryItem in list)
      {
        DocumentResourceContainer resourceContainer1 = resourceManager.FindResourceContainer(resourceDictionaryItem.DesignTimeSource) as DocumentResourceContainer;
        if (resourceContainer1 != null && !this.externalResourceDictionaries.Contains((ResourceContainer) resourceContainer1) && PlatformTypes.PlatformsCompatible(this.viewModel.ProjectContext.PlatformMetadata, resourceContainer1.ProjectContext.PlatformMetadata))
          this.externalResourceDictionaries.Add((ResourceContainer) resourceContainer1);
      }
      if (this.selectedExternalResourceDictionaryFile != null || this.externalResourceDictionaries.Count <= 0)
        return;
      this.selectedExternalResourceDictionaryFile = this.externalResourceDictionaries[0];
    }

    public void SetContextFlag(CreateResourceModel.ContextFlags newFlag)
    {
      this.contextFlags |= newFlag;
    }

    public void ClearContextFlag(CreateResourceModel.ContextFlags killFlag)
    {
      this.contextFlags &= ~killFlag;
    }

    public bool TestFlag(CreateResourceModel.ContextFlags set)
    {
      return (this.contextFlags & set) != CreateResourceModel.ContextFlags.None;
    }

    public int IndexInResourceSite(DocumentNode node)
    {
      ResourceSite currentResourceSite = this.CurrentResourceSite;
      if (currentResourceSite != null)
      {
        DocumentCompositeNode resourcesDictionary = currentResourceSite.ResourcesDictionary;
        if (resourcesDictionary != null)
          return CreateResourceModel.IndexInResourceSite(node, resourcesDictionary);
      }
      return -1;
    }

    public static int IndexInResourceSite(DocumentNode node, DocumentCompositeNode dictionaryNode)
    {
      for (; node.Parent != null; node = (DocumentNode) node.Parent)
      {
        if (node.Parent == dictionaryNode)
          return dictionaryNode.Children.IndexOf(node);
      }
      return -1;
    }

    public void ResetResourceKey()
    {
      this.RegenerateUniqueResourceKey();
      this.OnPropertyChanged("KeyString");
    }

    private void RegenerateUniqueResourceKey()
    {
      if (!this.userHasModifiedKeyString)
      {
        string name = this.TargetTypeName + (this.resourceType != (Type) null ? this.resourceType.Name : string.Empty) + "1";
        if (this.CurrentResourceSite != null && !this.applyAutomatically)
        {
          string prefix;
          int suffixValue;
          if (!ResourceSite.ParseKeyString(name, out prefix, out suffixValue))
            suffixValue = 1;
          this.keyString = prefix + suffixValue.ToString();
          while (this.AnalyzeKeyStringWorker() != CreateResourceModel.KeyStringDocumentStatus.None)
          {
            ++suffixValue;
            this.keyString = prefix + suffixValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
      }
      this.AnalyzeKeyString();
    }

    private void AnalyzeKeyString()
    {
      CreateResourceModel.KeyStringDocumentStatus stringDocumentStatus = this.keyStringDocumentStatus;
      this.keyStringDocumentStatus = this.AnalyzeKeyStringWorker();
      if (this.keyStringDocumentStatus == stringDocumentStatus)
        return;
      this.OnPropertyChanged("KeyStringHasIssues");
      this.OnPropertyChanged("ResourceIsValid");
      this.OnPropertyChanged("CommitActionName");
      this.OnPropertyChanged("KeyStringWarningText");
    }

    private CreateResourceModel.KeyStringDocumentStatus AnalyzeKeyStringWorker()
    {
      CreateResourceModel.KeyStringDocumentStatus stringDocumentStatus1 = CreateResourceModel.KeyStringDocumentStatus.None;
      if (string.IsNullOrEmpty(this.keyString))
      {
        stringDocumentStatus1 = CreateResourceModel.KeyStringDocumentStatus.Empty;
      }
      else
      {
        if (this.viewModel == null || this.viewModel.Document == null)
          return stringDocumentStatus1;
        if (this.ValidateKeyAsName && !new SceneNodeIDHelper(this.viewModel, this.NameScope).IsValidElementID(this.NamedElement, this.keyString))
          stringDocumentStatus1 |= CreateResourceModel.KeyStringDocumentStatus.InvalidName;
        if (this.nearestResourceScopeElement != null)
        {
          IDocumentContext documentContext = this.viewModel.Document.DocumentContext;
          DocumentNode keyNode = !this.applyAutomatically ? (DocumentNode) documentContext.CreateNode(this.keyString) : (DocumentNode) documentContext.CreateNode(PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) documentContext.TypeResolver.GetType(this.TargetType)));
          ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator(this.viewModel.DocumentRootResolver);
          List<DocumentCompositeNode> resourcesHostNodePath1 = new List<DocumentCompositeNode>();
          DocumentNode conflictingResource1 = expressionEvaluator.EvaluateResourceAndCollectionPath(this.nearestResourceScopeElement.DocumentNodePath, ResourceReferenceType.Dynamic, keyNode, (ICollection<DocumentCompositeNode>) resourcesHostNodePath1, (ICollection<IDocumentRoot>) null);
          CreateResourceModel.KeyStringDocumentStatus stringDocumentStatus2 = stringDocumentStatus1 | this.DoesKeyConflict(conflictingResource1, resourcesHostNodePath1);
          List<DocumentCompositeNode> resourcesHostNodePath2 = new List<DocumentCompositeNode>();
          DocumentNode conflictingResource2 = expressionEvaluator.EvaluateResourceAndCollectionPath(this.nearestResourceScopeElement.DocumentNodePath, ResourceReferenceType.Static, keyNode, (ICollection<DocumentCompositeNode>) resourcesHostNodePath2, (ICollection<IDocumentRoot>) null);
          stringDocumentStatus1 = stringDocumentStatus2 | this.DoesKeyConflict(conflictingResource2, resourcesHostNodePath2);
        }
        else if (this.CurrentResourceSite != null)
        {
          DocumentCompositeNode resource = this.CurrentResourceSite.FindResource(this.viewModel.DocumentRootResolver, this.keyString, (ICollection<DocumentCompositeNode>) null, (ICollection<IDocumentRoot>) null);
          if (resource != null)
          {
            if (!this.resourceType.IsAssignableFrom(resource.TargetType) && !resource.TargetType.IsAssignableFrom(this.resourceType))
              stringDocumentStatus1 |= CreateResourceModel.KeyStringDocumentStatus.IncompatibleTypes;
            stringDocumentStatus1 |= CreateResourceModel.KeyStringDocumentStatus.Direct;
          }
        }
      }
      return stringDocumentStatus1;
    }

    private CreateResourceModel.KeyStringDocumentStatus DoesKeyConflict(DocumentNode conflictingResource, List<DocumentCompositeNode> resourcesHostNodePath)
    {
      CreateResourceModel.KeyStringDocumentStatus stringDocumentStatus = CreateResourceModel.KeyStringDocumentStatus.None;
      if (conflictingResource != null)
      {
        DocumentCompositeNode currentResourcesDictionary = this.CurrentResourceSite != null ? this.CurrentResourceSite.HostNode : (DocumentCompositeNode) null;
        if (currentResourcesDictionary != null)
        {
          int index = resourcesHostNodePath.FindIndex((Predicate<DocumentCompositeNode>) (arg => object.ReferenceEquals((object) currentResourcesDictionary, (object) arg)));
          if (!this.resourceType.IsAssignableFrom(conflictingResource.TargetType) && !conflictingResource.TargetType.IsAssignableFrom(this.resourceType))
            stringDocumentStatus |= CreateResourceModel.KeyStringDocumentStatus.IncompatibleTypes;
          if (index == -1)
            stringDocumentStatus |= CreateResourceModel.KeyStringDocumentStatus.Masked;
          else if (index == resourcesHostNodePath.Count - 1)
            stringDocumentStatus |= CreateResourceModel.KeyStringDocumentStatus.Direct;
          else
            stringDocumentStatus |= CreateResourceModel.KeyStringDocumentStatus.Masking;
        }
      }
      return stringDocumentStatus;
    }

    private bool CheckStatus(CreateResourceModel.KeyStringDocumentStatus status, CreateResourceModel.KeyStringDocumentStatus test)
    {
      return (status & test) == test;
    }

    public DocumentCompositeNode CreateResource(DocumentNode newResourceNode, IPropertyId targetTypeProperty, int index)
    {
      ResourceSite currentResourceSite = this.CurrentResourceSite;
      if (currentResourceSite == null)
        return (DocumentCompositeNode) null;
      IDocumentContext documentContext = currentResourceSite.DocumentContext;
      if (newResourceNode.Context != documentContext)
        newResourceNode = newResourceNode.Clone(documentContext);
      if (this.TargetType != (Type) null)
      {
        IType type = documentContext.TypeResolver.GetType(this.TargetType);
        ((DocumentCompositeNode) newResourceNode).Properties[targetTypeProperty] = (DocumentNode) documentContext.CreateNode(PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) type));
      }
      if (this.selectedLocation == this.thisDocumentResourceDictionaries && this.selectedResourceDictionary != null)
      {
        this.selectedResourceDictionary.Instance.EnsureResourceDictionaryNode();
        this.currentResourceSiteCacheValid = false;
      }
      DocumentNode keyNode;
      if (this.ApplyAutomatically)
      {
        keyNode = (DocumentNode) null;
      }
      else
      {
        string uniqueResourceKey = currentResourceSite.GetUniqueResourceKey(this.KeyString);
        keyNode = (DocumentNode) documentContext.CreateNode(uniqueResourceKey);
      }
      SceneDocument externalDocument = this.ExternalDocument;
      DocumentCompositeNode resource;
      if (externalDocument != null)
      {
        using (SceneEditTransaction editTransaction = externalDocument.CreateEditTransaction(StringTable.PropertyCreateResourceInFileDescription))
        {
          resource = currentResourceSite.CreateResource(keyNode, newResourceNode, index);
          editTransaction.Commit();
        }
      }
      else
        resource = currentResourceSite.CreateResource(keyNode, newResourceNode, index);
      return resource;
    }

    public DocumentNode CreateResourceReference(IDocumentContext documentContext, DocumentCompositeNode resourceNode, bool useStaticResource)
    {
      DocumentNode resourceEntryKey = ResourceNodeHelper.GetResourceEntryKey(resourceNode);
      if (resourceEntryKey == null)
        return (DocumentNode) null;
      DocumentNode keyNode = resourceEntryKey.Clone(documentContext);
      if (useStaticResource)
        return (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(documentContext, keyNode);
      return (DocumentNode) DocumentNodeUtilities.NewDynamicResourceNode(documentContext, keyNode);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (propertyName == "SelectedLocation" || propertyName == "SelectedResourceDictionary" || (propertyName == "SelectedExternalResourceDictionaryFile" || propertyName == "TargetType") || propertyName == "ResourceType")
      {
        this.currentResourceSiteCacheValid = false;
        this.RegenerateUniqueResourceKey();
      }
      else if (propertyName == "ApplyAutomatically" || propertyName == "KeyString")
        this.AnalyzeKeyString();
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [Flags]
    public enum ContextFlags
    {
      None = 0,
      CanApplyAutomatically = 1,
      CanOnlyUseCurrentDocument = 2,
      CanOnlyDefineKey = 4,
    }

    [Flags]
    private enum KeyStringDocumentStatus
    {
      None = 0,
      Empty = 1,
      InvalidChars = 2,
      Direct = 4,
      Masked = 8,
      Masking = 16,
      IncompatibleTypes = 32,
      InvalidName = 64,
      DirectIncompatibleTypes = IncompatibleTypes | Direct,
      MaskedIncompatibleTypes = IncompatibleTypes | Masked,
      MaskingIncompatibleTypes = IncompatibleTypes | Masking,
    }
  }
}
