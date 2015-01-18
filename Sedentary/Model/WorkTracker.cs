using System;
using System.Windows.Threading;
using Sedentary.Framework;
using Sedentary.Properties;

namespace Sedentary.Model
{
	public class WorkTracker
	{
		private readonly IPhysicalStateAnalyzer _analyzer;
		private readonly IdleWatcher _idleWatcher;
		private readonly Statistics _stats;

		private readonly ITrayIcon _tray;
		private DispatcherTimer _timer;
		private bool _wasExceeded;

		public WorkTracker(AppRequirements requirements, Statistics stats, IPhysicalStateAnalyzer analyzer, IdleWatcher idleWatcher,
			ITrayIcon tray)
		{
			_stats = stats;
			_analyzer = analyzer;
			_idleWatcher = idleWatcher;
			_tray = tray;
		}

		public DispatcherTimer Timer
		{
			get { return _timer; }
		}

	
		public event Action OnUserAway
		{
			add { _idleWatcher.IdleStarted += value; }
			remove { _idleWatcher.IdleStarted -= value; }
		}

		public event Action<TimeSpan> NoEventsOnTimeWindow
		{
			add { _idleWatcher.NoEventsOnTimeWindow += value; }
			remove { _idleWatcher.NoEventsOnTimeWindow -= value; }
		}

		public void Start()
		{
			Tracer.Write("Started");

			_timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};

			_tray.Init();

			_idleWatcher.Start();
			_idleWatcher.UserActive += OnUserActive;
			_idleWatcher.IdleStarted += OnIdleStarted;

			_timer.Start();
			_timer.Tick += OnTimerOnTick;
		}

		private void OnIdleStarted()
		{
			SetState(WorkState.Away);
		}

		private void OnUserActive(TimeSpan idleTime)
		{
			if (_stats.CurrentState == WorkState.Away)
			{
				Tracer.Write("User returned. Idle time was: {0}", idleTime);
				RestoreLastState();
			}
		}

		private void RestoreLastState()
		{
			SetState(_stats.PreviousPeriod.State);
		}

		private void OnPositionSwitch()
		{
			SetState(_stats.IsSitting ? WorkState.Standing : WorkState.Sitting);
		}

		private void SetState(WorkState workState)
		{
			if (workState == _stats.CurrentState)
			{
				return;
			}

			TimeSpan startTime = DateTime.Now.TimeOfDay;

			if (workState == WorkState.Away)
			{
				startTime = DateTime.Now.TimeOfDay.Subtract(Requirements.AwayThreshold);
			}

			_stats.SetState(workState, startTime);

			_tray.Refresh();
		}

		private void OnTimerOnTick(object s, EventArgs o)
		{
			if (_stats.IsSitting)
			{
				if (!_wasExceeded && _analyzer.IsSittingLimitExceeded)
				{
					OnSittingLimitExceeded();
					_wasExceeded = true;
				}
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