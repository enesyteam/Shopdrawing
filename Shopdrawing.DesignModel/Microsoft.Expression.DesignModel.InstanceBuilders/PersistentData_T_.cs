using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class PersistentData<T>
	{
		private T data;

		private DocumentNodePath target;

		private DocumentNode innerSource;

		public T Data
		{
			get
			{
				return this.data;
			}
		}

		public DocumentNode InnerSource
		{
			get
			{
				return this.innerSource;
			}
		}

		public DocumentNodePath Target
		{
			get
			{
				return this.target;
			}
		}

		public PersistentData(T data, DocumentNodePath target, DocumentNode innerSource)
		{
			this.data = data;
			this.target = target;
			this.innerSource = innerSource;
		}
	}
}