using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Conversion;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class UpgradeProjectCommand : ProjectCommand
	{
		private IProjectConverter converter;

		private ConversionType sourceType;

		private ConversionType targetType;

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandConvertProjectName;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				ProjectBase projectBase = this.SelectedProjectOrNull() as ProjectBase;
				if (projectBase == null)
				{
					return false;
				}
				return this.converter.GetVersion(new ConversionTarget(projectBase.ProjectStore)) == this.sourceType;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (!this.IsAvailable)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		internal UpgradeProjectCommand(IProjectConverter converter, ConversionType sourceType, ConversionType targetType, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			this.converter = converter;
			this.sourceType = sourceType;
			this.targetType = targetType;
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				using (ProjectUpgradeLogger projectUpgradeLogger = new ProjectUpgradeLogger())
				{
					IProject project = this.SelectedProjectOrNull();
					ISolutionManagement solutionManagement = this.Solution() as ISolutionManagement;
					if (base.Services.PromptUserYesNo(StringTable.UpgradeProjectWarning))
					{
						if (this.SaveSolution(true))
						{
							if (this.SaveSolution(true))
							{
								DocumentReference documentReference = project.DocumentReference;
								solutionManagement.CloseProject(project);
								IProjectStore projectStore = null;
								try
								{
									projectStore = ProjectStoreHelper.CreateProjectStore(documentReference, base.Services, ProjectStoreHelper.DefaultProjectCreationChain);
									using (IDisposable disposable = this.SuspendWatchers())
									{
										if (this.converter.Convert(new ConversionTarget(projectStore), this.sourceType, this.targetType))
										{
											using (IDisposable disposable1 = projectUpgradeLogger.SuspendLogging())
											{
												projectStore.Dispose();
												projectStore = ProjectStoreHelper.CreateProjectStore(documentReference, base.Services, ProjectStoreHelper.DefaultProjectCreationChain);
												if (solutionManagement.AddProject(projectStore) == null)
												{
													projectStore.Dispose();
												}
											}
											solutionManagement.OpenInitialViews();
										}
										else
										{
											projectStore.Dispose();
											return;
										}
									}
									UpgradeWizard.SaveLogAndPromptUser(projectUpgradeLogger, base.Services, solutionManagement.DocumentReference.Path, true);
								}
								catch
								{
									if (projectStore != null)
									{
										projectStore.Dispose();
									}
									UpgradeWizard.SaveLogAndPromptUser(projectUpgradeLogger, base.Services, solutionManagement.DocumentReference.Path, false);
									throw;
								}
							}
						}
					}
				}
			});
		}
	}
}