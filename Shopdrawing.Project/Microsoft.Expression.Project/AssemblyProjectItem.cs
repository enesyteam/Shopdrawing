using Microsoft.Expression.Framework.Documents;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class AssemblyProjectItem : AssemblyReferenceProjectItem
	{
		private bool globalAssemblyCache;

		protected override string BuildItemName
		{
			get
			{
				return "Reference";
			}
			set
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string settingBuildItemNameOnWrongProjectItemType = ExceptionStringTable.SettingBuildItemNameOnWrongProjectItemType;
				object[] name = new object[] { base.GetType().Name };
				throw new InvalidOperationException(string.Format(currentCulture, settingBuildItemNameOnWrongProjectItemType, name));
			}
		}

		internal bool GlobalAssemblyCache
		{
			get
			{
				return this.globalAssemblyCache;
			}
		}

		protected override string Include
		{
			set
			{
				base.Include = Path.GetFileName(value);
				base.SetMetadata("HintPath", value);
			}
		}

		public AssemblyProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(project, documentReference, documentType, serviceProvider)
		{
			this.UpdateGlobalAssemblyCacheProperty();
		}

		private void UpdateGlobalAssemblyCacheProperty()
		{
			MSBuildBasedProject project = base.Project as MSBuildBasedProject;
			bool flag = false;
			if (project != null)
			{
				ProjectAssembly referencedAssembly = project.GetReferencedAssembly(this);
				if (referencedAssembly != null)
				{
					flag = (referencedAssembly.RuntimeAssembly != null ? referencedAssembly.RuntimeAssembly.GlobalAssemblyCache : false);
				}
			}
			this.globalAssemblyCache = flag;
		}
	}
}