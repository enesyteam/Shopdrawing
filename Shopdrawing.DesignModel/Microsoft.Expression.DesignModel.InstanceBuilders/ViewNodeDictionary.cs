using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class ViewNodeDictionary
	{
		private IDictionary<Guid, ISerializationContext> serializationDictionary;

		private IDictionary<object, Guid> instanceDictionary;

		public ViewNodeDictionary()
		{
			this.serializationDictionary = new Dictionary<Guid, ISerializationContext>();
			this.instanceDictionary = new Dictionary<object, Guid>();
		}

		public void Add(object contextOwner, ISerializationContext context)
		{
			this.instanceDictionary[contextOwner] = context.RegistryToken;
			context.Owner = contextOwner;
		}

		public void Add(ISerializationContext context)
		{
			this.serializationDictionary[context.RegistryToken] = context;
		}

		public ISerializationContext GetSerializationContext(object contextOwner)
		{
			Guid guid;
			ISerializationContext item = null;
			if (this.instanceDictionary.TryGetValue(contextOwner, out guid))
			{
				item = this.serializationDictionary[guid];
			}
			return item;
		}

		public ISerializationContext GetSerializationContextById(ViewNodeId id)
		{
			ISerializationContext serializationContext;
			if (this.serializationDictionary.TryGetValue(id.SerializationContextIndex, out serializationContext) && serializationContext.IsValid(id))
			{
				return serializationContext;
			}
			return null;
		}

		public ViewNode GetViewNode(ViewNodeId id)
		{
			ISerializationContext serializationContext;
			if (!this.serializationDictionary.TryGetValue(id.SerializationContextIndex, out serializationContext) || !serializationContext.IsValid(id))
			{
				return null;
			}
			return serializationContext.GetViewNode(id);
		}

		public void Remove(object contextOwner)
		{
			Guid guid;
			if (this.instanceDictionary.TryGetValue(contextOwner, out guid))
			{
				this.serializationDictionary.Remove(guid);
				this.instanceDictionary.Remove(contextOwner);
			}
		}

		public void Remove(Guid serializationContextRegistryToken)
		{
			this.serializationDictionary.Remove(serializationContextRegistryToken);
		}

		public void Scavenge(ViewNodeManager disposingManager)
		{
			List<object> objs = new List<object>();
			List<Guid> guids = new List<Guid>();
			foreach (KeyValuePair<Guid, ISerializationContext> keyValuePair in this.serializationDictionary)
			{
				if (keyValuePair.Value.ViewNodeManager != disposingManager)
				{
					continue;
				}
				if (keyValuePair.Value.Owner == null)
				{
					guids.Add(keyValuePair.Key);
				}
				else
				{
					objs.Add(keyValuePair.Value.Owner);
				}
			}
			foreach (object obj in objs)
			{
				this.Remove(obj);
			}
			foreach (Guid guid in guids)
			{
				this.Remove(guid);
			}
		}
	}
}