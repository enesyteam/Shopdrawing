using Microsoft.Expression.DesignModel.ViewObjects;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewTextSelection : IViewTextRange, IViewObject
	{
		void Select(IViewTextPointer selectionStart, IViewTextPointer selectionEnd);
	}
}