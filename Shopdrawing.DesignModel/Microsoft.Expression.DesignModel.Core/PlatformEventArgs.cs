using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class PlatformEventArgs : EventArgs
	{
		public IPlatform Platform
		{
			get;
			private set;
		}

		public PlatformEventArgs(IPlatform platform)
		{
			this.Platform = platform;
		}
	}
}