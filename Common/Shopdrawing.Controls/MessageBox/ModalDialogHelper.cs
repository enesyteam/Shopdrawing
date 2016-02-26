using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Shopdrawing.Framework.Controls
{
	public class ModalDialogHelper : IDisposable
	{
		private List<Window> windows;

		public ModalDialogHelper()
		{
			this.EnterThreadModal();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.ExitThreadModal();
			}
		}

		private void EnableThreadWindows(bool state)
		{
			if (this.windows != null)
			{
				for (int i = 0; i < this.windows.Count; i++)
				{
					this.windows[i].IsHitTestVisible = state;
				}
			}
		}

		private void EnterThreadModal()
		{
			foreach (Window window in Application.Current.Windows)
			{
				if (!window.IsHitTestVisible)
				{
					continue;
				}
				if (this.windows == null)
				{
					this.windows = new List<Window>();
				}
				this.windows.Add(window);
			}
			this.EnableThreadWindows(false);
		}

		private void ExitThreadModal()
		{
			this.EnableThreadWindows(true);
			this.windows = null;
		}
	}
}