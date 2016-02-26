using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Project.UserInterface
{
	public class FileDropUtility
	{
		private List<IDocumentType> supportedDocumentTypes;

		private IProjectManager projectManager;

		public FileDropUtility(IProjectManager projectManager, FrameworkElement target, IDocumentType[] supportedDocumentTypes)
		{
			this.projectManager = projectManager;
			this.supportedDocumentTypes = new List<IDocumentType>();
			if (target != null)
			{
				target.AllowDrop = true;
			}
			IDocumentType[] documentTypeArray = supportedDocumentTypes;
			for (int i = 0; i < (int)documentTypeArray.Length; i++)
			{
				IDocumentType documentType = documentTypeArray[i];
				if (!this.supportedDocumentTypes.Contains(documentType))
				{
					this.supportedDocumentTypes.Add(documentType);
				}
			}
		}

		public string[] GetSupportedFiles(IDataObject dataObject)
		{
			SafeDataObject safeDataObject = new SafeDataObject(dataObject);
			List<string> strs = new List<string>();
			string[] data = (string[])safeDataObject.GetData(DataFormats.FileDrop);
			if (data != null)
			{
				string[] strArrays = data;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					IProject project = this.projectManager.ItemSelectionSet.SelectedProjects.SingleOrNull<IProject>();
					if (project != null && this.IsDocumentTypeSupported(project.GetDocumentType(str)))
					{
						strs.Add(str);
					}
				}
			}
			ItemSelectionSet itemSelectionSets = (ItemSelectionSet)safeDataObject.GetData("BlendProjectItem");
			if (itemSelectionSets != null)
			{
				foreach (IProjectItem selection in itemSelectionSets.Selection)
				{
					if (!this.IsDocumentTypeSupported(selection.DocumentType))
					{
						continue;
					}
					strs.Add(selection.DocumentReference.Path);
				}
			}
			return strs.ToArray();
		}

		private bool IsDocumentTypeSupported(IDocumentType documentType)
		{
			bool flag;
			List<IDocumentType>.Enumerator enumerator = this.supportedDocumentTypes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IDocumentType current = enumerator.Current;
					if (!current.GetType().IsAssignableFrom(documentType.GetType()))
					{
						continue;
					}
					flag = true;
					return flag;
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
}