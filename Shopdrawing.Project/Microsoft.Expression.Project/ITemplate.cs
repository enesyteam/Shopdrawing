using System;
using System.Windows;

namespace Microsoft.Expression.Project
{
	public interface ITemplate
	{
		bool BuildOnLoad
		{
			get;
		}

		string DefaultName
		{
			get;
		}

		string Description
		{
			get;
		}

		string DisplayName
		{
			get;
		}

		bool Hidden
		{
			get;
		}

		FrameworkElement Icon
		{
			get;
		}

		string Identifier
		{
			get;
		}

		string MaximumFrameworkVersion
		{
			get;
		}

		string MinimumFrameworkVersion
		{
			get;
		}

		int NumberOfParentCategoriesToRollUp
		{
			get;
		}

		string ProjectSubType
		{
			get;
		}

		string ProjectSubTypes
		{
			get;
		}

		string ProjectType
		{
			get;
		}

		bool ProvideDefaultName
		{
			get;
		}

		int SortOrder
		{
			get;
		}

		string TemplateGroupID
		{
			get;
		}

		string TemplateID
		{
			get;
		}
	}
}