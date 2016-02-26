namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewHitTestResult : IViewObject
	{
		IViewObject HitObject
		{
			get;
		}
	}
}