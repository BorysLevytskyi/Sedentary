using System;
using System.Windows;
using System.Windows.Threading;
using Sedentary.Framework;
using Sedentary.ViewModels;

namespace Sedentary.Views
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private DispatcherTimer _timer;

		public MainWindow()
		{
			Tracer.WriteMethod();

			InitializeComponent();
			DataContext = new MainWindowModel(App.Tracker.Statistics, App.Tracker.Analyzer);

			StartTimer();
		}

		public MainWindowModel Model
		{
			get { return (MainWindowModel) DataContext; }
		}

		private void StartTimer()
		{
			_timer = new DispatcherTimer();
			_timer.Tick += (s, e) => Model.Refresh();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.IsEnabled = true;
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			if (WindowState == WindowState.Minimized)
			{
				Hide();
			}
		}

		private void ClearStatistics(object sender, RoutedEventArgs e)
		{
			Model.ClearStatistics();
		}
	}
}