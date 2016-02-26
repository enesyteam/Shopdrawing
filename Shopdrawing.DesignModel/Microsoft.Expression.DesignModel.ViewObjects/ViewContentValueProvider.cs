using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public class ViewContentValueProvider
	{
		private object valueProviderObject;

		public object Object
		{
			get
			{
				return this.valueProviderObject;
			}
			set
			{
				this.ObjectType = value.GetType();
				this.valueProviderObject = value;
			}
		}

		public Type ObjectType
		{
			get;
			private set;
		}

		public Size? OverriddenSize
		{
			get;
			set;
		}

		public ViewContentValueProvider(object valueProviderElement, Size? size)
		{
			this.Object = valueProviderElement;
			this.OverriddenSize = size;
		}
	}
}