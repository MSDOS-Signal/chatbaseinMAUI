using System.Globalization;
using MauiApp1.Models;

namespace MauiApp1.Converters
{
    public class MessageTypeToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is MessageType messageType && parameter is string targetTypeString)
            {
                if (targetTypeString == "User")
                    return messageType == MessageType.User;
                else if (targetTypeString == "Assistant")
                    return messageType == MessageType.Assistant;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
