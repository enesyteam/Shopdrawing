using System;

namespace Microsoft.Expression.Extensibility.Project.Templates
{
	public abstract class VSTemplateTemplateData
	{
        public abstract bool ExpressionBlendPrototypingEnabled
		{
			get;
		}

        public abstract string ProjectSubType
		{
			get;
		}

        public abstract string ProjectSubTypes
		{
			get;
		}

        public abstract string ProjectType
		{
			get;
		}

        public abstract string TemplateID
		{
			get;
		}

        public VSTemplateTemplateData()
		{
		}
	}
}