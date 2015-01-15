using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class Statistics
	{
		private readonly ObservableCollection<WorkPeriod> _periods;
		private WorkPeriod _currentPeriod;
		private WorkPeriod _prevPeriod;

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

			SetState(WorkState.Sitting);
		}

		public bool IsAway
		{
			get { return _currentPeriod.State == WorkState.Away; }
		}

		public bool IsSitting
		{
			get { return _currentPeriod.State == WorkState.Sitting; }
		}

		public WorkState CurrentState
		{
			get { return _currentPeriod.State; }
		}

		public WorkPeriod CurrentPeriod
		{
			get { return _currentPeriod; }
		}

		public WorkPeriod PreviousPeriod
		{
			get { return _prevPeriod; }
		}

		public TimeSpan TotalSittingTime
		{
			get { return _periods.Where(p => p.State == WorkState.Sitting).Select(p => p.Length).Aggregate((x, y) => x + y); }
		}

		public TimeSpan CurrentPeriodLength
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
			SetState(state, DateTime.Now.TimeOfDay);
		}

		public void SetState(WorkState state, TimeSpan startTime)
		{
			var newPeriod = new WorkPeriod(state, startTime);

			if (_currentPeriod != null)
			{
				_currentPeriod.End(newPeriod.StartTime);
			}

			_prevPeriod = _currentPeriod;
			_periods.Add(_currentPeriod = newPeriod);

			OnChanged();
		}

		public void ChangePeriodState(WorkPeriod period, WorkState state)
		{
			var index = _periods.IndexOf(period);
			_periods[index] = period.SetState(state);
			OnChanged();
		}

	    private void MergePeriods()
	    {
	        // var groups = IEnumerable<IList<WorkPeriod>>();
	    }

		public void Clear()
		{
			_periods.Clear();
			_prevPeriod = null;
			SetState(WorkState.Sitting);
		}
	}
}