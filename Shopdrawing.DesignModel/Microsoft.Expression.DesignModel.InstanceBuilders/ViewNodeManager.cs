using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	[DebuggerDisplay("{System.IO.Path.GetFileName(this.context.DocumentContext.DocumentUrl)} - {this.root}")]
	public abstract class ViewNodeManager
	{
		private IInstanceBuilderContext context;

		private ExpressionCache expressionCache = new ExpressionCache();

		private ViewNodeManager.RelatedDocumentTable relatedDocumentTable;

		private ViewNode root;

		private DocumentNodePath rootNodePath;

		private DocumentNodePath editingContainer;

		private DocumentNodePath candidateEditingContainer;

		private IDisposable rootToken;

		private List<ViewNode> invalidRoots = new List<ViewNode>();

		private IInstanceBuilder fallbackInstanceBuilder;

		private HashSet<ViewNode> damagedTemplates = new HashSet<ViewNode>();

		private HashSet<ViewNode> postponedTemplates = new HashSet<ViewNode>();

		private Dictionary<ViewNode, ViewNode> instanceReferences = new Dictionary<ViewNode, ViewNode>();

		internal Dictionary<ViewNode, List<ViewNode>> referencingInstances = new Dictionary<ViewNode, List<ViewNode>>();

		private HashSet<ViewNode> invalidatedReferences = new HashSet<ViewNode>();

		private Dictionary<ViewNode, DocumentNodePath> unresolvedReferences = new Dictionary<ViewNode, DocumentNodePath>();

		private List<ViewNodeManager.SingleInstanceReference> unresolvedSingleInstanceReferences = new List<ViewNodeManager.SingleInstanceReference>();

		private bool isUpdating;

		public readonly static Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeIdConverter ViewNodeIdConverter;

		private List<DocumentNode> perftemp_pathAsNodes = new List<DocumentNode>();

		private List<DocumentNodePath> perftemp_containerOwnerPaths = new List<DocumentNodePath>();

		public DocumentNodePath CandidateEditingContainer
		{
			get
			{
				return this.candidateEditingContainer;
			}
			set
			{
				if (this.candidateEditingContainer != value)
				{
					List<ViewNode> viewNodes = new List<ViewNode>(2);
					if (this.root != null && this.candidateEditingContainer != null && this.candidateEditingContainer.IsValid(true))
					{
						ViewNode nearestAncestorToCorrespondingViewNode = this.GetNearestAncestorToCorrespondingViewNode(this.candidateEditingContainer);
						if (nearestAncestorToCorrespondingViewNode != null)
						{
							this.InvalidateCandidateEditingContainer(nearestAncestorToCorrespondingViewNode);
							viewNodes.Add(nearestAncestorToCorrespondingViewNode);
						}
					}
					this.candidateEditingContainer = value;
					if (this.root != null && this.candidateEditingContainer != null && this.candidateEditingContainer.IsValid(true))
					{
						ViewNode viewNode = this.GetNearestAncestorToCorrespondingViewNode(this.candidateEditingContainer);
						if (viewNode != null)
						{
							this.InvalidateCandidateEditingContainer(viewNode);
							viewNodes.Add(viewNode);
						}
					}
					this.InvalidateInternal(viewNodes, false);
				}
			}
		}

		public bool ContainsInvalidReferences
		{
			get
			{
				if (this.invalidatedReferences.Count > 0 || this.unresolvedReferences.Count > 0)
				{
					return true;
				}
				return this.unresolvedSingleInstanceReferences.Count > 0;
			}
		}

		public DocumentNodePath EditingContainer
		{
			get
			{
				return this.editingContainer;
			}
			set
			{
				if (this.editingContainer != value)
				{
					List<ViewNode> viewNodes = new List<ViewNode>(2);
					this.CandidateEditingContainer = null;
					if (this.editingContainer != null && !this.editingContainer.Equals(this.rootNodePath))
					{
						if (this.editingContainer.IsValid(true) && (value == null || value.Equals(this.rootNodePath) || !value.IsAncestorOf(this.editingContainer)))
						{
							ViewNode nearestAncestorToCorrespondingViewNode = this.GetNearestAncestorToCorrespondingViewNode(this.editingContainer);
							nearestAncestorToCorrespondingViewNode.MergeState(InstanceState.Invalid);
							viewNodes.Add(nearestAncestorToCorrespondingViewNode);
						}
						if (PlatformTypes.FrameworkTemplate.IsAssignableFrom(this.editingContainer.Node.Type))
						{
							this.Invalidate(this.editingContainer.Node, InstanceState.Invalid);
						}
					}
					DocumentNodePath documentNodePath = this.editingContainer;
					this.editingContainer = value;
					if (this.editingContainer != null && !this.editingContainer.Equals(this.rootNodePath) && this.editingContainer.IsValid(true) && (documentNodePath == null || documentNodePath.Equals(this.rootNodePath) || !documentNodePath.IsAncestorOf(this.editingContainer)))
					{
						ViewNode viewNode = this.GetNearestAncestorToCorrespondingViewNode(this.editingContainer);
						viewNode.MergeState(InstanceState.Invalid);
						viewNodes.Add(viewNode);
					}
					this.InvalidateInternal(viewNodes, false);
				}
			}
		}

		protected IExceptionDictionary ExceptionDictionary
		{
			get
			{
				return this.context.ExceptionDictionary;
			}
		}

		protected IInstanceBuilderContext InstanceBuilderContext
		{
			get
			{
				return this.context;
			}
		}

		protected IInstanceBuilderFactory InstanceBuilderFactory
		{
			get
			{
				return this.context.InstanceBuilderFactory;
			}
		}

		public IInstanceDictionary InstanceDictionary
		{
			get
			{
				return this.context.InstanceDictionary;
			}
		}

		protected Dictionary<ViewNode, ViewNode> InstanceReferences
		{
			get
			{
				return this.instanceReferences;
			}
		}

		public bool IsRootInvalid
		{
			get
			{
				if (this.Root != null && this.Root.InstanceState == InstanceState.Invalid)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsUpdating
		{
			get
			{
				return this.isUpdating;
			}
		}

		public ICollection<IDocumentRoot> RelatedDocuments
		{
			get
			{
				return this.relatedDocumentTable.RelatedDocuments;
			}
		}

		public ViewNode Root
		{
			get
			{
				return this.root;
			}
			private set
			{
				if (this.root != value)
				{
					if (this.root != null)
					{
						this.root.Properties.Clear();
						this.root.Children.Clear();
						this.OnViewNodeRemoving(null, this.root);
					}
					this.root = value;
					if (this.root != null)
					{
						this.OnViewNodeAdded(null, this.root);
					}
				}
			}
		}

		public DocumentNodePath RootNodePath
		{
			get
			{
				return this.rootNodePath;
			}
			set
			{
				this.ChangeRootNodePath(value, true, this.rootNodePath != value);
			}
		}

		public object ValidRootInstance
		{
			get
			{
				if (this.Root == null || this.Root.Instance == ClrObjectInstanceBuilder.InvalidObjectSentinel)
				{
					return null;
				}
				return this.Root.Instance;
			}
		}

		static ViewNodeManager()
		{
			ViewNodeManager.ViewNodeIdConverter = new Microsoft.Expression.DesignModel.InstanceBuilders.ViewNodeIdConverter();
		}

		protected ViewNodeManager(IInstanceBuilder fallbackInstanceBuilder)
		{
			this.fallbackInstanceBuilder = fallbackInstanceBuilder;
			this.relatedDocumentTable = new ViewNodeManager.RelatedDocumentTable();
		}

		public abstract void AddInstantiatedElement(ViewNode target, object element);

		public void AddPostponedInstanceReference(ViewNode parentViewNode, object parentInstance, ViewNode evaluatedValue)
		{
			if ((parentViewNode != null && parentViewNode.Parent != null || parentViewNode == this.Root) && evaluatedValue != null && evaluatedValue.DocumentNode != null && evaluatedValue.DocumentNode.DocumentRoot != null)
			{
				DocumentNodePath documentNodePath = new DocumentNodePath(evaluatedValue.DocumentNode.DocumentRoot.RootNode, evaluatedValue.DocumentNode);
				this.AddPostponedInstanceReference(evaluatedValue, parentInstance, documentNodePath);
			}
		}

		public void AddPostponedInstanceReference(ViewNode sourceViewNode, object parentInstance, DocumentNodePath evaluatedValue)
		{
			bool viewNodeManager = this.isUpdating;
			if (!viewNodeManager && this.Root != null && sourceViewNode.DocumentNode.DocumentRoot != this.Root.DocumentNode.DocumentRoot)
			{
				IInstanceBuilderContext viewContext = this.context.GetViewContext(evaluatedValue.Node.DocumentRoot);
				if (viewContext != null)
				{
					viewNodeManager = viewContext.ViewNodeManager.isUpdating;
				}
			}
			if (!viewNodeManager)
			{
				this.ResolveReference(sourceViewNode, parentInstance, evaluatedValue);
				return;
			}
			ViewNodeManager.SingleInstanceReference singleInstanceReference = new ViewNodeManager.SingleInstanceReference()
			{
				SourceViewNode = sourceViewNode,
				ParentInstance = parentInstance,
				EvaluatedValue = evaluatedValue
			};
			this.unresolvedSingleInstanceReferences.Add(singleInstanceReference);
		}

		public void AddPostponedReference(ViewNode source, DocumentNodePath evaluatedValue)
		{
			source.Instance = null;
			source.InstanceState = InstanceState.Invalid;
			ISerializationContext serializationContext = this.context.SerializationContext;
			if (serializationContext != null)
			{
				serializationContext.AddPostponedReference(source);
				return;
			}
			if (source.Parent == null || !(source.Parent.Instance is DocumentNode))
			{
				this.unresolvedReferences.Add(source, evaluatedValue);
			}
			else
			{
				IInstantiatedElementViewNode parent = source.Parent as IInstantiatedElementViewNode;
				if (parent != null)
				{
					foreach (object instantiatedElement in parent.InstantiatedElements)
					{
						this.AddPostponedInstanceReference(source, instantiatedElement, evaluatedValue);
					}
				}
			}
		}

		public void AddReference(ViewNode source, ViewNode target)
		{
			this.instanceReferences.Add(source, target);
			target.ViewNodeManager.AddReferencedViewNode(source, target);
		}

		private void AddReferencedViewNode(ViewNode source, ViewNode target)
		{
			List<ViewNode> viewNodes;
			if (!this.referencingInstances.TryGetValue(target, out viewNodes))
			{
				viewNodes = new List<ViewNode>();
				this.referencingInstances[target] = viewNodes;
			}
			viewNodes.Add(source);
		}

		public bool AddRelatedDocumentRoot(ViewNode referenceSource, IDocumentRoot documentRoot)
		{
			if (this.Root != null && this.Root.DocumentNode.DocumentRoot != documentRoot)
			{
				ReadOnlyCollection<ViewNode> item = this.relatedDocumentTable[documentRoot];
				if (item == null || !item.Contains(referenceSource))
				{
					this.relatedDocumentTable.Add(documentRoot, referenceSource);
					return true;
				}
			}
			return false;
		}

		private void ChangeRootNodePath(DocumentNodePath nodePath, bool clearExceptions, bool resetEditingContainer)
		{
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				if (this.rootToken != null)
				{
					this.rootToken.Dispose();
					this.rootToken = null;
				}
				this.rootNodePath = nodePath;
				if (nodePath == null)
				{
					this.Root = null;
				}
				else
				{
					Type targetType = nodePath.Node.TargetType;
					if (this.InstanceBuilderContext.RootTargetTypeReplacement != null && this.InstanceBuilderContext.RootTargetTypeReplacement.SourceType.Equals(nodePath.Node.Type))
					{
						targetType = this.GetInstantiatableType(nodePath.ContainerNode.TypeResolver, nodePath.Node.TargetType).RuntimeType;
					}
					IInstanceBuilder builder = this.InstanceBuilderFactory.GetBuilder(targetType);
					this.Root = builder.GetViewNode(this.context, nodePath.Node);
					this.rootToken = this.context.ChangeContainerRoot(this.root);
				}
				this.referencingInstances.Clear();
				this.instanceReferences.Clear();
				this.damagedTemplates.Clear();
				this.postponedTemplates.Clear();
				this.invalidRoots.Clear();
				this.invalidatedReferences.Clear();
				this.unresolvedReferences.Clear();
				this.unresolvedSingleInstanceReferences.Clear();
				this.expressionCache.Clear();
				if (clearExceptions)
				{
					this.ExceptionDictionary.Clear();
				}
				this.context.WarningDictionary.Clear();
				this.context.InstanceDictionary.Clear();
				this.context.UserControlInstances.Clear();
				this.context.EffectManager.ClearCache();
				if (resetEditingContainer)
				{
					this.editingContainer = null;
					this.candidateEditingContainer = null;
				}
			}
		}

		private bool CheckUpdateStep(ViewNodeManager.UpdateStep updateStep, ref ViewNodeManager.UpdateStep lastCompletedStep, ref int tryCount)
		{
			if (this.invalidRoots.Count <= 0)
			{
				return true;
			}
			if ((int)lastCompletedStep < (int)updateStep)
			{
				lastCompletedStep = updateStep;
				return false;
			}
			tryCount = tryCount + 1;
			return false;
		}

		private void ClearExceptionsInSubtree(ViewNode root, bool includeRoot)
		{
			if (includeRoot)
			{
				this.ExceptionDictionary.Remove(root);
			}
			foreach (ViewNode child in root.Children)
			{
				this.ClearExceptionsInSubtree(child, true);
			}
			foreach (ViewNode value in root.Properties.Values)
			{
				this.ClearExceptionsInSubtree(value, true);
			}
		}

		public bool ContainsStoryboardDamagedTemplate(ViewNode templateViewNode)
		{
			return this.postponedTemplates.Contains(templateViewNode);
		}

		public bool CreateInstance(IInstanceBuilder builder, ViewNode viewNode)
		{
			bool flag;
			if (!this.ExceptionDictionary.Contains(viewNode))
			{
				try
				{
					flag = builder.Instantiate(this.context, viewNode);
				}
				catch (Exception exception)
				{
					this.RecoverAndCreateFallbackInstance(viewNode, exception);
					return true;
				}
				return flag;
			}
			bool flag1 = this.fallbackInstanceBuilder.Instantiate(this.context, viewNode);
			this.fallbackInstanceBuilder.Initialize(this.context, viewNode, flag1);
			ViewNodeManager.RemoveFromListWhen<ViewNode>(this.invalidRoots, (ViewNode item) => viewNode.IsAncestorOf(item.Parent));
			this.ClearExceptionsInSubtree(viewNode, false);
			return flag1;
		}

		public virtual void DisposeInternal()
		{
			List<ViewNode> viewNodes;
			foreach (KeyValuePair<ViewNode, ViewNode> instanceReference in this.instanceReferences)
			{
				ViewNode key = instanceReference.Key;
				ViewNode value = instanceReference.Value;
				if (value.ViewNodeManager == this || !value.ViewNodeManager.referencingInstances.TryGetValue(value, out viewNodes))
				{
					continue;
				}
				viewNodes.Remove(key);
				if (viewNodes.Count != 0)
				{
					continue;
				}
				value.ViewNodeManager.referencingInstances.Remove(value);
			}
			foreach (KeyValuePair<ViewNode, List<ViewNode>> referencingInstance in this.referencingInstances)
			{
				foreach (ViewNode viewNode in referencingInstance.Value)
				{
					if (viewNode.ViewNodeManager == this)
					{
						continue;
					}
					viewNode.ViewNodeManager.instanceReferences.Remove(viewNode);
				}
			}
			this.instanceReferences = null;
			this.referencingInstances = null;
			this.invalidatedReferences = null;
			this.unresolvedReferences = null;
			this.unresolvedSingleInstanceReferences = null;
			this.fallbackInstanceBuilder = null;
			this.rootNodePath = null;
			this.root = null;
			if (this.relatedDocumentTable != null)
			{
				this.relatedDocumentTable.Dispose();
				this.relatedDocumentTable = null;
			}
			if (this.rootToken != null)
			{
				this.rootToken.Dispose();
				this.rootToken = null;
			}
			this.context = null;
			this.expressionCache.Clear();
			this.invalidRoots.Clear();
			this.damagedTemplates.Clear();
			this.postponedTemplates.Clear();
		}

		protected abstract bool EnsureElementInDictionary(object root, ViewNode knownAncestor);

		private void EnsureInstantiatedObjects(IAttachViewRoot siteRoot)
		{
			if (this.InstanceDictionary.InstantiatedElementRoots.Count > 0)
			{
				int count = this.InstanceDictionary.InstantiatedElementRoots.Count;
				int num = 0;
				while (num < count)
				{
					if (count == this.InstanceDictionary.InstantiatedElementRoots.Count)
					{
						ViewNode item = this.InstanceDictionary.InstantiatedElementRoots[num];
						IInstantiatedElementViewNode instantiatedElementViewNode = item as IInstantiatedElementViewNode;
						if (instantiatedElementViewNode != null)
						{
							foreach (object instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
							{
								this.EnsureElementInDictionary(instantiatedElement, item);
							}
						}
						this.EnsureElementInDictionary(item.Instance, item);
						num++;
					}
					else
					{
						num = 0;
						count = this.InstanceDictionary.InstantiatedElementRoots.Count;
					}
				}
				this.InstanceDictionary.InstantiatedElementRoots.Clear();
				try
				{
					siteRoot.UpdateLayout();
				}
				catch (Exception exception)
				{
					this.OnElementException(exception);
				}
			}
		}

		private void EnsureRootSited(IAttachViewRoot siteRoot)
		{
			try
			{
				siteRoot.EnsureRootSited();
			}
			catch (Exception exception)
			{
				this.OnException(siteRoot.ViewRoot, exception);
			}
		}

		private List<ViewNode> GenerateRoots(ViewNode rootViewNode, DocumentNode target, InstanceState state)
		{
			List<ViewNode> viewNodes;
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				List<ViewNode> viewNodes1 = new List<ViewNode>();
				List<KeyValuePair<DocumentNode, ViewNode>> keyValuePairs = new List<KeyValuePair<DocumentNode, ViewNode>>();
				this.GetCorrespondingViewNodesInternal(target, rootViewNode, viewNodes1, keyValuePairs);
				List<ViewNode> viewNodes2 = new List<ViewNode>(viewNodes1);
				foreach (KeyValuePair<DocumentNode, ViewNode> keyValuePair in keyValuePairs)
				{
					ViewNode value = keyValuePair.Value;
					DocumentNode key = keyValuePair.Key;
					if (this.instanceReferences.ContainsKey(value) || this.invalidatedReferences.Contains(value) || value.InstanceState == InstanceState.Invalid || this.ExceptionDictionary.Contains(value) || PlatformTypes.MultiTrigger.IsAssignableFrom(value.Type))
					{
						continue;
					}
					InstanceState valid = InstanceState.Valid;
					if (key.IsProperty && (value.InstanceState.InvalidProperties == null || !value.InstanceState.InvalidProperties.Contains(key.SitePropertyKey)))
					{
						valid = new InstanceState(key.SitePropertyKey);
					}
					else if (key.IsChild)
					{
						int siteChildIndex = key.SiteChildIndex;
						InstanceState instanceState = value.InstanceState;
						if (!instanceState.IsChildInvalid || instanceState.ChildIndex != siteChildIndex || instanceState.ChildAction != DocumentNodeChangeAction.Replace)
						{
							valid = InstanceState.Invalid;
						}
					}
					if (valid == InstanceState.Valid)
					{
						continue;
					}
					value.MergeState(valid);
					viewNodes2.Add(value);
					this.OnViewNodeInvalidating(this.context, value, null, viewNodes2, true);
				}
				foreach (ViewNode viewNode in viewNodes1)
				{
					if (this.unresolvedReferences.ContainsKey(viewNode) || this.instanceReferences.ContainsKey(viewNode) || this.invalidatedReferences.Contains(viewNode))
					{
						continue;
					}
					viewNode.MergeState(state);
					ViewNode item = null;
					if (state.IsPropertyInvalid)
					{
						item = viewNode.Properties[state.InvalidProperties[0]];
					}
					if (state.IsChildInvalid && (state.ChildAction == DocumentNodeChangeAction.Remove || state.ChildAction == DocumentNodeChangeAction.Replace) && viewNode.Children.Count > state.ChildIndex)
					{
						item = viewNode.Children[state.ChildIndex];
					}
					if (item != null)
					{
						item.MergeState(InstanceState.Invalid);
					}
					this.OnViewNodeInvalidating(this.context, viewNode, item, viewNodes2, false);
				}
				foreach (KeyValuePair<DocumentNode, ViewNode> keyValuePair1 in keyValuePairs)
				{
					ViewNode value1 = keyValuePair1.Value;
					if (!this.ExceptionDictionary.Contains(value1))
					{
						continue;
					}
					value1.MergeState(InstanceState.Invalid);
					viewNodes2.Add(value1);
					this.OnViewNodeInvalidating(this.context, value1, null, viewNodes2, false);
				}
				viewNodes = viewNodes2;
			}
			return viewNodes;
		}

		private List<ViewNode> GenerateRoots(IDocumentRoot changedRoot)
		{
			ReadOnlyCollection<ViewNode> item = this.relatedDocumentTable[changedRoot];
			if (item == null)
			{
				return null;
			}
			List<ViewNode> viewNodes = new List<ViewNode>(item);
			foreach (ViewNode viewNode in viewNodes)
			{
				viewNode.MergeState(InstanceState.Invalid);
			}
			return viewNodes;
		}

		private List<ViewNode> GenerateRoots(IDocumentRoot relatedDocumentRoot, DocumentNode target, InstanceState state)
		{
			ReadOnlyCollection<ViewNode> item = this.relatedDocumentTable[relatedDocumentRoot];
			List<ViewNode> viewNodes = new List<ViewNode>();
			if (item != null)
			{
				foreach (ViewNode viewNode in item)
				{
					if (viewNode.DocumentNode.Marker != null)
					{
						viewNodes.AddRange(this.GenerateRoots(viewNode, target, state));
					}
					else
					{
						viewNode.MergeState(InstanceState.Invalid);
						viewNodes.Add(viewNode);
					}
				}
			}
			return viewNodes;
		}

		public DocumentNodePath GetCorrespondingNodePath(ViewNode viewNode)
		{
			ViewNode i;
			DocumentNodePath rootNodePath = this.RootNodePath;
			if (rootNodePath == null)
			{
				return null;
			}
			FrugalStructList<DocumentNode> frugalStructList = new FrugalStructList<DocumentNode>();
			FrugalStructList<DocumentNode> frugalStructList1 = new FrugalStructList<DocumentNode>();
			FrugalStructList<IProperty> frugalStructList2 = new FrugalStructList<IProperty>();
			for (i = viewNode; i.Parent != null; i = i.Parent)
			{
				if (this.IsContainerRoot(i) && i.IsProperty)
				{
					frugalStructList.Add(i.DocumentNode);
					frugalStructList1.Add(i.Parent.DocumentNode);
					frugalStructList2.Add(i.SitePropertyKey);
				}
			}
			if (i != this.root)
			{
				throw new InvalidOperationException(ExceptionStringTable.CannotGetNodePathForViewNodeWhichIsNotAPartOfViewTree);
			}
			for (int j = frugalStructList.Count - 1; j >= 0; j--)
			{
				rootNodePath = rootNodePath.GetPathInContainer(frugalStructList1[j]);
				rootNodePath = rootNodePath.GetPathInSubContainer(frugalStructList2[j], frugalStructList[j]);
			}
			rootNodePath = rootNodePath.GetPathInContainer(viewNode.DocumentNode);
			return rootNodePath;
		}

		public IList<DocumentNodePath> GetCorrespondingNodePaths(DocumentNode target)
		{
			List<DocumentNodePath> documentNodePaths = new List<DocumentNodePath>();
			if (this.root == null)
			{
				documentNodePaths.Add(new DocumentNodePath(target.DocumentRoot.RootNode, target));
			}
			else
			{
				List<ViewNode> viewNodes = new List<ViewNode>();
				List<KeyValuePair<DocumentNode, ViewNode>> keyValuePairs = new List<KeyValuePair<DocumentNode, ViewNode>>();
				this.GetCorrespondingViewNodesInternal(target, this.root, viewNodes, keyValuePairs);
				foreach (ViewNode viewNode in viewNodes)
				{
					documentNodePaths.Add(this.GetCorrespondingNodePath(viewNode));
				}
				foreach (KeyValuePair<DocumentNode, ViewNode> keyValuePair in keyValuePairs)
				{
					ViewNode value = keyValuePair.Value;
					documentNodePaths.Add(this.GetCorrespondingNodePath(value).GetPathInContainer(target));
				}
			}
			return documentNodePaths;
		}

		public ViewNode GetCorrespondingViewNode(DocumentNodePath nodePath)
		{
			ViewNode nearestAncestorToCorrespondingViewNode = this.GetNearestAncestorToCorrespondingViewNode(nodePath);
			if (nearestAncestorToCorrespondingViewNode.DocumentNode != nodePath.Node)
			{
				throw new InvalidOperationException(ExceptionStringTable.CannotGetViewNodeWhenNodePathIsNotAPartOfDocument);
			}
			return nearestAncestorToCorrespondingViewNode;
		}

		private void GetCorrespondingViewNodesInternal(DocumentNode target, ViewNode rootViewNode, List<ViewNode> knownNodes, List<KeyValuePair<DocumentNode, ViewNode>> traversalFailures)
		{
            if (target == rootViewNode.DocumentNode)
            {
                knownNodes.Add(rootViewNode);
            }
            else
            {
                if (target.Parent == null || target.Parent == null)
                    return;
                DocumentCompositeNode parent = target.Parent;
                this.GetCorrespondingViewNodesInternal((DocumentNode)parent, rootViewNode, knownNodes, traversalFailures);
                for (int index1 = 0; index1 < knownNodes.Count; ++index1)
                {
                    if (target.IsChild)
                    {
                        int index2 = parent.Children.IndexOf(target);
                        ViewNode viewNode;
                        if (index2 < knownNodes[index1].Children.Count && (viewNode = knownNodes[index1].Children[index2]) != null && viewNode.DocumentNode == target)
                        {
                            knownNodes[index1] = viewNode;
                        }
                        else
                        {
                            traversalFailures.Add(new KeyValuePair<DocumentNode, ViewNode>(target, knownNodes[index1]));
                            knownNodes.RemoveAt(index1);
                            --index1;
                        }
                    }
                    else
                    {
                        ViewNode viewNode = knownNodes[index1].Properties[target.SitePropertyKey];
                        if (viewNode != null && viewNode.DocumentNode == target)
                        {
                            knownNodes[index1] = viewNode;
                        }
                        else
                        {
                            traversalFailures.Add(new KeyValuePair<DocumentNode, ViewNode>(target, knownNodes[index1]));
                            knownNodes.RemoveAt(index1);
                            --index1;
                        }
                    }
                }
            }
            List<ViewNode> expressionValue = this.expressionCache.GetExpressionValue(target);
            if (expressionValue == null)
                return;
            for (int index = 0; index < expressionValue.Count; ++index)
            {
                ViewNode descendant = expressionValue[index];
                if (descendant.Parent.DocumentNode != target.Parent && rootViewNode.IsAncestorOf(descendant) && !DocumentNodeUtilities.IsBinding(descendant.DocumentNode))
                    knownNodes.Add(descendant);
            }
		}

		private ViewNode GetExceptionInSubtree(ViewNode root)
		{
			ViewNode viewNode;
			if (this.ExceptionDictionary.GetException(root) != null)
			{
				return root;
			}
			foreach (ViewNode child in root.Children)
			{
				ViewNode exceptionInSubtree = this.GetExceptionInSubtree(child);
				if (exceptionInSubtree == null)
				{
					continue;
				}
				viewNode = exceptionInSubtree;
				return viewNode;
			}
			using (IEnumerator<ViewNode> enumerator = root.Properties.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ViewNode exceptionInSubtree1 = this.GetExceptionInSubtree(enumerator.Current);
					if (exceptionInSubtree1 == null)
					{
						continue;
					}
					viewNode = exceptionInSubtree1;
					return viewNode;
				}
				return null;
			}
			return viewNode;
		}

		public IType GetInstantiatableType(ITypeResolver typeResolver, Type targetType)
		{
			PlatformBase platform = this.InstanceBuilderContext.Platform as PlatformBase;
			IType type = typeResolver.GetType(targetType);
			if (type != null && platform != null && (!type.HasDefaultConstructor(true) || type.IsAbstract))
			{
				try
				{
					Type type1 = (new ControlEditingDesignTypeGenerator(typeResolver)).DefineType(targetType);
					if (type1 != targetType)
					{
						IType type2 = typeResolver.GetType(type1);
						if (PlatformTypes.FrameworkElement.IsAssignableFrom(type2))
						{
							type = type2;
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			return type;
		}

		private ViewNode GetNearestAncestorToCorrespondingViewNode(DocumentNodePath nodePath)
		{
			int j;
			if (nodePath.RootNode != this.rootNodePath.RootNode)
			{
				throw new InvalidOperationException(ExceptionStringTable.CannotGetViewNodeWhenNodePathIsNotAPartOfDocument);
			}
			nodePath.GetPathAsNodes(this.perftemp_pathAsNodes);
			this.perftemp_containerOwnerPaths.Clear();
			for (DocumentNodePath i = nodePath; i != null; i = i.GetContainerOwnerPath())
			{
				this.perftemp_containerOwnerPaths.Add(i);
			}
			ViewNode viewNode = this.root;
			int count = this.perftemp_containerOwnerPaths.Count - 1;
			for (j = 0; j < this.perftemp_pathAsNodes.Count && viewNode.DocumentNode != this.perftemp_pathAsNodes[j]; j++)
			{
				if (this.perftemp_pathAsNodes[j] == this.perftemp_containerOwnerPaths[count].Node && count > 0)
				{
					count--;
				}
			}
			if (j >= this.perftemp_pathAsNodes.Count || viewNode.DocumentNode != this.perftemp_pathAsNodes[j])
			{
				return null;
			}
			for (j++; j < this.perftemp_pathAsNodes.Count; j++)
			{
				if (viewNode.DocumentNode != this.perftemp_containerOwnerPaths[count].Node || count <= 0)
				{
					DocumentNode item = this.perftemp_pathAsNodes[j];
					if (!item.IsProperty)
					{
						int num = item.Parent.Children.IndexOf(item);
						ViewNode item1 = null;
						if (num < viewNode.Children.Count && num >= 0)
						{
							item1 = viewNode.Children[num];
						}
						if (item1 == null || item1.DocumentNode != item)
						{
							return viewNode;
						}
						viewNode = item1;
					}
					else
					{
						ViewNode viewNode1 = viewNode.Properties[item.SitePropertyKey];
						if (viewNode1 == null || viewNode1.DocumentNode != item)
						{
							return viewNode;
						}
						viewNode = viewNode1;
					}
				}
				else
				{
					count--;
					DocumentNodePath documentNodePath = this.perftemp_containerOwnerPaths[count];
					IProperty property = viewNode.DocumentNode.TypeResolver.ResolveProperty(documentNodePath.ContainerOwnerProperty);
					ViewNode item2 = viewNode.Properties[property];
					if (item2 == null || item2.DocumentNode != documentNodePath.ContainerNode)
					{
						return viewNode;
					}
					viewNode = item2;
				}
			}
			return viewNode;
		}

		public DocumentNodePath GetSingleCorrespondingNodePath(DocumentNode target)
		{
			if (this.root != null)
			{
				List<ViewNode> viewNodes = new List<ViewNode>();
				List<KeyValuePair<DocumentNode, ViewNode>> keyValuePairs = new List<KeyValuePair<DocumentNode, ViewNode>>();
				this.GetCorrespondingViewNodesInternal(target, this.root, viewNodes, keyValuePairs);
				if (viewNodes.Count > 0)
				{
					return this.GetCorrespondingNodePath(viewNodes[0]);
				}
			}
			return new DocumentNodePath(target.DocumentRoot.RootNode, target);
		}

		private InstanceState GetStateAsParentState(ViewNode parent, ViewNode child, InstanceState childState)
		{
			InstanceState valid;
			switch (childState.StateType)
			{
				case InstanceStateType.Invalid:
				{
					if (!child.IsProperty)
					{
						DocumentNode documentNode = child.DocumentNode;
						if (documentNode.Parent == null || documentNode.Parent != parent.DocumentNode)
						{
							valid = InstanceState.Valid;
							break;
						}
						else
						{
							valid = new InstanceState(documentNode.Parent.Children.IndexOf(documentNode), DocumentNodeChangeAction.Replace);
							break;
						}
					}
					else
					{
						valid = new InstanceState(child.SitePropertyKey);
						break;
					}
				}
				case InstanceStateType.Uninitialized:
				{
					valid = InstanceState.Valid;
					break;
				}
				case InstanceStateType.PropertyOrChildInvalid:
				case InstanceStateType.DescendantInvalid:
				case InstanceStateType.ChildAndDescendantInvalid:
				{
					valid = InstanceState.DescendantInvalid;
					break;
				}
				default:
				{
					goto case InstanceStateType.Uninitialized;
				}
			}
			return valid;
		}

		public virtual void Initialize(IInstanceBuilderContext context)
		{
			this.context = context;
		}

		public void InitializeInstance(IInstanceBuilder builder, ViewNode viewNode, bool isNewInstance)
		{
			IDisposable disposable = this.UpdateResourceEvaluationToken(viewNode);
			try
			{
				try
				{
					builder.Initialize(this.context, viewNode, isNewInstance);
				}
				catch (Exception exception)
				{
					this.OnException(viewNode, exception);
				}
			}
			finally
			{
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
			}
		}

		public object Instantiate(ViewNode viewNode)
		{
			IInstanceBuilder builder = this.context.InstanceBuilderFactory.GetBuilder(viewNode.TargetType);
			bool flag = this.CreateInstance(builder, viewNode);
			if (viewNode.InstanceState == InstanceState.Uninitialized)
			{
				this.InitializeInstance(builder, viewNode, flag);
			}
			return viewNode.Instance;
		}

		public void Invalidate(DocumentNode target, InstanceState state)
		{
			List<ViewNode> viewNodes = this.GenerateRoots(this.root, target, state);
			if (viewNodes != null)
			{
				this.InvalidateInternal(viewNodes, false);
			}
		}

		public void Invalidate(IDocumentRoot changedRoot)
		{
			List<ViewNode> viewNodes = this.GenerateRoots(changedRoot);
			if (viewNodes != null)
			{
				viewNodes.RemoveAll((ViewNode viewNode) => {
					if (viewNode.Parent != null)
					{
						return false;
					}
					return viewNode != this.root;
				});
				this.InvalidateInternal(viewNodes, false);
			}
		}

		public bool Invalidate(IDocumentRoot relatedDocumentRoot, DocumentNode target, InstanceState state)
		{
			bool flag = false;
			List<ViewNode> viewNodes = this.GenerateRoots(relatedDocumentRoot, target, state);
			if (viewNodes != null)
			{
				viewNodes.RemoveAll((ViewNode viewNode) => {
					if (viewNode.Parent != null)
					{
						return false;
					}
					return viewNode != this.root;
				});
			}
			bool flag1 = (PlatformTypes.ResourceDictionary.IsAssignableFrom(target.Type) || target.Parent != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(target.Parent.Type) ? true : PlatformTypes.DictionaryEntry.IsAssignableFrom(target.Type));
			if (flag1 || viewNodes.Count > 0)
			{
				this.InvalidateInternal(viewNodes, flag1);
				flag = true;
			}
			return flag;
		}

		private void InvalidateCandidateEditingContainer(ViewNode candidateEditingContainerNode)
		{
			if (!PlatformTypes.DependencyObject.IsAssignableFrom(candidateEditingContainerNode.Type) || PlatformTypes.Style.IsAssignableFrom(candidateEditingContainerNode.Type))
			{
				candidateEditingContainerNode.MergeState(InstanceState.Invalid);
			}
			else
			{
				DocumentCompositeNode documentNode = (DocumentCompositeNode)candidateEditingContainerNode.DocumentNode;
				foreach (KeyValuePair<IProperty, ViewNode> property in candidateEditingContainerNode.Properties)
				{
					IType propertyType = property.Key.PropertyType;
					DocumentNode item = documentNode.Properties[property.Key];
					if (!PlatformTypes.Style.IsAssignableFrom(propertyType) && !PlatformTypes.FrameworkTemplate.IsAssignableFrom(propertyType) || item == null || DocumentNodeUtilities.IsBinding(item) || DocumentNodeUtilities.IsTemplateBinding(item))
					{
						continue;
					}
					candidateEditingContainerNode.MergeState(new InstanceState(property.Key));
				}
			}
		}

		private void InvalidateInternal(List<ViewNode> invalidRoots, bool forceValidateExpressionCache)
		{
			List<ExpressionSite> expressionSites;
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				if (!forceValidateExpressionCache)
				{
					foreach (ViewNode invalidRoot in invalidRoots)
					{
						if (PlatformTypes.ResourceDictionary.IsAssignableFrom(invalidRoot.Type) || invalidRoot.Parent != null && PlatformTypes.ResourceDictionary.IsAssignableFrom(invalidRoot.Parent.Type) || PlatformTypes.DictionaryEntry.IsAssignableFrom(invalidRoot.Type))
						{
							forceValidateExpressionCache = true;
							break;
						}
						else
						{
							DocumentNode documentNode = invalidRoot.DocumentNode;
							if (invalidRoot.InstanceState.IsPropertyInvalid)
							{
								foreach (IPropertyId invalidProperty in invalidRoot.InstanceState.InvalidProperties)
								{
									if (!invalidProperty.Equals(documentNode.Type.Metadata.NameProperty) && !documentNode.Type.Metadata.ContentProperties.Contains(invalidProperty))
									{
										continue;
									}
									forceValidateExpressionCache = true;
									break;
								}
							}
							else if (invalidRoot.InstanceState.StateType == InstanceStateType.Invalid)
							{
								DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
								if (documentCompositeNode != null && documentCompositeNode.Properties[documentCompositeNode.NameProperty] != null)
								{
									forceValidateExpressionCache = true;
									break;
								}
							}
							if (!invalidRoot.InstanceState.IsChildInvalid && invalidRoot.InstanceState != InstanceState.Invalid || documentNode.Parent == null || !documentNode.Parent.Type.Metadata.ContentProperties.Contains(documentNode.SitePropertyKey))
							{
								continue;
							}
							forceValidateExpressionCache = true;
							break;
						}
					}
				}
				if (forceValidateExpressionCache)
				{
					IList<ViewNode> viewNodes = this.expressionCache.Validate(this.context, out expressionSites);
					for (int i = 0; i < viewNodes.Count; i++)
					{
						ViewNode item = viewNodes[i];
						ExpressionSite expressionSite = expressionSites[i];
						item.MergeState((expressionSite.IsProperty ? new InstanceState(expressionSite.PropertyKey) : new InstanceState(expressionSite.ChildIndex, DocumentNodeChangeAction.Replace)));
						invalidRoots.Add(item);
						this.OnViewNodeInvalidating(this.context, item, null, invalidRoots, false);
					}
				}
				this.PropagateInvalidations(invalidRoots, false);
			}
		}

		private void InvalidateReference(ViewNode reference)
		{
			this.instanceReferences.Remove(reference);
			this.invalidatedReferences.Add(reference);
		}

		public void InvalidateReferences(ViewNode viewNode)
		{
			List<ViewNode> viewNodes = null;
			if (this.referencingInstances.TryGetValue(viewNode, out viewNodes))
			{
				foreach (ViewNode viewNode1 in viewNodes)
				{
					viewNode1.ViewNodeManager.InvalidateReference(viewNode1);
				}
				this.referencingInstances.Remove(viewNode);
			}
		}

		public bool InvalidateTemplates(bool fullInvalidate)
		{
			bool flag = false;
			if (this.postponedTemplates.Count > 0)
			{
				flag = (flag ? true : this.InvalidateTemplates(this.postponedTemplates));
			}
			if (fullInvalidate && this.damagedTemplates.Count > 0)
			{
				flag = (flag ? true : this.InvalidateTemplates(this.damagedTemplates));
			}
			return flag;
		}

		private bool InvalidateTemplates(ICollection<ViewNode> templateNodes)
		{
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				List<ViewNode> viewNodes = new List<ViewNode>(templateNodes);
				templateNodes.Clear();
				foreach (ViewNode viewNode in viewNodes)
				{
					viewNode.MergeState(InstanceState.Invalid);
				}
				using (IDisposable disposable1 = this.context.ForceAllowIncrementalTemplateUpdates(false))
				{
					this.PropagateInvalidations(viewNodes, false);
				}
			}
			return true;
		}

		public bool InvalidateUserControls(string closedDocumentPath)
		{
			bool flag;
			if (this.context.UserControlInstances.Count == 0)
			{
				return false;
			}
			List<ViewNode> viewNodes = new List<ViewNode>();
			foreach (ViewNode userControlInstance in this.context.UserControlInstances)
			{
				IUserControlInstanceBuilder builder = this.InstanceBuilderFactory.GetBuilder(userControlInstance.TargetType) as IUserControlInstanceBuilder;
				if (builder == null || !builder.NeedsRebuild(this.context, userControlInstance, closedDocumentPath))
				{
					continue;
				}
				userControlInstance.MergeState(InstanceState.Invalid);
				viewNodes.Add(userControlInstance);
			}
			if (viewNodes.Count <= 0)
			{
				return false;
			}
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				this.PropagateInvalidations(viewNodes, false);
				flag = true;
			}
			return flag;
		}

		public bool IsContainerRoot(ViewNode target)
		{
			if (target == this.root)
			{
				return true;
			}
			if (target.Parent == null)
			{
				return false;
			}
			if (target.Parent.DocumentNode != target.DocumentNode.Parent || PlatformTypes.Style.IsAssignableFrom(target.Type))
			{
				return true;
			}
			return PlatformTypes.FrameworkTemplate.IsAssignableFrom(target.Type);
		}

		public static bool IsTemplate(ViewNode potentialTemplate)
		{
			if (potentialTemplate == null)
			{
				return false;
			}
			return PlatformTypes.FrameworkTemplate.IsAssignableFrom(potentialTemplate.Type);
		}

		private IDisposable NotifyUpdatingInternal(bool isUpdating)
		{
			return new ViewNodeManager.ForceIsUpdatingToken(this, isUpdating);
		}

		protected void OnElementException(Exception exception)
		{
			ViewNode viewNode;
			IViewVisual viewVisual = this.InstanceBuilderContext.Platform.TryGetElementForException(exception);
			if (viewVisual != null)
			{
				viewNode = this.InstanceDictionary.GetViewNode(viewVisual.PlatformSpecificObject, false);
			}
			else
			{
				viewNode = null;
			}
			ViewNode viewNode1 = viewNode;
			this.OnEnsureLayoutRequired(EventArgs.Empty);
			if (viewNode1 != null)
			{
				this.OnException(viewNode1, exception);
				return;
			}
			this.OnException(this.root, exception);
		}

		protected void OnEnsureLayoutRequired(EventArgs e)
		{
			if (ViewNodeManager.EnsureLayoutRequired != null)
			{
				ViewNodeManager.EnsureLayoutRequired(this, e);
			}
		}

		public Exception OnException(ViewNode viewNode, Exception exception)
		{
			return this.OnException(viewNode, exception, true);
		}

		internal Exception OnException(ViewNode viewNode, Exception exception, bool addToInvalidRoots)
		{
			Exception exception1;
			ViewNode viewNode1;
			ViewNode exceptionInSubtree = this.GetExceptionInSubtree(viewNode);
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				if (exceptionInSubtree == null)
				{
					exception1 = exception;
					viewNode1 = viewNode;
				}
				else
				{
					exception1 = this.ExceptionDictionary.GetException(exceptionInSubtree);
					viewNode1 = exceptionInSubtree;
				}
				this.ClearExceptionsInSubtree(viewNode, true);
				viewNode.InstanceState = InstanceState.Invalid;
				DocumentNode documentNode = viewNode1.DocumentNode;
				InstanceBuilderException instanceBuilderException = exception1 as InstanceBuilderException;
				if (instanceBuilderException != null)
				{
					viewNode1 = (instanceBuilderException.ExceptionTarget != null ? instanceBuilderException.ExceptionTarget : viewNode1);
					documentNode = (instanceBuilderException.ExceptionSource != null ? instanceBuilderException.ExceptionSource : viewNode1.DocumentNode);
				}
				if (viewNode.IsAncestorOf(viewNode1) || viewNode.IsAncestorOf(exceptionInSubtree))
				{
					this.ExceptionDictionary.SetException(viewNode, documentNode, exception1);
				}
				if (viewNode1 != viewNode && viewNode1.Parent != null)
				{
					this.PropagateInvalidations(new List<ViewNode>()
					{
						viewNode1
					}, true);
				}
				if (addToInvalidRoots)
				{
					this.invalidRoots.Add(viewNode);
				}
				viewNode.Children.Clear();
				viewNode.Properties.Clear();
				ViewNodeManager.RemoveFromListWhen<ViewNode>(this.invalidRoots, (ViewNode item) => {
					if (item.Parent != null)
					{
						return false;
					}
					return item != this.root;
				});
				ViewNodeManager.RemoveFromListWhen<ViewNode>(this.InstanceDictionary.InstantiatedElementRoots, (ViewNode item) => {
					if (viewNode == item)
					{
						return true;
					}
					if (item.Parent != null)
					{
						return false;
					}
					return item != this.root;
				});
			}
			return exception1;
		}

		internal void OnInstanceChanged(ViewNode viewNode, object oldInstance, object newInstance)
		{
			this.InstanceDictionary.OnInstanceChanged(viewNode, oldInstance, newInstance);
		}

		internal virtual void OnNameScopeChanged(ViewNode containerRoot, INameScope nameScope)
		{
		}

		public void OnTemplateDamaged(ViewNode templateViewNode)
		{
			if (!this.postponedTemplates.Contains(templateViewNode))
			{
				this.damagedTemplates.Add(templateViewNode);
			}
		}

		public void OnTemplateStoryboardDamaged(ViewNode templateViewNode)
		{
			this.postponedTemplates.Add(templateViewNode);
			this.damagedTemplates.Remove(templateViewNode);
		}

		internal void OnViewNodeAdded(ViewNode parent, ViewNode child)
		{
			if (parent != null)
			{
				bool flag = (child.DocumentNode.Parent == null ? false : child.DocumentNode.Parent != parent.DocumentNode);
				DocumentNode documentNode = child.DocumentNode;
				if (!flag && child.DocumentNode.Type.IsExpression)
				{
					if (!child.IsProperty)
					{
						flag = true;
					}
					else if (!DocumentNodeUtilities.IsBinding(child.DocumentNode))
					{
						IInstanceBuilder builder = this.InstanceBuilderFactory.GetBuilder(parent.TargetType);
						flag = builder.ShouldTryExpandExpression(this.context, parent, child.SitePropertyKey, child.DocumentNode);
					}
					else
					{
						IPropertyId propertyId = child.DocumentNode.TypeResolver.ResolveProperty(Microsoft.Expression.DesignModel.Metadata.KnownProperties.BindingElementNameProperty);
						DocumentCompositeNode documentCompositeNode = child.DocumentNode as DocumentCompositeNode;
						if (propertyId != null && documentCompositeNode != null)
						{
							DocumentNode item = documentCompositeNode.Properties[propertyId];
							if (item != null && this.context.NameScope != null)
							{
								flag = true;
								DocumentPrimitiveNode documentPrimitiveNode = item as DocumentPrimitiveNode;
								if (documentPrimitiveNode != null)
								{
									DocumentNodeStringValue value = documentPrimitiveNode.Value as DocumentNodeStringValue;
									if (value != null)
									{
										string str = value.Value;
										object obj = this.context.NameScope.FindName(str);
										if (obj != null)
										{
											ViewNode viewNode = this.context.InstanceDictionary.GetViewNode(obj, false);
											if (viewNode != null)
											{
												documentNode = viewNode.DocumentNode;
											}
										}
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					this.expressionCache.CacheExpressionValue(child, documentNode);
				}
			}
		}

		private void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode parent, ViewNode child, List<ViewNode> invalidRoots, bool isExceptionRelatedChange)
		{
			InstanceState instanceState = parent.InstanceState;
			bool flag = false;
			if (child != null)
			{
				InstanceState stateAsParentState = this.GetStateAsParentState(parent, child, child.InstanceState);
				parent.MergeState(stateAsParentState);
			}
			if (!isExceptionRelatedChange && this.ExceptionDictionary.Contains(parent))
			{
				this.ExceptionDictionary.Remove(parent);
				parent.MergeState(InstanceState.Invalid);
			}
			if (instanceState.StateType != parent.InstanceState.StateType)
			{
				invalidRoots.Add(parent);
				flag = true;
			}
			IInstanceBuilder builder = this.context.InstanceBuilderFactory.GetBuilder(parent.DocumentNode.TargetType);
			builder.OnViewNodeInvalidating(context, parent, child, ref flag, invalidRoots);
		}

		public virtual void OnViewNodeRemoving(ViewNode parent, ViewNode child)
		{
			ViewNode viewNode;
			List<ViewNode> viewNodes;
			if (parent != null)
			{
				IInstanceBuilder builder = this.context.InstanceBuilderFactory.GetBuilder(parent.TargetType);
				builder.OnChildRemoving(this.context, parent, child);
			}
			this.InstanceDictionary.OnViewNodeRemoved(child);
			if (this.InstanceBuilderContext.CrossDocumentUpdateContext != null)
			{
				this.InstanceBuilderContext.CrossDocumentUpdateContext.OnViewNodeRemoving(this.InstanceBuilderContext, child);
			}
			if (parent != null && !child.DocumentNode.IsInDocument && this.ExceptionDictionary.Contains(child))
			{
				this.ExceptionDictionary.Remove(child);
			}
			if (this.context.WarningDictionary.Contains(child))
			{
				this.context.WarningDictionary.Remove(child);
			}
			this.context.UserControlInstances.Remove(child);
			this.relatedDocumentTable.OnViewNodeRemoving(child);
			this.InvalidateReferences(child);
			if (this.instanceReferences.TryGetValue(child, out viewNode))
			{
				if (viewNode.ViewNodeManager.referencingInstances.TryGetValue(viewNode, out viewNodes))
				{
					viewNodes.Remove(child);
					if (viewNodes.Count == 0)
					{
						viewNode.ViewNodeManager.referencingInstances.Remove(viewNode);
					}
				}
				this.instanceReferences.Remove(child);
			}
			this.unresolvedReferences.Remove(child);
			this.invalidatedReferences.Remove(child);
			this.damagedTemplates.Remove(child);
			this.postponedTemplates.Remove(child);
		}

		public void OnViewScopeChanged(ViewNode newViewScope, IAttachViewRoot siteRoot)
		{
			if (newViewScope != null)
			{
				this.InstanceDictionary.InstantiatedElementRoots.Add(newViewScope);
			}
			this.UpdateInstances(siteRoot);
		}

		private void PropagateInvalidations(List<ViewNode> invalidRoots, bool isExceptionRelatedChange)
		{
			while (invalidRoots.Count > 0)
			{
				ViewNode item = invalidRoots[invalidRoots.Count - 1];
				invalidRoots.RemoveAt(invalidRoots.Count - 1);
				ViewNode parent = item.Parent;
				if (parent == null)
				{
					continue;
				}
				this.OnViewNodeInvalidating(this.context, parent, item, invalidRoots, isExceptionRelatedChange);
			}
		}

		private void RecoverAndCreateFallbackInstance(ViewNode viewNode, Exception e)
		{
			Exception exception = this.OnException(viewNode, e);
			bool flag = this.fallbackInstanceBuilder.Instantiate(this.context, viewNode);
			this.fallbackInstanceBuilder.Initialize(this.context, viewNode, flag);
			ViewNodeManager.RemoveFromListWhen<ViewNode>(this.invalidRoots, (ViewNode item) => viewNode.IsAncestorOf(item.Parent));
			InstanceBuilderException instanceBuilderException = exception as InstanceBuilderException;
			if (instanceBuilderException == null || instanceBuilderException.ExceptionTarget == null || instanceBuilderException.ExceptionTarget == viewNode)
			{
				this.ClearExceptionsInSubtree(viewNode, false);
				return;
			}
			if (instanceBuilderException != null && instanceBuilderException.ExceptionTarget != viewNode)
			{
				viewNode.InstanceState = InstanceState.Invalid;
			}
		}

		internal abstract void RegisterContext(ISerializationContext context);

		internal abstract void RegisterContextOwner(object contextOwner, ISerializationContext existingContext);

		protected static void RemoveFromListWhen<T>(IList<T> list, Predicate<T> condition)
		{
			int item = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (!condition(list[i]))
				{
					if (item < i)
					{
						list[item] = list[i];
					}
					item++;
				}
			}
			for (int j = list.Count - 1; j >= item; j--)
			{
				list.RemoveAt(j);
			}
		}

		protected static void RemoveFromListWhen(IList list, Predicate<object> condition)
		{
			int item = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (!condition(list[i]))
				{
					if (item < i)
					{
						list[item] = list[i];
					}
					item++;
				}
			}
			for (int j = list.Count - 1; j >= item; j--)
			{
				list.RemoveAt(j);
			}
		}

		private void ResolveReference(ViewNode reference, object parentInstance, DocumentNodePath evaluatedValue)
		{
			IInstanceBuilderContext viewContext;
			if (reference != null && reference.DocumentNode != null)
			{
				ViewNode nearestAncestorToCorrespondingViewNode = null;
				if (evaluatedValue.Node.DocumentRoot == this.Root.DocumentNode.DocumentRoot)
				{
					viewContext = this.context;
				}
				else if (evaluatedValue.Node.DocumentRoot == null)
				{
					viewContext = null;
				}
				else
				{
					viewContext = this.context.GetViewContext(evaluatedValue.Node.DocumentRoot);
				}
				bool flag = this.ExceptionDictionary.Contains(reference);
				object instance = reference.Instance;
				if (instance == null && !flag && evaluatedValue.IsValid(true) && viewContext != null)
				{
					ViewNodeManager viewNodeManager = viewContext.ViewNodeManager;
					if (viewNodeManager.Root != null && viewNodeManager.Root.InstanceState == InstanceState.Valid)
					{
						nearestAncestorToCorrespondingViewNode = viewNodeManager.GetNearestAncestorToCorrespondingViewNode(evaluatedValue);
						if (nearestAncestorToCorrespondingViewNode != null && nearestAncestorToCorrespondingViewNode.DocumentNode == evaluatedValue.Node && nearestAncestorToCorrespondingViewNode.Instance != null && nearestAncestorToCorrespondingViewNode.InstanceState == InstanceState.Valid && !(nearestAncestorToCorrespondingViewNode.Instance is DocumentNode))
						{
							bool flag1 = false;
							foreach (DocumentNode key in viewNodeManager.ExceptionDictionary.Keys)
							{
								if (!nearestAncestorToCorrespondingViewNode.DocumentNode.IsAncestorOf(key))
								{
									continue;
								}
								flag1 = true;
								break;
							}
							if (!flag1)
							{
								instance = nearestAncestorToCorrespondingViewNode.Instance;
							}
						}
						if (instance == null)
						{
							nearestAncestorToCorrespondingViewNode = null;
						}
					}
				}
				if (instance == null || flag)
				{
					ViewNode parent = reference.Parent;
					IInstanceBuilder builder = this.context.InstanceBuilderFactory.GetBuilder(parent.TargetType);
					IProperty sitePropertyKey = reference.SitePropertyKey;
					DocumentNode propertyValue = this.context.GetPropertyValue(parent, sitePropertyKey);
					try
					{
						using (IDisposable disposable = this.NotifyUpdatingInternal(true))
						{
							using (IDisposable disposable1 = this.context.DisablePostponedResourceEvaluation())
							{
								builder.UpdateProperty(this.context, parent, sitePropertyKey, propertyValue);
							}
						}
					}
					catch (Exception exception)
					{
						this.OnException(reference, exception);
					}
				}
				else
				{
					ViewNode viewNode = reference.Parent;
					IInstanceBuilder instanceBuilder = this.context.InstanceBuilderFactory.GetBuilder(viewNode.TargetType);
					try
					{
						if (parentInstance == null)
						{
							reference.Instance = instance;
						}
						reference.InstanceState = InstanceState.Valid;
						instanceBuilder.ModifyValue(this.context, viewNode, parentInstance, reference.SitePropertyKey, instance, PropertyModification.Set);
						if (nearestAncestorToCorrespondingViewNode != null && !this.instanceReferences.ContainsKey(reference))
						{
							this.AddReference(reference, nearestAncestorToCorrespondingViewNode);
						}
					}
					catch (Exception exception1)
					{
						this.OnException(reference, exception1);
					}
				}
			}
		}

		public void ResolveReferences()
		{
			if (this.invalidatedReferences.Count > 0)
			{
				foreach (ViewNode viewNode in new List<ViewNode>(this.invalidatedReferences))
				{
					ViewNode parent = viewNode.Parent;
					IProperty sitePropertyKey = viewNode.SitePropertyKey;
					if (parent == null || sitePropertyKey == null)
					{
						continue;
					}
					IInstanceBuilder builder = this.InstanceBuilderFactory.GetBuilder(parent.TargetType);
					DocumentNode propertyValue = this.context.GetPropertyValue(parent, sitePropertyKey);
					try
					{
						using (IDisposable disposable = this.NotifyUpdatingInternal(true))
						{
							builder.UpdateProperty(this.context, parent, sitePropertyKey, propertyValue);
						}
					}
					catch (Exception exception)
					{
						this.OnException(parent, exception);
					}
				}
				this.invalidatedReferences.Clear();
			}
			if (this.unresolvedReferences.Count > 0)
			{
				foreach (KeyValuePair<ViewNode, DocumentNodePath> keyValuePair in new List<KeyValuePair<ViewNode, DocumentNodePath>>(this.unresolvedReferences))
				{
					this.ResolveReference(keyValuePair.Key, null, keyValuePair.Value);
				}
				this.unresolvedReferences.Clear();
			}
			if (this.unresolvedSingleInstanceReferences.Count > 0)
			{
				foreach (ViewNodeManager.SingleInstanceReference singleInstanceReference in new List<ViewNodeManager.SingleInstanceReference>(this.unresolvedSingleInstanceReferences))
				{
					this.ResolveReference(singleInstanceReference.SourceViewNode, singleInstanceReference.ParentInstance, singleInstanceReference.EvaluatedValue);
				}
				this.unresolvedSingleInstanceReferences.Clear();
			}
		}

		public bool TryGetCorrespondingViewNode(DocumentNodePath nodePath, out ViewNode viewNode)
		{
			viewNode = null;
			if (this.root != null && nodePath.RootNode == this.rootNodePath.RootNode)
			{
				viewNode = this.GetNearestAncestorToCorrespondingViewNode(nodePath);
			}
			if (viewNode == null)
			{
				return false;
			}
			return viewNode.DocumentNode == nodePath.Node;
		}

		private bool UpdateCore(IAttachViewRoot siteRoot, bool isInstancePass, bool isMainUpdatePass)
		{
			int num = 0;
			int num1 = -1;
			ViewNodeManager.UpdateStep updateStep = ViewNodeManager.UpdateStep.Initial;
			while (updateStep < ViewNodeManager.UpdateStep.Final && num <= 1)
			{
				num1++;
				bool count = this.invalidRoots.Count > 0;
				using (IDisposable disposable = this.NotifyUpdatingInternal(true))
				{
					if (count)
					{
						this.PropagateInvalidations(this.invalidRoots, true);
					}
					if (this.ExceptionDictionary.Contains(this.root))
					{
						break;
					}
					else if (isInstancePass || count)
					{
						this.UpdateInternal(this.root);
						if (!this.CheckUpdateStep(ViewNodeManager.UpdateStep.UpdateInternal, ref updateStep, ref num))
						{
							continue;
						}
					}
				}
				if (!isInstancePass)
				{
					if (this.root == null)
					{
						break;
					}
					this.ResolveReferences();
					if (!this.CheckUpdateStep(ViewNodeManager.UpdateStep.ResolveReferences, ref updateStep, ref num))
					{
						continue;
					}
					if (siteRoot != null && isMainUpdatePass)
					{
						this.EnsureRootSited(siteRoot);
					}
					if (!this.CheckUpdateStep(ViewNodeManager.UpdateStep.SiteRoot, ref updateStep, ref num))
					{
						continue;
					}
					if (siteRoot != null)
					{
						this.EnsureInstantiatedObjects(siteRoot);
					}
					if (!this.CheckUpdateStep(ViewNodeManager.UpdateStep.EnsureInstantiatedObjects, ref updateStep, ref num))
					{
						continue;
					}
				}
				updateStep = ViewNodeManager.UpdateStep.Final;
			}
			if (this.invalidRoots.Count > 0)
			{
				ViewNode item = this.invalidRoots[0];
				DocumentNode key = null;
				Exception value = null;
				foreach (KeyValuePair<DocumentNode, Exception> exceptionDictionary in this.ExceptionDictionary)
				{
					if (!item.DocumentNode.IsAncestorOf(exceptionDictionary.Key))
					{
						continue;
					}
					key = exceptionDictionary.Key;
					value = exceptionDictionary.Value;
				}
				if (value == null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string instanceBuilderCannotInstantiateType = ExceptionStringTable.InstanceBuilderCannotInstantiateType;
					object[] targetType = new object[] { this.root.TargetType };
					string str = string.Format(currentCulture, instanceBuilderCannotInstantiateType, targetType);
					key = this.root.DocumentNode;
					value = new InstanceBuilderException(str);
				}
				this.ExceptionDictionary.SetException(this.root, key, value);
			}
			if (!this.ExceptionDictionary.Contains(this.root))
			{
				return num1 == 0;
			}
			this.ChangeRootNodePath(this.rootNodePath, false, false);
			this.root.Instance = null;
			this.root.InstanceState = InstanceState.Invalid;
			return false;
		}

		internal void UpdateDelayedNodes(List<ViewNode> viewNodes)
		{
			if (viewNodes.Count == 0)
			{
				return;
			}
			if (this.context.CrossDocumentUpdateContext != null && this.context.CrossDocumentUpdateContext.IsDelaying)
			{
				return;
			}
			using (IDisposable disposable = this.NotifyUpdatingInternal(true))
			{
				this.InvalidateInternal(viewNodes, false);
				this.UpdateCore(null, true, true);
			}
		}

		private void UpdateInstance(ViewNode viewNode)
		{
			IDisposable disposable = this.UpdateResourceEvaluationToken(viewNode);
			try
			{
				try
				{
					IInstanceBuilder builder = this.context.InstanceBuilderFactory.GetBuilder(viewNode.TargetType);
					builder.UpdateInstance(this.context, viewNode);
				}
				catch (Exception exception)
				{
					this.OnException(viewNode, exception);
				}
			}
			finally
			{
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
			}
		}

		public bool UpdateInstances(IAttachViewRoot siteRoot)
		{
			bool flag;
			IDisposable disposable;
			if (siteRoot != null)
			{
				disposable = siteRoot.EnsureCrossDocumentUpdateContext();
			}
			else
			{
				disposable = null;
			}
			using (disposable)
			{
				flag = this.UpdateCore(siteRoot, true, true);
			}
			return flag;
		}

		private void UpdateInternal(ViewNode viewNode)
		{
			DocumentNode documentNode = viewNode.DocumentNode;
			InstanceState instanceState = viewNode.InstanceState;
			if (instanceState.StateType != InstanceStateType.Invalid)
			{
				if (instanceState.IsPropertyOrChildInvalid)
				{
					this.UpdateInstance(viewNode);
				}
				if (instanceState.IsDescendantInvalid)
				{
					viewNode.InstanceState = InstanceState.Valid;
					IInstanceBuilder builder = this.context.InstanceBuilderFactory.GetBuilder(viewNode.TargetType);
					foreach (ViewNode child in viewNode.Children)
					{
						InstanceState instanceState1 = child.InstanceState;
						if (instanceState1 == InstanceState.Valid)
						{
							continue;
						}
						this.UpdateInternal(child);
						builder.OnDescendantUpdated(this.context, viewNode, child, instanceState1);
					}
					foreach (ViewNode value in viewNode.Properties.Values)
					{
						InstanceState instanceState2 = value.InstanceState;
						if (instanceState2 == InstanceState.Valid)
						{
							continue;
						}
						if (!this.IsContainerRoot(value))
						{
							this.UpdateInternal(value);
						}
						else
						{
							using (IDisposable disposable = this.context.ChangeContainerRoot(value))
							{
								this.UpdateInternal(value);
							}
						}
						builder.OnDescendantUpdated(this.context, viewNode, value, instanceState2);
					}
				}
			}
			else if (viewNode.Parent == null && viewNode == this.root)
			{
				DocumentNodePath editingContainer = this.EditingContainer;
				DocumentNodePath candidateEditingContainer = this.CandidateEditingContainer;
				this.ChangeRootNodePath(this.RootNodePath, true, false);
				this.editingContainer = editingContainer;
				this.candidateEditingContainer = candidateEditingContainer;
				this.Instantiate(this.root);
				return;
			}
		}

		public bool UpdateReferences(IAttachViewRoot siteRoot, bool isMainUpdatePass)
		{
			return this.UpdateCore(siteRoot, false, isMainUpdatePass);
		}

		private IDisposable UpdateResourceEvaluationToken(ViewNode viewNode)
		{
			IDisposable disposable = null;
			if (this.candidateEditingContainer != null && this.context.AllowPostponingResourceEvaluation && this.candidateEditingContainer.Node == viewNode.DocumentNode)
			{
				DocumentNodePath correspondingNodePath = this.GetCorrespondingNodePath(viewNode);
				if (correspondingNodePath != null && correspondingNodePath.Equals(this.candidateEditingContainer))
				{
					disposable = this.context.DisablePostponedResourceEvaluation();
				}
			}
			return disposable;
		}

		public static event EventHandler EnsureLayoutRequired;

		private sealed class ForceIsUpdatingToken : IDisposable
		{
			private bool isUpdating;

			private ViewNodeManager manager;

			public ForceIsUpdatingToken(ViewNodeManager manager, bool value)
			{
				this.manager = manager;
				this.isUpdating = manager.isUpdating;
				this.manager.isUpdating = value;
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.manager.isUpdating = this.isUpdating;
					this.manager = null;
				}
			}

			void System.IDisposable.Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		private class RelatedDocumentTable : IDisposable
		{
			private Dictionary<IDocumentRoot, List<ViewNode>> viewNodesForDocumentRoot;

			private Dictionary<ViewNode, List<IDocumentRoot>> documentRootsForViewNode;

			public ReadOnlyCollection<ViewNode> this[IDocumentRoot index]
			{
				get
				{
					List<ViewNode> viewNodes;
					if (!this.viewNodesForDocumentRoot.TryGetValue(index, out viewNodes))
					{
						return null;
					}
					return new ReadOnlyCollection<ViewNode>(viewNodes);
				}
			}

			public ICollection<IDocumentRoot> RelatedDocuments
			{
				get
				{
					return this.viewNodesForDocumentRoot.Keys;
				}
			}

			public RelatedDocumentTable()
			{
				this.viewNodesForDocumentRoot = new Dictionary<IDocumentRoot, List<ViewNode>>();
				this.documentRootsForViewNode = new Dictionary<ViewNode, List<IDocumentRoot>>();
			}

			public void Add(IDocumentRoot documentRoot, ViewNode viewNode)
			{
				List<IDocumentRoot> documentRoots;
				List<ViewNode> viewNodes;
				if (!this.documentRootsForViewNode.TryGetValue(viewNode, out documentRoots))
				{
					documentRoots = new List<IDocumentRoot>();
					this.documentRootsForViewNode[viewNode] = documentRoots;
				}
				if (!documentRoots.Contains(documentRoot))
				{
					documentRoots.Add(documentRoot);
				}
				if (!this.viewNodesForDocumentRoot.TryGetValue(documentRoot, out viewNodes))
				{
					viewNodes = new List<ViewNode>();
					this.viewNodesForDocumentRoot[documentRoot] = viewNodes;
				}
				if (!viewNodes.Contains(viewNode))
				{
					viewNodes.Add(viewNode);
				}
			}

			private void ClearBindings(object targetObject)
			{
				DependencyObject dependencyObject = targetObject as DependencyObject;
				if (dependencyObject != null)
				{
					Freezable freezable = dependencyObject as Freezable;
					if (freezable == null || !freezable.IsFrozen)
					{
						try
						{
							LocalValueEnumerator localValueEnumerator = dependencyObject.GetLocalValueEnumerator();
							while (localValueEnumerator.MoveNext())
							{
								if (!(localValueEnumerator.Current.Value is BindingExpression))
								{
									continue;
								}
								dependencyObject.ClearValue(localValueEnumerator.Current.Property);
							}
						}
						catch
						{
						}
					}
				}
			}

			public void Dispose()
			{
				this.viewNodesForDocumentRoot.Clear();
				this.documentRootsForViewNode.Clear();
				GC.SuppressFinalize(this);
			}

			public void OnViewNodeRemoving(ViewNode viewNode)
			{
				List<IDocumentRoot> documentRoots;
				if (this.documentRootsForViewNode.TryGetValue(viewNode, out documentRoots))
				{
					this.documentRootsForViewNode.Remove(viewNode);
					foreach (IDocumentRoot documentRoot in documentRoots)
					{
						List<ViewNode> item = this.viewNodesForDocumentRoot[documentRoot];
						item.Remove(viewNode);
						if (item.Count != 0)
						{
							continue;
						}
						this.viewNodesForDocumentRoot.Remove(documentRoot);
					}
				}
				IInstantiatedElementViewNode instantiatedElementViewNode = viewNode as IInstantiatedElementViewNode;
				if (instantiatedElementViewNode != null && !PlatformTypes.Style.IsAssignableFrom(viewNode.Type) && !PlatformTypes.FrameworkTemplate.IsAssignableFrom(viewNode.Type))
				{
					foreach (object instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
					{
						this.ClearBindings(instantiatedElement);
					}
				}
				this.ClearBindings(viewNode.Instance);
			}
		}

		private class SingleInstanceReference
		{
			public DocumentNodePath EvaluatedValue
			{
				get;
				set;
			}

			public object ParentInstance
			{
				get;
				set;
			}

			public ViewNode SourceViewNode
			{
				get;
				set;
			}

			public SingleInstanceReference()
			{
			}
		}

		private enum UpdateStep
		{
			Initial,
			UpdateInternal,
			ResolveReferences,
			SiteRoot,
			EnsureInstantiatedObjects,
			Final
		}
	}
}