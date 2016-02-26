using System;

namespace Microsoft.Expression.Project.UserInterface
{
	public abstract class ItemModel
	{
		public abstract string DisplayName
		{
			get;
		}

		public abstract bool IsHeaderItem
		{
			get;
		}

		protected ItemModel()
		{
		}
	}
}