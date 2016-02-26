using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	public class AssemblyLoadedEvent : AssemblyLoggingEvent
	{
		public string FullName
		{
			get;
			private set;
		}

		public string Location
		{
			get;
			private set;
		}

		public override string Name
		{
			get
			{
				return "Assembly Loaded";
			}
		}

		public override XElement XmlElement
		{
			get
			{
				XElement xmlElement = base.XmlElement;
				xmlElement.AddFirst(base.BuildAssemblyElement("Assembly", this.FullName, this.Location));
				return xmlElement;
			}
		}

		public AssemblyLoadedEvent(Assembly assembly) : this(assembly.FullName, AssemblyLoggingEvent.GetAssemblyLocation(assembly), Microsoft.Expression.Project.AssemblyLoadingAppDomain.Main)
		{
		}

		public AssemblyLoadedEvent(string fullName, string location, Microsoft.Expression.Project.AssemblyLoadingAppDomain appDomain) : base(appDomain)
		{
			this.FullName = fullName;
			this.Location = location;
		}
	}
}