using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class Statistics
	{
		public Statistics(IList<WorkPeriod> periods)
		{
			_periods = new ObservableCollection<WorkPeriod>(periods);

			_currentPeriod = periods.FirstOrDefault(p => !p.IsCompleted) ?? WorkPeriod.Start(WorkState.Sitting);

			if (!_periods.Any())
			{
				_periods.Add(_currentPeriod);
			}
		}

		public Statistics()
		{
			_periods = new ObservableCollection<WorkPeriod>();

			StartPeriod(new WorkPeriod(WorkState.Sitting, DateTime.Now.TimeOfDay));
		}

		public bool IsAway
		{
			get { return _currentPeriod.State == WorkState.Away; }
		}

		public bool IsSitting
		{
			get { return _currentPeriod.State == WorkState.Sitting; }
		}

		public WorkPeriod CurrentPeriod
		{
			get { return _currentPeriod; }
		}

		public TimeSpan TotalSittingTime
		{
			get { return _periods.Where(p => p.State == WorkState.Sitting).Select(p => p.Length).Aggregate((x, y) => x + y); }
		}

		// TODO: Move to notification controller
		public bool IsSittingLimitExceeded
		{
			get { return _currentPeriod.State == WorkState.Sitting && _currentPeriod.Length >= WorkTracker.MaxSittingTime; }
		}

		public double SittingTimeCompletionRate
		{
			get { return IsSitting ? Math.Round((double)_currentPeriod.Length.Ticks / WorkTracker.MaxSittingTime.Ticks, 2) : 0; }
		}

		// TODO: Implement cooldown minutes per sitting minutes
		public double CoolDownRate
		{
			get
			{
				double rate = 0;
				
				if (_prevPeriod.State == WorkState.Sitting)
				{
					rate = Math.Min(1, (double)_currentPeriod.Length.Ticks / (double)WorkTracker.Cooldown.Ticks);
				}

				Tracer.WritePropertyValue(rate);
				return rate;
			}
		}

		public TimeSpan SessionTime
		{
			get { return _currentPeriod.Length; }
		}

		public ObservableCollection<WorkPeriod> Periods
		{
			get { return _periods; }
		}

		public event Action Changed;

		protected virtual void OnChanged()
		{
			Action handler = Changed;
			if (handler != null) handler();
		}

		public void SetState(WorkState state)
		{
			StartPeriod(new WorkPeriod(state, DateTime.Now.TimeOfDay));
			Tracer.Write("Work State changed to: {0}", state);
			OnChanged();
		}

		private void StartPeriod(WorkPeriod period)
		{
			if (_currentPeriod != null)
			{
				_currentPeriod.End();
			}

			_prevPeriod = _currentPeriod;
			_periods.Add(_currentPeriod = period);
		}

		private readonly ObservableCollection<WorkPeriod> _periods;
		private WorkPeriod _currentPeriod;
		private WorkPeriod _prevPeriod;

		public void RestoreState()
		{
			if (_prevPeriod != null)
			{
				StartPeriod(_prevPeriod);
			}
		}
	}

	public enum WorkState
	{
		Sitting,
		Standing,
		Away
	}
}