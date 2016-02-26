using System;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	public interface ICodeDocumentType : IDocumentType
	{
		string ProjectFileExtension
		{
			get;
		}

		ImageSource ProjectIcon
		{
			get;
		}

		string PropertiesPath
		{
			get;
		}
	}
}