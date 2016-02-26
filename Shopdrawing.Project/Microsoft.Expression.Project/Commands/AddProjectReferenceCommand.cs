using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Project.Commands
{
	internal class AddProjectReferenceCommand : ProjectDynamicMenuCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandAddProjectReferenceName;
			}
		}

		public override string EmptyMenuItemText
		{
			get
			{
				return StringTable.ProjectItemContextMenuNoValidProjectsToReference;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				if (this.Solution() is WebProjectSolution)
				{
					return false;
				}
				return base.IsAvailable;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				IProject project = this.SelectedProjectOrNull();
				if (!base.IsEnabled || project == null)
				{
					return false;
				}
				return project.GetCapability<bool>("CanAddProjectReference");
			}
		}

		public override IEnumerable Items
		{
			get
			{
				bool flag;
				ArrayList arrayLists = new ArrayList();
				IProject project = this.SelectedProjectOrNull();
				if (project == null)
				{
					return arrayLists;
				}
				ISolution solution = this.Solution();
				List<IProject> list = solution.Projects.ToList<IProject>();
				int count = list.Count;
				int num = list.IndexOf(project);
				if (count >= 2)
				{
					bool[][] flagArray = new bool[count][];
					for (int i = 0; i < count; i++)
					{
						flagArray[i] = new bool[count];
						flagArray[i][i] = true;
						foreach (ProjectAssembly referencedAssembly in list[i].ReferencedAssemblies)
						{
							if (!(referencedAssembly.ProjectItem is ProjectReferenceProjectItem))
							{
								continue;
							}
							IProject project1 = solution.FindProject(referencedAssembly.ProjectItem.DocumentReference) as IProject;
							if (project1 == null)
							{
								continue;
							}
							flagArray[i][list.IndexOf(project1)] = true;
						}
					}
					for (int j = 0; j < count; j++)
					{
						for (int k = 0; k < count; k++)
						{
							for (int l = 0; l < count; l++)
							{
								bool[] flagArray1 = flagArray[k];
								int num1 = l;
								if (flagArray[k][l])
								{
									flag = true;
								}
								else
								{
									flag = (!flagArray[k][j] ? false : flagArray[j][l]);
								}
								flagArray1[num1] = flag;
							}
						}
					}
					for (int m = 0; m < count; m++)
					{
						if (!flagArray[num][m] && !flagArray[m][num] && list[m] is MSBuildBasedProject)
						{
							MenuItem menuItem = new MenuItem()
							{
								Header = list[m].Name
							};
							IWindowService service = base.Services.GetService(typeof(IWindowService)) as IWindowService;
							menuItem.Command = new AddProjectReferenceCommand.AddSpecificProjectReferenceCommand(service, list[num], list[m]);
							arrayLists.Add(menuItem);
						}
					}
				}
				return arrayLists;
			}
		}

		public AddProjectReferenceCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
		}

		private class AddSpecificProjectReferenceCommand : System.Windows.Input.ICommand
		{
			private IProject projectReferencing;

			private IProject projectToReference;

			private IWindowService windowService;

			public AddSpecificProjectReferenceCommand(IWindowService windowService, IProject projectReferencing, IProject projectToReference)
			{
				this.windowService = windowService;
				this.projectReferencing = projectReferencing;
				this.projectToReference = projectToReference;
			}

			public bool CanExecute(object arg)
			{
				return true;
			}

			public void Execute(object arg)
			{
				if (this.projectReferencing.AddProjectReference(this.projectToReference) != null)
				{
					ProjectCommand.ActivateProjectPane(this.windowService);
				}
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