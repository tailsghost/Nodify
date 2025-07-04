using Nodify.ViewModels;
using System.Globalization;
using System.Windows.Data;

namespace Nodify.Converts;

public class BooleanToConnectorsConvert : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter?.ToString() == "1")
        {
            if (value is not NodeViewModel vm) throw new ArgumentException("Пришли неверные данные");
            return vm.Inputs;
        } else if (parameter?.ToString() == "2")
        {
            if (value is not NodeViewModel vm) throw new ArgumentException("Пришли неверные данные");
            return vm.Outputs;
        }


        throw new ArgumentException("Пришли неверные данные");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

