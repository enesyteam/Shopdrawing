using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	public abstract class AssemblyLoggingEvent
	{
		public Microsoft.Expression.Project.AssemblyLoadingAppDomain AssemblyLoadingAppDomain
		{
			get;
			private set;
		}

		public abstract string Name
		{
			get;
		}

		public string StackTrace
		{
			get;
			set;
		}

		public DateTime Timestamp
		{
			get;
			private set;
		}

		public virtual XElement XmlElement
		{
			get
			{
				XElement xElement = new XElement("Event");
				xElement.SetAttributeValue("Name", this.Name);
				xElement.SetAttributeValue("AppDomain", this.AssemblyLoadingAppDomain.ToString());
				xElement.SetAttributeValue("TimeStamp", this.Timestamp.ToLongTimeString());
				if (this.StackTrace != null)
				{
					xElement.Add(new XElement("StackTrace", this.StackTrace));
				}
				return xElement;
			}
		}

		protected AssemblyLoggingEvent(Microsoft.Expression.Project.AssemblyLoadingAppDomain appDomain)
		{
			this.AssemblyLoadingAppDomain = appDomain;
			this.Timestamp = DateTime.Now;
		}

		protected XElement BuildAssemblyElement(string elementName, string fullName, string location)
		{
			XElement xElement = new XElement(elementName);
			xElement.SetAttributeValue("FullName", fullName);
			xElement.SetAttributeValue("Location", location);
			return xElement;
		}

		protected static string GetAssemblyLocation(Assembly assembly)
		{
			string location;
			if (assembly.IsDynamic)
			{
				return "Dynamic assembly";
			}
			try
			{
				location = assembly.Location;
			}
			catch (NotSupportedException notSupportedException)
			{
				location = "Unknown location";
			}
			return location;
		}
	}
}