using System;
using System.Windows.Threading;

namespace Sedentary.Framework
{
	public class CountdownTimer
	{
		private readonly TimeSpan _interval;
		private readonly DispatcherTimer _timer;
		private TimeSpan _startTime;

		public CountdownTimer(TimeSpan interval)
		{
			_interval = interval;
			_timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
			_timer.Tick += OnTick;
		}

		public void OnTick(object sender, EventArgs e)
		{
			OnTick();

			if (TimeLeft <= TimeSpan.Zero)
			{
				_timer.Stop();
				OnDone();
			}
		}

		public TimeSpan TimeLeft
		{
			get
			{
				if (_startTime == TimeSpan.Zero)
				{
					return _startTime;
				}

				return _interval - (DateTime.Now.TimeOfDay - _startTime);
			}
		}

		public void Stop()
		{
			_timer.Stop();
			_startTime = TimeSpan.Zero;
		}

		public event Action Done;
		public event Action Tick;

		protected virtual void OnTick()
		{
			var handler = Tick;
			if (handler != null) handler();
		}

		protected virtual void OnDone()
		{
			Action handler = Done;
			if (handler != null) handler();
		}

		public void Start()
		{
			_startTime = DateTime.Now.TimeOfDay;
			_timer.Start();
		}
	}
}