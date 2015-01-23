using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Sedentary.Framework;
using Sedentary.Model;

namespace Sedentary.ViewModels
{
	public class UserReturnViewModel : Screen
	{
		private readonly WorkPeriod _awayPeriod;
		private readonly CountdownTimer _countDown;

		public UserReturnViewModel(WorkPeriod awayPeriod)
		{
			_awayPeriod = awayPeriod;

			_countDown = new CountdownTimer(TimeSpan.FromSeconds(60));
			_countDown.Tick += () => NotifyOfPropertyChange(() => SecondsLeft);
			_countDown.Done += WasSitting;
			_countDown.Start();
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

	}
}
