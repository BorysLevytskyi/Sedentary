﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class Statistics
	{
		private readonly List<WorkPeriod> _periods;
		private WorkPeriod _currentPeriod;
		private WorkPeriod _prevPeriod;

		public Statistics(IList<WorkPeriod> periods)
		{
			_periods = new List<WorkPeriod>(periods);

			_currentPeriod = periods.FirstOrDefault(p => !p.IsCompleted) ?? WorkPeriod.Start(WorkState.Sitting);

			if (!_periods.Any())
			{
				_periods.Add(_currentPeriod);
			}
		}

		public Statistics()
		{
			_periods = new List<WorkPeriod>();

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

		public TimeSpan CurrentPeriodLength
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

			MergePeriods();
			OnChanged();
		}

		public void ChangePeriodState(WorkPeriod period, WorkState state)
		{
			var index = _periods.IndexOf(period);

			if (index == -1)
			{
				Tracer.Write("Incorrect index of the period {0}", period);
				return;
			}

			_periods[index] = period.SetState(state);

			MergePeriods();
			OnChanged();
		}

	    private void MergePeriods()
	    {
		    var newPeriods = _periods.SplitBySequences((p1, p2) => p1.State == p2.State).Reduce(WorkPeriod.Merge).ToList();
	        _periods.Clear();

		    foreach (var p in newPeriods)
		    {
			   _periods.Add(p);
		    }

		    _currentPeriod = _periods.Single(p => !p.IsCompleted);
	    }

		public void Clear()
		{
			_periods.Clear();
			_prevPeriod = null;
			SetState(WorkState.Sitting);
		}
	}
}