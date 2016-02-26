using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ProjectTemplateCategoryTreeItem : VirtualizingTreeItem<ProjectTemplateCategoryTreeItem>
	{
		private string[] partialTemplateGroupID;

		private IEnumerable<IProjectTemplate> creatableProjectTemplates;

		private CreateProjectDialog createProjectDialog;

		public override string DisplayName
		{
			get
			{
				if (base.Parent == null)
				{
					return StringTable.AllProjects;
				}
				return this.partialTemplateGroupID[(int)this.partialTemplateGroupID.Length - 1];
			}
			set
			{
			}
		}

		public ICommand DoubleClickCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnDoubleClick));
			}
		}

		public override string FullName
		{
			get
			{
				if (base.Parent == null)
				{
					return this.DisplayName;
				}
				string empty = string.Empty;
				for (int i = 0; i < (int)this.partialTemplateGroupID.Length - 1; i++)
				{
					empty = string.Concat(empty, this.partialTemplateGroupID[i], ProjectTemplateCategoryTreeItem.Separator);
				}
				return string.Concat(empty, this.partialTemplateGroupID[(int)this.partialTemplateGroupID.Length - 1]);
			}
		}

		public bool HasChildren
		{
			get
			{
				return base.Children.Count > 0;
			}
		}

		public bool HasExpandIcon
		{
			get
			{
				if (!this.HasChildren)
				{
					return false;
				}
				return this.Level > 0;
			}
		}

		public bool HasIcon
		{
			get
			{
				return this.Icon != null;
			}
		}

		public Microsoft.Expression.Framework.Controls.Icon Icon
		{
			get
			{
				if (this.DisplayName.Equals("WPF", StringComparison.OrdinalIgnoreCase))
				{
					return new Microsoft.Expression.Framework.Controls.Icon()
					{
						Source = FileTable.GetImageSource("Resources\\WPF.png")
					};
				}
				if (!this.DisplayName.Equals("Silverlight", StringComparison.OrdinalIgnoreCase))
				{
					return this.createProjectDialog.TemplateManager.FindCategoryIcon(this.DisplayName);
				}
				return new Microsoft.Expression.Framework.Controls.Icon()
				{
					Source = FileTable.GetImageSource("Resources\\Silverlight.png")
				};
			}
		}

		public int Level
		{
			get
			{
				if ((int)this.partialTemplateGroupID.Length <= 1 && string.IsNullOrEmpty(this.partialTemplateGroupID[0]))
				{
					return 0;
				}
				return (int)this.partialTemplateGroupID.Length;
			}
		}

		public ICommand SelectCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnSelect));
			}
		}

		private static char Separator
		{
			get
			{
				return ';';
			}
		}

		public ProjectTemplateCategoryTreeItem(string partialTemplateGroupId, IEnumerable<IProjectTemplate> creatableProjectTemplates, CreateProjectDialog createProjectDialog)
		{
			this.createProjectDialog = createProjectDialog;
			char[] separator = new char[] { ProjectTemplateCategoryTreeItem.Separator };
			this.partialTemplateGroupID = partialTemplateGroupId.Split(separator);
			this.creatableProjectTemplates = creatableProjectTemplates;
			base.PropertyChanged += new PropertyChangedEventHandler(this.ProjectTemplateCategoryTreeItem_PropertyChanged);
			this.AddChildren();
		}

		public void AddChildren()
		{
			List<string> strs = new List<string>();
			foreach (IProjectTemplate creatableProjectTemplate in this.creatableProjectTemplates)
			{
				string empty = string.Empty;
				if (string.IsNullOrEmpty(creatableProjectTemplate.TemplateGroupID) || !this.IsChildTemplateGroupID(creatableProjectTemplate.TemplateGroupID))
				{
					continue;
				}
				string childTemplateGroupId = this.GetChildTemplateGroupId(creatableProjectTemplate.TemplateGroupID);
				if (strs.Contains(childTemplateGroupId))
				{
					continue;
				}
				strs.Add(childTemplateGroupId);
				ProjectTemplateCategoryTreeItem projectTemplateCategoryTreeItem = new ProjectTemplateCategoryTreeItem(childTemplateGroupId, this.creatableProjectTemplates, this.createProjectDialog)
				{
					Depth = base.Depth + 1
				};
				base.AddChild(projectTemplateCategoryTreeItem);
			}
		}

		protected void ClearSelection()
		{
			if (this.createProjectDialog != null && this.createProjectDialog.SelectedCategoryItem != null)
			{
				this.createProjectDialog.SelectedCategoryItem.IsSelected = false;
			}
		}

		public override int CompareTo(ProjectTemplateCategoryTreeItem treeItem)
		{
			return string.Compare(this.FullName, treeItem.FullName, StringComparison.OrdinalIgnoreCase);
		}

		public IProjectTemplate FindActualTemplate(IProjectTemplate selectedTemplate, string projectType)
		{
			IProjectTemplate projectTemplate;
			if (selectedTemplate == null)
			{
				return null;
			}
			using (IEnumerator<IProjectTemplate> enumerator = this.creatableProjectTemplates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProjectTemplate current = enumerator.Current;
					if (!this.IsTemplateAvailable(current) || !current.TemplateID.Equals(selectedTemplate.TemplateID, StringComparison.OrdinalIgnoreCase) || !current.TemplateID.Equals(selectedTemplate.TemplateID, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(projectType) || !string.IsNullOrEmpty(current.ProjectType)) && (projectType == null || current.ProjectType == null || !projectType.Equals(current.ProjectType, StringComparison.OrdinalIgnoreCase)))
					{
						continue;
					}
					projectTemplate = current;
					return projectTemplate;
				}
				return selectedTemplate;
			}
			return projectTemplate;
		}

		private string GetChildTemplateGroupId(string templateGroupId)
		{
			string empty = string.Empty;
			char[] separator = new char[] { ProjectTemplateCategoryTreeItem.Separator };
			string[] strArrays = templateGroupId.Split(separator);
			for (int i = 0; i < this.Level; i++)
			{
				empty = string.Concat(empty, strArrays[i], ProjectTemplateCategoryTreeItem.Separator);
			}
			return string.Concat(empty, strArrays[this.Level]);
		}

		private bool IsChildTemplateGroupID(string templateGroupID)
		{
			if ((int)this.partialTemplateGroupID.Length == 1 && string.IsNullOrEmpty(this.partialTemplateGroupID[0]))
			{
				return true;
			}
			if (string.IsNullOrEmpty(templateGroupID))
			{
				return false;
			}
			char[] separator = new char[] { ProjectTemplateCategoryTreeItem.Separator };
			string[] strArrays = templateGroupID.Split(separator);
			if ((int)strArrays.Length <= (int)this.partialTemplateGroupID.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)this.partialTemplateGroupID.Length; i++)
			{
				if (string.Compare(this.partialTemplateGroupID[i], strArrays[i], StringComparison.OrdinalIgnoreCase) != 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsTemplateAvailable(IProjectTemplate template)
		{
			if (string.IsNullOrEmpty(template.TemplateGroupID) && base.Parent == null)
			{
				return true;
			}
			if (!this.FullName.Equals(template.TemplateGroupID, StringComparison.OrdinalIgnoreCase) && !this.IsChildTemplateGroupID(template.TemplateGroupID))
			{
				return false;
			}
			string templateGroupID = template.TemplateGroupID;
			char[] separator = new char[] { ProjectTemplateCategoryTreeItem.Separator };
			if ((int)templateGroupID.Split(separator).Length - base.Depth > template.NumberOfParentCategoriesToRollUp)
			{
				return false;
			}
			return true;
		}

		public bool IsTemplateShown(IProjectTemplate template, string projectType)
		{
			bool flag;
			if (!this.IsTemplateAvailable(template))
			{
				return false;
			}
			IProjectTemplate projectTemplate = null;
			bool flag1 = false;
			using (IEnumerator<IProjectTemplate> enumerator = this.creatableProjectTemplates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProjectTemplate current = enumerator.Current;
					if (!template.TemplateID.Equals(current.TemplateID, StringComparison.OrdinalIgnoreCase) || !this.IsTemplateAvailable(current))
					{
						continue;
					}
					if (projectType != null && projectType.Equals(current.ProjectType, StringComparison.OrdinalIgnoreCase))
					{
						flag1 = true;
						if (current == template)
						{
							flag = true;
							return flag;
						}
					}
					if (projectTemplate != null)
					{
						continue;
					}
					projectTemplate = current;
				}
				if (!flag1 && projectTemplate != null && projectTemplate == template)
				{
					return true;
				}
				return false;
			}
			return flag;
		}

		private void OnDoubleClick()
		{
			if (this.Level > 0)
			{
				this.IsExpanded = !this.IsExpanded;
			}
		}

		private void OnSelect()
		{
			this.Select();
		}

		private void ProjectTemplateCategoryTreeItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSelected")
			{
				this.createProjectDialog.SelectedCategoryItem = this;
			}
		}

		public override void Select()
		{
			base.Select();
			this.ClearSelection();
			base.IsSelected = true;
			this.createProjectDialog.SelectedCategoryItem = this;
			for (ProjectTemplateCategoryTreeItem i = base.Parent; i != null; i = i.Parent)
			{
				i.IsExpanded = true;
			}
		}

		public bool SelectCategoryByFullName(string categoryFullName)
		{
			bool flag;
			if (this.FullName.Equals(categoryFullName, StringComparison.OrdinalIgnoreCase))
			{
				this.Select();
				return true;
			}
			char[] separator = new char[] { ProjectTemplateCategoryTreeItem.Separator };
			string[] strArrays = categoryFullName.Split(separator);
			using (IEnumerator<ProjectTemplateCategoryTreeItem> enumerator = base.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProjectTemplateCategoryTreeItem current = enumerator.Current;
					if (!current.DisplayName.Equals(strArrays[current.Level - 1], StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					flag = current.SelectCategoryByFullName(categoryFullName);
					return flag;
				}
				return false;
			}
			return flag;
		}
	}
}