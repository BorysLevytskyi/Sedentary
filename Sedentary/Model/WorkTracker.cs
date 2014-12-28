using System;
using System.Windows.Threading;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class WorkTracker : IDisposable
	{
		public static readonly TimeSpan IdleThreshold = TimeSpan.FromSeconds(60);
		public static readonly TimeSpan MaxSittingTime = TimeSpan.FromMinutes(60);
		public static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(5);

		private IdleWatcher _idleWatcher;
		private WorkState _lastState;
		private Statistics _stats;
		private DispatcherTimer _timer;

		private TrayIcon _tray;
		private bool _wasExceeded;

		public DispatcherTimer Timer
		{
			get { return _timer; }
		}

		public Statistics Statistics
		{
			get { return _stats; }
		}

		public void Dispose()
		{
			_idleWatcher.Dispose();
			_tray.Dispose();
		}

		public void Start()
		{
			Tracer.Write("Started");

			_stats = new Statistics();
			_tray = new TrayIcon(_stats);
			_idleWatcher = new IdleWatcher();
			_timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};

			_tray.Init();

			_idleWatcher.StartDetection();
			_idleWatcher.UserActive += OnUserInput;

			_tray.OnPositionSwitch += OnPositionSwitch;

			_timer.Start();
			_timer.Tick += OnTimerOnTick;
		}

		private void OnUserInput(TimeSpan idleTime)
		{
			if (_stats.CurrentPeriod.State == WorkState.Away)
			{
				RestoreLastState();
			}
		}

		private void RestoreLastState()
		{
			SetState(_lastState);
		}

		private void OnPositionSwitch()
		{
			SetState(_stats.IsSitting ? WorkState.Standing : WorkState.Sitting);
		}

		private void SetState(WorkState workState)
		{
			_lastState = _stats.CurrentPeriod.State;
			_stats.SetState(workState);
			_tray.Refresh();
		}

		private void OnTimerOnTick(object s, EventArgs o)
		{
			if (_stats.IsSitting)
			{
				if (!_wasExceeded && _stats.IsSittingLimitExceeded)
				{
					OnSittingLimitExceeded();
					_wasExceeded = true;
				}
			}

			if (_stats.IsAway && _idleWatcher.IdleTime >= IdleThreshold)
			{
				Tracer.Write("Detected idle time");
				SetState(WorkState.Away);
			}

			_tray.Refresh();
		}

		private void OnSittingLimitExceeded()
		{
			Tracer.Write("Sitting limit exceeded");
			_tray.ShowWarning();
		}
	}
}