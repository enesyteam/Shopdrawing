namespace W7StyleSample
{
	#region

   using System.Windows.Interop;

   using W7StyleSample.Model;
	using System.Diagnostics;
	using System.Windows;
	using System;
	using System.Windows.Threading;
	using System.Windows.Controls;

	#endregion

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		#region Constructors and Destructors
		Node firstNode;
		public MainWindow()
		{
			InitializeComponent();

			firstNode = new Node { Name = "element" };
			for (int i = 0; i < 2000; i++)
			{
				var node = new Node { Name = string.Format("element{0}", i) };
				node.Children.Add(new Node { Name = string.Format("element / {0}", i) });
				firstNode.Children.Add(node);
			}
		}

		#endregion

	   public static readonly DependencyProperty TimeToLoadProperty = DependencyProperty.Register(
	      "TimeToLoad", typeof(long), typeof(MainWindow), new PropertyMetadata(default(long)));

      public long TimeToLoad
	   {
	      get
	      {
            return (long)GetValue(TimeToLoadProperty);
	      }
	      set
	      {
	         SetValue(TimeToLoadProperty, value);
	      }
	   }

	   private Stopwatch sw;
		private void OnLoad(object sender, RoutedEventArgs e)
		{
		   sw = new Stopwatch();
		   sw.Start();
			DataContext = firstNode;
         ComponentDispatcher.ThreadIdle += new EventHandler(ComponentDispatcher_ThreadIdle);
		}

      void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
      {
         ComponentDispatcher.ThreadIdle -= new EventHandler(ComponentDispatcher_ThreadIdle);
         sw.Stop();
         TimeToLoad = sw.ElapsedMilliseconds;
      }

		private void OnClear(object sender, RoutedEventArgs e)
		{
			DataContext = null;
		   TimeToLoad = 0;
		}
	}
}