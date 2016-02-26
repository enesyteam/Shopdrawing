using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.Project.Build
{
	internal class HostLogger : ILogger
	{
		private IProjectBuildContext buildTarget;

		private IMessageLoggingService messageLogger;

		private IServiceProvider serviceProvider;

		private IEventSource eventSource;

		private IDictionary<int, string> projectIdToProjectPathDictionary = new Dictionary<int, string>();

		private HostLogger.AlterableConsoleLogger consoleLogger;

		private StringBuilder deferredMessages;

		public string Parameters
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public LoggerVerbosity Verbosity
		{
			get
			{
				return this.consoleLogger.Verbosity;
			}
			set
			{
				this.consoleLogger.Verbosity = value;
			}
		}

		public HostLogger(IProjectBuildContext buildTarget, IServiceProvider serviceProvider)
		{
			this.buildTarget = buildTarget;
			this.messageLogger = (IMessageLoggingService)serviceProvider.GetService(typeof(IMessageLoggingService));
			this.serviceProvider = serviceProvider;
			this.consoleLogger = new HostLogger.AlterableConsoleLogger(LoggerVerbosity.Normal);
			this.consoleLogger.SetWriter(new WriteHandler(this.Writer));
		}

		private string GetProjectPathFromBuildEventContext(BuildEventContext context)
		{
			string displayName;
			int projectContextId = context.ProjectContextId;
			if (!this.projectIdToProjectPathDictionary.TryGetValue(projectContextId, out displayName))
			{
				displayName = this.buildTarget.DisplayName;
			}
			return displayName;
		}

		public void Initialize(IEventSource eventSource)
		{
			this.consoleLogger.Initialize(eventSource);
			this.eventSource = eventSource;
			this.eventSource.AnyEventRaised += new AnyEventHandler(this.LoggerEventHandler);
		}

		private object LogDeferredErrorOrWarning(object argsObject)
		{
			BuildEventArgs buildEventArg = argsObject as BuildEventArgs;
			if (buildEventArg != null)
			{
				string projectPathFromBuildEventContext = this.GetProjectPathFromBuildEventContext(buildEventArg.BuildEventContext);
				BuildErrorEventArgs buildErrorEventArg = buildEventArg as BuildErrorEventArgs;
				if (buildErrorEventArg == null)
				{
					BuildWarningEventArgs buildWarningEventArg = (BuildWarningEventArgs)buildEventArg;
					string empty = string.Empty;
					if (buildWarningEventArg.Code != null)
					{
						string code = buildWarningEventArg.Code;
					}
					string file = string.Empty;
					if (buildWarningEventArg.File != null)
					{
						file = buildWarningEventArg.File;
					}
					this.buildTarget.BuildErrors.Add(new BuildErrorTask(ErrorSeverity.Warning, buildWarningEventArg.Message, projectPathFromBuildEventContext, file, buildWarningEventArg.LineNumber, buildWarningEventArg.ColumnNumber, this.serviceProvider));
				}
				else
				{
					string str = string.Empty;
					if (buildErrorEventArg.Code != null)
					{
						string code1 = buildErrorEventArg.Code;
					}
					string empty1 = string.Empty;
					if (buildErrorEventArg.File != null)
					{
						empty1 = buildErrorEventArg.File;
					}
					this.buildTarget.BuildErrors.Add(new BuildErrorTask(ErrorSeverity.Error, buildErrorEventArg.Message, projectPathFromBuildEventContext, empty1, buildErrorEventArg.LineNumber, buildErrorEventArg.ColumnNumber, this.serviceProvider));
				}
			}
			return null;
		}

		private void LogDeferredMessages()
		{
			StringBuilder stringBuilder;
			lock (this)
			{
				stringBuilder = this.deferredMessages;
				this.deferredMessages = null;
			}
			string str = stringBuilder.ToString();
			this.messageLogger.Write(str);
		}

		internal static void LogFormattedError(IProjectBuildContext buildTarget, string message, string project, int line, int column, string file)
		{
			buildTarget.BuildErrors.Add(new HostLogger.ErrorTask(ErrorSeverity.Error, message, project, file, line, column));
		}

		internal static void LogFormattedWarning(IProjectBuildContext buildTarget, string message, string project, int line, int column, string file)
		{
			buildTarget.BuildErrors.Add(new HostLogger.ErrorTask(ErrorSeverity.Warning, message, project, file, line, column));
		}

		private void LoggerEventHandler(object sender, BuildEventArgs args)
		{
			if (args is ProjectStartedEventArgs || args is ProjectFinishedEventArgs)
			{
				UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.ApplicationIdle, () => this.LogProjectBuildEvent(args));
				return;
			}
			if ((args is BuildWarningEventArgs || args is BuildErrorEventArgs) && !string.IsNullOrEmpty(args.Message))
			{
				UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.ApplicationIdle, () => this.LogDeferredErrorOrWarning(args));
			}
		}

		private object LogProjectBuildEvent(object argsObject)
		{
			BuildEventArgs buildEventArg = argsObject as BuildEventArgs;
			if (buildEventArg != null)
			{
				ProjectStartedEventArgs projectStartedEventArg = buildEventArg as ProjectStartedEventArgs;
				if (projectStartedEventArg != null)
				{
					int projectContextId = projectStartedEventArg.BuildEventContext.ProjectContextId;
					if (!this.projectIdToProjectPathDictionary.ContainsKey(projectContextId))
					{
						this.projectIdToProjectPathDictionary[projectContextId] = projectStartedEventArg.ProjectFile;
					}
				}
				ProjectFinishedEventArgs projectFinishedEventArg = buildEventArg as ProjectFinishedEventArgs;
				if (projectFinishedEventArg != null)
				{
					int num = projectFinishedEventArg.BuildEventContext.ProjectContextId;
					if (this.projectIdToProjectPathDictionary.ContainsKey(num))
					{
						this.projectIdToProjectPathDictionary.Remove(num);
					}
				}
			}
			return null;
		}

		public void Shutdown()
		{
			this.consoleLogger.Shutdown();
			this.eventSource.AnyEventRaised -= new AnyEventHandler(this.LoggerEventHandler);
		}

		private void Writer(string message)
		{
			bool flag = false;
			lock (this)
			{
				if (this.deferredMessages == null)
				{
					this.deferredMessages = new StringBuilder();
					flag = true;
				}
				this.deferredMessages.Append(message);
			}
			if (flag)
			{
				UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.LogDeferredMessages));
			}
		}

		private class AlterableConsoleLogger : ConsoleLogger
		{
			public AlterableConsoleLogger(LoggerVerbosity verbosity) : base(verbosity)
			{
			}

			public void SetWriter(WriteHandler writer)
			{
				base.WriteHandler = writer;
			}
		}

		private sealed class ErrorTask : IErrorTask
		{
			private ErrorSeverity severity;

			private string description;

			private string project;

			private string file;

			private int line;

			private int column;

			public int? Column
			{
				get
				{
					return new int?(this.column);
				}
			}

			public string Description
			{
				get
				{
					return this.description;
				}
			}

			public string ExtendedDescription
			{
				get
				{
					return this.description;
				}
			}

			public string File
			{
				get
				{
					return this.file;
				}
			}

			public string FullFileName
			{
				get
				{
					return this.file;
				}
			}

			public ICommand InvokeCommand
			{
				get
				{
					return new DelegateCommand(() => {
					});
				}
			}

			public int? Line
			{
				get
				{
					return new int?(this.line);
				}
			}

			public string Project
			{
				get
				{
					return this.project;
				}
			}

			public ErrorSeverity Severity
			{
				get
				{
					return this.severity;
				}
			}

			public ErrorTask(ErrorSeverity severity, string description, string project, string file, int line, int column)
			{
				this.severity = severity;
				this.description = description;
				this.project = project;
				this.file = file;
				this.line = line;
				this.column = column;
			}
		}
	}
}