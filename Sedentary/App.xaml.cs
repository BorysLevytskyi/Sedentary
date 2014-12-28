using System.Windows;
using Sedentary.Framework;
using Sedentary.Model;

namespace Sedentary
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static WorkTracker _tracker;

		public static WorkTracker Tracker
		{
			get { return _tracker; }
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			Tracer.WriteMethod();

			base.OnStartup(e);

			_tracker = new WorkTracker();
			_tracker.Start();
		}

	    protected override void OnExit(ExitEventArgs e)
	    {
	        base.OnExit(e);

            _tracker.Dispose();
	    }
	}
}
