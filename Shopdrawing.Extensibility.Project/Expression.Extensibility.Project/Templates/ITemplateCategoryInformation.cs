using System;
using System.Windows.Controls;

namespace Microsoft.Expression.Extensibility.Project.Templates
{
	public interface ITemplateCategoryInformation
	{
		string DisplayName
		{
			get;
		}

		string FullName
		{
			get;
		}

		System.Windows.Controls.Image Image
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}