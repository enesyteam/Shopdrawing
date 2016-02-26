using System;

namespace Microsoft.Expression.Extensibility.Project.Templates
{
    public abstract class VSTemplate
	{
		public abstract VSTemplateTemplateData TemplateData
		{
			get;
		}

        public abstract string Type
		{
			get;
		}

		protected VSTemplate()
		{
		}
	}
}