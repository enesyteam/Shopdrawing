namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IViewNodeManagerFactory
	{
		ViewNodeManager Create();

		IInstanceDictionary CreateInstanceDictionary(ViewNodeManager manager);
	}
}