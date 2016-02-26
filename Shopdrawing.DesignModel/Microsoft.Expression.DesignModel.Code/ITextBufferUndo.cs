using System;

namespace Microsoft.Expression.DesignModel.Code
{
	public interface ITextBufferUndo
	{
		event EventHandler<TextUndoCompletedEventArgs> UndoUnitAdded;
	}
}