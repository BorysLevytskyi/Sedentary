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

		
		public IdleWatcher(TimeSpan idleThreshold)
		{
			_idleThreshold = idleThreshold;
			_handler = new UserActivityListener();
		}

		public TimeSpan IdleTime
		{
			get { return DateTime.Now.TimeOfDay - _lastInput; }
		}

		public event Action<TimeSpan> UserActive;

		public event Action IdleStarted;

		public event Action<TimeSpan> NoEventsOnTimeWindow;

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

			EventCountLogger.LogEvents(this);

			var idleTime = DateTime.Now.TimeOfDay - _lastInput;

			if (idleTime >= _idleThreshold)
			{
				Tracer.Write("Detected idle time. Last input is {0}", _lastInput.RoundToSeconds());
				OnIdleStarted();
			}
		}

		private void OnUserInput(object sender, EventArgs e)
		{
			EventCountLogger.Increment();

			TimeSpan period = IdleTime;
			_lastInput = DateTime.Now.TimeOfDay;
			_idleStarted = false;

			OnUserActive(period);
		}

		public void Dispose()
		{
			_handler.Dispose();
		}

		protected virtual void OnUserActive(TimeSpan period)
		{
			Action<TimeSpan> handler = UserActive;
			if (handler != null) handler(period);
		}

		private static class EventCountLogger
		{
			private static TimeSpan _currentEventsLoggingWindow = TimeSpan.Zero;
			private static readonly TimeSpan EventsCountLoggerWindowSize = TimeSpan.FromSeconds(60);
			private static int _eventsCountPerTimeWindow = 0;


			public static void Increment()
			{
				Interlocked.Increment(ref _eventsCountPerTimeWindow);
			}

			public static void LogEvents(IdleWatcher watcher)
			{
				var now = DateTime.Now.TimeOfDay;
				var currentTimeWindow = now.RoundTo(EventsCountLoggerWindowSize);

				if (currentTimeWindow > _currentEventsLoggingWindow)
				{
					Tracer.Write<IdleWatcher>("{0} events logged between {1} and {2}. Last input is {3}",
						_eventsCountPerTimeWindow,
						_currentEventsLoggingWindow,
						now.RoundToSeconds(),
						watcher._lastInput.RoundToSeconds());

					int count = Interlocked.Exchange(ref _eventsCountPerTimeWindow, 0);

					if (count == 0 && watcher.NoEventsOnTimeWindow != null)
					{
						watcher.NoEventsOnTimeWindow(currentTimeWindow);
					}

					_currentEventsLoggingWindow = currentTimeWindow;
				}
			}
		}
	}
}