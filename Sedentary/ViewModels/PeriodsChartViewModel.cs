using System;
using System.Linq;
using System.Windows.Threading;
using Caliburn.Micro;
using Sedentary.Framework;
using Sedentary.Model;

namespace Sedentary.ViewModels
{
	public class PeriodsChartViewModel : PropertyChangedBase
	{
		private readonly Statistics _stats;
		private TimeSpan _timeScale;
		private readonly BindableCollection<PeriodModel> _workPeriods;
		private readonly DispatcherTimer _timer;

		public PeriodsChartViewModel(Statistics stats)
		{
			_stats = stats;
			_stats.Changed += OnStatsChanged;
			_workPeriods = new BindableCollection<PeriodModel>(stats.Periods.Select(p => new PeriodModel(p)));
			_workPeriods.CollectionChanged += (s,e) => UpdateTimeScale();
			_timer = TimerFactory.StartTimer(TimeSpan.FromSeconds(1), OnTick);

			UpdateTimeScale();
		}

		private void OnTick()
		{
			Execute.OnUIThread(() => WorkPeriods.Last().Update());
		}

		private void OnStatsChanged()
		{
			_workPeriods.Clear();
			_workPeriods.AddRange(_stats.Periods.Select(p => new PeriodModel(p)));
		}

		public TimeSpan TimeScale
		{
			get { return _timeScale; }
			set
			{
				if (value.Equals(_timeScale)) return;
				_timeScale = value;
				NotifyOfPropertyChange(() => TimeScale);
			}
		}

		public BindableCollection<PeriodModel> WorkPeriods
		{
			get { return _workPeriods; }
		}

		public void SetSittingState(PeriodModel model)
		{
			var period = model.Period;
			if (period.State != WorkState.Sitting)
			{
				_stats.ChangePeriodState(period, WorkState.Sitting);
			}
		}

		public void SetStandingState(PeriodModel model)
		{
			var period = model.Period;
			if (period.State != WorkState.Standing)
			{
				_stats.ChangePeriodState(period, WorkState.Standing);
			}
		}

		private void UpdateTimeScale()
		{
			const int chunkSizeInSeconds = 30*60; // 30 min
			var periodsTotalInSeconds = (int) WorkPeriods.Sum(p => p.Period.Length.TotalSeconds);

			var chunksCount = (int) Math.Ceiling(periodsTotalInSeconds/(double) chunkSizeInSeconds);

			TimeScale = TimeSpan.FromSeconds(Math.Max(chunkSizeInSeconds, chunksCount*chunkSizeInSeconds));
		}
	}

	public class PeriodModel: PropertyChangedBase
	{
		private readonly WorkPeriod _period;

		public PeriodModel(WorkPeriod period)
		{
			_period = period;
		}

		public WorkState State
		{
			get { return Period.State; }
		}

		public TimeSpan Length
		{
			get { return Period.Length; }
		}

		public WorkPeriod Period { get { return _period; } }

		public void Update()
		{
			NotifyOfPropertyChange(() => Period.Length);
			NotifyOfPropertyChange(() => Period.StartTime);
			NotifyOfPropertyChange(() => Period.EndTime);
			NotifyOfPropertyChange(() => Period.State);
		}
	}
}