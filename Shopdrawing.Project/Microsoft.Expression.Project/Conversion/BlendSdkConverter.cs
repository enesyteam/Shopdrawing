using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project.Conversion
{
	internal class BlendSdkConverter : ProjectConverter
	{
		private static List<AssemblyName> oldAssemblies;

		public override string Identifier
		{
			get
			{
				return "BlendSdkConverter";
			}
		}

		public BlendSdkConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		private List<AssemblyName> GetBlendSdkAssemblies()
		{
			if (BlendSdkConverter.oldAssemblies == null)
			{
				List<AssemblyName> assemblyNames = new List<AssemblyName>()
				{
					new AssemblyName("System.Windows.Interactivity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("Microsoft.Expression.Interactions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("Microsoft.Expression.Prototyping.SketchControls, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("Microsoft.Expression.Prototyping.Interactivity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("System.Windows.Interactivity, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("Microsoft.Expression.Interactions, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("Microsoft.Expression.Prototyping.SketchControls, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
					new AssemblyName("Microsoft.Expression.Prototyping.Interactivity, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
				};
				BlendSdkConverter.oldAssemblies = assemblyNames;
			}
			return BlendSdkConverter.oldAssemblies;
		}

		public override ConversionType GetVersion(ConversionTarget project)
		{
			ConversionType conversionType;
			if (!project.IsProject)
			{
				return ConversionType.Unsupported;
			}
			using (IEnumerator<IProjectItemData> enumerator = project.ProjectStore.GetItems("Reference").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProjectItemData current = enumerator.Current;
					AssemblyName assemblyName = ProjectConverterBase.GetAssemblyName(current.Value);
					if (assemblyName == null || !this.IsOverqualifiedSdkAssembly(current, assemblyName))
					{
						continue;
					}
					conversionType = ConversionType.BlendSdk3;
					return conversionType;
				}
				return ConversionType.Unknown;
			}
			return conversionType;
		}

		private bool IsOverqualifiedSdkAssembly(IProjectItemData reference, AssemblyName referenceAssemblyName)
		{
			bool flag;
			List<AssemblyName>.Enumerator enumerator = this.GetBlendSdkAssemblies().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssemblyName current = enumerator.Current;
					byte[] publicKeyToken = referenceAssemblyName.GetPublicKeyToken();
					if (!string.Equals(current.Name, referenceAssemblyName.Name, StringComparison.OrdinalIgnoreCase) || !(referenceAssemblyName.Version == null) && !current.Version.Equals(referenceAssemblyName.Version) || publicKeyToken != null && (int)publicKeyToken.Length != 0 && !ProjectAssemblyHelper.ComparePublicKeyTokens(current.GetPublicKeyToken(), publicKeyToken))
					{
						continue;
					}
					flag = !string.Equals(reference.Value, referenceAssemblyName.Name, StringComparison.OrdinalIgnoreCase);
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

		protected override bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion)
		{
			if (targetVersion != ConversionType.BlendSdk4)
			{
				return false;
			}
			foreach (IProjectItemData item in projectStore.GetItems("Reference"))
			{
				AssemblyName assemblyName = ProjectConverterBase.GetAssemblyName(item.Value);
				if (assemblyName == null || !this.IsOverqualifiedSdkAssembly(item, assemblyName))
				{
					continue;
				}
				string path = projectStore.DocumentReference.Path;
				string updateItemMetadataAction = StringTable.UpdateItemMetadataAction;
				object[] value = new object[] { "Include", "Reference", item.Value, item.Value, assemblyName.Name };
				ProjectLog.LogSuccess(path, updateItemMetadataAction, value);
				item.Value = assemblyName.Name;
			}
			return true;
		}
	}
}