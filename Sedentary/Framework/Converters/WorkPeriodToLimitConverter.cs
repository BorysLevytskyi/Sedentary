using System;
using System.Globalization;
using System.Windows.Data;
using Sedentary.Model;

namespace Sedentary.Framework.Converters
{
	public class WorkPeriodToLimitConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var p = (WorkPeriod) value;
			TimeSpan limit = App.Tracker.Requirements.MaxSittingTime + TimeSpan.FromMinutes(5);
			return p.State == WorkState.Sitting && (p.Length) > limit;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}