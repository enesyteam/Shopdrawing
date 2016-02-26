// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.EventsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class EventsModel : NotifyingObject
  {
    private DesignerContext designerContext;
    private SceneNodeSubscription<object, EventHandlerModel> eventSubscription;
    private EventsModel.Model model;
    private bool hiddenMode;

    public bool IsEnabled
    {
      get
      {
        if (this.model != null)
          return this.model.State == EventsModel.State.Enabled;
        return false;
      }
    }

    public bool HiddenMode
    {
      get
      {
        return this.hiddenMode;
      }
      set
      {
        this.hiddenMode = value;
        this.UpdateModel();
      }
    }

    public string Message
    {
      get
      {
        if (this.model != null && this.model.State == EventsModel.State.NoCodeBehind)
          return StringTable.EventPaneNoCodeBehindMessage;
        return string.Empty;
      }
    }

    public ICollection<EventHandlerModel> EventHandlers
    {
      get
      {
        if (this.model == null)
          return (ICollection<EventHandlerModel>) ReadOnlyCollections<EventHandlerModel>.Empty;
        return this.model.EventHandlers;
      }
    }

    internal EventsModel(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.eventSubscription = new SceneNodeSubscription<object, EventHandlerModel>();
      this.eventSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentChild, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (obj => obj.DocumentNode.IsProperty && typeof (Delegate).IsAssignableFrom(PlatformTypeHelper.GetPropertyType(obj.DocumentNode.SitePropertyKey))), SearchScope.NodeTreeSelf), (ISearchPredicate) new TargetTypePredicate(PlatformTypes.Delegate))
      });
      this.Attach();
    }

    private void Attach()
    {
      this.designerContext.ProjectManager.OptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.OnOptionsModelPropertyChanged);
      this.eventSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, EventHandlerModel>.PathNodeInsertedHandler(this.EventSubscription_Inserted));
      this.eventSubscription.PathNodeRemoved += new SceneNodeSubscription<object, EventHandlerModel>.PathNodeRemovedListener(this.EventSubscription_Removed);
      this.eventSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, EventHandlerModel>.PathNodeContentChangedListener(this.EventSubscription_ContentChanged);
      this.UpdateModel();
    }

    private void Detach()
    {
      this.designerContext.ProjectManager.OptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.OnOptionsModelPropertyChanged);
      this.eventSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<object, EventHandlerModel>.PathNodeInsertedHandler) null);
      this.eventSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, EventHandlerModel>.PathNodeRemovedListener(this.EventSubscription_Removed);
      this.eventSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, EventHandlerModel>.PathNodeContentChangedListener(this.EventSubscription_ContentChanged);
      this.eventSubscription.CurrentViewModel = (SceneViewModel) null;
      this.UpdateModel();
    }

    private void OnOptionsModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == ProjectSystemOptionsModel.UseVisualStudioEventHandlerSupportProperty))
        return;
      if (this.model != null)
      {
        this.model.Dispose();
        this.model = (EventsModel.Model) null;
      }
      this.UpdateModel();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.IsRadicalChange || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable | SceneViewModel.ViewStateBits.ElementSelection))
        this.UpdateModel();
      this.eventSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      if (e.OldView != null && e.NewView == null)
        this.Detach();
      else if (e.OldView == null && e.NewView != null)
        this.Attach();
      else
        this.UpdateModel();
    }

    private EventHandlerModel EventSubscription_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      DocumentCompositeNode documentCompositeNode = newPathNode.Parent.DocumentNode as DocumentCompositeNode;
      EventHandlerModel eventHandlerModel1 = (EventHandlerModel) null;
      if (documentCompositeNode != null)
      {
        IPropertyId propertyId = (IPropertyId) newPathNode.Parent.GetPropertyForChild(newPathNode);
        if (propertyId is IEvent)
        {
          foreach (EventHandlerModel eventHandlerModel2 in (IEnumerable<EventHandlerModel>) this.EventHandlers)
          {
            if (eventHandlerModel2.EventKey == propertyId)
            {
              eventHandlerModel1 = eventHandlerModel2;
              break;
            }
          }
          if (eventHandlerModel1 != null)
            eventHandlerModel1.Refresh();
        }
      }
      return eventHandlerModel1;
    }

    private void EventSubscription_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, EventHandlerModel oldContent)
    {
      if (oldContent == null)
        return;
      oldContent.Refresh();
    }

    private void EventSubscription_ContentChanged(object sender, SceneNode pathNode, EventHandlerModel content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (content == null)
        return;
      content.Refresh();
    }

    private SceneElement GetSelection()
    {
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      if (activeSceneViewModel != null && activeSceneViewModel.IsEditable && activeSceneViewModel.DefaultView.IsDesignSurfaceVisible)
      {
        SceneElementSelectionSet elementSelectionSet = activeSceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count == 1 && (elementSelectionSet.Selection[0] is BaseFrameworkElement || elementSelectionSet.Selection[0] is UIElement3DElement))
          return elementSelectionSet.Selection[0];
      }
      return (SceneElement) null;
    }

    private void UpdateModel()
    {
      SceneElement selection = this.GetSelection();
      if (selection != null && !this.HiddenMode)
      {
        if (this.model != null && selection.DocumentNode == this.model.Node)
          return;
      }
      else if (this.model == null)
        return;
      if (this.model != null)
      {
        this.model.Dispose();
        this.model = (EventsModel.Model) null;
      }
      if (selection != null && !this.HiddenMode)
      {
        SceneViewModel viewModel = selection.ViewModel;
        List<SceneNode> list = new List<SceneNode>();
        DocumentCompositeNode node = selection.DocumentNode as DocumentCompositeNode;
        if (node != null)
          list.Add((SceneNode) selection);
        this.model = new EventsModel.Model(this.designerContext, viewModel.Document, node);
        this.eventSubscription.SetBasisNodes(viewModel, (IEnumerable<SceneNode>) list);
      }
      this.OnPropertyChanged("IsEnabled");
      this.OnPropertyChanged("Message");
      this.OnPropertyChanged("EventHandlers");
    }

    public enum State
    {
      Unknown,
      NoCodeBehind,
      Enabled,
    }

    private sealed class Model : IDisposable
    {
      private readonly DesignerContext designerContext;
      private ISolution solution;
      private IProject project;
      private readonly SceneDocument document;
      private readonly DocumentCompositeNode node;
      private ICollection<EventHandlerModel> eventHandlers;
      private EventsModel.State state;

      public DocumentCompositeNode Node
      {
        get
        {
          return this.node;
        }
      }

      public EventsModel.State State
      {
        get
        {
          this.CacheStateIfNecessary();
          return this.state;
        }
      }

      public ICollection<EventHandlerModel> EventHandlers
      {
        get
        {
          this.CacheStateIfNecessary();
          return this.eventHandlers;
        }
      }

      public Model(DesignerContext designerContext, SceneDocument document, DocumentCompositeNode node)
      {
        this.designerContext = designerContext;
        this.document = document;
        this.node = node;
        this.state = EventsModel.State.Unknown;
      }

      public void Dispose()
      {
        this.project = (IProject) null;
      }

      private void CacheStateIfNecessary()
      {
        if (this.eventHandlers != null)
          return;
        this.eventHandlers = (ICollection<EventHandlerModel>) new List<EventHandlerModel>();
        this.solution = this.designerContext.ProjectManager.CurrentSolution;
        this.project = ProjectHelper.GetProject(this.designerContext.ProjectManager, this.document.DocumentContext);
        if (this.project == null)
          return;
        ITypeId typeId = (ITypeId) this.document.XamlDocument.CodeBehindClass;
        if (typeId == null)
        {
          this.state = EventsModel.State.NoCodeBehind;
        }
        else
        {
          if (this.node == null)
            return;
          IProjectItem projectItem = this.project.FindItem(this.document.DocumentReference);
          if (projectItem == null)
            return;
          IProjectItem codeBehindItem = projectItem.CodeBehindItem;
          if (codeBehindItem == null)
          {
            this.state = EventsModel.State.NoCodeBehind;
          }
          else
          {
            ICodeGeneratorHost codeGeneratorHost = codeBehindItem.DocumentType as ICodeGeneratorHost;
            if (codeGeneratorHost == null)
              return;
            ITypeDeclaration typeInFile = this.designerContext.CodeModelService.FindTypeInFile((object) this.solution, (object) codeBehindItem, (IEnumerable<string>) EventsModel.Model.GetProjectItemPath(codeBehindItem), typeId.FullName);
            CodeDomProvider codeDomProvider = codeGeneratorHost.CodeDomProvider;
            EventHandlerProvider eventHandlerProvider = new EventHandlerProvider(this.designerContext, this.document, typeInFile);
            this.state = EventsModel.State.Enabled;
            IProjectContext projectContext = this.document.ProjectContext;
            IType type = this.node.Type;
            List<EventInformation> list = new List<EventInformation>(EventInformation.GetEventsForType((ITypeResolver) projectContext, type, MemberType.LocalEvent));
            EventDescriptorCollection events = TypeUtilities.GetEvents(type.RuntimeType);
            list.Sort(new Comparison<EventInformation>(EventInformation.CompareNames));
            foreach (EventInformation eventInfo in list)
            {
              EventDescriptor eventDescriptor = events[eventInfo.EventName];
              AttributeCollection eventAttributes = eventDescriptor != null ? TypeUtilities.GetAttributes((MemberDescriptor) eventDescriptor) : (AttributeCollection) null;
              this.eventHandlers.Add(new EventHandlerModel(eventInfo, this.node, codeDomProvider, (IEventHandlerProvider) eventHandlerProvider, eventAttributes));
            }
          }
        }
      }

      private static string[] GetProjectItemPath(IProjectItem projectItem)
      {
        List<string> list = new List<string>();
        for (; projectItem != null; projectItem = projectItem.Parent)
          list.Add(projectItem.DocumentReference.DisplayName);
        list.Reverse();
        return list.ToArray();
      }
    }
  }
}
