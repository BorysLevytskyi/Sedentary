using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
		
		[STAThread]
		static void Main()
		{
			ApplicationStart.Start();
		}

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

			IoC.Get<ApplicationLifetimeService>().OnStart();

			AppDomain.CurrentDomain.UnhandledException += TraceUnhandledException;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
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

	public class SingleInstance : IDisposable
	{
		private Mutex _mutex;

		public bool IsFirstRun()
		{
			bool newOneCreated;
			_mutex = new Mutex(true, "SedenteryApplicationSingleInstanceKey", out newOneCreated);
			return newOneCreated;
		}

		public void Dispose()
		{
			if (_mutex != null)
			{
				_mutex.ReleaseMutex();
			}
		}
	}

	public class ApplicationStart
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		public static void Start()
		{
			using (var inst = new SingleInstance())
			{
				if (inst.IsFirstRun())
				{
					var app = new App();
					app.InitializeComponent();
					app.Run();
				}
				else
				{
					BringFrontExistingProcess();
				}
			}
		}

		private static void BringFrontExistingProcess()
		{
			Process current = Process.GetCurrentProcess();
			foreach (Process process in Process.GetProcessesByName(current.ProcessName))
			{
				if (process.Id != current.Id)
				{
					SetForegroundWindow(process.MainWindowHandle);
					break;
				}
			}
		}
	}
}