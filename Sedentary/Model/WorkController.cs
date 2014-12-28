using System;
using System.Windows.Threading;
using SittingTracker.Framework;

namespace SittingTracker.Model
{
	public class WorkController : IDisposable
	{
		private IdleWatcher _idleWatcher;
		private Statistics _stats;
		private DispatcherTimer _timer;

		private TrayIcon _tray;

		public DispatcherTimer Timer
		{
			get { return _timer; }
		}

		public Statistics Statistics
		{
			get { return _stats; }
		}

		public void Start()
		{
			Tracer.TraceMe("Started");

			_stats = new Statistics();
			_tray = new TrayIcon(_stats);
			_idleWatcher = new IdleWatcher(TimeSpan.FromMinutes(1));
			_timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

			_tray.Init();

			//_idleWatcher.StartDetection(Timer);
			_idleWatcher.IdleStart += IdleStart;
			_idleWatcher.IdleEnd += IdleEnd;

			_tray.OnPositionSwitch += OnPositionSwitch;

			_timer.Start();
			_timer.Tick += OnTimerOnTick;
		}

		private void OnPositionSwitch()
		{
			_stats.SetState(_stats.IsSitting ? WorkState.Standing : WorkState.Sitting);
			_tray.UpdateIcon();
		}

		private void IdleEnd()
		{
			_stats.RestoreState();
			_tray.UpdateIcon();
		}

		private void IdleStart()
		{
			_stats.SetState(WorkState.Away);
			_tray.UpdateIcon();
		}

		private void OnTimerOnTick(object s, EventArgs o)
		{
			if (!_stats.IsUserAway)
			{
				bool wasExceeded = _stats.IsSittingLimitExceeded;

				if (!wasExceeded && _stats.IsSittingLimitExceeded)
				{
					OnSittingIntervalExceeded();
				}
			}
		}

		private void OnSittingIntervalExceeded()
		{
			_tray.UpdateIcon();
			_tray.ShowWarning();
		}

		public void Dispose()
		{
			_idleWatcher.Dispose();
		}
	}
}