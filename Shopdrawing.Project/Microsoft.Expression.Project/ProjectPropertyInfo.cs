using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	public class ProjectPropertyInfo
	{
		private static Dictionary<string, ProjectPropertyInfo> propertyInfo;

		private string propertyName;

		private string displayName;

		private Dictionary<string, ProjectPropertyValue> propertyValues;

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string Property
		{
			get
			{
				return this.propertyName;
			}
		}

		private static Dictionary<string, ProjectPropertyInfo> PropertyInfo
		{
			get
			{
				if (ProjectPropertyInfo.propertyInfo == null)
				{
					ProjectPropertyInfo.propertyInfo = new Dictionary<string, ProjectPropertyInfo>();
					ProjectPropertyInfo projectPropertyInfo = new ProjectPropertyInfo("TargetFrameworkVersion", StringTable.MSBuildPropertyTargetFrameworkVersionDisplayName);
					projectPropertyInfo.AddValueInfo("2.0", "2.0");
					projectPropertyInfo.AddValueInfo("3.0", "3.0");
					projectPropertyInfo.AddValueInfo("3.5", "3.5");
					projectPropertyInfo.AddValueInfo("4.0", "4.0");
					ProjectPropertyInfo.propertyInfo.Add("TargetFrameworkVersion", projectPropertyInfo);
					ProjectPropertyInfo.propertyInfo.Add("ProjectGuid", new ProjectPropertyInfo("ProjectGuid", StringTable.MSBuildPropertyProjectGuidDisplayName));
				}
				return ProjectPropertyInfo.propertyInfo;
			}
		}

		public IDictionary<string, ProjectPropertyValue> PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		static ProjectPropertyInfo()
		{
		}

		public ProjectPropertyInfo(string propertyName, string propertyDisplayName)
		{
			this.propertyName = propertyName;
			this.displayName = propertyDisplayName;
			this.propertyValues = new Dictionary<string, ProjectPropertyValue>();
		}

		public void AddValueInfo(string valueName, string valueDisplayName)
		{
			this.propertyValues.Add(valueName, new ProjectPropertyValue(valueName, valueDisplayName, this));
		}

		public static ProjectPropertyValue CreatePropertyValue(string propertyName, string propertyValue)
		{
			if (ProjectPropertyInfo.PropertyInfo[propertyName].PropertyValues.ContainsKey(propertyValue))
			{
				return ProjectPropertyInfo.PropertyInfo[propertyName].PropertyValues[propertyValue];
			}
			return new ProjectPropertyValue(propertyValue, propertyValue, ProjectPropertyInfo.PropertyInfo[propertyName]);
		}

		public bool IsValueOf(ProjectPropertyValue value)
		{
			if (value != null && value.ParentProperty.Property == this.Property)
			{
				return true;
			}
			return false;
		}
	}
}