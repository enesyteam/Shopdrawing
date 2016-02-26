using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public class ViewObjectBuilder<PlatformType> : IViewObjectBuilder
	where PlatformType : class
	{
		private ViewObjectBuilder<PlatformType>.ViewObjectCreate createDelegate;

		internal ViewObjectBuilder(ViewObjectBuilder<PlatformType>.ViewObjectCreate createDelegate)
		{
			this.createDelegate = createDelegate;
		}

		public IViewObject Create(object platformSpecificObject)
		{
			return this.createDelegate((PlatformType)(platformSpecificObject as PlatformType));
		}

		public Type GetBaseType()
		{
			return typeof(PlatformType);
		}

		public delegate IViewObject ViewObjectCreate(PlatformType platformSpecificObject);
	}
}