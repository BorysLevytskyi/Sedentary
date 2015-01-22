using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Sedentary.Model;
using Screen = System.Windows.Forms.Screen;

namespace Sedentary.ViewModels
{
	public class ShellViewModel : PropertyChangedBase
	{
		private readonly Analyzer _analyzer;
		private readonly Statistics _stats;

		public ShellViewModel(Statistics stats, Analyzer analyzer, WorkTracker tracker)
		{
			_stats = stats;
			_analyzer = analyzer;
			_stats.Changed += Refresh;

			tracker.UserReturn += OnUserReturned;
		}

		public PeriodsChartViewModel Chart { get; set; }

		public bool IsSitting
		{
			get { return _stats.IsSitting; }
		}

		public string TotalSittingTime
		{
			get { return _stats.TotalSittingTime.ToString(@"h\h\ m\m"); }
		}

		public WorkPeriod CurrentPeriod
		{
			get { return _stats.CurrentPeriod; }
		}

		public List<WorkPeriod> Periods
		{
			get { return _stats.Periods.ToList(); }
		}

		public bool IsSittingLimitExceeded
		{
			get { return _analyzer.IsSittingLimitExceeded; }
		}

		public double PressureRatio
		{
			get { return _analyzer.GetPressureRatio(); }
		}

		public void ClearStatistics()
		{
			_stats.Clear();
		}

		public void OnUserReturned(WorkPeriod awayPeriod)
		{
			var screen = Screen.PrimaryScreen;

			IoC.Get<IWindowManager>().ShowWindow(
				new UserReturnViewModel(awayPeriod)
				{
					SetSitting = () => _stats.ChangePeriodState(awayPeriod, WorkState.Sitting),
					SetStanding = () => _stats.ChangePeriodState(awayPeriod, WorkState.Standing),
				}, null,
				new Dictionary<string, object>
					{
						{ "Top", screen.WorkingArea.Height - 300 },
						{ "Left", screen.WorkingArea.Width - 600},
						{ "Title", "Welcome back"}
					});
		}
	}
}