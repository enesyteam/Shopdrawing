namespace System.Windows.Controls
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Input;

	/// <summary>
	/// Logic for the multiple selection
	/// </summary>
	public class SelectionMultiple : ISelectionStrategy
	{
		private readonly TreeViewEx treeViewEx;

		private BorderSelectionLogic borderSelectionLogic;

		private object lastShiftRoot;

		public SelectionMultiple(TreeViewEx treeViewEx)
		{
			this.treeViewEx = treeViewEx;
		}

		#region Properties

		internal static bool IsControlKeyDown
		{
			get
			{
				return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
			}
		}

		private static bool IsShiftKeyDown
		{
			get
			{
				return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
			}
		}

		#endregion

		public void ApplyTemplate()
		{
			borderSelectionLogic = new BorderSelectionLogic(
						treeViewEx,
						TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, false));
		}

		public void Select(TreeViewExItem item)
		{
			if (!treeViewEx.CheckSelectionAllowed(item.DataContext, true)) return;

			if (IsControlKeyDown && treeViewEx.SelectedItems.Contains(item.DataContext))
			{
				UnSelect(item);
			}
			else
			{
				SelectCore(item);
			}
		}

		internal void UnSelectByRectangle(TreeViewExItem item)
		{
			if (!treeViewEx.CheckSelectionAllowed(item.DataContext, false)) return;

			treeViewEx.SelectedItems.Remove(item.DataContext);
			item.IsSelected = false;
			if (item.DataContext == lastShiftRoot)
			{
				lastShiftRoot = null;
			}
		}

		internal void SelectByRectangle(TreeViewExItem item)
		{
			if (!treeViewEx.CheckSelectionAllowed(item.DataContext, true)) return;
			treeViewEx.SelectedItems.Add(item.DataContext);
			FocusHelper.Focus(item);
			item.IsSelected = true;
			lastShiftRoot = item.DataContext;
		}

		public void SelectFromUiAutomation(TreeViewExItem item)
		{
			SelectCore(item);
		}

		public void SelectFromProperty(TreeViewExItem item, bool isSelected)
		{
			// we do not check if selection is allowed, because selecting on that way is no user action.
			// Hopefully the programmer knows what he does...
			if (isSelected)
			{
				treeViewEx.SelectedItems.Add(item.DataContext);
			}
			else
			{
				treeViewEx.SelectedItems.Remove(item.DataContext);
			}
		}

		private void SelectCore(TreeViewExItem item)
		{
			if (IsControlKeyDown)
			{
				if (!treeViewEx.CheckSelectionAllowed(item.DataContext, true)) return;

				if (treeViewEx.SelectedItems.Contains(item.DataContext))
					throw new InvalidOperationException("The item must not be contained.");

				treeViewEx.SelectedItems.Add(item.DataContext);
				item.IsSelected = true;
				lastShiftRoot = item.DataContext;
			}
			else if (IsShiftKeyDown && treeViewEx.SelectedItems.Count > 0)
			{
				object firstSelectedItem = lastShiftRoot ?? treeViewEx.SelectedItems.First();
				TreeViewExItem shiftRootItem = treeViewEx.GetTreeViewItemsFor(new List<object> { firstSelectedItem }).First();

				IEnumerable<object> items = treeViewEx.GetNodesToSelectBetween(shiftRootItem, item).Select(x => x.DataContext);

				IEnumerable<object> itemsToSelect = GetItemsNotInCollection((IEnumerable<object>)treeViewEx.SelectedItems, items);
				IEnumerable<object> itemsToUnSelect = GetItemsNotInCollection(items, (IEnumerable<object>)treeViewEx.SelectedItems);
				if (!treeViewEx.CheckSelectionAllowed(itemsToSelect, itemsToUnSelect)) return;
				 
				treeViewEx.SelectedItems.Clear();

				foreach (var node in TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, false))
				{
					if (items.Contains(node.DataContext))
					{
						treeViewEx.SelectedItems.Add(node.DataContext);
						node.IsSelected = true;
					}
					else
					{
						node.IsSelected = false;
					}
				}
			}
			else
			{
				if (!treeViewEx.CheckSelectionAllowed(item.DataContext, true)) return;

				// check if selection is already item, otherwise set it
				if (!(treeViewEx.SelectedItems.Count == 1 && treeViewEx.SelectedItems[0] == item.DataContext))
				{
					foreach (var treeViewItem in TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, false))
					{
						treeViewItem.IsSelected = false;
					}

					treeViewEx.SelectedItems.Clear();
					treeViewEx.SelectedItems.Add(item.DataContext);
				}

				lastShiftRoot = item.DataContext;
			}

			FocusHelper.Focus(item);
		}

		public IEnumerable<object> GetItemsNotInCollection(IEnumerable<object> collection1, IEnumerable<object> collection2)
		{
			return collection2.Where(x => !collection1.Contains(x));
		}

		public void SelectCurrentBySpace()
		{
			TreeViewExItem item = GetFocusedItem();
			Select(item);
		}

		private TreeViewExItem GetFocusedItem()
		{
			foreach (var item in TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, false))
			{
				if (item.IsFocused) return item;
			}

			return null;
		}

		public void SelectNextFromKey()
		{
			List<TreeViewExItem> items = TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, true).ToList();
			TreeViewExItem item;
			TreeViewExItem focusedItem = GetFocusedItem();
			item = treeViewEx.GetNextItem(focusedItem, items);

			if (item == null)
			{
				return;
			}

			// if ctrl is pressed just focus it, so it can be selected by space. Otherwise select it.
			if (IsControlKeyDown)
			{
				FocusHelper.Focus(item);
			}
			else
			{
				SelectCore(item);
			}
		}

		public void SelectPreviousFromKey()
		{
			List<TreeViewExItem> items = TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, true).ToList();
			TreeViewExItem item;
			TreeViewExItem focusedItem = GetFocusedItem();
			item = treeViewEx.GetPreviousItem(focusedItem, items);

			if (item == null)
			{
				return;
			}

			// if ctrl is pressed just focus it, so it can be selected by space. Otherwise select it.
			if (IsControlKeyDown)
			{
				FocusHelper.Focus(item);
			}
			else
			{
				SelectCore(item);
			}
		}

		public void UnSelect(TreeViewExItem item)
		{
			if (!treeViewEx.CheckSelectionAllowed(item.DataContext, false)) return;

			treeViewEx.SelectedItems.Remove(item.DataContext);
			item.IsSelected = false;
			if (item.DataContext == lastShiftRoot)
			{
				lastShiftRoot = null;
			}
			FocusHelper.Focus(item);
		}

		public void Dispose()
		{
			if (borderSelectionLogic != null)
			{
				borderSelectionLogic.Dispose();
				borderSelectionLogic = null;
			}

			GC.SuppressFinalize(this);
		}

		#region ISelectionStrategy Members


		public void SelectFirst()
		{
			TreeViewExItem item = TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, false).FirstOrDefault();
			if (item != null)
			{
				Select(item);
			}
		}

		public void SelectLast()
		{
			TreeViewExItem item = TreeViewEx.RecursiveTreeViewItemEnumerable(treeViewEx, false).LastOrDefault();
			if(item != null)
			{
				Select(item);
			}
		}

		#endregion
	}
}
