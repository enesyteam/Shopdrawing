using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.Project
{
	internal sealed class FileSystemProjectStore : ProjectStoreBase
	{
		private Dictionary<string, string> properties = new Dictionary<string, string>();

		private FileSystemProjectStore(Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(documentReference)
		{
		}

		internal static IProjectStore CreateInstance(Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			if (documentReference == null)
			{
				throw new ArgumentNullException("documentReference");
			}
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			if (!documentReference.IsValidPathFormat)
			{
				throw new ArgumentOutOfRangeException("documentReference", "Document reference must be a valid path.");
			}
			if (File.Exists(documentReference.Path))
			{
				return null;
			}
			if (!Directory.Exists(documentReference.Path))
			{
				throw new FileNotFoundException("Directory not found.", documentReference.Path);
			}
			return new FileSystemProjectStore(documentReference);
		}

		public override string GetProperty(string name)
		{
			string str = null;
			this.properties.TryGetValue(name, out str);
			return str;
		}

		public override bool IsPropertyWritable(string name)
		{
			return true;
		}

		protected override bool SetProperty(string name, string value, bool persisted)
		{
			if (!this.properties.ContainsKey(name))
			{
				this.properties.Add(name, value);
			}
			else
			{
				this.properties[name] = value;
			}
			return true;
		}
	}
}