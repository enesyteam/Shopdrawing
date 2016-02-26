using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.Project.Build
{
	public static class CodeGenerator
	{
		internal static string FixUpIdentifier(string identifier, char[] additionalValidChars)
		{
			if (string.IsNullOrEmpty(identifier))
			{
				return "_";
			}
			char[] charArray = identifier.ToCharArray();
			for (int i = 0; i < (int)charArray.Length; i++)
			{
				if ((int)additionalValidChars.Length == 0 || !additionalValidChars.Contains<char>(charArray[i]))
				{
					switch (char.GetUnicodeCategory(charArray[i]))
					{
						case UnicodeCategory.UppercaseLetter:
						case UnicodeCategory.LowercaseLetter:
						case UnicodeCategory.TitlecaseLetter:
						case UnicodeCategory.ModifierLetter:
						case UnicodeCategory.OtherLetter:
						case UnicodeCategory.LetterNumber:
						{
							break;
						}
						case UnicodeCategory.NonSpacingMark:
						case UnicodeCategory.SpacingCombiningMark:
						case UnicodeCategory.DecimalDigitNumber:
						case UnicodeCategory.Format:
						case UnicodeCategory.ConnectorPunctuation:
						{
							if (i != 0)
							{
								break;
							}
							else
							{
								goto case UnicodeCategory.PrivateUse;
							}
						}
						case UnicodeCategory.EnclosingMark:
						case UnicodeCategory.OtherNumber:
						case UnicodeCategory.SpaceSeparator:
						case UnicodeCategory.LineSeparator:
						case UnicodeCategory.ParagraphSeparator:
						case UnicodeCategory.Control:
						case UnicodeCategory.Surrogate:
						case UnicodeCategory.PrivateUse:
						{
							charArray[i] = '\u005F';
							break;
						}
						default:
						{
							goto case UnicodeCategory.PrivateUse;
						}
					}
				}
			}
			return new string(charArray);
		}

		public static string GenerateIdentifierFromPath(ICodeDocumentType codeDocumentType, string fullPath)
		{
			string fileName;
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(fullPath))
			{
				fileName = Path.GetFileName(fullPath);
				if (fileName.Length > 1)
				{
					int num = fileName.IndexOf('.', 1);
					if (num > 0)
					{
						fileName = fileName.Substring(0, num);
					}
				}
			}
			else
			{
				fileName = (new DirectoryInfo(fullPath)).Name;
			}
			return Microsoft.Expression.Project.Build.CodeGenerator.MakeSafeIdentifier(codeDocumentType, fileName, false);
		}

		public static string GetApplicationNamespaceName(ICodeDocumentType codeDocumentType, IProject project)
		{
			string str = Microsoft.Expression.Project.Build.CodeGenerator.GenerateIdentifierFromPath(codeDocumentType, project.DocumentReference.Path);
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			return "Application";
		}

		public static string GetSceneClassName(ICodeDocumentType codeDocumentType, IDocumentItem documentItem)
		{
			return Microsoft.Expression.Project.Build.CodeGenerator.GenerateIdentifierFromPath(codeDocumentType, documentItem.DocumentReference.Path);
		}

		public static string MakeSafeIdentifier(ICodeDocumentType codeDocumentType, string identifier, bool escape)
		{
			if (codeDocumentType.Name != "Javascript")
			{
				char[] chrArray = new char[] { '\u005F' };
				identifier = Microsoft.Expression.Project.Build.CodeGenerator.FixUpIdentifier(identifier, chrArray);
			}
			else
			{
				char[] chrArray1 = new char[] { '\u005F', '$' };
				identifier = Microsoft.Expression.Project.Build.CodeGenerator.FixUpIdentifier(identifier, chrArray1);
			}
			if (escape)
			{
				ICodeGeneratorHost codeGeneratorHost = codeDocumentType as ICodeGeneratorHost;
				if (codeGeneratorHost != null)
				{
					identifier = codeGeneratorHost.CodeDomProvider.CreateEscapedIdentifier(identifier);
				}
			}
			if (identifier == "_" && codeDocumentType.Name == "VB")
			{
				return "__";
			}
			return identifier;
		}
	}
}