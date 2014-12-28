using System;

namespace Sedentary.Framework
{
	public class Countdown
	{
		private int _counter = 0;

		private int _limit;

		public event Action Done;

		protected virtual void OnDone()
		{
			Action handler = Done;
			if (handler != null) handler();
		}

		public Countdown(int limit)
		{
			_limit = limit;
		}

		public void Reset()
		{
			_counter = 0;
		}

		public void Decrement()
		{
			_counter++;

			if (_counter == _limit)
			{
				OnDone();
				Reset();
			}
		}
	}
}