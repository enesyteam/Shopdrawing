using System;

namespace Microsoft.Expression.DesignModel.Code
{
	public interface ITextUndoTransaction
	{
		bool IsHidden
		{
			get;
		}

		void DisallowMerge();

		void Redo();

		void Undo();
	}
}