using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IPreviewControl
	{
		uint? ChangeStampWhenInstantiated
		{
			get;
		}

		string SourcePath
		{
			get;
		}
	}
}