using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class DictionaryInstanceBuilder : ClrObjectInstanceBuilder
	{
		public override Type BaseType
		{
			get
			{
				return typeof(IDictionary);
			}
		}

		public DictionaryInstanceBuilder()
		{
		}

		protected virtual void AddEntry(ViewNode dictionaryNode, object resourceKey, object value)
		{
			IInstantiatedElementViewNode instantiatedElementViewNode = dictionaryNode as IInstantiatedElementViewNode;
			if (instantiatedElementViewNode != null)
			{
				foreach (IDictionary instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
				{
					instantiatedElement[resourceKey] = value;
				}
			}
			IDictionary instance = dictionaryNode.Instance as IDictionary;
			if (instance != null)
			{
				instance[resourceKey] = value;
			}
		}

		protected virtual object GetDictionaryKey(ITypeResolver typeResolver, DictionaryEntry entry)
		{
			return entry.Key;
		}

		public override void Initialize(IInstanceBuilderContext context, ViewNode viewNode, bool isNewInstance)
		{
			if (context.IsSerializationScope || viewNode.Instance == null || viewNode.InstanceState != InstanceState.Uninitialized)
			{
				base.Initialize(context, viewNode, isNewInstance);
				return;
			}
			DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
			DocumentCompositeNode documentCompositeNode = documentNode;
			if (documentNode != null)
			{
				this.InstantiateProperties(context, viewNode, documentCompositeNode);
				if (documentCompositeNode.SupportsChildren)
				{
					for (int i = 0; i < documentCompositeNode.Children.Count; i++)
					{
						DocumentNode item = documentCompositeNode.Children[i];
						IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(item.TargetType);
						ViewNode viewNode1 = builder.GetViewNode(context, item);
						viewNode.Children.Add(viewNode1);
						this.InstantiateChild(context, viewNode, viewNode1);
					}
				}
			}
			viewNode.InstanceState = InstanceState.Valid;
			this.OnInitialized(context, viewNode, viewNode.Instance);
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			if (context.IsSerializationScope)
			{
				return base.Instantiate(context, viewNode);
			}
			if (viewNode.Instance != null)
			{
				return false;
			}
			viewNode.Instance = this.InstantiateTargetType(context, viewNode);
			return true;
		}

		protected virtual void InstantiateChild(IInstanceBuilderContext context, ViewNode dictionaryNode, ViewNode childNode)
		{
			context.ViewNodeManager.Instantiate(childNode);
			DictionaryEntry instance = (DictionaryEntry)childNode.Instance;
			object dictionaryKey = this.GetDictionaryKey(childNode.TypeResolver, instance);
			if (dictionaryKey != null)
			{
				this.AddEntry(dictionaryNode, dictionaryKey, instance.Value);
			}
		}

		private void RemoveChildViewNodeFromInstance(ViewNode viewNode, ViewNode childViewNode)
		{
			if (!(childViewNode.Instance is DocumentNode) && !(viewNode.Instance is DocumentNode))
			{
				object instance = childViewNode.Instance;
				if (instance is DictionaryEntry)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)instance;
					object dictionaryKey = this.GetDictionaryKey(viewNode.TypeResolver, dictionaryEntry);
					this.RemoveEntry(viewNode, dictionaryKey);
				}
			}
		}

		protected virtual void RemoveEntry(ViewNode dictionaryNode, object resourceKey)
		{
			if (resourceKey != null)
			{
				IInstantiatedElementViewNode instantiatedElementViewNode = dictionaryNode as IInstantiatedElementViewNode;
				if (instantiatedElementViewNode != null)
				{
					foreach (IDictionary instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
					{
						instantiatedElement.Remove(resourceKey);
					}
				}
				IDictionary instance = dictionaryNode.Instance as IDictionary;
				if (instance != null)
				{
					instance.Remove(resourceKey);
				}
			}
		}

		public override void UpdateChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, DocumentNodeChangeAction action, DocumentNode childNode)
		{
			if (action == DocumentNodeChangeAction.Remove && childIndex < viewNode.Children.Count)
			{
				ViewNode item = viewNode.Children[childIndex];
				this.RemoveChildViewNodeFromInstance(viewNode, item);
				viewNode.Children.Remove(item);
			}
			if (action == DocumentNodeChangeAction.Add || action == DocumentNodeChangeAction.Replace)
			{
				IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(childNode.TargetType);
				ViewNode viewNode1 = builder.GetViewNode(context, childNode);
				if (action != DocumentNodeChangeAction.Replace)
				{
					viewNode.Children.Insert(childIndex, viewNode1);
				}
				else
				{
					this.RemoveChildViewNodeFromInstance(viewNode, viewNode.Children[childIndex]);
					viewNode.Children[childIndex] = viewNode1;
				}
				this.InstantiateChild(context, viewNode, viewNode1);
			}
		}
	}
}