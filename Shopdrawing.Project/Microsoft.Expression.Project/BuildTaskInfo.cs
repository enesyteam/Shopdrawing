using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class BuildTaskInfo
	{
		public string BuildTask
		{
			get;
			private set;
		}

		public IDictionary<string, string> MetadataInfo
		{
			get;
			private set;
		}

		public BuildTaskInfo(string buildTask, IDictionary<string, string> metadataInfo)
		{
			this.BuildTask = buildTask;
			this.MetadataInfo = metadataInfo;
		}
	}
}