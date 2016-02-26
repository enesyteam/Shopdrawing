using Microsoft.Expression.Framework.Documents;
using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class ProjectReferenceProjectItem : AssemblyReferenceProjectItem
	{
		protected override string BuildItemName
		{
			get
			{
				return "ProjectReference";
			}
			set
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string settingBuildItemNameOnWrongProjectItemType = ExceptionStringTable.SettingBuildItemNameOnWrongProjectItemType;
				object[] name = new object[] { base.GetType().Name };
				throw new InvalidOperationException(string.Format(currentCulture, settingBuildItemNameOnWrongProjectItemType, name));
			}
		}

		public ProjectReferenceProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(project, documentReference, documentType, serviceProvider)
		{
		}
	}
}