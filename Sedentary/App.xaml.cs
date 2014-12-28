using System.Windows;
using SittingTracker.Framework;
using SittingTracker.Model;

namespace SittingTracker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static WorkController _controller;

		public static WorkController Controller
		{
			get { return _controller; }
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			_controller = new WorkController();
			_controller.Start();
		}

	    protected override void OnExit(ExitEventArgs e)
	    {
	        base.OnExit(e);

            _controller.Dispose();
			Tracer.Stop();
	    }
	}
}
