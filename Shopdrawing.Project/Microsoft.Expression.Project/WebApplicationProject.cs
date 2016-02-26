using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Framework.WebServer;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	internal sealed class WebApplicationProject : ExecutableProjectBase
	{
		private const string VisualStudioProjectExtensionSection = "VisualStudio";

		private const string DevelopmentServerPort = "DevelopmentServerPort";

		private const string AutoAssignPort = "AutoAssignPort";

		private static int CassiniTimeout;

		private IProjectItem startupScene;

		private IWebServerService webServerService;

		private int? serverHandle;

		public readonly static string SilverlightHostFileName;

		public readonly static string PreviewSilverlightHostFileName;

		public readonly static string TemplateDirectoryName;

		public readonly static string WebpageFileName;

		private IDocumentType HtmlDocumentType
		{
			get
			{
				return base.Services.DocumentTypes()[DocumentTypeNamesHelper.Html];
			}
		}

		public override System.Diagnostics.ProcessStartInfo ProcessStartInfo
		{
			get
			{
				bool flag;
				if (this.StartupItem == null)
				{
					return null;
				}
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(Path.GetDirectoryName(base.DocumentReference.Path));
				Uri potentialServerLocation = this.GetPotentialServerLocation();
				System.Diagnostics.ProcessStartInfo processStartInfo = base.Services.OutOfBrowserDeploymentService().TryPerformOutOfBrowserDeployment(this, documentReference, potentialServerLocation, out flag);
				if (processStartInfo == null || flag)
				{
					this.EnsureServer(documentReference);
				}
				if (processStartInfo == null)
				{
					string sessionAddress = this.webServerService.GetSessionAddress(this.serverHandle.Value, this.StartupItem.DocumentReference.Path);
					if (sessionAddress == null)
					{
						return null;
					}
					processStartInfo = new System.Diagnostics.ProcessStartInfo((new Uri(sessionAddress, UriKind.Absolute)).AbsoluteUri)
					{
						UseShellExecute = true,
						Verb = "Open"
					};
				}
				return processStartInfo;
			}
		}

		public override string StartArguments
		{
			get
			{
				return string.Empty;
			}
		}

		public override string StartProgram
		{
			get
			{
				return string.Empty;
			}
		}

		protected override IDocumentType StartupDocumentType
		{
			get
			{
				return this.HtmlDocumentType;
			}
		}

		public override IProjectItem StartupItem
		{
			get
			{
				if (this.startupScene == null)
				{
					IDocumentType htmlDocumentType = this.HtmlDocumentType;
					string evaluatedPropertyValue = base.GetEvaluatedPropertyValue("StartPageUrl");
					if (evaluatedPropertyValue != null)
					{
						foreach (IProjectItem item in (IEnumerable<IProjectItem>)base.Items)
						{
							string projectRelativeDocumentReference = item.ProjectRelativeDocumentReference;
							char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
							if (evaluatedPropertyValue != projectRelativeDocumentReference.TrimStart(directorySeparatorChar))
							{
								continue;
							}
							this.startupScene = item;
							break;
						}
					}
					if (this.startupScene == null)
					{
						foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>)base.Items)
						{
							if (projectItem.DocumentType != htmlDocumentType)
							{
								continue;
							}
							this.startupScene = projectItem;
							break;
						}
					}
				}
				return this.startupScene;
			}
			set
			{
				string str;
				if (this.startupScene != value)
				{
					IProjectItem projectItem = this.startupScene;
					this.startupScene = value;
					IProjectStore projectStore = base.ProjectStore;
					if (value == null)
					{
						str = null;
					}
					else
					{
						string projectRelativeDocumentReference = value.ProjectRelativeDocumentReference;
						char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
						str = projectRelativeDocumentReference.TrimStart(directorySeparatorChar);
					}
					projectStore.SetProperty("StartPageUrl", str);
					base.OnStartupSceneChanged(new ProjectItemChangedEventArgs(projectItem, this.startupScene));
				}
			}
		}

		public override ICollection<string> TemplateProjectSubtypes
		{
			get
			{
				ICollection<string> templateProjectSubtypes = base.TemplateProjectSubtypes;
				templateProjectSubtypes.Add("Website");
				return templateProjectSubtypes;
			}
		}

		public override string WorkingDirectory
		{
			get
			{
				return string.Empty;
			}
		}

		static WebApplicationProject()
		{
			WebApplicationProject.CassiniTimeout = 5000;
			WebApplicationProject.SilverlightHostFileName = "Silverlight.js";
			WebApplicationProject.PreviewSilverlightHostFileName = "Silverlight.preview.js";
			WebApplicationProject.TemplateDirectoryName = "HTML";
			WebApplicationProject.WebpageFileName = "Default.html";
		}

		private WebApplicationProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider) : base(projectStore, codeDocumentType, projectType, serviceProvider)
		{
			this.webServerService = serviceProvider.GetService<IWebServerService>();
		}

		public static IProject Create(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
		{
			return KnownProjectBase.TryCreate(() => new WebApplicationProject(projectStore, codeDocumentType, projectType, serviceProvider));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.serverHandle.HasValue)
				{
					this.webServerService.StopBrowsingSession(this.serverHandle.Value);
				}
				this.serverHandle = null;
			}
			base.Dispose(disposing);
		}

		private void EnsureServer(Microsoft.Expression.Framework.Documents.DocumentReference serverLocation)
		{
			if (this.serverHandle.HasValue && !this.webServerService.IsServerReachable(this.serverHandle.Value, WebApplicationProject.CassiniTimeout))
			{
				this.webServerService.StopBrowsingSession(this.serverHandle.Value);
				this.serverHandle = null;
			}
			ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
			if (!this.serverHandle.HasValue && currentSolution != null)
			{
				WebServerSettings webServerSetting = new WebServerSettings(serverLocation.Path);
				int? webServerPort = this.GetWebServerPort();
				if (!webServerPort.HasValue || !this.IsPortAvailable(webServerPort.Value))
				{
					object projectProperty = currentSolution.SolutionSettingsManager.GetProjectProperty(this, "Port");
					if (projectProperty is int && this.IsPortAvailable((int)projectProperty))
					{
						webServerSetting.Port = (int)projectProperty;
					}
				}
				else
				{
					webServerSetting.Port = webServerPort.Value;
				}
				this.serverHandle = new int?(this.webServerService.StartServer(webServerSetting));
			}
			if (this.serverHandle.HasValue && currentSolution != null)
			{
				string sessionAddress = this.webServerService.GetSessionAddress(this.serverHandle.Value);
				if (!string.IsNullOrEmpty(sessionAddress))
				{
					Uri uri = new Uri(sessionAddress, UriKind.Absolute);
					if (uri.Port >= 0)
					{
						currentSolution.SolutionSettingsManager.SetProjectProperty(this, "Port", uri.Port);
					}
				}
			}
		}

		public override T GetCapability<T>(string name)
		{
			string str = name;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "CanHaveStartupItem")
				{
					return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(true);
				}
				if (str1 == "CanAddAssemblyReference")
				{
					return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(false);
				}
			}
			return base.GetCapability<T>(name);
		}

		private Uri GetPotentialServerLocation()
		{
			Uri uri;
			ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
			if (currentSolution != null)
			{
				object projectProperty = currentSolution.SolutionSettingsManager.GetProjectProperty(this, "Port");
				if (projectProperty is int && Uri.TryCreate(string.Concat("http://localhost:", (int)projectProperty, "/"), UriKind.Absolute, out uri))
				{
					return uri;
				}
			}
			return new Uri("http://localhost/", UriKind.Absolute);
		}

		private int? GetWebServerPort()
		{
			string str;
			string str1;
			bool flag;
			int num;
			int? nullable;
			string value;
			string value1;
			string projectExtension = base.GetProjectExtension("VisualStudio");
			if (!string.IsNullOrEmpty(projectExtension))
			{
				str = null;
				str1 = null;
				try
				{
					XDocument xDocument = XDocument.Parse(projectExtension, LoadOptions.None);
					XElement xElement = xDocument.Descendants("AutoAssignPort").FirstOrDefault<XElement>();
					if (xElement != null)
					{
						value = xElement.Value;
					}
					else
					{
						value = null;
					}
					str = value;
					XElement xElement1 = xDocument.Descendants("DevelopmentServerPort").FirstOrDefault<XElement>();
					if (xElement1 != null)
					{
						value1 = xElement1.Value;
					}
					else
					{
						value1 = null;
					}
					str1 = value1;
					goto Label0;
				}
				catch (Exception exception)
				{
					nullable = null;
				}
				return nullable;
			}
			int? nullable1 = null;
			return nullable1;
		Label0:
			if (str != null && bool.TryParse(str, out flag) && !flag && str1 != null && int.TryParse(str1, out num) && num != 0)
			{
				return new int?(num);
			}
			else
			{
				nullable1 = null;
				return nullable1;
			}
		}

		private bool IsPortAvailable(int port)
		{
			bool flag;
			try
			{
				IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
				int num = 0;
				while (num < (int)activeTcpListeners.Length)
				{
					if (activeTcpListeners[num].Port != port)
					{
						num++;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				return true;
			}
			catch (NetworkInformationException networkInformationException)
			{
				flag = false;
			}
			return flag;
		}

		protected override void RenameProjectItemInternal(IProjectItem projectItem, Microsoft.Expression.Framework.Documents.DocumentReference newDocumentReference)
		{
			if (projectItem == this.StartupItem)
			{
				IProjectStore projectStore = base.ProjectStore;
				string projectRelativeDocumentReference = projectItem.ProjectRelativeDocumentReference;
				char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
				projectStore.SetProperty("StartPageUrl", projectRelativeDocumentReference.TrimStart(directorySeparatorChar));
			}
		}
	}
}