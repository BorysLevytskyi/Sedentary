using System;
using System.Diagnostics;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class WorkPeriod
	{
		private readonly WorkState _state;
		private readonly TimeSpan _startTime;
		private TimeSpan _endTime;

		public WorkPeriod()
		{}

		public WorkPeriod(WorkState state, TimeSpan startTime) : this(state, startTime, TimeSpan.Zero)
		{}

		public WorkPeriod(WorkState state, TimeSpan startTime, TimeSpan endTime)
		{
			_state = state;
			_startTime = startTime;
			_endTime = endTime;
		}

		public WorkState State
		{
			get { return _state; }
		}

		public TimeSpan StartTime
		{
			get { return _startTime; }
		}

		public TimeSpan EndTime
		{
			get { return _endTime; }
		}

		public TimeSpan Length
		{
			get
			{
				return IsCompleted 
					? _endTime - _startTime 
					: DateTime.Now.TimeOfDay - _startTime;
			}
		}

		public bool IsCompleted
		{
			get { return _endTime > TimeSpan.Zero; }
		}

		public void End()
		{
			Debug.Assert(!IsCompleted, "Trying to end already completed period");
			Tracer.Write("{0} period ended", this);

			_endTime = DateTime.Now.TimeOfDay;
		}

		public override string ToString()
		{
			return string.Format(@"{0}: {1:hh\:mm\:ss} - {2:hh\:mm\:ss}", _state, _startTime,
				IsCompleted ? _endTime : DateTime.Now.TimeOfDay);
		}

	    public static WorkPeriod Start(WorkState state)
	    {
	        return new WorkPeriod(state, DateTime.Now.TimeOfDay);
	    }
	}
}