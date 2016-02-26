using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Microsoft.Expression.Project
{
	public sealed class ItemSelectionSet : ICollection<IDocumentItem>, IEnumerable<IDocumentItem>, IEnumerable
	{
		private List<IDocumentItem> selection = new List<IDocumentItem>();

		private ReadOnlyCollection<IDocumentItem> readOnlySelection;

		private List<IProject> selectedProjects = new List<IProject>();

		private ReadOnlyCollection<IProject> readOnlySelectedProjects;

		public int Count
		{
			get
			{
				return this.selection.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		public IEnumerable<IProject> SelectedProjects
		{
			get
			{
				return this.readOnlySelectedProjects;
			}
		}

		public IEnumerable<IDocumentItem> Selection
		{
			get
			{
				return this.readOnlySelection;
			}
		}

		bool System.Collections.Generic.ICollection<Microsoft.Expression.Project.IDocumentItem>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ItemSelectionSet()
		{
			this.readOnlySelection = new ReadOnlyCollection<IDocumentItem>(this.selection);
			this.readOnlySelectedProjects = new ReadOnlyCollection<IProject>(this.selectedProjects);
		}

		public void Clear()
		{
			if (!this.IsEmpty)
			{
				this.selection.Clear();
				this.OnSelectionChanged();
			}
		}

		public void ExtendSelection(IDocumentItem element)
		{
			if (!this.IsSelected(element))
			{
				this.selection.Add(element);
				this.OnSelectionChanged();
			}
		}

		public bool IsSelected(IDocumentItem element)
		{
			return this.selection.Contains(element);
		}

		private void OnSelectionChanged()
		{
			this.selectedProjects.Clear();
			foreach (IDocumentItem documentItem in this.selection)
			{
				IProject project = documentItem as IProject;
				if (project == null)
				{
					IProjectItem projectItem = documentItem as IProjectItem;
					if (projectItem != null)
					{
						project = projectItem.Project;
					}
				}
				if (project == null || this.selectedProjects.Contains(project))
				{
					continue;
				}
				this.selectedProjects.Add(project);
			}
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, EventArgs.Empty);
			}
		}

		public void RemoveSelection(IDocumentItem element)
		{
			if (this.IsSelected(element))
			{
				this.selection.Remove(element);
				this.OnSelectionChanged();
			}
		}

		public void RemoveSelection(Func<IDocumentItem, bool> predicate)
		{
			for (int i = this.selection.Count - 1; i >= 0; i--)
			{
				if (predicate(this.selection[i]))
				{
					this.selection.RemoveAt(i);
				}
			}
			this.OnSelectionChanged();
		}

		public void SetSelection(IDocumentItem element)
		{
			if (this.Count != 1 || this.selection[0] != element)
			{
				this.selection.Clear();
				this.selection.Add(element);
				this.OnSelectionChanged();
			}
		}

		void System.Collections.Generic.ICollection<Microsoft.Expression.Project.IDocumentItem>.Add(IDocumentItem item)
		{
			this.ExtendSelection(item);
		}

		bool System.Collections.Generic.ICollection<Microsoft.Expression.Project.IDocumentItem>.Contains(IDocumentItem item)
		{
			return this.IsSelected(item);
		}

		void System.Collections.Generic.ICollection<Microsoft.Expression.Project.IDocumentItem>.CopyTo(IDocumentItem[] array, int arrayIndex)
		{
			this.selection.CopyTo(array, arrayIndex);
		}

		bool System.Collections.Generic.ICollection<Microsoft.Expression.Project.IDocumentItem>.Remove(IDocumentItem item)
		{
			this.RemoveSelection(item);
			return true;
		}

		IEnumerator<IDocumentItem> System.Collections.Generic.IEnumerable<Microsoft.Expression.Project.IDocumentItem>.GetEnumerator()
		{
			return this.readOnlySelection.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.readOnlySelection).GetEnumerator();
		}

		public void ToggleSelection(IDocumentItem element)
		{
			if (!this.selection.Contains(element))
			{
				this.selection.Add(element);
			}
			else
			{
				this.selection.Remove(element);
			}
			this.OnSelectionChanged();
		}

		public event EventHandler SelectionChanged;
	}
}