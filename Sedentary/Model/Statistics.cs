using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

			Tracer.Write("----");
			Tracer.Write("Current time: {0}", DateTime.Now.TimeOfDay);
			Tracer.Write("Start time: {0}", startTime);
			Tracer.Write("Work State changed to: {0}", state);
			Tracer.Write("New period: {0}", newPeriod);
			Tracer.Write("Session Time being: {0}", this.CurrentPeriodLength);

			OnChanged();
		}

		public void RestoreState()
		{
			if (_prevPeriod != null)
			{
				SetState(_prevPeriod.State, DateTime.Now.TimeOfDay);
			}
		}
	}
}