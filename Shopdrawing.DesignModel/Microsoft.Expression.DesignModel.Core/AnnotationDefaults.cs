using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class AnnotationDefaults
	{
		public static string Author
		{
			get
			{
				return string.Empty;
			}
		}

		public static string AuthorInitials
		{
			get
			{
				return string.Empty;
			}
		}

		public static string Id
		{
			get
			{
				return string.Empty;
			}
		}

		public static double Left
		{
			get
			{
				return double.NaN;
			}
		}

		public static string References
		{
			get
			{
				return string.Empty;
			}
		}

		public static int SerialNumber
		{
			get
			{
				return 1;
			}
		}

		public static string Text
		{
			get
			{
				return string.Empty;
			}
		}

		public static DateTime Timestamp
		{
			get
			{
				return DateTime.UtcNow;
			}
		}

		public static double Top
		{
			get
			{
				return double.NaN;
			}
		}

		public static bool VisibleAtRuntime
		{
			get
			{
				return true;
			}
		}
	}
}