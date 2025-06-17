using System.Globalization;
using System.Windows.Data;

namespace Nodify.Converts;

public class HalfOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!double.TryParse(value?.ToString(), out var coord)) return 0;
        if (double.TryParse(parameter?.ToString(), out var size)) return coord - size / 2;

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

