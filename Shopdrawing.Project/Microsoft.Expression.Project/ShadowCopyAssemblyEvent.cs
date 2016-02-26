using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	public class ShadowCopyAssemblyEvent : AssemblyLoggingEvent
	{
		public override string Name
		{
			get
			{
				return "Shadow Copy";
			}
		}

		public string OriginalLocation
		{
			get;
			private set;
		}

		public Assembly Shadow
		{
			get;
			private set;
		}

		public override XElement XmlElement
		{
			get
			{
				XElement xmlElement = base.XmlElement;
				XElement xElement = base.BuildAssemblyElement("ShadowAssembly", this.Shadow.FullName, this.Shadow.Location);
				xElement.SetAttributeValue("OriginalLocation", this.OriginalLocation);
				xmlElement.AddFirst(xElement);
				return xmlElement;
			}
		}

		public ShadowCopyAssemblyEvent(string originalLocation, Assembly shadow) : base(Microsoft.Expression.Project.AssemblyLoadingAppDomain.Main)
		{
			this.OriginalLocation = originalLocation;
			this.Shadow = shadow;
		}
	}
}