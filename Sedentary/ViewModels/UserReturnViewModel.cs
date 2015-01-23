using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Sedentary.Framework;
using Sedentary.Model;

namespace Sedentary.ViewModels
{
	public class UserReturnViewModel : Screen
	{
		private readonly WorkPeriod _awayPeriod;
		private readonly CountdownTimer _countDown;
		private Window _window;
		private Rectangle _screenArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

		public UserReturnViewModel(WorkPeriod awayPeriod)
		{
			_awayPeriod = awayPeriod;

			_countDown = new CountdownTimer(TimeSpan.FromSeconds(60));
			_countDown.Tick += () => NotifyOfPropertyChange(() => SecondsLeft);
			_countDown.Done += WasSitting;
			_countDown.Start();
		}

		protected override void OnViewAttached(object view, object context)
		{
			base.OnViewAttached(view, context);

			if (view is Window)
			{
				_window = view as Window;
			}
		}

		public WorkPeriod AwayPeriod
		{
			get { return _awayPeriod; }
		}

		public int SecondsLeft
		{
			get { return (int) _countDown.TimeLeft.TotalSeconds; }
		}

		public System.Action SetSitting { get; set; }

		public System.Action SetStanding { get; set; }

		public void WasSitting()
		{
			_countDown.Stop();

			if (SetSitting != null)
			{
				SetSitting();
			}
			TryClose();
		}

		public void WasNotSitting()
		{
			_countDown.Stop();

			if (SetStanding != null)
			{
				SetStanding();
			}
			TryClose();
		}

		public double TopOffset
		{
			get { return _screenArea.Height - _window.ActualHeight; }
		}

		public double ScreenHeight
		{
			get { return _screenArea.Height; }
		}

		public double LeftOffset
		{
			get { return _screenArea.Width - _window.ActualWidth; }
		}

	}
}
