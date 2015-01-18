using System;
using System.Globalization;
using System.Windows.Data;
using Caliburn.Micro;
using Sedentary.Model;

namespace Sedentary.Framework.Converters
{
	public class WorkPeriodToLimitConverter : IValueConverter
	{
		private readonly TimeSpan _maxSittingTime;

		public WorkPeriodToLimitConverter()
		{
			_maxSittingTime = IoC.Get<AppRequirements>().MaxSittingTime;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var p = (WorkPeriod) value;
			TimeSpan limit = _maxSittingTime + TimeSpan.FromMinutes(5);
			return p.State == WorkState.Sitting && (p.Length) > limit;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}