using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Caliburn.Micro;
using Sedentary.Model;

namespace Sedentary.ViewModels
{
	public class PeriodsChartViewModel : PropertyChangedBase
	{
		private readonly Statistics _stats;
		private TimeSpan _timeScale;

		public PeriodsChartViewModel(Statistics stats)
		{
			_stats = stats;
			_stats.Periods.CollectionChanged += OnPeriodsChanged;
			UpdateTimeScale();
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

		public ObservableCollection<WorkPeriod> WorkPeriods
		{
			get { return _stats.Periods; }
		}

		public void SetSittingState(WorkPeriod period)
		{
			if (period.State != WorkState.Sitting)
			{
				_stats.ChangePeriodState(period, WorkState.Sitting);
			}
		}

		public void SetStandingState(WorkPeriod period)
		{
			if (period.State != WorkState.Standing)
			{
				_stats.ChangePeriodState(period, WorkState.Standing);
			}
		}

		private void OnPeriodsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateTimeScale();
		}

		private void UpdateTimeScale()
		{
			const int chunkSizeInSeconds = 30*60; // 30 min
			var periodsTotalInSeconds = (int) WorkPeriods.Sum(p => p.Length.TotalSeconds);

			var chunksCount = (int) Math.Ceiling(periodsTotalInSeconds/(double) chunkSizeInSeconds);

			TimeScale = TimeSpan.FromSeconds(Math.Max(chunkSizeInSeconds, chunksCount*chunkSizeInSeconds));
		}
	}
}