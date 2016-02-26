using System;
using System.Windows.Controls.Primitives;

namespace Shopdrawing.Controls
{
	public class StickyButton : ToggleButton, INinchableControl
	{
		public bool IsNinched
		{
			get
			{
				return !base.IsChecked.HasValue;
			}
		}

		public StickyButton()
		{
		}

		public void Ninch()
		{
			base.IsChecked = null;
		}
	}
}