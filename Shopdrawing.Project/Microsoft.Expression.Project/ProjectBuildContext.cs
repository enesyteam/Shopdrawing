using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectBuildContext : IProjectBuildContext, IBuildWorker
	{
		private Func<Microsoft.Build.Execution.ProjectInstance, IEnumerable<ILogger>, string[], Microsoft.Expression.Project.Build.BuildResult> buildWorker;

		private Action<Microsoft.Expression.Project.Build.BuildResult> buildCompleteCallback;

		private Func<Microsoft.Build.Execution.ProjectInstance> projectInstanceProvider;

		public IErrorTaskCollection BuildErrors
		{
			get
			{
				return JustDecompileGenerated_get_BuildErrors();
			}
			set
			{
				JustDecompileGenerated_set_BuildErrors(value);
			}
		}

		private IErrorTaskCollection JustDecompileGenerated_BuildErrors_k__BackingField;

		public IErrorTaskCollection JustDecompileGenerated_get_BuildErrors()
		{
			return this.JustDecompileGenerated_BuildErrors_k__BackingField;
		}

		private void JustDecompileGenerated_set_BuildErrors(IErrorTaskCollection value)
		{
			this.JustDecompileGenerated_BuildErrors_k__BackingField = value;
		}

		public IBuildWorker BuildWorker
		{
			get
			{
				return this;
			}
		}

		public string DisplayName
		{
			get
			{
				return JustDecompileGenerated_get_DisplayName();
			}
			set
			{
				JustDecompileGenerated_set_DisplayName(value);
			}
		}

		private string JustDecompileGenerated_DisplayName_k__BackingField;

		public string JustDecompileGenerated_get_DisplayName()
		{
			return this.JustDecompileGenerated_DisplayName_k__BackingField;
		}

		private void JustDecompileGenerated_set_DisplayName(string value)
		{
			this.JustDecompileGenerated_DisplayName_k__BackingField = value;
		}

		private IEnumerable<ILogger> Loggers
		{
			get;
			set;
		}

		public Microsoft.Build.Execution.ProjectInstance ProjectInstance
		{
			get
			{
				return JustDecompileGenerated_get_ProjectInstance();
			}
			set
			{
				JustDecompileGenerated_set_ProjectInstance(value);
			}
		}

		private Microsoft.Build.Execution.ProjectInstance JustDecompileGenerated_ProjectInstance_k__BackingField;

		public Microsoft.Build.Execution.ProjectInstance JustDecompileGenerated_get_ProjectInstance()
		{
			return this.JustDecompileGenerated_ProjectInstance_k__BackingField;
		}

		private void JustDecompileGenerated_set_ProjectInstance(Microsoft.Build.Execution.ProjectInstance value)
		{
			this.JustDecompileGenerated_ProjectInstance_k__BackingField = value;
		}

		private ProjectBuildContext(Func<Microsoft.Build.Execution.ProjectInstance, IEnumerable<ILogger>, string[], Microsoft.Expression.Project.Build.BuildResult> buildWorker, Action<Microsoft.Expression.Project.Build.BuildResult> buildCompleteCallback, Func<Microsoft.Build.Execution.ProjectInstance> projectInstanceProvider, IEnumerable<ILogger> loggers, string displayName, IErrorTaskCollection buildErrors)
		{
			this.buildWorker = buildWorker;
			this.buildCompleteCallback = buildCompleteCallback;
			this.projectInstanceProvider = projectInstanceProvider;
			this.Loggers = loggers;
			this.DisplayName = displayName;
			this.BuildErrors = buildErrors;
		}

		public Microsoft.Expression.Project.Build.BuildResult Build(params string[] targetNames)
		{
			return ((IBuildWorker)this).Build(this.Loggers, targetNames);
		}

		public void BuildCompleted(Microsoft.Expression.Project.Build.BuildResult buildResult)
		{
			this.buildCompleteCallback(buildResult);
		}

		public static ProjectBuildContext CreateBuildContext(Func<Microsoft.Build.Execution.ProjectInstance, IEnumerable<ILogger>, string[], Microsoft.Expression.Project.Build.BuildResult> buildWorker, Action<Microsoft.Expression.Project.Build.BuildResult> buildCompleteCallback, Func<Microsoft.Build.Execution.ProjectInstance> projectInstanceProvider, IEnumerable<ILogger> loggers, string displayName, IErrorTaskCollection buildErrors)
		{
			return new ProjectBuildContext(buildWorker, buildCompleteCallback, projectInstanceProvider, loggers, displayName, buildErrors);
		}

		Microsoft.Expression.Project.Build.BuildResult Microsoft.Expression.Project.Build.IBuildWorker.Build(IEnumerable<ILogger> loggers, params string[] targetNames)
		{
			this.ProjectInstance = this.projectInstanceProvider();
			return this.buildWorker(this.ProjectInstance, loggers, targetNames);
		}
	}
}