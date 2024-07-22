// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Globalization;

namespace dCalculator.Converters;

public class StringEqBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var param = parameter?.ToString()?.Split('|');
        return param?.Any(x => Equals(x, value?.ToString()));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
