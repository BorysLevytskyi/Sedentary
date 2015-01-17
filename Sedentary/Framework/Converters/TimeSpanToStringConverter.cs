using System;
using System.Globalization;
using System.Windows.Data;

namespace Sedentary.Framework.Converters
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is TimeSpan ? ((TimeSpan) value).ToHumanTimeString() : "value is not a time span";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		} 
	}
}