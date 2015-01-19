using System;
using System.Threading;
using System.Windows.Threading;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class IdleWatcher
	{
		private readonly TimeSpan _idleThreshold;
		private TimeSpan _lastInput;
		private DispatcherTimer _timer;
		private bool _idleStarted;

		
		public IdleWatcher(Requirements requirements)
		{
			_idleThreshold = requirements.AwayThreshold;
		}

		public TimeSpan IdleTime
		{
			get { return DateTime.Now.TimeOfDay - _lastInput; }
		}

		public event Action<TimeSpan> UserActive;

		public event Action IdleStarted;

		protected virtual void OnIdleStarted()
		{
			_idleStarted = true;
			Action handler = IdleStarted;
			if (handler != null) handler();
		}

		public void Start()
		{
			_lastInput = DateTime.Now.TimeOfDay;
			_timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			_timer.Tick += OnTimerTick;
			_timer.Start();
		}

		public void OnTimerTick(object sender, EventArgs e)
		{
		    var idleTime = SystemInfo.IdleTime;

		    if (_idleStarted)
			{
			    if (idleTime <= _idleThreshold)
			    {
			        FireIdleEnded(idleTime);
			        _idleStarted = false;
			    }

				return;
			}

			if (idleTime >= _idleThreshold)
			{
				Tracer.Write("Detected idle time. Last input is {0}", idleTime);
				OnIdleStarted();
			}
		}

		protected virtual void FireIdleEnded(TimeSpan period)
		{
			Action<TimeSpan> handler = UserActive;
			if (handler != null) handler(period);
		}
	}
}