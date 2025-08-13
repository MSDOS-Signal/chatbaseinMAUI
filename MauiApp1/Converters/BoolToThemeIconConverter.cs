using System.Globalization;

namespace MauiApp1.Converters
{
    public class BoolToThemeIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isDarkMode)
            {
                return isDarkMode ? "☀️" : "🌙";
            }
            return "🌙";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
