using System;

namespace Microsoft.Expression.Project
{
    public class TemplateArgument
    {
        private string name;

        private string @value;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Value
        {
            get
            {
                return this.@value;
            }
        }

        public TemplateArgument(string name, string value)
        {
            this.name = name;
            this.@value = value;
        }
    }
}