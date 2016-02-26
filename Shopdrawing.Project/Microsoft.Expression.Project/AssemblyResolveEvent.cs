using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	public class AssemblyResolveEvent : AssemblyLoggingEvent
	{
		public string AssemblyName
		{
			get;
			private set;
		}

		public override string Name
		{
			get
			{
				return "Assembly Resolve";
			}
		}

		public bool Success
		{
			get;
			private set;
		}

		public override XElement XmlElement
		{
			get
			{
				XElement xmlElement = base.XmlElement;
				XElement xElement = new XElement("Assembly");
				xElement.SetAttributeValue("Name", this.AssemblyName);
				xElement.SetAttributeValue("Success", this.Success.ToString());
				xmlElement.AddFirst(xElement);
				return xmlElement;
			}
		}

		public AssemblyResolveEvent(string assemblyName, bool success, Microsoft.Expression.Project.AssemblyLoadingAppDomain appDomain) : base(appDomain)
		{
			this.AssemblyName = assemblyName;
			this.Success = success;
		}
	}
}