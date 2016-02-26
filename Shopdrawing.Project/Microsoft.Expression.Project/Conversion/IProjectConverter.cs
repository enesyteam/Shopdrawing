using System;

namespace Microsoft.Expression.Project.Conversion
{
	internal interface IProjectConverter
	{
		bool AllowDisabling
		{
			get;
		}

		string ConversionPrompt
		{
			get;
		}

		string Identifier
		{
			get;
		}

		bool IsEnabled
		{
			get;
			set;
		}

		bool Convert(ConversionTarget file, ConversionType initialState, ConversionType targetState);

		ConversionType GetVersion(ConversionTarget file);
	}
}