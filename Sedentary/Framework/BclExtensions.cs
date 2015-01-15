using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sedentary.Framework
{
	public static class BclExtensions
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

		public static double InRangeOf(this double value, double min, double max)
		{
			return Math.Max(Math.Min(value, min), max);
		}

	    public static IEnumerable<IList<TElement>> SplitBySequences<TElement>(
	        this IEnumerable<TElement> source)
	    {
	        var c = EqualityComparer<TElement>.Default;
	        return source.SplitBySequences(c.Equals);
	    }

		public static IEnumerable<IList<TElement>> SplitBySequences<TElement>(
			this IEnumerable<TElement> source,
			Func<TElement, TElement, bool> matcher)
		{
			var list = new List<TElement>();

			TElement prev = default(TElement);
			int index = 0;

			foreach (TElement current in source)
			{
				if (index++ == 0 || matcher(prev, current))
				{
					list.Add(prev = current);
					continue;
				}

				var newList = new List<TElement> {current};

				yield return list;
				list = newList;
				prev = current;
			}

			yield return list;
		}
	}
}
