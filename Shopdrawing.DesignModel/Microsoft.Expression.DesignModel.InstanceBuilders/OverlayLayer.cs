using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	[ContentProperty("Children")]
	public class OverlayLayer : FrameworkElement
	{
		private OverlayLayer.ElementCollection children;

		public IList<UIElement> Children
		{
			get
			{
				return this.children;
			}
		}

		protected override int VisualChildrenCount
		{
			get
			{
				return this.Children.Count;
			}
		}

		public OverlayLayer()
		{
			this.children = new OverlayLayer.ElementCollection(this);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement child in this.Children)
			{
				if (child == null)
				{
					continue;
				}
				double num = 0;
				double num1 = 0;
				double left = Canvas.GetLeft(child);
				if (!double.IsNaN(left))
				{
					num = left;
				}
				double top = Canvas.GetTop(child);
				if (!double.IsNaN(top))
				{
					num1 = top;
				}
				child.Arrange(new Rect(new Point(num, num1), child.DesiredSize));
			}
			return finalSize;
		}

		public double GetLeft(UIElement child)
		{
			return Canvas.GetLeft(child);
		}

		public double GetTop(UIElement child)
		{
			return Canvas.GetTop(child);
		}

		protected override Visual GetVisualChild(int index)
		{
			return this.Children[index];
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
			for (int i = 0; i < this.Children.Count; i++)
			{
				UIElement item = this.Children[i];
				if (item != null)
				{
					item.Measure(size);
				}
			}
			return new Size();
		}

		public void SafelyRemoveChildren()
		{
			this.children.SafelyClear();
		}

		public void SetLeft(UIElement child, double value)
		{
			Canvas.SetLeft(child, value);
			base.InvalidateArrange();
		}

		public void SetTop(UIElement child, double value)
		{
			Canvas.SetTop(child, value);
			base.InvalidateArrange();
		}

		private class ElementCollection : IList<UIElement>, ICollection<UIElement>, IEnumerable<UIElement>, IEnumerable
		{
			private OverlayLayer parent;

			private List<UIElement> children;

			public int Count
			{
				get
				{
					return this.children.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			UIElement System.Collections.Generic.IList<System.Windows.UIElement>.this[int index]
			{
				get
				{
					return this.children[index];
				}
				set
				{
					if (this.children[index] != value)
					{
						if (value == null)
						{
							throw new ArgumentNullException();
						}
						this.RemoveInternal(this.children[index]);
						this.AddInternal(value);
						this.children[index] = value;
					}
				}
			}

			public ElementCollection(OverlayLayer parent)
			{
				this.parent = parent;
				this.children = new List<UIElement>();
			}

			public void Add(UIElement item)
			{
				this.AddInternal(item);
				this.children.Add(item);
			}

			private void AddInternal(UIElement element)
			{
				if (element == null)
				{
					throw new ArgumentNullException("element");
				}
				this.parent.AddVisualChild(element);
				this.parent.InvalidateMeasure();
			}

			public void Clear()
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					this.RemoveInternal(this.children[i]);
				}
				this.children.Clear();
			}

			public bool Contains(UIElement item)
			{
				return this.children.Contains(item);
			}

			public void CopyTo(UIElement[] array, int arrayIndex)
			{
				this.children.CopyTo(array, arrayIndex);
			}

			public int IndexOf(UIElement item)
			{
				return this.children.IndexOf(item);
			}

			public void Insert(int index, UIElement item)
			{
				this.AddInternal(item);
				this.children.Insert(index, item);
			}

			public bool Remove(UIElement item)
			{
				if (!this.Contains(item))
				{
					return false;
				}
				this.RemoveInternal(item);
				this.children.Remove(item);
				return true;
			}

			public void RemoveAt(int index)
			{
				this.RemoveInternal(this.children[index]);
				this.children.RemoveAt(index);
			}

			private void RemoveInternal(UIElement element)
			{
				if (element == null)
				{
					throw new ArgumentNullException("element");
				}
				this.parent.RemoveVisualChild(element);
			}

			public void SafelyClear()
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					this.SafelyRemoveInternal(this.children[i]);
				}
				this.children.Clear();
			}

			private void SafelyRemoveInternal(UIElement element)
			{
				if (element != null)
				{
					try
					{
						this.parent.RemoveVisualChild(element);
					}
					catch
					{
					}
				}
			}

			IEnumerator<UIElement> System.Collections.Generic.IEnumerable<System.Windows.UIElement>.GetEnumerator()
			{
				return ((IEnumerable<UIElement>)this.children).GetEnumerator();
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.children.GetEnumerator();
			}
		}
	}
}