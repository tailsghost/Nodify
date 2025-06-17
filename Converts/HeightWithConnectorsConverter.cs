using System.Globalization;
using System.Windows.Data;

namespace Nodify.Converts;

public class HeightWithConnectorsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(!double.TryParse(value?.ToString(), out var height)) return 0;

        if(!int.TryParse(parameter?.ToString(), out var result)) return 0;

        return height + result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

