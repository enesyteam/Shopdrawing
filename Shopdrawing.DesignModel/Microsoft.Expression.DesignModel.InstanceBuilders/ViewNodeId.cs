using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	[TypeConverter(typeof(ViewNodeIdConverter))]
	public class ViewNodeId
	{
		private int viewNodeIndex;

		private Guid serializationContextIndex;

		public Guid SerializationContextIndex
		{
			get
			{
				return this.serializationContextIndex;
			}
		}

		public int ViewNodeIndex
		{
			get
			{
				return this.viewNodeIndex;
			}
		}

		public ViewNodeId(int viewNodeIndex, Guid serializationContextIndex)
		{
			this.viewNodeIndex = viewNodeIndex;
			this.serializationContextIndex = serializationContextIndex;
		}
	}
}