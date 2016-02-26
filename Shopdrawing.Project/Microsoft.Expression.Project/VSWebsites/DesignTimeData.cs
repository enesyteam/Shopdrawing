using System;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.VSWebsites
{
	public class DesignTimeData
	{
		private VSWebsitesWebsite[] websites;

		[XmlElement("Website", typeof(VSWebsitesWebsite))]
		public VSWebsitesWebsite[] Websites
		{
			get
			{
				return this.websites;
			}
			set
			{
				this.websites = value;
			}
		}

		public DesignTimeData()
		{
		}
	}
}