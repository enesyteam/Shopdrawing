using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public static class TextDecorationCollectionHelper
	{
		public static bool AreEqual(TextDecorationCollection lhsCollection, TextDecorationCollection rhsCollection)
		{
			if (lhsCollection == null && rhsCollection == null)
			{
				return true;
			}
			if (lhsCollection == null || rhsCollection == null)
			{
				return false;
			}
			if (lhsCollection.Count != rhsCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < lhsCollection.Count; i++)
			{
				TextDecoration item = lhsCollection[i];
				TextDecoration textDecoration = rhsCollection[i];
				if ((item != null || textDecoration != null) && (item == null || textDecoration == null || item.Location != textDecoration.Location || item.PenOffset != textDecoration.PenOffset || item.PenOffsetUnit != textDecoration.PenOffsetUnit || item.PenThicknessUnit != textDecoration.PenThicknessUnit || !object.Equals(item.Pen, textDecoration.Pen)))
				{
					return false;
				}
			}
			return true;
		}
	}
}