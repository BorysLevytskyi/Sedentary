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
			return p.State == WorkState.Sitting && (p.Length + TimeSpan.FromMinutes(5)) > App.Tracker.Requirements.MaxSittingTime;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}