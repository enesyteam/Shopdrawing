using Microsoft.Expression.Extensibility.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	public abstract class ExecutableProjectBase : MSBuildBasedProject, IExecutable
	{
		private bool isExecuting;

		public bool CanExecute
		{
			get
			{
				return this.StartProgram != null;
			}
		}

		protected virtual bool IsControlLibrary
		{
			get
			{
				return false;
			}
		}

		public bool IsExecuting
		{
			get
			{
				return this.isExecuting;
			}
		}

		public virtual System.Diagnostics.ProcessStartInfo ProcessStartInfo
		{
			get
			{
				return null;
			}
		}

		public abstract string StartArguments
		{
			get;
		}

		public abstract string StartProgram
		{
			get;
		}

		public virtual string WorkingDirectory
		{
			get
			{
				string evaluatedPropertyValue = base.GetEvaluatedPropertyValue("WorkingDirectory");
				if (string.IsNullOrEmpty(evaluatedPropertyValue))
				{
					evaluatedPropertyValue = Path.GetDirectoryName(this.FullTargetPath);
				}
				if (!Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(evaluatedPropertyValue))
				{
					evaluatedPropertyValue = Environment.GetEnvironmentVariable("SystemRoot") ?? Environment.SystemDirectory;
				}
				return evaluatedPropertyValue;
			}
		}

		protected ExecutableProjectBase(Microsoft.Expression.Project.IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider) : base(projectStore, codeDocumentType, projectType, serviceProvider)
		{
		}

		public bool Execute()
		{
			bool flag;
			this.SetRunning(true);
			if (base.Services.ProjectAdapterService() != null)
			{
				IProjectExecutionAdapter projectExecutionAdapter = base.Services.ProjectAdapterService().FindAdapter<IProjectExecutionAdapter>(this);
				if (projectExecutionAdapter != null)
				{
					try
					{
						if (projectExecutionAdapter.Execute(new Microsoft.Expression.Extensibility.Project.Project(this), new ExecuteCompleteCallback(this.executeCompleteCallback)))
						{
							flag = true;
						}
						else
						{
							this.SetRunning(false);
							flag = false;
						}
					}
					catch (Exception exception)
					{
						this.SetRunning(false);
						flag = false;
					}
					return flag;
				}
			}
			Process process = null;
			string startProgram = this.StartProgram;
			System.Diagnostics.ProcessStartInfo processStartInfo = this.ProcessStartInfo;
			if (processStartInfo == null && !string.IsNullOrEmpty(startProgram))
			{
				processStartInfo = new System.Diagnostics.ProcessStartInfo(startProgram)
				{
					WorkingDirectory = this.WorkingDirectory
				};
				string startArguments = this.StartArguments;
				if (startArguments != null)
				{
					processStartInfo.Arguments = startArguments;
				}
				processStartInfo.UseShellExecute = false;
				processStartInfo.RedirectStandardError = true;
			}
			if (processStartInfo != null)
			{
				try
				{
					process = Process.Start(processStartInfo);
				}
				catch (Win32Exception win32Exception1)
				{
					Win32Exception win32Exception = win32Exception1;
					IMessageDisplayService messageDisplayService = base.Services.MessageDisplayService();
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string projectBuilderLaunchFailedDialogMessage = StringTable.ProjectBuilderLaunchFailedDialogMessage;
					object[] str = new object[] { startProgram, win32Exception.ToString() };
					messageDisplayService.ShowError(string.Format(currentCulture, projectBuilderLaunchFailedDialogMessage, str));
				}
			}
			if (process == null || process.HasExited)
			{
				this.SetRunning(false);
			}
			else
			{
				process.EnableRaisingEvents = true;
				process.Exited += new EventHandler(this.Process_Exited);
				process.ErrorDataReceived += new DataReceivedEventHandler(this.OnErrorDataReceivedFromProcess);
				process.BeginErrorReadLine();
			}
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectRun);
			return process != null;
		}

		private void executeCompleteCallback()
		{
			UIThreadDispatcher.Instance.Invoke(DispatcherPriority.Normal, () => this.SetRunning(false));
		}

		public override T GetCapability<T>(string name)
		{
			string str = name;
			if (str == null || !(str == "CanBeStartupProject"))
			{
				return base.GetCapability<T>(name);
			}
			if (this.IsControlLibrary)
			{
				return default(T);
			}
			return TypeHelper.ConvertType<T>(true);
		}

		private void OnErrorDataReceivedFromProcess(object sender, DataReceivedEventArgs e)
		{
			if (base.Services != null)
			{
				string data = e.Data;
				if (!string.IsNullOrEmpty(data))
				{
					base.Services.ExpressionInformationService().MainWindowRootElement.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback((object o) => {
						this.Services.MessageLoggingService().WriteLine(data);
						this.Services.WindowService().PaletteRegistry["Designer_ResultsPane"].IsVisible = true;
						return null;
					}), null);
				}
			}
		}

		private void Process_Exited(object sender, EventArgs args)
		{
			this.executeCompleteCallback();
			((Process)sender).CancelErrorRead();
		}

		private void SetRunning(bool value)
		{
			this.isExecuting = value;
		}
	}
}