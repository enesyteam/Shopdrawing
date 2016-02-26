using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;
using System.Windows.Forms;

namespace Microsoft.Expression.Project
{
	public static class ProjectPathHelper
	{
		private static char[] invalidProjectNameCharacters;

		static ProjectPathHelper()
		{
			ProjectPathHelper.invalidProjectNameCharacters = new char[] { '&', '#', '%', ';' };
		}

		public static bool AttemptToMakeWritable(DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			HandlerBasedProjectActionContext handlerBasedProjectActionContext = new HandlerBasedProjectActionContext(serviceProvider)
			{
				ExceptionHandler = (DocumentReference doc, Exception exception) => {
					if (!ErrorHandling.ShouldHandleExceptions(exception))
					{
						return false;
					}
					serviceProvider.MessageDisplayService().ShowError(string.Format(CultureInfo.CurrentCulture, StringTable.FileAccessErrorDialogMessage, new object[] { doc.DisplayNameShort, exception.Message }));
					return true;
				},
				CanOverwriteHandler = (DocumentReference doc) => ProjectPathHelper.PromptToOverwrite(doc, serviceProvider)
			};
			return ProjectPathHelper.AttemptToMakeWritable(documentReference, handlerBasedProjectActionContext);
		}

		public static bool AttemptToMakeWritable(DocumentReference documentReference, IProjectActionContext context)
		{
			bool flag;
			if (!documentReference.IsValidPathFormat || Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(documentReference.Path))
			{
				return false;
			}
			if (!Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(documentReference.Path))
			{
				return true;
			}
			if (context == null)
			{
				return false;
			}
			SourceControlHelper.UpdateSourceControl(EnumerableExtensions.AsEnumerable<DocumentReference>(documentReference), UpdateSourceControlActions.Checkout, context);
			try
			{
				FileAttributes attributes = File.GetAttributes(documentReference.Path);
				if ((attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
				{
					flag = true;
				}
				else if (!context.CanOverwrite(documentReference))
				{
					ProjectLog.LogError(documentReference.Path, StringTable.ActionCanceled, StringTable.MakeWritableAction, new object[0]);
					flag = false;
				}
				else if (!Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(documentReference.Path))
				{
					attributes = attributes & (FileAttributes.Hidden | FileAttributes.System | FileAttributes.Directory | FileAttributes.Archive | FileAttributes.Device | FileAttributes.Normal | FileAttributes.Temporary | FileAttributes.SparseFile | FileAttributes.ReparsePoint | FileAttributes.Compressed | FileAttributes.Offline | FileAttributes.NotContentIndexed | FileAttributes.Encrypted | FileAttributes.IntegrityStream | FileAttributes.NoScrubData);
					File.SetAttributes(documentReference.Path, attributes);
					ProjectLog.LogSuccess(documentReference.Path, StringTable.MakeWritableAction, new object[0]);
					flag = true;
				}
				else
				{
					ProjectLog.LogSuccess(documentReference.Path, StringTable.MakeWritableAction, new object[0]);
					flag = true;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				ProjectLog.LogError(documentReference.Path, exception, StringTable.MakeWritableAction, new object[0]);
				if (!context.HandleException(documentReference, exception))
				{
					throw;
				}
				return false;
			}
			return flag;
		}

		public static void CleanDirectory(string directoryName, bool deleteTopDirectoryOnError)
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryName))
			{
				return;
			}
			string[] files = Directory.GetFiles(directoryName);
			for (int i = 0; i < (int)files.Length; i++)
			{
				string str = files[i];
				try
				{
					File.SetAttributes(str, FileAttributes.Normal);
					File.Delete(str);
				}
				catch (IOException oException)
				{
					deleteTopDirectoryOnError = false;
					break;
				}
			}
			string[] directories = Directory.GetDirectories(directoryName);
			for (int j = 0; j < (int)directories.Length; j++)
			{
				ProjectPathHelper.CleanDirectory(directories[j], deleteTopDirectoryOnError);
			}
			if (deleteTopDirectoryOnError)
			{
				Directory.Delete(directoryName, true);
			}
		}

		public static int CompareForDisplay(string first, string second, CultureInfo cultureInfo)
		{
			int num = string.Compare(first, second, false, cultureInfo);
			if (num == 0)
			{
				num = string.CompareOrdinal(first, second);
			}
			return num;
		}

