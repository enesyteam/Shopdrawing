using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader.Internal
{
	internal class Debug
	{
		public Debug()
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool f)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool f, string message)
		{
			if (!f)
			{
				Debugger.Log(0, "assert", message);
				string stackTrace = Environment.StackTrace;
				Microsoft.MetadataReader.Internal.Debug.MessageBoxResult messageBoxResult = Microsoft.MetadataReader.Internal.Debug.MessageBox(string.Concat(message, "\r\n", stackTrace, "\r\nAbort - terminate the process\r\nRetry - break into the debugger\r\nIgnore - ignore the assert and continue running"));
				if (messageBoxResult == Microsoft.MetadataReader.Internal.Debug.MessageBoxResult.IDABORT)
				{
					Environment.Exit(1);
				}
				if (messageBoxResult == Microsoft.MetadataReader.Internal.Debug.MessageBoxResult.IDRETRY)
				{
					Debugger.Break();
				}
			}
		}

		[Conditional("DEBUG")]
		public static void Fail(string message)
		{
		}

		private static Microsoft.MetadataReader.Internal.Debug.MessageBoxResult MessageBox(string message)
		{
			return (Microsoft.MetadataReader.Internal.Debug.MessageBoxResult)Microsoft.MetadataReader.Internal.Debug.MessageBoxA(0, message, "LMR Assert failed", 50);
		}

		[DllImport("user32.dll", BestFitMapping=false, CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int MessageBoxA(int h, string m, string c, int type);

		private enum MessageBoxResult
		{
			IDABORT = 3,
			IDRETRY = 4,
			IDIGNORE = 5
		}
	}
}