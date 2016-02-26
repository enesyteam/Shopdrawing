namespace System.Windows.Controls
{
	#region

	using System.Collections.Generic;
	using System.Windows.Input;
    using System.Windows.Media;

	#endregion

	internal class BorderSelectionLogic : IDisposable
	{
		#region Constants and Fields

        private BorderSelectionAdorner border;

		private readonly IEnumerable<TreeViewExItem> items;

		private TreeViewEx content;

		private bool isFirstMove;

		private bool mouseDown;

		private Point startPoint;

		#endregion

		#region Constructors and Destructors

		public BorderSelectionLogic(TreeViewEx content, IEnumerable<TreeViewExItem> items)
		{
			this.content = content;
			this.items = items;

			content.MouseDown += OnMouseDown;
			content.MouseMove += OnMouseMove;
			content.MouseUp += OnMouseUp;
		}

		#endregion

		#region Public Methods and Operators

		public void Dispose()
		{
			if (content != null)
			{
				content.MouseDown -= OnMouseDown;
				content.MouseMove -= OnMouseMove;
				content.MouseUp -= OnMouseUp;
				content = null;
			}

			GC.SuppressFinalize(this);
		}

		#endregion

		#region Methods

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			mouseDown = true;
			startPoint = Mouse.GetPosition(content);

			// Debug.WriteLine("Initialize drwawing");
			isFirstMove = true;
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (mouseDown)
			{
				if (isFirstMove)
				{
                    isFirstMove = false;
                    border = new BorderSelectionAdorner(content);
					if (!SelectionMultiple.IsControlKeyDown) content.ClearSelectionByRectangle();
				}

				Point currentPoint = Mouse.GetPosition(content);
				double width = currentPoint.X - startPoint.X;
				double height = currentPoint.Y - startPoint.Y;
				double left = startPoint.X;
				double top = startPoint.Y;
                double x = startPoint.X < currentPoint.X ? startPoint.X : currentPoint.X;
                double y = startPoint.Y < currentPoint.Y ? startPoint.Y : currentPoint.Y;
				
                // System.Diagnostics.Debug.WriteLine(string.Format("Drawing: {0};{1};{2};{3}",startPoint.X,startPoint.Y,width,height));
				if (width < 0)
				{
					width = Math.Abs(width);
					left = startPoint.X - width;
				}

				if (height < 0)
				{
					height = Math.Abs(height);
					top = startPoint.Y - height;
				}
                
				border.Width = width;
				border.Height = height;
                border.UpdatePosition(new Point(x, y));

				double right = left + width;
				double bottom = top + height;

				// System.Diagnostics.Debug.WriteLine(string.Format("left:{1};right:{2};top:{3};bottom:{4}", null, left, right, top, bottom));
				foreach (var item in items)
				{
					if (!item.IsVisible)
					{
						continue;
					}

					FrameworkElement itemContent = (FrameworkElement)item.Template.FindName("headerBorder", item);
					Point p = itemContent.TransformToAncestor(content).Transform(new Point());
					double itemLeft = p.X;
					double itemRight = p.X + itemContent.ActualWidth;
					double itemTop = p.Y;
					double itemBottom = p.Y + itemContent.ActualHeight;

					// System.Diagnostics.Debug.WriteLine(string.Format("element:{0};itemleft:{1};itemright:{2};itemtop:{3};itembottom:{4}",item.DataContext,itemLeft,itemRight,itemTop,itemBottom));
					if (!(itemLeft > right || itemRight < left || itemTop > bottom || itemBottom < top))
					{
						if (!content.SelectedItems.Contains(item.DataContext))
							((SelectionMultiple)content.Selection).SelectByRectangle(item);

						// System.Diagnostics.Debug.WriteLine("Is selected: " + item);
					}
					else
					{
						if (!SelectionMultiple.IsControlKeyDown) ((SelectionMultiple)content.Selection).UnSelectByRectangle(item);
					}
				}

				e.Handled = true;
			}
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			mouseDown = false;
			border.Visibility = Visibility.Collapsed;

            border.Dispose();
			// Debug.WriteLine("End drwawing");
		}

		#endregion
	}
}