using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public class ViewContent
	{
		public object Content
		{
			get;
			set;
		}

		public bool IsHeightUnknown
		{
			get;
			set;
		}

		public bool IsSizeFixed
		{
			get;
			set;
		}

		public bool IsWidthUnknown
		{
			get;
			set;
		}

		public Microsoft.Expression.DesignModel.ViewObjects.ViewContentType ViewContentType
		{
			get;
			set;
		}

		public ViewContent(Microsoft.Expression.DesignModel.ViewObjects.ViewContentType viewContentType, object content, bool isWidthUnknown, bool isHeightUnknown) : this(viewContentType, content, isWidthUnknown, isHeightUnknown, false)
		{
		}

		public ViewContent(Microsoft.Expression.DesignModel.ViewObjects.ViewContentType viewContentType, object content, bool isWidthUnknown, bool isHeightUnknown, bool isSizeFixed)
		{
			this.ViewContentType = viewContentType;
			this.Content = content;
			this.IsWidthUnknown = isWidthUnknown;
			this.IsHeightUnknown = isHeightUnknown;
			this.IsSizeFixed = isSizeFixed;
		}
	}
}