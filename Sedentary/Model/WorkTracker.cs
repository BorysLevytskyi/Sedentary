using System;
using System.Windows.Threading;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class WorkTracker
	{
		private readonly Analyzer _analyzer;
		private readonly IdleWatcher _idleWatcher;
		private readonly Requirements _requirements;
		private readonly Statistics _stats;

		private readonly TrayIcon _tray;
		private DispatcherTimer _timer;
		private bool _wasExceeded;

		public WorkTracker(Requirements requirements, Statistics stats, Analyzer analyzer, IdleWatcher idleWatcher,
			TrayIcon tray)
		{
			_requirements = requirements;
			_stats = stats;
			_analyzer = analyzer;
			_idleWatcher = idleWatcher;
			_tray = tray;
		}

		public DispatcherTimer Timer
		{
			get { return _timer; }
		}

		public Requirements Requirements
		{
			get { return _requirements; }
		}

		public Statistics Statistics
		{
			get { return _stats; }
		}

		public Analyzer Analyzer
		{
			get { return _analyzer; }
		}

		public event Action OnUserAway
		{
			add { _idleWatcher.IdleStarted += value; }
			remove { _idleWatcher.IdleStarted -= value; }
		}

		public void Start()
		{
			Tracer.Write("Started");

			_timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};

			_tray.Init();

			_idleWatcher.Start();
			_idleWatcher.UserActive += OnUserActive;
			_idleWatcher.IdleStarted += OnIdleStarted;

			_tray.OnPositionSwitch += OnPositionSwitch;

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