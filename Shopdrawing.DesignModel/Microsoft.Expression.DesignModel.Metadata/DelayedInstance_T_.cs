using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class DelayedInstance<T>
	{
		private DelayedInstance<T>.Instantiate instantiate;

		private bool instantiated;

		private T @value;

		public bool IsInstantiated
		{
			get
			{
				return this.instantiated;
			}
		}

		public T Value
		{
			get
			{
				if (!this.instantiated)
				{
					this.@value = this.instantiate();
					this.instantiated = true;
				}
				return this.@value;
			}
		}

		public DelayedInstance(DelayedInstance<T>.Instantiate instantiate)
		{
			this.instantiate = instantiate;
		}

		public delegate T Instantiate();
	}
}