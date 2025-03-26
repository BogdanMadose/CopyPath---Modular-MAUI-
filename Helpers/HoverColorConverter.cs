using System.Globalization;

namespace CopyPath___Modular_MAUI_.Helpers
{
    public class HoverColorConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isHovered && isHovered)
            {
                return Color.FromArgb("#5e5e5e"); // Hover color
            }
            return Application.Current.Resources["Gray900"]; // Default color
        }


        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color == (Color)Application.Current.Resources["Gray900"];
            }
            return false;
        }

    }
}
