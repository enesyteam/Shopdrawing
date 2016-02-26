using Microsoft.Expression.DesignModel.Text;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Code
{
	public sealed class TextRange : ITextRange
	{
		public readonly static ITextRange Null;

		private readonly int start;

		private readonly int end;

		public int Length
		{
			get
			{
				return this.end - this.start;
			}
		}

		public int Offset
		{
			get
			{
				return this.start;
			}
		}

		static TextRange()
		{
			TextRange.Null = new TextRange(-1, -1);
		}

		public TextRange(int start, int end)
		{
			this.start = start;
			this.end = end;
		}

		public static bool IsNull(ITextRange range)
		{
			int offset = range.Offset;
			int length = offset + range.Length;
			if (offset != -1)
			{
				return false;
			}
			return length == -1;
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] objArray = new object[] { this.start, this.end };
			return string.Format(invariantCulture, "[{0},{1})", objArray);
		}

		public static ITextRange Union(ITextRange range, ITextRange other)
		{
			if (TextRange.IsNull(range))
			{
				return other;
			}
			if (TextRange.IsNull(other))
			{
				return range;
			}
			return new TextRange(Math.Min(range.Offset, other.Offset), Math.Max(range.Offset + range.Length, other.Offset + other.Length));
		}
	}
}