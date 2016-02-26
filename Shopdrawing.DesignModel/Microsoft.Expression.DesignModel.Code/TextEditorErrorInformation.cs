using System;

namespace Microsoft.Expression.DesignModel.Code
{
	public struct TextEditorErrorInformation
	{
		private int start;

		private int length;

		private string description;

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public int Start
		{
			get
			{
				return this.start;
			}
		}

		public TextEditorErrorInformation(int start, int length, string description)
		{
			this.start = start;
			this.length = length;
			this.description = description;
		}
	}
}