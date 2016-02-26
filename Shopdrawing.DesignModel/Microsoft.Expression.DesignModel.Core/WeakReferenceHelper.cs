using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class WeakReferenceHelper
	{
		public static T Unwrap<T>(object reference)
		where T : class
		{
			WeakReference weakReference = (WeakReference)reference;
			if (weakReference != null && weakReference.IsAlive)
			{
				return (T)weakReference.Target;
			}
			return default(T);
		}
	}
}