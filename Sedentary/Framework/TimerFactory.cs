using System;
using System.Windows.Threading;

namespace Sedentary.Framework
{
	public static class TimerFactory
	{
		public static DispatcherTimer StartTimer(TimeSpan interval, Action tick)
		{
			var timer = new DispatcherTimer {Interval = interval};
			timer.Tick += (sender, args) => tick();
			timer.Start();
			return timer;
		}
	}
}