using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project.UserInterface
{
	public class BasicItemModel : ItemModel
	{
		private DocumentReference documentReference;

		public override string DisplayName
		{
			get
			{
				return this.documentReference.DisplayName;
			}
		}

		public override bool IsHeaderItem
		{
			get
			{
				return false;
			}
		}

		public BasicItemModel(DocumentReference documentReference)
		{
			this.documentReference = documentReference;
		}
	}
}