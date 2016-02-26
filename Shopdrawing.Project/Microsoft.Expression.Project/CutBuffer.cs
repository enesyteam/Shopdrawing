using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Clipboard;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	public class CutBuffer
	{
		private DataObject lastCut;

		private IServiceProvider serviceProvider;

		private bool invokeIdle = true;

		private DispatcherTimer cutPasteUpdateTimer;

		public bool CanCopy
		{
			get
			{
				return this.CanCutOrCopy(false);
			}
		}

		public bool CanCut
		{
			get
			{
				return this.CanCutOrCopy(true);
			}
		}

		public bool CanPaste
		{
			get
			{
				IProject project = this.Services.ProjectManager().ItemSelectionSet.SelectedProjects.SingleOrNull<IProject>();
				if (project == null)
				{
					return false;
				}
				bool flag = false;
				string str = this.Services.ProjectManager().TargetFolderForProject(project);
				if (str != null)
				{
					bool flag1 = false;
					IProjectItem parent = project.FindItem(DocumentReference.Create(str));
					while (parent != null)
					{
						if (!parent.IsCut)
						{
							parent = parent.Parent;
						}
						else
						{
							flag1 = true;
							break;
						}
					}
					if (!flag1)
					{
						SafeDataObject safeDataObject = SafeDataObject.FromClipboard();
						if (safeDataObject != null)
						{
							if (safeDataObject.GetDataPresent(typeof(CutBuffer.ProjectCopyInformation)))
							{
								CutBuffer.ProjectCopyInformation data = safeDataObject.GetData(typeof(CutBuffer.ProjectCopyInformation)) as CutBuffer.ProjectCopyInformation;
								ISolution currentSolution = this.Services.ProjectManager().CurrentSolution;
								if (data == null || currentSolution == null)
								{
									flag = false;
								}
								else if (!currentSolution.IsSourceControlActive || !data.IsCut)
								{
									flag = true;
								}
								else
								{
									List<DocumentReference> documentReferences = new List<DocumentReference>();
									foreach (CutBuffer.ItemCopyInformation item in data.Items)
									{
										if (!string.IsNullOrEmpty(item.ItemUrl))
										{
											documentReferences.Add(DocumentReference.Create(item.ItemUrl));
										}
										if (string.IsNullOrEmpty(item.CodeBehindItemUrl))
										{
											continue;
										}
										documentReferences.Add(DocumentReference.Create(item.CodeBehindItemUrl));
									}
									flag = !documentReferences.Any<DocumentReference>((DocumentReference reference) => {
										SourceControlStatus cachedStatus = SourceControlStatusCache.GetCachedStatus(reference);
										switch (cachedStatus)
										{
											case SourceControlStatus.None:
											case SourceControlStatus.Add:
											{
												return false;
											}
											default:
											{
												if (cachedStatus == SourceControlStatus.CheckedIn)
												{
													return false;
												}
												return true;
											}
										}
									});
								}
							}
							else if (safeDataObject.GetDataPresent(DataFormats.FileDrop))
							{
								flag = true;
							}
							else if (safeDataObject.GetDataPresent(typeof(string[])))
							{
								flag = true;
								string[] strArrays = (string[])safeDataObject.GetData(typeof(string[]));
								for (int i = 0; i < (int)strArrays.Length; i++)
								{
									flag = this.DoesFileExist(strArrays[i]);
									if (!flag)
									{
										return flag;
									}
								}
							}
							else if (safeDataObject.GetDataPresent(typeof(string)))
							{
								string data1 = safeDataObject.GetData(typeof(string)) as string;
								flag = (data1 == null ? false : this.DoesFileExist(data1));
							}
							else if (safeDataObject.GetDataPresent(DataFormats.Bitmap))
							{
								flag = true;
							}
						}
					}
				}
				return flag;
			}
		}

		private DataObject LastCut
		{
			get
			{
				return this.lastCut;
			}
			set
			{
				this.lastCut = value;
			}
		}

		private IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public CutBuffer(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.cutPasteUpdateTimer = new DispatcherTimer()
			{
				Interval = new TimeSpan(0, 0, 5)
			};
			this.cutPasteUpdateTimer.Tick += new EventHandler(this.Timer_Tick);
			this.cutPasteUpdateTimer.Start();
		}

		public static IProjectItem AddImageDataFromClipboard(IProjectManager projectManager, IProject project)
		{
			InteropBitmap data;
			IProjectItem projectItem;
			try
			{
				data = (InteropBitmap)Clipboard.GetData(DataFormats.Bitmap);
				goto Label0;
			}
			catch (OutOfMemoryException outOfMemoryException)
			{
				LowMemoryMessage.Show();
				projectItem = null;
			}
			return projectItem;
		Label0:
			if (data == null)
			{
				return null;
			}
			FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap(data, PixelFormats.Bgr32, null, 0);
			string str = projectManager.TargetFolderForProject(project);
			string availableFilePath = ProjectPathHelper.GetAvailableFilePath("Image.png", str, null);
			using (FileStream fileStream = new FileStream(availableFilePath, FileMode.Create, FileAccess.Write))
			{
				PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
				pngBitmapEncoder.Frames.Add(BitmapFrame.Create(formatConvertedBitmap));
				pngBitmapEncoder.Save(fileStream);
				fileStream.Close();
			}
			DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
			{
				SourcePath = availableFilePath,
				TargetPath = availableFilePath
			};
			return project.AddItem(documentCreationInfo);
		}

		private static void AddToCutList(IProjectItem item, List<CutBuffer.ItemCopyInformation> cutList, bool isCut)
		{
			bool flag = false;
			int count = cutList.Count - 1;
			while (count >= 0)
			{
				string itemUrl = cutList[count].ItemUrl;
				if (itemUrl == item.DocumentReference.Path)
				{
					flag = true;
					break;
				}
				else if (!itemUrl.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase) || !item.DocumentReference.Path.StartsWith(itemUrl, StringComparison.OrdinalIgnoreCase))
				{
					if (item.DocumentReference.Path.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase) && itemUrl.StartsWith(item.DocumentReference.Path, StringComparison.OrdinalIgnoreCase))
					{
						cutList.RemoveAt(count);
					}
					count--;
				}
				else
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (!item.DocumentReference.Path.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
				{
					foreach (IProjectItem child in item.Children)
					{
						CutBuffer.AddToCutList(child, cutList, isCut);
					}
				}
				string path = null;
				IProjectItem codeBehindItem = item.CodeBehindItem;
				if (codeBehindItem != null)
				{
					path = codeBehindItem.DocumentReference.Path;
				}
				cutList.Add(new CutBuffer.ItemCopyInformation(item.DocumentReference.Path, path));
				item.IsCut = isCut;
			}
		}

		private void AppendAllSubfolders(string copiedFolder, string folderToBeWritten, List<string> folderList)
		{
			string[] directories = Directory.GetDirectories(copiedFolder);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				string str = directories[i];
				string str1 = string.Concat(folderToBeWritten, Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(str), Path.DirectorySeparatorChar);
				folderList.Add(str1);
				this.AppendAllSubfolders(str, str1, folderList);
			}
		}

		private void Application_Idle()
		{
			if (this.LastCut != null && !ClipboardService.IsCurrent(this.LastCut))
			{
				this.UpdateLastCut(null);
			}
			this.invokeIdle = true;
		}

		private bool CanCutOrCopy(bool isCut)
		{
			if (this.Services.ProjectManager().ItemSelectionSet.SelectedProjects.SingleOrNull<IProject>() == null)
			{
				return false;
			}
			bool flag = false;
			if (!this.Services.ProjectManager().ItemSelectionSet.IsEmpty)
			{
				IPrototypingProjectService prototypingProjectService = this.serviceProvider.PrototypingProjectService();
				bool flag1 = true;
				bool flag2 = false;
				foreach (IDocumentItem selection in this.Services.ProjectManager().ItemSelectionSet.Selection)
				{
					IProjectItem projectItem = selection as IProjectItem;
					if (projectItem == null || isCut && projectItem.IsLinkedFile || projectItem.IsVirtual || projectItem is AssemblyReferenceProjectItem || prototypingProjectService != null && !prototypingProjectService.CanCutOrCopy(projectItem, isCut))
					{
						flag2 = true;
						break;
					}
					else
					{
						if (!flag1 || !projectItem.FileExists)
						{
							continue;
						}
						flag1 = false;
					}
				}
				flag = (flag1 ? false : !flag2);
			}
			return flag;
		}

		public void Copy()
		{
			this.CutCopy(false);
		}

		private List<DocumentCreationInfo> CopyFolderFiles(string copiedFolder, string folderToBeWritten)
		{
			List<DocumentCreationInfo> documentCreationInfos = new List<DocumentCreationInfo>();
			string[] files = Directory.GetFiles(copiedFolder);
			for (int i = 0; i < (int)files.Length; i++)
			{
				string str = files[i];
				string str1 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(folderToBeWritten, Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(str));
				DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
				{
					SourcePath = str,
					TargetPath = str1
				};
				documentCreationInfos.Add(documentCreationInfo);
			}
			return documentCreationInfos;
		}

		private bool CopyItems(IProject activeProject, IEnumerable<CutBuffer.CopyInformation> itemsToCopy)
		{
			List<DocumentCreationInfo> documentCreationInfos = new List<DocumentCreationInfo>();
			foreach (CutBuffer.CopyInformation copyInformation in itemsToCopy)
			{
				if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(copyInformation.SourcePath))
				{
					if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(copyInformation.SourcePath))
					{
						continue;
					}
					List<string> strs = new List<string>()
					{
						""
					};
					this.AppendAllSubfolders(copyInformation.SourcePath, "", strs);
					foreach (string str in strs)
					{
						string str1 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(copyInformation.SourcePath, str);
						string str2 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(copyInformation.DestinationPath, str);
						DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
						{
							DocumentType = this.Services.DocumentTypeManager().DocumentTypes[DocumentTypeNamesHelper.Folder],
							SourcePath = str1,
							TargetPath = str2
						};
						documentCreationInfos.Add(documentCreationInfo);
						documentCreationInfos.AddRange(this.CopyFolderFiles(str1, str2));
					}
				}
				else
				{
					DocumentCreationInfo documentCreationInfo1 = new DocumentCreationInfo()
					{
						SourcePath = copyInformation.SourcePath,
						TargetPath = copyInformation.DestinationPath
					};
					documentCreationInfos.Add(documentCreationInfo1);
				}
			}
			IEnumerable<IProjectItem> projectItems = activeProject.AddItems(documentCreationInfos);
			if (itemsToCopy.CountIsLessThan<CutBuffer.CopyInformation>(1))
			{
				return true;
			}
			if (projectItems == null)
			{
				return false;
			}
			return projectItems.CountIsMoreThan<IProjectItem>(0);
		}

		public void Cut()
		{
			this.CutCopy(true);
		}

		private void CutCopy(bool isCut)
		{
			if (this.Services.ProjectManager().ItemSelectionSet.IsEmpty)
			{
				return;
			}
			IProject project = this.Services.ProjectManager().ItemSelectionSet.SelectedProjects.SingleOrNull<IProject>();
			if (project == null)
			{
				return;
			}
			this.UpdateLastCut(null);
			List<CutBuffer.ItemCopyInformation> itemCopyInformations = new List<CutBuffer.ItemCopyInformation>();
			foreach (IProjectItem projectItem in this.Services.ProjectManager().ItemSelectionSet.Selection.OfType<IProjectItem>())
			{
				if (isCut && projectItem.IsLinkedFile || !projectItem.FileExists || projectItem.IsVirtual || projectItem is AssemblyReferenceProjectItem)
				{
					continue;
				}
				CutBuffer.AddToCutList(projectItem, itemCopyInformations, isCut);
			}
			string[] itemUrl = new string[itemCopyInformations.Count];
			for (int i = 0; i < itemCopyInformations.Count; i++)
			{
				itemUrl[i] = itemCopyInformations[i].ItemUrl;
			}
			DataObject dataObject = new DataObject();
			dataObject.SetData(typeof(string[]), itemUrl);
			dataObject.SetData(DataFormats.FileDrop, itemUrl);
			dataObject.SetData(new CutBuffer.ProjectCopyInformation(isCut, project.DocumentReference.Path, itemCopyInformations));
			ClipboardService.SetDataObject(dataObject);
			this.LastCut = dataObject;
		}

		private string DetermineFileUrlForCopy(string itemToCopy, string targetFolder)
		{
			string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(targetFolder, Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(itemToCopy));
			if (Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(str, itemToCopy))
			{
				str = this.FindNewName(str);
			}
			return str;
		}

		private bool DoesFileExist(string path)
		{
			bool flag = false;
			if (this.IsPathValid(path))
			{
				try
				{
					flag = (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path) ? true : Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path));
				}
				catch (ArgumentException argumentException)
				{
					flag = false;
				}
			}
			return flag;
		}

		private string FindNewName(string fileUrlToBeWritten)
		{
			string str;
			string parentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(fileUrlToBeWritten);
			string fileOrDirectoryName = Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(fileUrlToBeWritten);
			FileInfo fileInfo = new FileInfo(fileUrlToBeWritten);
			DirectoryInfo directoryInfo = new DirectoryInfo(fileUrlToBeWritten);
			int num = 1;
			while (fileInfo.Exists || directoryInfo.Exists)
			{
				if (num != 1)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string pasteSubsequentCopyFormat = StringTable.PasteSubsequentCopyFormat;
					object[] objArray = new object[] { fileOrDirectoryName, num };
					str = string.Format(currentCulture, pasteSubsequentCopyFormat, objArray);
				}
				else
				{
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string pasteFirstCopyFormat = StringTable.PasteFirstCopyFormat;
					object[] objArray1 = new object[] { fileOrDirectoryName };
					str = string.Format(cultureInfo, pasteFirstCopyFormat, objArray1);
				}
				fileUrlToBeWritten = Path.Combine(parentDirectory, str);
				fileInfo = new FileInfo(fileUrlToBeWritten);
				directoryInfo = new DirectoryInfo(fileUrlToBeWritten);
				num++;
			}
			return fileUrlToBeWritten;
		}

		private CutBuffer.ProjectCopyInformation GetCopyInformation()
		{
			CutBuffer.ProjectCopyInformation projectCopyInformation = null;
			SafeDataObject safeDataObject = SafeDataObject.FromClipboard();
			if (safeDataObject != null)
			{
				if (safeDataObject.GetDataPresent(typeof(CutBuffer.ProjectCopyInformation)))
				{
					CutBuffer.ProjectCopyInformation data = safeDataObject.GetData(typeof(CutBuffer.ProjectCopyInformation)) as CutBuffer.ProjectCopyInformation;
					if (data != null)
					{
						projectCopyInformation = new CutBuffer.ProjectCopyInformation(data);
					}
				}
				if (projectCopyInformation == null && safeDataObject.GetDataPresent(DataFormats.FileDrop))
				{
					string[] strArrays = safeDataObject.GetData(DataFormats.FileDrop) as string[];
					if (strArrays != null)
					{
						projectCopyInformation = new CutBuffer.ProjectCopyInformation(false, null, strArrays);
					}
				}
				if (projectCopyInformation == null && safeDataObject.GetDataPresent(typeof(string[])))
				{
					string[] data1 = safeDataObject.GetData(typeof(string[])) as string[];
					if (data1 != null)
					{
						projectCopyInformation = new CutBuffer.ProjectCopyInformation(false, null, data1);
					}
				}
				if (projectCopyInformation == null && safeDataObject.GetDataPresent(typeof(string)))
				{
					string str = safeDataObject.GetData(typeof(string)) as string;
					if (str != null)
					{
						string[] strArrays1 = new string[] { str };
						projectCopyInformation = new CutBuffer.ProjectCopyInformation(false, null, strArrays1);
					}
				}
			}
			return projectCopyInformation;
		}

		private bool IsPathValid(string path)
		{
			if (Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(path))
			{
				return false;
			}
			if (path.Length > 0 && path.IndexOfAny(Path.GetInvalidPathChars()) < 0)
			{
				string[] strArrays = path.Split(new char[] { ':' });
				if (((int)strArrays.Length == 1 || (int)strArrays.Length == 2 && strArrays[0].Length == 1) && Path.GetFileName(path).IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
				{
					return true;
				}
			}
			return false;
		}

		public void Paste()
		{
			IProject project = this.Services.ProjectManager().ItemSelectionSet.SelectedProjects.SingleOrNull<IProject>();
			if (project == null)
			{
				return;
			}
			CutBuffer.ProjectCopyInformation copyInformation = this.GetCopyInformation();
			if (copyInformation == null)
			{
				CutBuffer.AddImageDataFromClipboard(this.Services.ProjectManager(), project);
			}
			else
			{
				IProject project1 = null;
				if (copyInformation.IsCut)
				{
					project1 = this.Services.ProjectManager().CurrentSolution.Projects.FindMatchByUrl<IProject>(copyInformation.ProjectUrl);
				}
				if (copyInformation.Items != null && copyInformation.Items.Count > 0)
				{
					string str = this.Services.ProjectManager().TargetFolderForProject(project);
					if (str != null)
					{
						IEnumerable<CutBuffer.CopyInformation> copyInformations = copyInformation.Items.Where<CutBuffer.ItemCopyInformation>((CutBuffer.ItemCopyInformation itemToCopy) => {
							if (project1 == null)
							{
								return true;
							}
							return !Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(itemToCopy.ItemUrl), str);
						}).Select<CutBuffer.ItemCopyInformation, CutBuffer.CopyInformation>((CutBuffer.ItemCopyInformation itemToCopy) => new CutBuffer.CopyInformation(itemToCopy.ItemUrl, this.DetermineFileUrlForCopy(itemToCopy.ItemUrl, str)));
						if (!this.CopyItems(project, copyInformations))
						{
							return;
						}
					}
					if (project1 != null)
					{
						foreach (CutBuffer.ItemCopyInformation item in copyInformation.Items)
						{
							IProjectItem projectItem = project1.FindItem(DocumentReference.Create(item.ItemUrl));
							if (projectItem == null)
							{
								continue;
							}
							if (Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(item.ItemUrl), str))
							{
								projectItem.IsCut = false;
							}
							else
							{
								IProjectItem codeBehindItem = projectItem.CodeBehindItem;
								if (codeBehindItem != null)
								{
									IProject project2 = project1;
									IProjectItem[] projectItemArray = new IProjectItem[] { codeBehindItem };
									project2.RemoveItems(true, projectItemArray);
								}
								IProject project3 = project1;
								IProjectItem[] projectItemArray1 = new IProjectItem[] { projectItem };
								project3.RemoveItems(true, projectItemArray1);
							}
						}
						ClipboardService.SetDataObject(new DataObject());
						this.LastCut = null;
						return;
					}
				}
			}
		}

		private void Timer_Tick(object sender, EventArgs args)
		{
			if (this.invokeIdle)
			{
				this.invokeIdle = false;
				UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.Application_Idle));
			}
		}

		private void UpdateLastCut(DataObject thisCut)
		{
			if (this.lastCut != null && thisCut != this.lastCut && this.lastCut.GetDataPresent(typeof(CutBuffer.ProjectCopyInformation)))
			{
				CutBuffer.ProjectCopyInformation data = (CutBuffer.ProjectCopyInformation)this.lastCut.GetData(typeof(CutBuffer.ProjectCopyInformation));
				if (data.IsCut)
				{
					IProject project = this.Services.ProjectManager().CurrentSolution.Projects.FindMatchByUrl<IProject>(data.ProjectUrl);
					if (project != null)
					{
						foreach (CutBuffer.ItemCopyInformation item in data.Items)
						{
							IProjectItem projectItem = project.FindItem(DocumentReference.Create(item.ItemUrl));
							if (projectItem == null)
							{
								continue;
							}
							projectItem.IsCut = false;
						}
					}
				}
			}
			this.lastCut = thisCut;
		}

		private class CopyInformation
		{
			public string DestinationPath
			{
				get;
				private set;
			}

			public string SourcePath
			{
				get;
				private set;
			}

			public CopyInformation(string sourcePath, string destinationPath)
			{
				this.SourcePath = sourcePath;
				this.DestinationPath = destinationPath;
			}
		}

		[Serializable]
		internal class ItemCopyInformation
		{
			private string itemName;

			private string codeBehindItemName;

			public string CodeBehindItemUrl
			{
				get
				{
					return this.codeBehindItemName;
				}
			}

			public string ItemUrl
			{
				get
				{
					return this.itemName;
				}
			}

			public ItemCopyInformation(string itemName, string codeBehindItemName)
			{
				this.itemName = itemName;
				this.codeBehindItemName = codeBehindItemName;
			}
		}

		[Serializable]
		internal class ProjectCopyInformation
		{
			private bool isCut;

			private string projectUrl;

			private List<CutBuffer.ItemCopyInformation> items;

			public bool IsCut
			{
				get
				{
					return this.isCut;
				}
			}

			public List<CutBuffer.ItemCopyInformation> Items
			{
				get
				{
					return this.items;
				}
			}

			public string ProjectUrl
			{
				get
				{
					return this.projectUrl;
				}
			}

			public ProjectCopyInformation(bool isCut, string projectUrl, List<CutBuffer.ItemCopyInformation> items)
			{
				this.isCut = isCut;
				this.projectUrl = projectUrl;
				this.items = items;
			}

			public ProjectCopyInformation(bool isCut, string projectUrl, string[] fileNames)
			{
				this.isCut = isCut;
				this.projectUrl = projectUrl;
				this.items = new List<CutBuffer.ItemCopyInformation>();
				for (int i = 0; i < (int)fileNames.Length; i++)
				{
					this.items.Add(new CutBuffer.ItemCopyInformation(Microsoft.Expression.Framework.Documents.PathHelper.ResolvePath(fileNames[i]), null));
				}
			}

			public ProjectCopyInformation(CutBuffer.ProjectCopyInformation copy)
			{
				this.isCut = copy.IsCut;
				this.projectUrl = copy.ProjectUrl;
				this.items = copy.items;
			}
		}
	}
}