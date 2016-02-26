using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class DataContextWithoutSourceBreakFixer
	{
		private List<object> processingElements = new List<object>();

		public DataContextWithoutSourceBreakFixer()
		{
		}

		public bool IsProcessing(object element)
		{
			return this.processingElements.Contains(element);
		}

		public IDisposable Push(object element)
		{
			return new DataContextWithoutSourceBreakFixer.PushToken(this.processingElements, element);
		}

		private class PushToken : IDisposable
		{
			private List<object> processingElements;

			private object element;

			public PushToken(List<object> processingElements, object element)
			{
				this.processingElements = processingElements;
				this.element = element;
				this.processingElements.Add(this.element);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool isDisposing)
			{
				if (isDisposing)
				{
					this.processingElements.Remove(this.element);
				}
			}
		}
	}
}