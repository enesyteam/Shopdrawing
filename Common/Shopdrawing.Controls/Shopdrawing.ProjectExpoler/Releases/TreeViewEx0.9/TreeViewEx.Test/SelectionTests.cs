namespace TreeViewEx.Test.Model
{
	#region

	using System.Windows.Automation;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using TreeViewEx.Test.Model.Helper;
	using System.Threading;

	#endregion

    [TestClass]
    [DeploymentItem("SimpleSample.exe")]
    [DeploymentItem("TreeViewEx.dll")]
	public class SelectionTests
	{
		#region Constants and Fields

		private const string FileName = "SimpleSample.exe";

		private const string ProcessName = "W7StyleSample";

		#endregion

		#region Public Properties

		public TestContext TestContext { get; set; }

		#endregion

		#region Public Methods

		[TestMethod]
		public void SelectElement1()
		{
			using (TreeApplication app = new TreeApplication("SimpleSample"))
			{
				SimpleSampleTree sst = new SimpleSampleTree(app);
				sst.Element1.Select();
				Assert.IsTrue(sst.Element1.IsSelected);
			}
		}

		[TestMethod]
		public void SelectElement11()
		{
			using (TreeApplication app = new TreeApplication("SimpleSample"))
			{
				SimpleSampleTree sst = new SimpleSampleTree(app);
				sst.Element1.Expand();
				sst.Element11.Select();
				Assert.IsTrue(sst.Element11.IsSelected);
			}
		}

		[TestMethod]
		public void SelectElement11ByClickOnIt()
		{
			using (TreeApplication app = new TreeApplication("SimpleSample"))
			{
				SimpleSampleTree sst = new SimpleSampleTree(app);
				sst.Element1.Expand();
				sst.Element11.Select();
				Assert.IsTrue(sst.Element11.IsSelected);
			}
		}

		[TestMethod]
		public void DeSelectElementWithControlAndClick()
		{
			using (TreeApplication app = new TreeApplication("SimpleSample"))
			{
				SimpleSampleTree sst = new SimpleSampleTree(app);
				sst.Element1.Expand();
				Mouse.CtrlClick(sst.Element11);
				Thread.Sleep(100);
				Mouse.CtrlClick(sst.Element11);
				Assert.IsFalse(sst.Element11.IsSelected);
				// we cannot test for focus, because the automation peer sets focus by itself
				// Assert.IsFalse(sst.Element11.IsFocused);
			}
		}
		#endregion
	}
}