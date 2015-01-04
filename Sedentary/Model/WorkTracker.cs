using System;
using System.Windows.Threading;
using Sedentary.Framework;
using Sedentary.Model.Persistence;

namespace Sedentary.Model
{
	public class WorkTracker : IDisposable
	{
		private Requirements _requirements;
		private IdleWatcher _idleWatcher;
		private Statistics _stats;
		private DispatcherTimer _timer;
		private Analyzer _analyzer;

		private TrayIcon _tray;
		private bool _wasExceeded;

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

		public void Dispose()
		{
			_idleWatcher.Dispose();
			_tray.Dispose();
		}

		public void Start()
		{
			Tracer.Write("Started");

			_requirements = Requirements.Create();

			_stats = StatsRepo.Get();
			_analyzer = new Analyzer(_stats, Requirements);
			_tray = new TrayIcon(_stats, _analyzer);
			_idleWatcher = new IdleWatcher(Requirements.AwayThreshold);
			_timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};

			_tray.Init();

			_idleWatcher.Start();
			_idleWatcher.UserInput += OnUserInput;
			_idleWatcher.IdleStarted += OnIdleStarted;

			_tray.OnPositionSwitch += OnPositionSwitch;

			_timer.Start();
			_timer.Tick += OnTimerOnTick;
		}

		void OnIdleStarted()
		{
			SetState(WorkState.Away);
		}

		private void OnUserInput(TimeSpan idleTime)
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