		public static IList<MoveResult> CopySafe(IEnumerable<Microsoft.Expression.Project.MoveInfo> copyRequests, Microsoft.Expression.Project.MoveOptions copyOptions, bool shouldThrowIOExceptions)
		{
			return ProjectPathHelper.MoveSafeInternal(copyRequests, copyOptions, false, shouldThrowIOExceptions);
		}

		public static void DeleteWithUndo(string filePath)
		{
			string str = Microsoft.Expression.Framework.Documents.PathHelper.TrimTrailingDirectorySeparators(filePath);
			bool flag = File.Exists(str);
			Microsoft.Expression.Project.NativeMethods.SHFILEOPSTRUCT32 mainWindowHandle = new Microsoft.Expression.Project.NativeMethods.SHFILEOPSTRUCT32()
			{
				fFlags = 13396,
				hNameMappings = IntPtr.Zero
			};
			try
			{
				mainWindowHandle.hwnd = Process.GetCurrentProcess().MainWindowHandle;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!(exception is SecurityException) && !(exception is InvalidOperationException) && !(exception is NotSupportedException))
				{
					throw;
				}
				mainWindowHandle.hwnd = IntPtr.Zero;
			}
			mainWindowHandle.lpszProgressTitle = string.Empty;
			mainWindowHandle.pFrom = string.Concat(str, "\0\0");
			mainWindowHandle.pTo = null;
			mainWindowHandle.wFunc = 3;
			mainWindowHandle.fAnyOperationsAborted = false;
			if (IntPtr.Size == 4)
			{
				Microsoft.Expression.Project.NativeMethods.SHFileOperation32(ref mainWindowHandle);
			}
			else if (IntPtr.Size == 8)
			{
				Microsoft.Expression.Project.NativeMethods.SHFILEOPSTRUCT64 sHFILEOPSTRUCT64 = new Microsoft.Expression.Project.NativeMethods.SHFILEOPSTRUCT64()
				{
					fFlags = mainWindowHandle.fFlags,
					hNameMappings = mainWindowHandle.hNameMappings,
					hwnd = mainWindowHandle.hwnd,
					lpszProgressTitle = mainWindowHandle.lpszProgressTitle,
					pFrom = mainWindowHandle.pFrom,
					pTo = mainWindowHandle.pTo,
					wFunc = mainWindowHandle.wFunc,
					fAnyOperationsAborted = mainWindowHandle.fAnyOperationsAborted
				};
				Microsoft.Expression.Project.NativeMethods.SHFileOperation64(ref sHFILEOPSTRUCT64);
			}
			if (flag)
			{
				Microsoft.Expression.Project.NativeMethods.SHChangeNotify(4, 5, str, IntPtr.Zero);
				return;
			}
			Microsoft.Expression.Project.NativeMethods.SHChangeNotify(16, 5, str, IntPtr.Zero);
		}

		public static string GetAvailableFilePath(string fileOrDirectoryName, string targetFolder, IProject project)
		{
			return ProjectPathHelper.GetAvailableFilePath(fileOrDirectoryName, targetFolder, project, false);
		}

		public static string GetAvailableFilePath(string fileOrDirectoryName, string targetFolder, IProject project, bool tryNoDigitFirst)
		{
			string str;
			string str1;
			string str2;
			ProjectPathHelper.GetFileNameAndExtension(fileOrDirectoryName, out str, out str1);
			int num = 1;
			string str3 = null;
			bool flag = tryNoDigitFirst;
			do
			{
				if (!flag)
				{
					str2 = string.Concat(str, num.ToString(CultureInfo.InvariantCulture), str1);
					num++;
				}
				else
				{
					str2 = string.Concat(str, str1);
					flag = false;
				}
				string str4 = Path.Combine(targetFolder, str2);
				if (!ProjectPathHelper.IsFilePathAvailable(project, str4))
				{
					continue;
				}
				str3 = str4;
			}
			while (str3 == null);
			return str3;
		}

		public static string GetFileExtension(string fileOrDirectoryName)
		{
			string str;
			string str1;
			ProjectPathHelper.GetFileNameAndExtension(fileOrDirectoryName, out str, out str1);
			return str1;
		}

		private static void GetFileNameAndExtension(string fileOrDirectoryName, out string fileName, out string extension)
		{
			fileName = fileOrDirectoryName;
			extension = string.Empty;
			if (Microsoft.Expression.Framework.Documents.PathHelper.PathEndsInDirectorySeparator(fileOrDirectoryName))
			{
				char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
				fileName = fileOrDirectoryName.TrimEnd(directorySeparatorChar);
				extension = Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
				return;
			}
			if (fileOrDirectoryName.Length > 1)
			{
				int num = fileOrDirectoryName.IndexOf('.', 1);
				if (num > 0)
				{
					fileName = fileOrDirectoryName.Substring(0, num);
					extension = fileOrDirectoryName.Substring(num);
				}
			}
		}

