using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.VSWebsites
{
	internal class VSWebsitesHelper
	{
		private DesignTimeData designTimeData;

		private string filename = "Microsoft\\WebsiteCache\\Websites.xml";

		public VSWebsitesHelper()
		{
		}

		public VSWebsitesWebsite FindWebsite(string url)
		{
			VSWebsitesWebsite vSWebsitesWebsite = null;
			if (this.designTimeData == null)
			{
				this.ReadData();
			}
			if (this.designTimeData != null)
			{
				VSWebsitesWebsite[] websites = this.designTimeData.Websites;
				int num = 0;
				while (num < (int)websites.Length)
				{
					VSWebsitesWebsite vSWebsitesWebsite1 = websites[num];
					StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
					char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
					string str = url.TrimEnd(directorySeparatorChar);
					string rootUrl = vSWebsitesWebsite1.RootUrl;
					char[] chrArray = new char[] { Path.DirectorySeparatorChar };
					if (ordinalIgnoreCase.Compare(str, rootUrl.TrimEnd(chrArray)) != 0)
					{
						num++;
					}
					else
					{
						vSWebsitesWebsite = vSWebsitesWebsite1;
						break;
					}
				}
			}
			return vSWebsitesWebsite;
		}

		public void ReadData()
		{
			this.designTimeData = null;
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string str = Path.Combine(folderPath, this.filename);
			if (File.Exists(str))
			{
				try
				{
					TextReader streamReader = new StreamReader(new FileStream(str, FileMode.Open, FileAccess.Read));
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(DesignTimeData));
					this.designTimeData = (DesignTimeData)xmlSerializer.Deserialize(streamReader);
				}
				catch
				{
					this.designTimeData = null;
				}
			}
		}
	}
}