using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Note: This converter is available in the Nuget RussJudge.WPFValueConverters.
    /// Included here so that this Library is not dependent on RussJudge.WPFValueConverters.
    /// </summary>

    [ValueConversion(typeof(object), typeof(Visibility))]
    internal class NullToVisibilityConverter : IValueConverter
    {
        public static Visibility? StringToVisibility(string value)
        {

            if (Enum.TryParse(typeof(Visibility), value, out object? v))
            {
                return (Visibility)v;
            }
            else
            {
                return null;
            }
        }
        public static Tuple<Visibility?, Visibility?> ParmsToVisibility(int startIndex, params string[] values)
        {
            Visibility? item1 = Visibility.Visible;
            Visibility? item2 = Visibility.Collapsed;

            if (values.Length > startIndex)
            {
                item1 = StringToVisibility(values[startIndex]);
            }
            if (values.Length > startIndex + 1)
            {
                var vis = StringToVisibility(values[startIndex + 1]);
                if (vis == null)
                {
                    if (item1 == null)
                    {
                        item2 = null;
                    }
                    else
                    {
                        item2 = (item1 == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
                else
                {
                    item2 = vis;
                }
            }
            return new Tuple<Visibility?, Visibility?>(item1, item2);
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string parm)
            {
                var parms = parm.Split('|');
                var visibilityOptions = ParmsToVisibility(0, parms);
                var retVal = (value == null || (value is string val && string.IsNullOrEmpty(val.Trim()))) ? visibilityOptions.Item1 : visibilityOptions.Item2;
                return (retVal == null) ? DependencyProperty.UnsetValue : retVal;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
