using System;

namespace Microsoft.Expression.Project.Conversion
{
	internal class ConversionSupressor : IDisposable
	{
		private static int isSupressed;

		internal static bool IsSupressed
		{
			get
			{
				return ConversionSupressor.isSupressed > 0;
			}
		}

		static ConversionSupressor()
		{
		}

		internal ConversionSupressor()
		{
			ConversionSupressor.Supress();
		}

		public void Dispose()
		{
			ConversionSupressor.Unsupress();
			GC.SuppressFinalize(this);
		}

		private static void Supress()
		{
			ConversionSupressor.isSupressed = ConversionSupressor.isSupressed + 1;
		}

		private static void Unsupress()
		{
			ConversionSupressor.isSupressed = ConversionSupressor.isSupressed - 1;
		}
	}
}