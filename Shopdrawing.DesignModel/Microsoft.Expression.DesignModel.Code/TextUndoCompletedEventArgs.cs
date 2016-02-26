using System;

namespace Microsoft.Expression.DesignModel.Code
{
	public sealed class TextUndoCompletedEventArgs : EventArgs
	{
		private TextUndoCompletionResult result;

		private ITextUndoTransaction transaction;

		public TextUndoCompletionResult Result
		{
			get
			{
				return this.result;
			}
		}

		public ITextUndoTransaction Transaction
		{
			get
			{
				return this.transaction;
			}
		}

		public TextUndoCompletedEventArgs(TextUndoCompletionResult result, ITextUndoTransaction transaction)
		{
			this.result = result;
			this.transaction = transaction;
		}
	}
}