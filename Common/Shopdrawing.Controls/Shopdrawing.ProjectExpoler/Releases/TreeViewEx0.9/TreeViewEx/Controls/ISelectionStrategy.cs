namespace System.Windows.Controls
{
	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	internal interface ISelectionStrategy : IDisposable
	{
		void ApplyTemplate();

		void SelectFromUiAutomation(TreeViewExItem item);

		void UnSelect(TreeViewExItem item);

		void SelectPreviousFromKey();

		void SelectNextFromKey();

		void SelectCurrentBySpace();

		void Select(TreeViewExItem treeViewExItem);

		void SelectFromProperty(TreeViewExItem item, bool isSelected);

		void SelectFirst();

		void SelectLast();
	}
}
