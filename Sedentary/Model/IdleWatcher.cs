using System;
using Sedentary.Framework;

namespace Sedentary.Model
{
	public class IdleWatcher
	{
		private readonly UserActivityListener _handler;
		private TimeSpan _lastInput;

		public IdleWatcher()
		{
			_handler = new UserActivityListener();
		}

		public TimeSpan IdleTime
		{
			get { return DateTime.Now.TimeOfDay - _lastInput; }
		}

		public event Action<TimeSpan> UserActive;

		public void StartDetection()
		{
			_lastInput = DateTime.Now.TimeOfDay;
			_handler.UserActive += OnUserInput;
			_handler.Start();
		}

		private void OnUserInput()
		{
			var period = IdleTime;
			_lastInput = DateTime.Now.TimeOfDay;
			OnUserActive(period);
		}

		public void Dispose()
		{
			_handler.Dispose();
		}

		protected virtual void OnUserActive(TimeSpan period)
		{
			var handler = UserActive;
			if (handler != null) handler(period);
		}
	}
}