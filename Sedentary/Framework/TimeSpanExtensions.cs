using System;
using System.Text;

namespace Sedentary.Framework
{
	public static class TimeSpanExtensions
	{
		public static double GetCompletionRateFor(this TimeSpan timePassed, TimeSpan requiredTime)
		{
			return Math.Round((double)timePassed.Ticks / requiredTime.Ticks, 2);
		}

		public static string ToHumanTimeString(this TimeSpan time)
		{
			var sb = new StringBuilder();

			if (time.Days > 0)
			{
				sb.Append(time.Days + "d ");
			}

			if (time.Hours > 0)
			{
				sb.Append(time.Hours + "h ");
			}

			if (time.Minutes > 0)
			{
				sb.Append(time.Minutes + "m ");
			}

			if (time.Seconds > 0)
			{
				sb.Append(time.Seconds + "s ");
			}

			return sb.ToString(0, sb.Length - 1); // W/o last space
		}
	}
}
