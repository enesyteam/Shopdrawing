using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal sealed class ProjectUpgradeLogger : IProjectLogger, IDisposable
	{
		private bool isEnabled;

		private Dictionary<string, List<ProjectUpgradeLogger.LogItem>> log;

		public bool HasErrors
		{
			get
			{
				bool flag;
				Dictionary<string, List<ProjectUpgradeLogger.LogItem>>.ValueCollection.Enumerator enumerator = this.log.Values.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						List<ProjectUpgradeLogger.LogItem>.Enumerator enumerator1 = enumerator.Current.GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								if (enumerator1.Current.IsSuccess)
								{
									continue;
								}
								flag = true;
								return flag;
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.log.Count == 0;
			}
		}

		public ProjectUpgradeLogger()
		{
			this.isEnabled = true;
			this.log = new Dictionary<string, List<ProjectUpgradeLogger.LogItem>>(StringComparer.OrdinalIgnoreCase);
			ProjectLog.Register(this);
		}

		private void Add(string path, ProjectUpgradeLogger.LogItem newItem)
		{
			List<ProjectUpgradeLogger.LogItem> orCreateFileItems = this.GetOrCreateFileItems(path);
			orCreateFileItems.RemoveAll((ProjectUpgradeLogger.LogItem item) => ProjectUpgradeLogger.LogItem.Equals(item, newItem));
			orCreateFileItems.Add(newItem);
		}

		public void Dispose()
		{
			ProjectLog.Unregister(this);
			GC.SuppressFinalize(this);
		}

		private List<ProjectUpgradeLogger.LogItem> GetOrCreateFileItems(string path)
		{
			List<ProjectUpgradeLogger.LogItem> logItems;
			if (!this.log.TryGetValue(path, out logItems))
			{
				logItems = new List<ProjectUpgradeLogger.LogItem>();
				this.log.Add(path, logItems);
			}
			return logItems;
		}

		public void LogError(string path, Exception exception, string messageTemplate, params object[] args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (exception == null)
			{
				return;
			}
			Exception innerException = exception;
			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
			}
			string message = innerException.Message;
			if (string.IsNullOrEmpty(message))
			{
				message = innerException.ToString();
				if (string.IsNullOrEmpty(message))
				{
					message = innerException.GetType().FullName;
				}
			}
			this.LogError(path, message, messageTemplate, args);
		}

		public void LogError(string path, string errorMessage, string messageTemplate, params object[] args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			string str = string.Format(CultureInfo.CurrentCulture, messageTemplate, args);
			str = ProjectUpgradeLogger.NormalizeString(str);
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			errorMessage = ProjectUpgradeLogger.NormalizeString(errorMessage);
			if (string.IsNullOrEmpty(errorMessage))
			{
				return;
			}
			this.Add(path, new ProjectUpgradeLogger.LogItem(str, errorMessage));
		}

		public void LogSuccess(string path, string messageTemplate, params object[] args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			string str = string.Format(CultureInfo.CurrentCulture, messageTemplate, args);
			str = ProjectUpgradeLogger.NormalizeString(str);
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			this.Add(path, new ProjectUpgradeLogger.LogItem(str, null));
		}

		private static string NormalizeString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			string str1 = str.Trim();
			str1 = str1.Replace('\t', ' ');
			str1 = str1.Replace('\r', ' ');
			str1 = str1.Replace('\n', ' ');
			while (str1.IndexOf("  ", StringComparison.Ordinal) > 0)
			{
				str1 = str1.Replace("  ", "");
			}
			return str1;
		}

		public void Save(TextWriter writer)
		{
			foreach (KeyValuePair<string, List<ProjectUpgradeLogger.LogItem>> keyValuePair in this.log)
			{
				string str = new string('-', keyValuePair.Key.Length);
				writer.WriteLine(str);
				writer.WriteLine(keyValuePair.Key);
				writer.WriteLine(str);
				List<ProjectUpgradeLogger.LogItem> value = keyValuePair.Value;
				for (int i = 0; i < value.Count; i++)
				{
					ProjectUpgradeLogger.LogItem item = value[i];
					if (!item.IsSuccess)
					{
						writer.WriteLine(StringTable.ConversionErrorEntry, item.Message, item.Error);
					}
					else
					{
						writer.WriteLine(item.Message);
					}
				}
				writer.WriteLine();
				writer.WriteLine();
			}
		}

		public IDisposable SuspendLogging()
		{
			return new ProjectUpgradeLogger.SuspendLogToken(this);
		}

		[DebuggerDisplay("{Message}")]
		private class LogItem
		{
			public string Error
			{
				get;
				private set;
			}

			public bool IsSuccess
			{
				get
				{
					return string.IsNullOrEmpty(this.Error);
				}
			}

			public string Message
			{
				get;
				private set;
			}

			public LogItem(string message, string errorMessage)
			{
				this.Message = message;
				this.Error = errorMessage;
			}

			public static bool Equals(ProjectUpgradeLogger.LogItem left, ProjectUpgradeLogger.LogItem right)
			{
				if (left == right)
				{
					return true;
				}
				if (left == null || right == null)
				{
					return false;
				}
				if (string.Equals(left.Message, right.Message) && string.Equals(left.Error, right.Error))
				{
					return true;
				}
				return false;
			}
		}

		private class SuspendLogToken : IDisposable
		{
			private ProjectUpgradeLogger logger;

			private bool isEnabled;

			public SuspendLogToken(ProjectUpgradeLogger logger)
			{
				this.logger = logger;
				this.isEnabled = this.logger.isEnabled;
				this.logger.isEnabled = false;
			}

			public void Dispose()
			{
				if (this.logger != null)
				{
					this.logger.isEnabled = this.isEnabled;
					this.logger = null;
				}
				GC.SuppressFinalize(this);
			}
		}
	}
}