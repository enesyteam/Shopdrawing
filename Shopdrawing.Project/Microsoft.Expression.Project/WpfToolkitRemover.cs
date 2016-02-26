using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Conversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Expression.Project
{
	internal class WpfToolkitRemover
	{
		private static string wpfToolkitXmlns;

		private static string platformXmlns;

		private IProjectStore projectStore;

		private IProjectActionContext context;

		private string currentXamlPath;

		static WpfToolkitRemover()
		{
			WpfToolkitRemover.wpfToolkitXmlns = "http://schemas.microsoft.com/wpf/2008/toolkit";
			WpfToolkitRemover.platformXmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		}

		private WpfToolkitRemover(IProjectStore projectStore, IProjectActionContext context)
		{
			this.projectStore = projectStore;
			this.context = context;
		}

		private bool ContainsWpfToolkitNamespace(string xamlFilePath, out string rootElementName)
		{
			rootElementName = null;
			try
			{
				using (StreamReader streamReader = new StreamReader(xamlFilePath, Encoding.UTF8))
				{
					using (XmlReader xmlReader = XmlReader.Create(streamReader))
					{
						if (xmlReader.Read() && xmlReader.MoveToContent() == XmlNodeType.Element)
						{
							rootElementName = xmlReader.Name;
							if (xmlReader.MoveToFirstAttribute())
							{
								do
								{
									if (!(xmlReader.Value == WpfToolkitRemover.wpfToolkitXmlns) || !(xmlReader.LocalName == "xmlns") && !(xmlReader.Prefix == "xmlns"))
									{
										continue;
									}
									return true;
								}
								while (xmlReader.MoveToNextAttribute());
							}
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}

		private IProjectItemData GetWpfToolkitReference()
		{
			IProjectItemData projectItemDatum;
			IEnumerable<IProjectItemData> items = this.projectStore.GetItems("Reference");
			if (items == null)
			{
				return null;
			}
			using (IEnumerator<IProjectItemData> enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProjectItemData current = enumerator.Current;
					string value = current.Value;
					if (string.IsNullOrEmpty(value))
					{
						continue;
					}
					if (string.Compare(value, "WpfToolkit", StringComparison.OrdinalIgnoreCase) != 0)
					{
						if (!value.StartsWith("WpfToolkit,", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						projectItemDatum = current;
						return projectItemDatum;
					}
					else
					{
						projectItemDatum = current;
						return projectItemDatum;
					}
				}
				return null;
			}
			return projectItemDatum;
		}

		private static string ReplaceToolokitReference(Match match)
		{
			return match.Value.Replace(WpfToolkitRemover.wpfToolkitXmlns, WpfToolkitRemover.platformXmlns);
		}

		public string ReplaceWpfToolkitNamespace(string xaml, string rootElementName)
		{
			string str = "[_\\w][\\w\\d\\S]*";
			string str1 = "[\\s\\n]*";
			string str2 = "(?:'[^']*'|\"[^\"]*\")";
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] objArray = new object[] { str, str1, str2 };
			string str3 = string.Format(invariantCulture, "{0}{1}={1}{2}", objArray);
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			object[] objArray1 = new object[] { rootElementName, str1, str3 };
			string str4 = string.Format(cultureInfo, "<{0}(?:{1}{2})*{1}/?>", objArray1);
			Match match = (new Regex(str4)).Match(xaml);
			if (!match.Success)
			{
				ProjectLog.LogError(this.currentXamlPath, StringTable.UnknownError, StringTable.FixWpfToolkitNamespace, new object[0]);
				return null;
			}
			string value = match.Value;
			CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
			object[] objArray2 = new object[] { str1, str, WpfToolkitRemover.wpfToolkitXmlns };
			Regex regex = new Regex(string.Format(invariantCulture1, "{0}xmlns(?:\\:{1})?{0}={0}(?<quote>'|\"){2}\\k<quote>", objArray2));
			string str5 = regex.Replace(value, new MatchEvaluator(WpfToolkitRemover.ReplaceToolokitReference));
			string str6 = xaml.Substring(0, match.Index);
			string str7 = xaml.Substring(match.Index + match.Length);
			string str8 = string.Concat(str6, str5, str7);
			ProjectLog.LogSuccess(this.currentXamlPath, StringTable.FixWpfToolkitNamespace, new object[0]);
			return str8;
		}

		public static void Update(IProjectStore projectStore, IProjectActionContext context, ConversionType initialVersion, ConversionType targetVersion)
		{
			if (initialVersion != ConversionType.ProjectWpf30 && initialVersion != ConversionType.ProjectWpf35)
			{
				return;
			}
			if (targetVersion != ConversionType.ProjectWpf40)
			{
				return;
			}
			(new WpfToolkitRemover(projectStore, context)).Update();
		}

		private void Update()
		{
			IProjectItemData wpfToolkitReference = this.GetWpfToolkitReference();
			if (wpfToolkitReference == null)
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(this.projectStore.DocumentReference.Path);
			string[] strArrays = new string[] { "ApplicationDefinition", "Page", "Content" };
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				IEnumerable<IProjectItemData> items = this.projectStore.GetItems(strArrays[i]);
				if (items != null)
				{
					foreach (IProjectItemData item in items)
					{
						this.currentXamlPath = Path.Combine(directoryName, item.Value);
						this.UpdateToolkitXmlNamespace();
					}
				}
			}
			this.projectStore.RemoveItem(wpfToolkitReference);
		}

		private void UpdateToolkitXmlNamespace()
		{
			string str;
			if (string.Compare(Path.GetExtension(this.currentXamlPath), ".xaml", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return;
			}
			if (!this.ContainsWpfToolkitNamespace(this.currentXamlPath, out str))
			{
				return;
			}
			string str1 = File.ReadAllText(this.currentXamlPath, Encoding.UTF8);
			string str2 = this.ReplaceWpfToolkitNamespace(str1, str);
			if (str2 != null)
			{
				try
				{
					if (this.context == null || ProjectPathHelper.AttemptToMakeWritable(DocumentReference.Create(this.currentXamlPath), this.context))
					{
						File.WriteAllText(this.currentXamlPath, str2, Encoding.UTF8);
						ProjectLog.LogSuccess(this.currentXamlPath, StringTable.SaveAction, new object[0]);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					ProjectLog.LogError(this.currentXamlPath, exception, StringTable.SaveAction, new object[0]);
				}
			}
		}
	}
}