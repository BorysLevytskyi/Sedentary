using System;
using System.Globalization;
using System.Windows.Data;

namespace SittingTracker.Framework.Converter
{
	public class TimeSpanToWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var ts = (TimeSpan) value;
			const int totalWidth = 600;
			double percent = ((double)ts.Ticks / (double)TimeSpan.FromMinutes(1).Ticks);
			return (int)(totalWidth*percent);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}