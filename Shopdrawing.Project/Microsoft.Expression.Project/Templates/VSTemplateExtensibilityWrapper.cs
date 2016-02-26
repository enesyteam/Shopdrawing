using Microsoft.Expression.Extensibility.Project.Templates;
using System;

namespace Microsoft.Expression.Project.Templates
{
	internal class VSTemplateExtensibilityWrapper : Microsoft.Expression.Extensibility.Project.Templates.VSTemplate
	{
		private Microsoft.Expression.Project.Templates.VSTemplate vsTemplate;

		private Microsoft.Expression.Extensibility.Project.Templates.VSTemplateTemplateData vsTemplateTemplateData;

		internal override Microsoft.Expression.Extensibility.Project.Templates.VSTemplateTemplateData TemplateData
		{
			get
			{
				if (this.vsTemplateTemplateData == null)
				{
					this.vsTemplateTemplateData = new VSTemplateTemplateDataExtensibilityWrapper(this.vsTemplate.TemplateData);
				}
				return this.vsTemplateTemplateData;
			}
		}

		internal override string Type
		{
			get
			{
				return this.vsTemplate.Type;
			}
		}

		internal VSTemplateExtensibilityWrapper(Microsoft.Expression.Project.Templates.VSTemplate vsTemplate)
		{
			this.vsTemplate = vsTemplate;
		}
	}
}