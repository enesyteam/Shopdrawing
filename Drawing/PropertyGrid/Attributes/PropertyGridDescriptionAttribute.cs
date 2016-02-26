using System;

namespace DynamicGeometry
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class PropertyGridDescriptionAttribute : Attribute
    {
        public PropertyGridDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
