using System.Globalization;

namespace MauiApp1.Converters
{
    public class BoolToMarginConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isVisible && isVisible)
            {
                return new Thickness(280, 0, 0, 0); // 侧边栏宽度
            }
            return new Thickness(0);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
