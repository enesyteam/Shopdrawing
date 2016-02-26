using System;

namespace Microsoft.Expression.Project
{
    public class ProjectPropertyValue
    {
        private ProjectPropertyInfo parentProperty;

        private string @value;

        private string displayValue;

        public string DisplayValue
        {
            get
            {
                return this.displayValue;
            }
            set
            {
                this.displayValue = value;
            }
        }

        public ProjectPropertyInfo ParentProperty
        {
            get
            {
                return this.parentProperty;
            }
        }

        public string Value
        {
            get
            {
                return this.@value;
            }
            set
            {
                this.@value = value;
            }
        }

        public ProjectPropertyValue(string value, string displayValue, ProjectPropertyInfo parentProperty)
        {
            this.@value = value;
            this.displayValue = displayValue;
            this.parentProperty = parentProperty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            ProjectPropertyValue projectPropertyValue = obj as ProjectPropertyValue;
            if (projectPropertyValue == null)
            {
                return this.Equals(obj);
            }
            if (this.Value != projectPropertyValue.Value)
            {
                return false;
            }
            return this.parentProperty.Property == projectPropertyValue.parentProperty.Property;
        }

        public override int GetHashCode()
        {
            return this.ParentProperty.GetHashCode() ^ this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.DisplayValue;
        }
    }
}