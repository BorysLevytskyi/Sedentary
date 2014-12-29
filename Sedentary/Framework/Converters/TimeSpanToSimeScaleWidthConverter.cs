using System;
using System.Globalization;
using System.Windows.Data;

namespace Sedentary.Framework.Converters
{
	public class TimeSpanToSimeScaleWidthConverter : IMultiValueConverter
	{

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var timeSpan = (TimeSpan) values[0];
			var timeScale = TimeSpan.FromHours(8); // (TimeSpan) values[1]; TODO: Fix this bug
			var actualWidth = (double) values[2];

			var rate = timeSpan.GetCompletionRateFor(timeScale);
			double width = Math.Round(actualWidth * rate, 1);

			return Math.Max(1, width);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}