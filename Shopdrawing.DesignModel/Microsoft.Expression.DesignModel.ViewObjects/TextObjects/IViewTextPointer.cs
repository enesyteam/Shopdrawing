using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewTextPointer : IViewObject, IComparable<IViewTextPointer>
	{
		bool HasValidLayout
		{
			get;
		}

		bool IsAtInsertionPosition
		{
			get;
		}

		IViewParagraph Paragraph
		{
			get;
		}

		IViewObject Parent
		{
			get;
		}

		IViewObject GetAdjacentElement(LogicalDirection direction);

		Rect GetCharacterRect(LogicalDirection direction);

		IViewTextPointer GetInsertionPosition(LogicalDirection direction);

		IViewTextPointer GetNextInsertionPosition(LogicalDirection direction);

		int GetOffsetToPosition(IViewTextPointer position);

		IViewTextPointer GetPositionAtOffset(int offset);

		IViewTextPointer GetPositionAtOffset(int offset, LogicalDirection direction);

		bool IsInSameDocument(IViewTextPointer textPosition);
	}
}