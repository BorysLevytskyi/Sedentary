using System;
using System.Configuration;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class IdleWatcher
	{
		private readonly TimeSpan _idleThreshold;
		private readonly UserActivityListener _handler;
		private TimeSpan _lastInput;
		private DispatcherTimer _timer;
		private bool _idleStarted;

		private TimeSpan traceTimeWindow = TimeSpan.Zero;
		private int eventsCount = 0;

		public IdleWatcher(TimeSpan idleThreshold)
		{
			_idleThreshold = idleThreshold;
			_handler = new UserActivityListener();
		}

		public TimeSpan IdleTime
		{
			get { return DateTime.Now.TimeOfDay - _lastInput; }
		}

		public event Action<TimeSpan> UserInput;

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
			_timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
			_timer.Tick += OnTimerTick;
			_timer.Start();

			_handler.UserActive += OnUserInput;
			_handler.Start();
		}

		public void OnTimerTick(object sender, EventArgs e)
		{
			if (_idleStarted)
			{
				return;
			}

			LogEventsCount();

			var idleTime = DateTime.Now.TimeOfDay - _lastInput;

			if (idleTime >= _idleThreshold)
			{
				OnIdleStarted();
			}
		}

		private void OnUserInput(object sender, EventArgs e)
		{
			TimeSpan period = IdleTime;
			_lastInput = DateTime.Now.TimeOfDay;
			_idleStarted = false;

			eventsCount++;

			OnUserActive(period);
		}

		private void LogEventsCount()
		{
			var now = DateTime.Now.TimeOfDay;
			TimeSpan window = TimeSpan.FromSeconds(30);
			var currentWindow = now.Subtract(TimeSpan.FromTicks(now.Ticks%window.Ticks));

			lock (_sync) // TODO: Maybe there is no need to lock
			{
				if (currentWindow > traceTimeWindow)
				{
					Tracer.Write("{0} events logged between {1} and {2}", eventsCount, traceTimeWindow, now);
					traceTimeWindow = currentWindow;
				}
			}
		}

		public void Dispose()
		{
			_handler.Dispose();
		}

		protected virtual void OnUserActive(TimeSpan period)
		{
			Action<TimeSpan> handler = UserInput;
			if (handler != null) handler(period);
		}

		private readonly object _sync = new object();
	}
}