using Microsoft.Expression.SubsetFontTask.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.Project
{
	public class TemplateParser
	{
		private string templateLocation;

		private static Regex argument;

		private static Regex ifBlock;

		public string TemplateFolder
		{
			get
			{
				return this.templateLocation;
			}
		}

		static TemplateParser()
		{
			TemplateParser.argument = new Regex("\\$[\\w_]*\\$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			TemplateParser.ifBlock = new Regex("(^[\\s-[\\n\\r]]*)?\\$if\\$\\s*\\(\\s*(?<left>[^=!<>]*?)\\s*(?<operand>!=|==|<=|>=|<|>)\\s*(?<right>[^)]*?)\\s*\\)([\\s-[\\n\\r]]*\\r?\\n)?(?<content>.*?)(^[\\s-[\\n\\r]]*)?\\$endif\\$([\\s-[\\n\\r]]*\\r?\\n)?", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
		}

		public TemplateParser(string templateLocation)
		{
			this.templateLocation = templateLocation;
		}

		public TemplateParser(string codeFileExtension, string templateLocationName)
		{
			string empty = string.Empty;
			string upperInvariant = codeFileExtension.ToUpperInvariant();
			string str = upperInvariant;
			if (upperInvariant != null)
			{
				if (str == ".CS")
				{
					empty = "CSharp";
				}
				else if (str == ".VB")
				{
					empty = "VisualBasic";
				}
			}
			this.templateLocation = Path.Combine(Path.Combine(TemplateManager.BaseItemTemplateFolder, empty), templateLocationName);
		}

		internal static string EvaluateTemplate(string template)
		{
			if (string.IsNullOrEmpty(template))
			{
				return string.Empty;
			}
			return TemplateParser.ifBlock.Replace(template, (Match match) => {
				bool flag = false;
				string value = match.Groups["left"].Value;
				string str = match.Groups["right"].Value;
				string value1 = match.Groups["operand"].Value;
				string str1 = value1;
				if (value1 != null)
				{
					switch (str1)
					{
						case "==":
						{
							flag = string.Compare(value, str, StringComparison.Ordinal) == 0;
							break;
						}
						case "!=":
						{
							flag = string.Compare(value, str, StringComparison.Ordinal) != 0;
							break;
						}
						case "<":
						{
							flag = string.Compare(value, str, StringComparison.Ordinal) < 0;
							break;
						}
						case ">":
						{
							flag = string.Compare(value, str, StringComparison.Ordinal) > 0;
							break;
						}
						case "<=":
						{
							flag = string.Compare(value, str, StringComparison.Ordinal) <= 0;
							break;
						}
						case ">=":
						{
							flag = string.Compare(value, str, StringComparison.Ordinal) >= 0;
							break;
						}
						default:
						{
							return match.Value;
						}
					}
					if (!flag)
					{
						return string.Empty;
					}
					return match.Groups["content"].Value;
				}
				return match.Value;
			});
		}

		public static string LoadTemplate(TextReader reader)
		{
			return reader.ReadToEnd();
		}

		public string ParseTemplate(string templateName, ICollection<TemplateArgument> arguments)
		{
			string end;
			if (!TemplateBase.IsZipArchive(this.templateLocation))
			{
				using (StreamReader streamReader = new StreamReader(Path.Combine(this.templateLocation, templateName)))
				{
					end = streamReader.ReadToEnd();
				}
			}
			else
			{
				using (StreamReader streamReader1 = ZipHelper.CreateZipArchive(this.templateLocation).OpenText(templateName))
				{
					end = streamReader1.ReadToEnd();
				}
			}
			return TemplateParser.EvaluateTemplate(TemplateParser.ReplaceTemplateArguments(end, arguments));
		}

		public static string ParseTemplate(TextReader templateSource, IEnumerable<TemplateArgument> arguments)
		{
			string str = TemplateParser.LoadTemplate(templateSource);
			if (!string.IsNullOrEmpty(str))
			{
				str = TemplateParser.EvaluateTemplate(TemplateParser.ReplaceTemplateArguments(str, arguments));
			}
			return str;
		}

		public static string ReplaceTemplateArguments(string template, IEnumerable<TemplateArgument> arguments)
		{
			foreach (TemplateArgument argument in arguments)
			{
				string str1 = string.Concat("$", argument.Name, "$");
				template = template.Replace(str1, argument.Value);
			}
			return TemplateParser.argument.Replace(template, (Match match) => {
				string value = match.Value;
				string str = value;
				if (value != null)
				{
					if (str == "$if$" || str == "$endif$")
					{
						return match.Value;
					}
					if (str == "$$")
					{
						return "$";
					}
				}
				return string.Empty;
			});
		}
	}
}