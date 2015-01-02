using System;
using System.Windows.Threading;
using Sedentary.Framework;
using Sedentary.Model.Persistence;

namespace Sedentary.Model
{
	public class WorkTracker : IDisposable
	{
		public readonly Requirements Requirements = new Requirements
		{
//			AwayThreshold = TimeSpan.FromSeconds(60),
//			MaxSittingTime = TimeSpan.FromSeconds(600),
//			RequiredRestingTime = TimeSpan.FromSeconds(30)

			AwayThreshold = TimeSpan.FromSeconds(60),
//			AwayThreshold = TimeSpan.FromSeconds(10),
			MaxSittingTime = TimeSpan.FromHours(1),
			RequiredRestingTime = TimeSpan.FromMinutes(5)
		};

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

			_stats = StatsRepo.Get();
			_analyzer = new Analyzer(_stats, Requirements);
			_tray = new TrayIcon(_stats, _analyzer);
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

			if (!_stats.IsAway && _idleWatcher.IdleTime >= Requirements.AwayThreshold)
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