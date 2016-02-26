using Microsoft.Expression.Extensibility.Project.Templates;
using System;

namespace Microsoft.Expression.Project.Templates
{
	internal class VSTemplateTemplateDataExtensibilityWrapper : Microsoft.Expression.Extensibility.Project.Templates.VSTemplateTemplateData
	{
		private Microsoft.Expression.Project.Templates.VSTemplateTemplateData vsTemplateTemplateData;

		internal override bool ExpressionBlendPrototypingEnabled
		{
			get
			{
				return this.vsTemplateTemplateData.ExpressionBlendPrototypingEnabled;
			}
		}

		internal override string ProjectSubType
		{
			get
			{
				return this.vsTemplateTemplateData.ProjectSubType;
			}
		}

		internal override string ProjectSubTypes
		{
			get
			{
				return this.vsTemplateTemplateData.ProjectSubTypes;
			}
		}

		internal override string ProjectType
		{
			get
			{
				return this.vsTemplateTemplateData.ProjectType;
			}
		}

		internal override string TemplateID
		{
			get
			{
				return this.vsTemplateTemplateData.TemplateID;
			}
		}

		internal VSTemplateTemplateDataExtensibilityWrapper(Microsoft.Expression.Project.Templates.VSTemplateTemplateData vsTemplateTemplateData)
		{
			this.vsTemplateTemplateData = vsTemplateTemplateData;
		}
	}
}