using System;
using System.Diagnostics;
using System.Windows;
using Sedentary.Framework;
using Sedentary.Model;
using Sedentary.Model.Persistence;

namespace Sedentary
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static WorkTracker _tracker;

		public App()
		{
			Trace.WriteLine(string.Empty);
			Tracer.Write("APPLICATION STARTED");
		}

		public static WorkTracker Tracker
		{
			get { return _tracker; }
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			_tracker = new WorkTracker();
			_tracker.Start();

			AppDomain.CurrentDomain.UnhandledException += TraceUnhandledException;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			StatsRepo.Save(_tracker.Statistics);
			_tracker.Dispose();

			Trace.Flush();
		}

		private void TraceUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception)
			{
				Tracer.WriteError("Unhandled exception", e.ExceptionObject as Exception);
			}
			else
			{
				Tracer.Write("Unhandled error:\r\n{0}", e.ExceptionObject);
			}			
		}
	}
}