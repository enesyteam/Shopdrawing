using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Core
{
	public class BindingWrapper<T>
	where T : class
	{
		public T Value
		{
			get;
			set;
		}

		public BindingWrapper()
		{
		}

		public static object Unwrap(object value)
		{
			BindingWrapper<T> bindingWrapper = value as BindingWrapper<T>;
			if (bindingWrapper == null)
			{
				return value;
			}
			return bindingWrapper.Value;
		}
	}
}