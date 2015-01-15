using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using Sedentary.Framework;
using Sedentary.Model;
using Sedentary.ViewModels;

namespace Sedentary.Views
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class ShellView : Window
	{
		private DispatcherTimer _timer;

		public ShellView()
		{
			//Tracer.WriteMethod();

			InitializeComponent();

			StartTimer();
		}

		public ShellViewModel ViewModel
		{
			get { return (ShellViewModel) DataContext; }
		}

		private void StartTimer()
		{
			_timer = new DispatcherTimer();
			_timer.Tick += Tick;
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.IsEnabled = true;
		}

		void Tick(object sender, EventArgs e)
		{
			if (this.DataContext != null)
			{
				((PropertyChangedBase) DataContext).Refresh();
			}
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			if (WindowState == WindowState.Minimized)
			{
				Hide();
			}
		}
	}
}