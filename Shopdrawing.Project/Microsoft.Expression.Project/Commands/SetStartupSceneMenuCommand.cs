using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class SetStartupSceneMenuCommand : ProjectDynamicMenuCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandSetAsStartupName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				bool flag;
				if (base.IsAvailable && this.Solution() != null)
				{
					using (IEnumerator<IProject> enumerator = this.Solution().Projects.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.GetCapability<bool>("CanHaveStartupItem"))
							{
								continue;
							}
							flag = true;
							return flag;
						}
						return false;
					}
					return flag;
				}
				return false;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				IProject targetProject = this.TargetProject;
				if (!base.IsEnabled || targetProject == null || !targetProject.GetCapability<bool>("CanHaveStartupItem"))
				{
					return false;
				}
				IProject project = targetProject;
				return targetProject.Items.Any<IProjectItem>(new Func<IProjectItem, bool>(project.IsValidStartupItem));
			}
		}

		public override IEnumerable Items
		{
			get
			{
				ArrayList arrayLists = new ArrayList();
				IProject targetProject = this.TargetProject;
				if (targetProject != null)
				{
					IProject project = targetProject;
					foreach (IProjectItem projectItem in targetProject.Items.Where<IProjectItem>(new Func<IProjectItem, bool>(project.IsValidStartupItem)))
					{
						MenuItem menuItem = new MenuItem()
						{
							Header = targetProject.DocumentReference.GetRelativePath(projectItem.DocumentReference),
							IsChecked = projectItem == targetProject.StartupItem,
							Command = new SetStartupSceneMenuCommand.SetSpecificStartupSceneCommand(targetProject, projectItem)
						};
						arrayLists.Add(menuItem);
					}
				}
				return arrayLists;
			}
		}

		private IProject TargetProject
		{
			get
			{
				return this.SelectedProjectOrNull();
			}
		}

		public SetStartupSceneMenuCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			throw new InvalidOperationException();
		}

		private class SetSpecificStartupSceneCommand : System.Windows.Input.ICommand
		{
			private IProject activeProject;

			private IProjectItem item;

			public SetSpecificStartupSceneCommand(IProject activeProject, IProjectItem item)
			{
				this.activeProject = activeProject;
				this.item = item;
			}

			public bool CanExecute(object arg)
			{
				return true;
			}

			public void Execute(object arg)
			{
				this.activeProject.StartupItem = this.item;
			}

			public event EventHandler CanExecuteChanged
			{
				add
				{
				}
				remove
				{
				}
			}
		}
	}
}