		public static string GetFileNameWithoutExtension(string fileOrDirectoryName)
		{
			string str;
			string str1;
			ProjectPathHelper.GetFileNameAndExtension(fileOrDirectoryName, out str, out str1);
			return str;
		}

		public static string GetFolderPath(string dialogDescription, string vistaDialogTitle, string initialDirectory)
		{
			string selectedPath;
			if (!ExpressionFileDialog.CanPickFolders)
			{
				using (ModalDialogHelper modalDialogHelper = new ModalDialogHelper())
				{
					FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
					{
						Description = dialogDescription,
						SelectedPath = initialDirectory
					};
					if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
					{
						return null;
					}
					else
					{
						selectedPath = folderBrowserDialog.SelectedPath;
					}
				}
				return selectedPath;
			}
			else
			{
				ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog()
				{
					Title = vistaDialogTitle,
					InitialDirectory = initialDirectory,
					PickFolders = true
				};
				bool? nullable = expressionOpenFileDialog.ShowDialog();
				if (nullable.HasValue && nullable.Value)
				{
					return expressionOpenFileDialog.FileName;
				}
			}
			return null;
		}

		private static bool HasContentAlreadyBeenMoved(IEnumerable<Microsoft.Expression.Project.MoveInfo> movedDirectories, string fileName)
		{
			bool flag;
			DocumentReference documentReference = DocumentReference.Create(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(fileName));
			using (IEnumerator<Microsoft.Expression.Project.MoveInfo> enumerator = movedDirectories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Microsoft.Expression.Project.MoveInfo current = enumerator.Current;
					DocumentReference documentReference1 = DocumentReference.Create(Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(current.Source));
					if (!documentReference.Path.StartsWith(documentReference1.Path, StringComparison.OrdinalIgnoreCase))
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

		public static bool IsFilePathAvailable(IProject project, string fullPath)
		{
			string fileOrDirectoryName = Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(fullPath);
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(fullPath))
			{
				return false;
			}
			if (project == null)
			{
				return true;
			}
			return project.Items.FindMatchByFileName<IProjectItem>(fileOrDirectoryName) == null;
		}

		private static bool IsIOException(Exception e)
		{
			if (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is ArgumentNullException)
			{
				return true;
			}
			return e is NotSupportedException;
		}

		public static bool IsValidNewSolutionPathFileName(string name)
		{
			bool flag = Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(name, false);
			if (flag && name != null && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(name))
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsValidPath(string path)
		{
			bool flag = Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(path);
			if (flag)
			{
				int num = path.IndexOf(':');
				if (num > 0 && !Microsoft.Expression.Framework.Documents.PathHelper.IsValidDrive(path[num - 1]))
				{
					flag = false;
				}
			}
			return flag;
		}

		public static bool IsValidProjectFileName(string name)
		{
			if (name.IndexOfAny(ProjectPathHelper.invalidProjectNameCharacters) != -1)
			{
				return false;
			}
			return Microsoft.Expression.Framework.Documents.PathHelper.IsValidFileOrDirectoryName(name);
		}

		private static IList<MoveResult> MoveDirectorySafe(Microsoft.Expression.Project.MoveInfo moveRequest, bool deleteSource, bool shouldOverwrite)
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(moveRequest.Source) || !Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(moveRequest.Destination))
			{
				throw new ArgumentException("Request must be a directory", "moveRequest");
			}
			Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(moveRequest.Source);
			string source = moveRequest.Source;
			string destination = moveRequest.Destination;
			IList<MoveResult> moveResults = new List<MoveResult>();
			int length = source.Length;
			string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
			for (int i = 0; i < (int)files.Length; i++)
			{
				string str = files[i];
				Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(str);
				string str1 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(destination, str.Substring(length));
				string directoryName = Path.GetDirectoryName(str1);
				if (!Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				bool flag = Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(str1);
				if (!flag || shouldOverwrite)
				{
					if (flag)
					{
						Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(str1);
						File.Delete(str1);
					}
					File.Move(str, str1);
					MoveResult moveResult = new MoveResult()
					{
						Source = str,
						Destination = str1,
						MovedSuccessfully = true
					};
					moveResults.Add(moveResult);
				}
			}
			IEnumerable<string> strs = Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories).AppendItem<string>(source);
			foreach (string str2 in strs)
			{
				string str3 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(destination, str2.Substring(length));
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(str3))
				{
					continue;
				}
				Directory.CreateDirectory(str3);
			}
			try
			{
				if (deleteSource)
				{
					Directory.Delete(source, true);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if ((exception is IOException || exception is UnauthorizedAccessException) && (new DirectoryInfo(source)).Exists && ((int)Directory.GetFiles(source, "*", SearchOption.AllDirectories).Length != 0 || (int)Directory.GetDirectories(source, "*", SearchOption.AllDirectories).Length != 0))
				{
					throw;
				}
			}
			MoveResult moveResult1 = new MoveResult()
			{
				Destination = moveRequest.Destination,
				Source = moveRequest.Source,
				MovedSuccessfully = true
			};
			moveResults.Add(moveResult1);
			return moveResults;
		}

