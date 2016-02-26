using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class BuildTaskInfoPopulator
	{
		public BuildTaskInfoPopulator()
		{
		}

		protected virtual IEnumerable<DocumentCreationInfo> FillOutBuildTaskInfoInternal(IEnumerable<DocumentCreationInfo> creationInfo, IProject project)
		{
			return creationInfo;
		}

		public IEnumerable<DocumentCreationInfo> PopulateBuildTaskInfo(IEnumerable<DocumentCreationInfo> creationInfo, IProject project)
		{
			creationInfo = this.FillOutBuildTaskInfoInternal(creationInfo, project);
			return creationInfo.Select<DocumentCreationInfo, DocumentCreationInfo>((DocumentCreationInfo info) => {
				if (info.BuildTaskInfo == null)
				{
					info.BuildTaskInfo = info.DocumentType.DefaultBuildTaskInfo;
				}
				return info;
			});
		}
	}
}