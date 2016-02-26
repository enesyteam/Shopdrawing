using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.Project.Build
{
	public class BuildManager
	{
		private static bool notificationsDisabled;

		private static bool building;

		private static Thread currentBuildThread;

		private static Microsoft.Build.Evaluation.ProjectCollection projectCollection;

		private IServiceProvider serviceProvider;

		private static Version BuildEngineVersion;

		private static Queue<BuildManager.BuildRequest> asyncBuildRequestQueue;

		public static bool Building
		{
			get
			{
				return BuildManager.building;
			}
		}

		public static bool Finalizing
		{
			get
			{
				if (BuildManager.building)
				{
					return false;
				}
				return BuildManager.currentBuildThread != null;
			}
		}

		public static bool NotificationsDisabled
		{
			get
			{
				return BuildManager.notificationsDisabled;
			}
			set
			{
				BuildManager.notificationsDisabled = value;
			}
		}

		public static Microsoft.Build.Evaluation.ProjectCollection ProjectCollection
		{
			get
			{
				if (BuildManager.projectCollection == null)
				{
					BuildManager.projectCollection = BuildManager.CreateProjectCollection();
				}
				return BuildManager.projectCollection;
			}
		}

		static BuildManager()
		{
			BuildManager.BuildEngineVersion = ProjectAssemblyHelper.GetAssemblyName(typeof(Microsoft.Build.Evaluation.ProjectCollection).Assembly).Version;
			BuildManager.asyncBuildRequestQueue = new Queue<BuildManager.BuildRequest>();
		}

		internal BuildManager(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public void Build(IProjectBuildContext buildTarget, IExecutable executable, bool blockUntilComplete)
		{
			string[] strArrays = new string[] { "Build" };
			this.BuildCore(new BuildManager.BuildRequest(buildTarget, executable, strArrays, true), blockUntilComplete);
		}

		internal void Build(IProjectBuildContext buildTarget, IExecutable executable, bool blockUntilComplete, bool displayOutput)
		{
			string[] strArrays = new string[] { "Build" };
			this.BuildCore(new BuildManager.BuildRequest(buildTarget, executable, strArrays, displayOutput), blockUntilComplete);
		}

		private void BuildCore(BuildManager.BuildRequest request, bool blockUntilComplete)
		{
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectBuild);
			if (!blockUntilComplete)
			{
				BuildManager.asyncBuildRequestQueue.Enqueue(request);
				this.ProcessAsyncBuildRequest();
				return;
			}
			this.StartBuildCore(request);
			(new BuildManager.ProjectBuilder(this, request, null, false, true)).Build();
		}

		public static void CancelBuild()
		{
			if (BuildManager.currentBuildThread != null)
			{
				try
				{
					BuildManager.currentBuildThread.Abort();
					BuildManager.currentBuildThread.Join(2000);
					Thread.Sleep(50);
				}
				catch (SecurityException securityException)
				{
				}
				catch (ThreadStateException threadStateException)
				{
				}
				BuildManager.currentBuildThread = null;
				BuildManager.asyncBuildRequestQueue.Clear();
			}
		}

		internal void Clean(IProjectBuildContext buildTarget)
		{
			string[] strArrays = new string[] { "Clean" };
			this.BuildCore(new BuildManager.BuildRequest(buildTarget, null, strArrays, true), false);
		}

		internal static Microsoft.Build.Evaluation.ProjectCollection CreateProjectCollection()
		{
			Microsoft.Build.Evaluation.ProjectCollection projectCollection;
			using (BuildManager.EnvironmentCleaner environmentCleaner = new BuildManager.EnvironmentCleaner())
			{
				projectCollection = new Microsoft.Build.Evaluation.ProjectCollection();
			}
			return projectCollection;
		}

		internal static Microsoft.Build.Evaluation.Project GetProject(DocumentReference reference)
		{
			Microsoft.Build.Evaluation.Project current;
			IEnumerable<Microsoft.Build.Evaluation.Project> loadedProjects = BuildManager.ProjectCollection.GetLoadedProjects(reference.Path);
			if (loadedProjects != null)
			{
				using (IEnumerator<Microsoft.Build.Evaluation.Project> enumerator = loadedProjects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
					}
					else
					{
						return BuildManager.ProjectCollection.LoadProject(reference.Path);
					}
				}
				return current;
			}
			return BuildManager.ProjectCollection.LoadProject(reference.Path);
		}

		public static bool IsBuildEngineVersionAtLeast(int major, int minor)
		{
			if (BuildManager.BuildEngineVersion.Major >= major && (BuildManager.BuildEngineVersion.Major != major || BuildManager.BuildEngineVersion.Minor >= minor))
			{
				return true;
			}
			return false;
		}

		private void OnBuildCompleted(BuildCompletedEventArgs args, BuildManager.BuildRequest buildRequest)
		{
			if (!BuildManager.notificationsDisabled && this.BuildCompleted != null)
			{
				this.BuildCompleted(this, args);
			}
			if (BuildManager.asyncBuildRequestQueue.CountIsMoreThan<BuildManager.BuildRequest>(0) && BuildManager.asyncBuildRequestQueue.Peek() == buildRequest)
			{
				BuildManager.asyncBuildRequestQueue.Dequeue();
				this.ProcessAsyncBuildRequest();
			}
		}

		private void OnBuildStarting(BuildStartingEventArgs args)
		{
			if (!BuildManager.notificationsDisabled && this.BuildStarting != null)
			{
				this.BuildStarting(this, args);
			}
		}

		private void ProcessAsyncBuildRequest()
		{
			if (BuildManager.asyncBuildRequestQueue.CountIsLessThan<BuildManager.BuildRequest>(1) || BuildManager.building)
			{
				return;
			}
			BuildManager.BuildRequest buildRequest = BuildManager.asyncBuildRequestQueue.Peek();
			this.StartBuildCore(buildRequest);
			HostLogger hostLogger = null;
			if (buildRequest.DisplayOutput)
			{
				this.serviceProvider.MessageLoggingService().Clear();
				IMessageLoggingService messageLoggingService = this.serviceProvider.MessageLoggingService();
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string projectBuildStartedMessage = StringTable.ProjectBuildStartedMessage;
				object[] displayName = new object[] { buildRequest.BuildContext.DisplayName, string.Join("; ", buildRequest.Targets) };
				messageLoggingService.WriteLine(string.Format(currentCulture, projectBuildStartedMessage, displayName));
				hostLogger = new HostLogger(buildRequest.BuildContext, this.serviceProvider)
				{
					Verbosity = (Keyboard.IsKeyDown(Key.RightCtrl) ? LoggerVerbosity.Diagnostic : LoggerVerbosity.Minimal)
				};
			}
			BuildManager.ProjectBuilder projectBuilder = new BuildManager.ProjectBuilder(this, buildRequest, hostLogger, buildRequest.DisplayOutput, false);
			Thread thread = new Thread(new ThreadStart(projectBuilder.Build));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			BuildManager.currentBuildThread = thread;
			projectBuilder.PollBuildThread(thread);
		}

		internal void Rebuild(IProjectBuildContext buildTarget, IExecutable executable, bool blockUntilComplete)
		{
			string[] strArrays = new string[] { "Rebuild" };
			this.BuildCore(new BuildManager.BuildRequest(buildTarget, executable, strArrays, true), blockUntilComplete);
		}

		private void StartBuildCore(BuildManager.BuildRequest request)
		{
			BuildManager.building = true;
			this.OnBuildStarting(new BuildStartingEventArgs(request.BuildContext, request.DisplayOutput));
			request.BuildContext.BuildErrors.Clear();
		}

		public event EventHandler<BuildCompletedEventArgs> BuildCompleted;

		public event EventHandler<BuildStartingEventArgs> BuildStarting;

		private sealed class BuildInfo
		{
			internal BuildManager.BuildRequest BuildRequest
			{
				get;
				private set;
			}

			internal BuildResult BuildResult
			{
				get;
				private set;
			}

			internal Exception Exception
			{
				get;
				private set;
			}

			internal BuildInfo(BuildManager.BuildRequest buildRequest, BuildResult buildResult, Exception exception)
			{
				this.BuildRequest = buildRequest;
				this.BuildResult = buildResult;
				this.Exception = exception;
			}
		}

		private class BuildRequest
		{
			public IProjectBuildContext BuildContext
			{
				get;
				private set;
			}

			public bool DisplayOutput
			{
				get;
				private set;
			}

			public IExecutable Executable
			{
				get;
				private set;
			}

			public IEnumerable<string> Targets
			{
				get;
				private set;
			}

			public BuildRequest(IProjectBuildContext buildContext, IExecutable executable, IEnumerable<string> buildTargets, bool displayOutput)
			{
				this.BuildContext = buildContext;
				this.Executable = executable;
				this.Targets = buildTargets;
				this.DisplayOutput = displayOutput;
			}
		}

		private class EnvironmentCleaner : IDisposable
		{
			private readonly static string[] ConflictingVariables;

			private Dictionary<string, string> storedEnvironmentVariables;

			private bool IsDisposed
			{
				get
				{
					return this.storedEnvironmentVariables == null;
				}
			}

			static EnvironmentCleaner()
			{
				BuildManager.EnvironmentCleaner.ConflictingVariables = new string[] { "Platform", "PlatformName" };
			}

			public EnvironmentCleaner()
			{
				string[] conflictingVariables = BuildManager.EnvironmentCleaner.ConflictingVariables;
				for (int i = 0; i < (int)conflictingVariables.Length; i++)
				{
					string str = conflictingVariables[i];
					string environmentVariable = Environment.GetEnvironmentVariable(str);
					if (!string.IsNullOrEmpty(environmentVariable))
					{
						Environment.SetEnvironmentVariable(str, null);
						this.storedEnvironmentVariables.Add(str, environmentVariable);
					}
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (disposing && !this.IsDisposed)
				{
					foreach (KeyValuePair<string, string> storedEnvironmentVariable in this.storedEnvironmentVariables)
					{
						Environment.SetEnvironmentVariable(storedEnvironmentVariable.Key, storedEnvironmentVariable.Value);
					}
					this.storedEnvironmentVariables = null;
				}
			}
		}

		private sealed class ProjectBuilder
		{
			private BuildManager buildManager;

			private BuildManager.BuildRequest buildRequest;

			private ILogger logger;

			private IServiceProvider serviceProvider;

			private bool displayFeedback;

			private bool blockUntilComplete;

			private bool buildComplete;

			internal ProjectBuilder(BuildManager buildManager, BuildManager.BuildRequest request, ILogger logger, bool displayFeedback, bool blockUntilComplete)
			{
				this.buildManager = buildManager;
				this.buildRequest = request;
				this.logger = logger;
				this.displayFeedback = displayFeedback;
				this.blockUntilComplete = blockUntilComplete;
				this.serviceProvider = this.buildManager.serviceProvider;
			}

			internal void Build()
			{
				List<ILogger> loggers;
				BuildResult buildResult = BuildResult.Failed;
				Exception exception = null;
				try
				{
					try
					{
						if (this.logger != null)
						{
							loggers = new List<ILogger>()
							{
								this.logger
							};
						}
						else
						{
							loggers = null;
						}
						List<ILogger> loggers1 = loggers;
						buildResult = this.buildRequest.BuildContext.BuildWorker.Build(loggers1, this.buildRequest.Targets.ToArray<string>());
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						if (!ErrorHandling.ShouldHandleExceptions(exception1) && !(exception1 is IndexOutOfRangeException) && !(exception1 is NullReferenceException))
						{
							throw;
						}
						exception = exception1;
					}
				}
				finally
				{
					BuildManager.BuildInfo buildInfo = new BuildManager.BuildInfo(this.buildRequest, buildResult, exception);
					if (!this.blockUntilComplete)
					{
						UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.ApplicationIdle, () => this.CompleteBuild(buildInfo));
					}
					else
					{
						this.CompleteBuild(buildInfo);
					}
				}
			}

			private void BuildThreadExited(Thread thread)
			{
				if (!this.buildComplete)
				{
					this.CompleteBuild(new BuildManager.BuildInfo(null, BuildResult.Failed, null));
				}
			}

			private object CompleteBuild(object o)
			{
				IProjectBuildContext buildContext;
				IExecutable executable;
				try
				{
					this.buildComplete = true;
					BuildManager.building = false;
					BuildManager.BuildInfo buildInfo = (BuildManager.BuildInfo)o;
					if (buildInfo.BuildRequest != null)
					{
						buildContext = buildInfo.BuildRequest.BuildContext;
					}
					else
					{
						buildContext = null;
					}
					IProjectBuildContext projectBuildContext = buildContext;
					BuildResult buildResult = buildInfo.BuildResult;
					if (buildInfo.BuildRequest != null)
					{
						executable = buildInfo.BuildRequest.Executable;
					}
					else
					{
						executable = null;
					}
					IExecutable executable1 = executable;
					bool flag = (buildInfo.BuildRequest != null ? buildInfo.BuildRequest.DisplayOutput : true);
					if (projectBuildContext != null)
					{
						projectBuildContext.BuildCompleted(buildResult);
					}
					if (this.displayFeedback)
					{
						if (buildResult == BuildResult.Succeeded)
						{
							this.serviceProvider.MessageLoggingService().WriteLine(StringTable.ProjectBuildCompletedMessage);
						}
						else if (buildResult != BuildResult.Failed)
						{
							this.serviceProvider.MessageLoggingService().WriteLine(StringTable.ProjectBuildCanceledMessage);
						}
						else
						{
							if (buildInfo.Exception != null)
							{
								this.serviceProvider.MessageLoggingService().WriteLine(buildInfo.Exception.ToString());
							}
							this.serviceProvider.MessageLoggingService().WriteLine(StringTable.ProjectBuildFailedMessage);
						}
						if (buildResult != BuildResult.Succeeded)
						{
							this.serviceProvider.ErrorService().DisplayErrors();
						}
					}
					this.buildManager.OnBuildCompleted(new BuildCompletedEventArgs(projectBuildContext, executable1, buildResult, flag), buildInfo.BuildRequest);
				}
				finally
				{
					BuildManager.currentBuildThread = null;
					PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectBuild);
				}
				return null;
			}

			internal object PollBuildThread(object threadObject)
			{
				Thread thread = threadObject as Thread;
				if (thread != null && BuildManager.building)
				{
					if (!thread.IsAlive)
					{
						UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, () => this.BuildThreadExited(thread));
					}
					else
					{
						Thread.Sleep(100);
						UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.ApplicationIdle, () => this.PollBuildThread(thread));
					}
				}
				return null;
			}
		}
	}
}