using System;
using System.Diagnostics;
using System.Windows.Threading;
using SittingTracker.Framework;

namespace SittingTracker.Model
{
	public class IdleWatcher
	{
		private static Countdown _countdown;
		private static ClientIdleHandler _handler;
		private bool _isIdle;

		public IdleWatcher(TimeSpan maxIdleTime)
		{
			_countdown = new Countdown((int) maxIdleTime.TotalSeconds);
			_handler = new ClientIdleHandler();
		}

		public bool IsIdle
		{
			get { return _isIdle; }
		}

		public event Action IdleStart;

		public event Action IdleEnd;

		private void OnUserReturned()
		{
			_isIdle = false;
			Action handler = IdleEnd;
			if (handler != null) handler();
			Tracer.TraceMe("Idle ended");
		}

		private void OnUserAway()
		{
			_isIdle = true;
			Action handler = IdleStart;
			if (handler != null) handler();

			Tracer.TraceMe("Idle started");
		}

		public void StartDetection(DispatcherTimer timer)
		{
			timer.Tick += TimerOnTick;
			_countdown.Done += OnUserAway;
			_handler.UserActive += OnIdleOnUserActive;
			_handler.Start();
		}

		private void OnIdleOnUserActive()
		{
			_countdown.Reset();

			if (IsIdle)
			{
				OnUserReturned();
			}
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			_countdown.Decrement();
		}

	    public void Dispose()
	    {
	        _handler.Dispose();
	    }
	}
}