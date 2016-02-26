using Microsoft.Expression.DesignModel.ViewObjects;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewBlockContainer : IViewObject
	{
		ICollection<IViewBlock> Blocks
		{
			get;
		}

		IViewTextPointer ContentEnd
		{
			get;
		}

		IViewTextPointer ContentStart
		{
			get;
		}

		IViewBlock FirstBlock
		{
			get;
		}

		IViewBlock LastBlock
		{
			get;
		}
	}
}