using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sedentary.Framework
{
	static public class TimeSpanExtensions
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

			if (sb.Length == 0)
			{
				return "0s";
			}

			return sb.ToString(0, sb.Length - 1); // W/o last space
		}

		public static TimeSpan TrimMilliseconds(this TimeSpan timeSpan)
		{
			return timeSpan.RoundTo(TimeSpan.FromSeconds(1));
		}

		public static TimeSpan RoundTo(this TimeSpan timeSpan, TimeSpan windowSize)
		{
			return timeSpan.Subtract(TimeSpan.FromTicks(timeSpan.Ticks % windowSize.Ticks));
		}

		public static TimeSpan SummarizeTime(this IEnumerable<TimeSpan> source)
		{
			return TimeSpan.FromTicks(source.Sum(p => p.Ticks));
		}
	}
}