using System;
using System.Reflection;


namespace Shopdrawing.Framework.Runtime
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class CommandMethodAttribute : Attribute
    {
        private string m_groupName;
        private string m_globalName;
        public MethodInfo MethodInfo { get; set; }
        public string GlobalName
        {
            get
            {
                return this.m_globalName;
            }
        }

        public string GroupName
        {
            get
            {
                return this.m_groupName;
            }
        }

        #region Const
        public CommandMethodAttribute(string groupName, string globalName)
        {
            this.m_groupName = groupName;
            this.m_globalName = globalName;
        }
        #endregion
    }
}
