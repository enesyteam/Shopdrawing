using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Project.UserInterface
{
	public abstract class HierarchicalNode : VirtualizingTreeItem<HierarchicalNode>, IDragDropHandler, IDisposable
	{
		private IDocumentItem documentItem;

		private IServiceProvider serviceProvider;

		private System.Windows.Controls.ContextMenu contextMenu;

		public virtual System.Windows.Controls.ContextMenu ContextMenu
		{
			get
			{
				return this.contextMenu;
			}
			protected set
			{
				this.contextMenu = value;
				if (value != null)
				{
					FocusScopeManager.SetAllowedFocus(value, true);
				}
				base.OnPropertyChanged("ContextMenu");
			}
		}

		public ICommand CreateContextMenuCommand
		{
			get
			{
				HierarchicalNode hierarchicalNode = this;
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(hierarchicalNode.OnCreateContextMenuCommand));
			}
		}

		public ICommand EnsureSelectCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnEnsureSelectCommand));
			}
		}

		public ICommand EnsureSelectionContainsCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnEnsureSelectionContainsCommand));
			}
		}

		public int IndentDepth
		{
			get
			{
				return Math.Max(0, base.Depth - this.ProjectPane.SolutionDepth - 1);
			}
		}

		private bool IsDisposed
		{
			get
			{
				return this.ProjectPane == null;
			}
		}

		protected Microsoft.Expression.Project.UserInterface.ProjectPane ProjectPane
		{
			get;
			set;
		}

		public ICommand SelectCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnSelectCommand));
			}
		}

		internal IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		protected virtual bool ShouldReturnFocus
		{
			get
			{
				if (Keyboard.FocusedElement == null || this.IsDisposed)
				{
					return false;
				}
				return this.ProjectPane.IsKeyboardFocusWithin;
			}
		}

		public Microsoft.Expression.Framework.SourceControl.SourceControlStatus SourceControlStatus
		{
			get
			{
				if (this.documentItem is AssemblyReferenceProjectItem)
				{
					return Microsoft.Expression.Framework.SourceControl.SourceControlStatus.None;
				}
				return SourceControlStatusCache.GetCachedStatus(this.documentItem);
			}
		}

		public ICommand ToggleSelectCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnToggleSelectCommand));
			}
		}

		internal HierarchicalNode()
		{
		}

		internal HierarchicalNode(IDocumentItem documentItem, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane)
		{
			this.ProjectPane = projectPane;
			this.documentItem = documentItem;
			this.serviceProvider = projectPane.Services;
			if (documentItem != null)
			{
				base.IsSelected = this.Services.ProjectManager().ItemSelectionSet.IsSelected(this.documentItem);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (HierarchicalNode child in base.Children)
				{
					child.Dispose();
				}
				base.Children.Clear();
				this.ProjectPane = null;
			}
		}

		protected abstract void ExpandAllParents();

		protected abstract void OnCreateContextMenuCommand();

		public virtual void OnDragBegin(DragBeginEventArgs e)
		{
		}

		public virtual void OnDragEnter(DragEventArgs e)
		{
		}

		public virtual void OnDragLeave(DragEventArgs e)
		{
		}

		public virtual void OnDragOver(DragEventArgs e)
		{
		}

		public virtual void OnDrop(DragEventArgs e)
		{
		}

		private void OnEnsureSelectCommand()
		{
			if (!this.IsDisposed && !this.Services.ProjectManager().ItemSelectionSet.IsSelected(this.documentItem))
			{
				this.Services.ProjectManager().ItemSelectionSet.SetSelection(this.documentItem);
				this.OnSelectionChanged();
			}
		}

		private void OnEnsureSelectionContainsCommand()
		{
			if (!this.IsDisposed && !this.Services.ProjectManager().ItemSelectionSet.IsSelected(this.documentItem))
			{
				this.Services.ProjectManager().ItemSelectionSet.ExtendSelection(this.documentItem);
				this.OnSelectionChanged();
			}
		}

		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
		}

		public void OnQueryContinueDrag(QueryContinueDragEventArgs e)
		{
		}

		private void OnSelectCommand()
		{
			if (!this.IsDisposed && (this.Services.ProjectManager().ItemSelectionSet.Count != 1 || !this.Services.ProjectManager().ItemSelectionSet.Selection.Contains<IDocumentItem>(this.documentItem)))
			{
				this.Services.ProjectManager().ItemSelectionSet.SetSelection(this.documentItem);
				this.OnSelectionChanged();
			}
		}

		protected void OnSelectionChanged()
		{
			if (!this.IsDisposed)
			{
				if (this.ShouldReturnFocus)
				{
					Keyboard.Focus(null);
				}
				bool flag = this.Services.ProjectManager().ItemSelectionSet.IsSelected(this.documentItem);
				if (base.IsSelected != flag)
				{
					if (flag)
					{
						this.ExpandAllParents();
						this.ProjectPane.ScrollIntoView(this);
					}
					base.IsSelected = flag;
				}
			}
		}

		private void OnToggleSelectCommand()
		{
			if (!this.IsDisposed)
			{
				this.Services.ProjectManager().ItemSelectionSet.ToggleSelection(this.documentItem);
				this.OnSelectionChanged();
			}
		}

		internal virtual void UpdateSelection()
		{
			this.OnSelectionChanged();
		}
	}
}