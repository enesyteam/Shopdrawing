using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class CrossDocumentUpdateContext : ICrossDocumentUpdateContext
	{
		private IViewRootResolver viewRootResolver;

		private List<CrossDocumentUpdateContext.ContextInfo> contexts = new List<CrossDocumentUpdateContext.ContextInfo>();

		public bool IsDelaying
		{
			get
			{
				return JustDecompileGenerated_get_IsDelaying();
			}
			set
			{
				JustDecompileGenerated_set_IsDelaying(value);
			}
		}

		private bool JustDecompileGenerated_IsDelaying_k__BackingField;

		public bool JustDecompileGenerated_get_IsDelaying()
		{
			return this.JustDecompileGenerated_IsDelaying_k__BackingField;
		}

		private void JustDecompileGenerated_set_IsDelaying(bool value)
		{
			this.JustDecompileGenerated_IsDelaying_k__BackingField = value;
		}

		public CrossDocumentUpdateContext(IViewRootResolver viewRootResolver)
		{
			this.viewRootResolver = viewRootResolver;
		}

		public void BeginUpdate(bool shouldDelay)
		{
			this.IsDelaying = shouldDelay;
		}

		public void DelayInstanceBuilding(IInstanceBuilderContext context, ViewNode viewNode)
		{
			if (this.IsDelaying && this.contexts != null)
			{
				this.GetContextInfo(context, true).AddDelayedNode(viewNode);
			}
		}

		public void EndUpdate()
		{
			IAttachViewRoot attachedViewRoot;
			this.IsDelaying = false;
			List<CrossDocumentUpdateContext.ContextInfo> contextInfos = this.contexts;
			this.contexts = null;
			for (int i = contextInfos.Count - 1; i >= 0; i--)
			{
				CrossDocumentUpdateContext.ContextInfo item = contextInfos[i];
				if (item.DelayedNodes != null)
				{
					item.Context.ViewNodeManager.UpdateDelayedNodes(item.DelayedNodes);
				}
			}
			for (int j = contextInfos.Count - 1; j >= 0; j--)
			{
				CrossDocumentUpdateContext.ContextInfo contextInfo = contextInfos[j];
				InstanceBuilderContextBase context = contextInfo.Context as InstanceBuilderContextBase;
				if (context != null)
				{
					attachedViewRoot = context.AttachedViewRoot;
				}
				else
				{
					attachedViewRoot = null;
				}
				contextInfo.Context.ViewNodeManager.UpdateReferences(attachedViewRoot, false);
			}
		}

		private CrossDocumentUpdateContext.ContextInfo GetContextInfo(IInstanceBuilderContext context, bool createIfMissing)
		{
			CrossDocumentUpdateContext.ContextInfo contextInfo = this.contexts.Find((CrossDocumentUpdateContext.ContextInfo info) => info.Context == context);
			if (contextInfo == null && createIfMissing)
			{
				contextInfo = new CrossDocumentUpdateContext.ContextInfo(context);
				this.contexts.Add(contextInfo);
			}
			return contextInfo;
		}

		public IInstanceBuilderContext GetViewContext(IDocumentRoot documentRoot)
		{
			IInstanceBuilderContext viewContext = this.viewRootResolver.GetViewContext(documentRoot);
			if (viewContext == null)
			{
				return null;
			}
			ViewNode root = viewContext.ViewNodeManager.Root;
			if (root != null && root.InstanceState == InstanceState.Invalid)
			{
				using (IDisposable disposable = viewContext.ChangeCrossDocumentUpdateContext(this))
				{
					viewContext.ViewNodeManager.UpdateInstances(null);
				}
				if (this.contexts != null)
				{
					this.GetContextInfo(viewContext, true);
				}
			}
			return viewContext;
		}

		public void OnViewNodeRemoving(IInstanceBuilderContext context, ViewNode viewNode)
		{
			if (this.contexts != null)
			{
				CrossDocumentUpdateContext.ContextInfo contextInfo = this.GetContextInfo(context, false);
				if (contextInfo != null && contextInfo.DelayedNodes != null)
				{
					contextInfo.DelayedNodes.Remove(viewNode);
				}
			}
		}

		[DebuggerDisplay("{System.IO.Path.GetFileName(this.Context.DocumentContext.DocumentUrl)} - {this.DelayedNodes != null ? this.DelayedNodes.Count : 0}")]
		private class ContextInfo
		{
			public IInstanceBuilderContext Context
			{
				get;
				private set;
			}

			public List<ViewNode> DelayedNodes
			{
				get;
				private set;
			}

			public ContextInfo(IInstanceBuilderContext context)
			{
				this.Context = context;
			}

			public void AddDelayedNode(ViewNode viewNode)
			{
				if (this.DelayedNodes == null)
				{
					this.DelayedNodes = new List<ViewNode>();
				}
				this.DelayedNodes.Add(viewNode);
			}
		}
	}
}