using System;
using System.Collections.Generic;
using System.Linq;

namespace SittingTracker.Model
{
	public class Statistics
	{
		public Statistics()
		{
			_periods = new List<WorkPeriod>();
			StartPeriod(new WorkPeriod(WorkState.Sitting, DateTime.Now.TimeOfDay));
		}

		public bool IsUserAway
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
			get { return _currentPeriod.State == WorkState.Sitting && _currentPeriod.Length >= SittingLimit; }
		}

		public TimeSpan SessionTime
		{
			get { return _currentPeriod.Length; }
		}

		public IEnumerable<WorkPeriod> Periods
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
			OnChanged();
		}

		private void StartPeriod(WorkPeriod period)
		{
			if (_currentPeriod != null)
			{
				_currentPeriod.End();
			}

			_periods.Add(_currentPeriod = period);
		}

		private readonly TimeSpan SittingLimit = TimeSpan.FromMinutes(60);
		private readonly List<WorkPeriod> _periods;
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