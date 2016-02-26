using Microsoft.Expression.Framework.Documents;
using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class ComReferenceProjectItem : AssemblyReferenceProjectItem
	{
		protected override string BuildItemName
		{
			get
			{
				return "COMReference";
			}
			set
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string settingBuildItemNameOnWrongProjectItemType = ExceptionStringTable.SettingBuildItemNameOnWrongProjectItemType;
				object[] name = new object[] { base.GetType().Name };
				throw new InvalidOperationException(string.Format(currentCulture, settingBuildItemNameOnWrongProjectItemType, name));
			}
		}

		public override bool FileExists
		{
			get
			{
				MSBuildBasedProject project = base.Project as MSBuildBasedProject;
				if (project != null && project.GetReferencedAssembly(this) == null)
				{
					return false;
				}
				return true;
			}
		}

		public ComReferenceProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(project, documentReference, documentType, serviceProvider)
		{
		}
	}
}