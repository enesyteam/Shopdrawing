using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	public class AppDomainListEvent : AssemblyLoggingEvent
	{
		private List<Assembly> assemblies = new List<Assembly>();

		public override string Name
		{
			get
			{
				return "Current AppDomain Assemblies";
			}
		}

		public override XElement XmlElement
		{
			get
			{
				XElement xmlElement = base.XmlElement;
				foreach (Assembly assembly in this.assemblies)
				{
					xmlElement.Add(base.BuildAssemblyElement("Assembly", assembly.FullName, AssemblyLoggingEvent.GetAssemblyLocation(assembly)));
				}
				return xmlElement;
			}
		}

		public AppDomainListEvent() : base(Microsoft.Expression.Project.AssemblyLoadingAppDomain.Main)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < (int)assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				this.assemblies.Add(assembly);
			}
		}
	}
}