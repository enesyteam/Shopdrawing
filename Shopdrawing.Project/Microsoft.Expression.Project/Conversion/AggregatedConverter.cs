using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Conversion
{
	internal abstract class AggregatedConverter : ProjectConverterBase
	{
		protected abstract IList<IProjectConverter> Converters
		{
			get;
		}

		protected abstract bool IgnoreEnabled
		{
			get;
		}

		protected abstract IDictionary<ConversionType, ConversionType> VersionMapping
		{
			get;
		}

		public AggregatedConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		protected abstract IEnumerable<ConversionTarget> GetConversionTargets();

		protected override bool InternalConvert(ConversionTarget file, ConversionType initialState, ConversionType targetState)
		{
			IEnumerable<UpgradeAction> conversionTargets = 
				from target in this.GetConversionTargets()
				from upgradeAction in ConversionHelper.CheckAndAddFile(target, this.Converters, this.VersionMapping, this.IgnoreEnabled)
				select upgradeAction;
			bool flag = true;
			foreach (UpgradeAction conversionTarget in conversionTargets)
			{
				if (conversionTarget.Converter.Convert(conversionTarget.TargetFile, conversionTarget.InitialType, conversionTarget.TargetType))
				{
					continue;
				}
				flag = false;
			}
			return flag;
		}
	}
}