		public static IList<MoveResult> MoveSafe(IEnumerable<Microsoft.Expression.Project.MoveInfo> moveRequests, Microsoft.Expression.Project.MoveOptions moveOptions, bool shouldThrowIOExceptions)
		{
			return ProjectPathHelper.MoveSafeInternal(moveRequests, moveOptions, true, shouldThrowIOExceptions);
		}

		private static IList<MoveResult> MoveSafeInternal(IEnumerable<Microsoft.Expression.Project.MoveInfo> moveRequests, Microsoft.Expression.Project.MoveOptions moveOptions, bool isMove, bool shouldThrowIOExceptions)
		{
			IList<MoveResult> moveResults;
			if (moveRequests == null)
			{
				throw new ArgumentNullException("moveRequests");
			}
			IList<MoveResult> moveResults1 = new List<MoveResult>();
			if (!moveRequests.Any<Microsoft.Expression.Project.MoveInfo>())
			{
				return moveResults1;
			}
			List<Microsoft.Expression.Project.MoveInfo> list = (
				from moveInfo in moveRequests
				where ProjectPathHelper.ValidateMoveInfo(moveInfo, moveOptions)
				select moveInfo).ToList<Microsoft.Expression.Project.MoveInfo>();
			List<Microsoft.Expression.Project.MoveInfo> moveInfos = new List<Microsoft.Expression.Project.MoveInfo>();
			List<Microsoft.Expression.Project.MoveInfo>.Enumerator enumerator = list.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Microsoft.Expression.Project.MoveInfo current = enumerator.Current;
					try
					{
						bool flag = (moveOptions & Microsoft.Expression.Project.MoveOptions.OverwriteDestination) == Microsoft.Expression.Project.MoveOptions.OverwriteDestination;
						bool flag1 = isMove;
						if (Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(current.Source))
						{
							if (ProjectPathHelper.HasContentAlreadyBeenMoved(moveInfos, current.Source))
							{
								MoveResult moveResult = new MoveResult()
								{
									Source = current.Source,
									Destination = current.Destination,
									MovedSuccessfully = true
								};
								moveResults1.Add(moveResult);
							}
							else
							{
								using (IEnumerator<MoveResult> enumerator1 = ProjectPathHelper.MoveDirectorySafe(current, flag1, flag).GetEnumerator())
								{
									while (enumerator1.MoveNext())
									{
										moveResults1.Add(enumerator1.Current);
									}
								}
							}
							moveInfos.Add(current);
						}
						else if (!ProjectPathHelper.HasContentAlreadyBeenMoved(moveInfos, current.Source))
						{
							Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(current.Source);
							string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(current.Destination);
							if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryNameOrRoot))
							{
								Directory.CreateDirectory(directoryNameOrRoot);
							}
							if (flag && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(current.Destination))
							{
								Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(current.Destination);
							}
							File.Copy(current.Source, current.Destination, flag);
							MoveResult moveResult1 = new MoveResult()
							{
								Source = current.Source,
								Destination = current.Destination,
								MovedSuccessfully = true
							};
							moveResults1.Add(moveResult1);
							if (flag1)
							{
								File.Delete(current.Source);
							}
						}
					}
					catch (Exception exception)
					{
						if (!ProjectPathHelper.IsIOException(exception) || shouldThrowIOExceptions)
						{
							throw;
						}
						else
						{
							moveResults = null;
							return moveResults;
						}
					}
				}
				return moveResults1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return moveResults;
		}

		public static bool PromptToOverwrite(DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			MessageBoxArgs messageBoxArg = new MessageBoxArgs();
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string overwriteReadOnlyFileDialogMessage = StringTable.OverwriteReadOnlyFileDialogMessage;
			object[] path = new object[] { documentReference.Path };
			messageBoxArg.Message = string.Format(currentCulture, overwriteReadOnlyFileDialogMessage, path);
			messageBoxArg.Button = MessageBoxButton.YesNo;
			messageBoxArg.Image = MessageBoxImage.Exclamation;
			messageBoxArg.AutomationId = "OverwriteReadOnlyProjectDialog";
			return serviceProvider.MessageDisplayService().ShowMessage(messageBoxArg) == MessageBoxResult.Yes;
		}

		private static bool ValidateMoveInfo(Microsoft.Expression.Project.MoveInfo moveInfo, Microsoft.Expression.Project.MoveOptions moveOptions)
		{
			if (string.IsNullOrEmpty(moveInfo.Source) || string.IsNullOrEmpty(moveInfo.Destination))
			{
				throw new ArgumentException("moveRequests");
			}
			if (!Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(moveInfo.Source))
			{
				return false;
			}
			moveInfo.Source = Microsoft.Expression.Framework.Documents.PathHelper.ResolvePath(moveInfo.Source);
			moveInfo.Destination = Microsoft.Expression.Framework.Documents.PathHelper.ResolvePath(moveInfo.Destination);
			if (moveInfo.Source.Equals(moveInfo.Destination, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(moveInfo.Destination))
			{
				if ((moveOptions & Microsoft.Expression.Project.MoveOptions.OverwriteDestination) != Microsoft.Expression.Project.MoveOptions.OverwriteDestination)
				{
					throw new InvalidOperationException("Destination files exist");
				}
				Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(moveInfo.Destination);
			}
			return true;
		}

		public static string ValidateNewSolutionPathFileName(string name)
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(name, false))
			{
				return null;
			}
			if (name == null || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(name))
			{
				return null;
			}
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string projectDialogSolutionFileAlreadyExists = StringTable.ProjectDialogSolutionFileAlreadyExists;
			object[] objArray = new object[] { name };
			return string.Format(currentCulture, projectDialogSolutionFileAlreadyExists, objArray);
		}

		public static string ValidatePath(string path)
		{
			string projectDialogInvalidDrive = Microsoft.Expression.Framework.Documents.PathHelper.ValidatePath(path);
			if (projectDialogInvalidDrive == null)
			{
				int num = path.IndexOf(':');
				if (num > 0 && !Microsoft.Expression.Framework.Documents.PathHelper.IsValidDrive(path[num - 1]))
				{
					projectDialogInvalidDrive = StringTable.ProjectDialogInvalidDrive;
				}
			}
			return projectDialogInvalidDrive;
		}

		public static string ValidateProjectFileName(string name)
		{
			if (name.IndexOfAny(ProjectPathHelper.invalidProjectNameCharacters) != -1)
			{
				return StringTable.InvalidNameProjectItemCharacter;
			}
			return Microsoft.Expression.Framework.Documents.PathHelper.ValidateFileOrDirectoryName(name);
		}

		public sealed class TemporaryDirectory : IDisposable
		{
			public string Path
			{
				get;
				private set;
			}

			public TemporaryDirectory() : this(false)
			{
			}

			public TemporaryDirectory(bool throwIOError)
			{
				this.InternalInit(throwIOError, false);
			}

			public TemporaryDirectory(bool throwIOError, bool noTrailing8Dot3FolderStructure)
			{
				this.InternalInit(throwIOError, noTrailing8Dot3FolderStructure);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				try
				{
					Directory.Delete(this.Path, true);
				}
				catch (IOException oException)
				{
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
				}
			}

			~TemporaryDirectory()
			{
				this.Dispose(false);
			}

			public string GenerateTemporaryFileName()
			{
				return Path.Combine(this.Path, Path.GetRandomFileName());
			}

			private void InternalInit(bool throwIOError, bool noTrailing8Dot3FolderStructure)
			{
				string randomFileName = Path.GetRandomFileName();
				if (noTrailing8Dot3FolderStructure)
				{
					randomFileName = randomFileName.Replace(".", "_");
				}
				this.Path = string.Concat(Path.Combine(Path.GetTempPath(), randomFileName), Path.DirectorySeparatorChar);
				try
				{
					Directory.CreateDirectory(this.Path);
				}
				catch (IOException oException)
				{
					if (throwIOError)
					{
						throw;
					}
					this.Path = string.Empty;
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
					if (throwIOError)
					{
						throw;
					}
					this.Path = string.Empty;
				}
			}
		}
	}
}