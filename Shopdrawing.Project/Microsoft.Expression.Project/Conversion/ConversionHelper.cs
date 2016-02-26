using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.Project.Conversion
{
	internal static class ConversionHelper
	{
		internal static IEnumerable<UpgradeAction> CheckAndAddFile(ConversionTarget file, IEnumerable<IProjectConverter> converters, IDictionary<ConversionType, ConversionType> versionMapping, bool ignoreEnabled)
		{
			foreach (IProjectConverter projectConverter in converters)
			{
				if (!ignoreEnabled && !projectConverter.IsEnabled)
				{
					continue;
				}
				ConversionType version = projectConverter.GetVersion(file);
				ConversionType item = versionMapping[version];
				if (version == item || item == ConversionType.DoNothing || item == ConversionType.Unsupported || item == ConversionType.Unknown)
				{
					continue;
				}
				UpgradeAction upgradeAction = new UpgradeAction()
				{
					TargetFile = file,
					InitialType = version,
					TargetType = item,
					Converter = projectConverter
				};
				yield return upgradeAction;
			}
		}
	}
}