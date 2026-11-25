using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Example
{
    /// <summary>
    /// Converts true to false and false to true.
    /// </summary>
    /// <remarks>
    /// The same converter (and many more) is available in the NuGet package RussJudge.WPFValueConverters.
    /// The example keeps its own copy so that it can be built for every framework that
    /// RussJudge.WPFControlsAndStylizer targets, without depending on another package.
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class OppositeBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool flag ? !flag : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool flag ? !flag : DependencyProperty.UnsetValue;
        }
    }
}
