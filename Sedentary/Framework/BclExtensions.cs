using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sedentary.Framework
{
	public static class BclExtensions
	{
		public static double InRangeOf(this double value, double min, double max)
		{
			return Math.Max(Math.Min(value, min), max);
		}

	    public static IEnumerable<IList<T>> SplitBySequences<T>(
	        this IEnumerable<T> source)
	    {
	        var c = EqualityComparer<T>.Default;
	        return source.SplitBySequences(c.Equals);
	    }

		public static IEnumerable<IList<T>> SplitBySequences<T>(
			this IEnumerable<T> source,
			Func<T, T, bool> comparer)
		{
			var list = new List<T>();

			T prev = default(T);
			int index = 0;

			foreach (T current in source)
			{
				if (index++ == 0 || comparer(prev, current))
				{
					list.Add(prev = current);
					continue;
				}

				var newList = new List<T> {current};

				yield return list;
				list = newList;
				prev = current;
			}

			yield return list;
		}

		public static IEnumerable<T> Reduce<T>(
			this IEnumerable<IList<T>> source, 
			Func<IList<T>, T> reduce)
		{
			return source.Select(sequence => sequence.Count == 1 ? sequence[0] : reduce(sequence));
		}
	}
}
