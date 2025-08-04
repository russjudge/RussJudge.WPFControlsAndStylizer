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
    internal class ValueMatchToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else
            {
                if (parameter is string parm)
                {
                    var parms = parm.Split("|");
                    var visibilityOptions = NullToVisibilityConverter.ParmsToVisibility(2, parms);

                    var retVal = OperatorTest(value, parms[0], parms[1]) ? visibilityOptions.Item1 : visibilityOptions.Item2;
                    return retVal ?? DependencyProperty.UnsetValue;
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public static bool OperatorTest(object val, string operatorValue, string compareValue)
        {
            string? value = val?.ToString();

            value ??= string.Empty;

            var retVal = operatorValue switch
            {
                "<" or "&lt;" => value.CompareTo(compareValue) < 0,
                "<=" or "&lt;=" => value.CompareTo(compareValue) <= 0,
                ">" or "&gt;" => value.CompareTo(compareValue) > 0,
                ">=" or "&gt;=" => value.CompareTo(compareValue) >= 0,
                "=" => value.Equals(compareValue),
                "!=" => !value.Equals(compareValue),
                _ => throw new NotSupportedException("Specified Operator is not supported."),
            };
            return retVal;
        }
    }
}
