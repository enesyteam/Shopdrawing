using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Conversion
{
	internal class UpgradeAction
	{
		internal IProjectConverter Converter
		{
			get;
			set;
		}

		internal ConversionType InitialType
		{
			get;
			set;
		}

		internal string Path
		{
			get
			{
				return this.TargetFile.DocumentReference.Path;
			}
		}

		internal ISolutionManagement Solution
		{
			get;
			set;
		}

		internal ConversionTarget TargetFile
		{
			get;
			set;
		}

		internal ConversionType TargetType
		{
			get;
			set;
		}

		internal string Text
		{
			get;
			set;
		}

		public UpgradeAction()
		{
		}

		internal bool DoUpgrade()
		{
			return this.Converter.Convert(this.TargetFile, this.InitialType, this.TargetType);
		}

		public override string ToString()
		{
			string[] str = new string[] { "[", this.InitialType.ToString(), " -> ", this.TargetType.ToString(), "] ", this.Path };
			return string.Concat(str);
		}
	}
}