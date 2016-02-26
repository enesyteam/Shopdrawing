using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.VSWebsites
{
	public class VSWebsitesWebsite
	{
		private string rootUrlField;

		private string startPageField;

		private int vwdPortField;

		[XmlAttribute]
		public string RootUrl
		{
			get
			{
				return this.rootUrlField;
			}
			set
			{
				this.rootUrlField = value;
			}
		}

		[XmlAttribute("startpage")]
		public string StartPage
		{
			get
			{
				return this.startPageField;
			}
			set
			{
				this.startPageField = value;
			}
		}

		public string StartPageFullPath
		{
			get
			{
				if (this.StartPage == null)
				{
					return null;
				}
				return Path.Combine(this.RootUrl, this.StartPage);
			}
		}

		[XmlAttribute("vwdport")]
		public int VwdPort
		{
			get
			{
				return this.vwdPortField;
			}
			set
			{
				this.vwdPortField = value;
			}
		}

		public VSWebsitesWebsite()
		{
		}
	}
}