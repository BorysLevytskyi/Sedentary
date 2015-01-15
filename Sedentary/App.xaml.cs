using System;
using System.Diagnostics;
using System.Windows;
using Autofac;
using Caliburn.Micro;
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
		public App()
		{
			Trace.WriteLine(string.Empty);
			Tracer.Write("APPLICATION STARTED");
			this.InitializeComponent();
			Tracer.Write("Initialized");
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			AppDomain.CurrentDomain.UnhandledException += TraceUnhandledException;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			var stats = IoC.Get<Statistics>();
			StatsRepo.Save(stats);

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