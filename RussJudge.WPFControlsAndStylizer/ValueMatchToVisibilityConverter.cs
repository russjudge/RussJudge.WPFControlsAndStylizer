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
            bool retVal;

            string? value = val?.ToString();

            if (value == null)
            {
                value = string.Empty;
            }
            switch (operatorValue)
            {
                case "<":
                case "&lt;":
                    retVal = value.CompareTo(compareValue) < 0;
                    break;
                case "<=":
                case "&lt;=":
                    retVal = value.CompareTo(compareValue) <= 0;
                    break;
                case ">":
                case "&gt;":
                    retVal = value.CompareTo(compareValue) > 0;
                    break;
                case ">=":
                case "&gt;=":
                    retVal = value.CompareTo(compareValue) >= 0;
                    break;
                case "=":
                    retVal = value.Equals(compareValue);
                    break;
                case "!=":
                    retVal = !value.Equals(compareValue);
                    break;
                default:
                    throw new NotSupportedException("Specified Operator is not supported.");
            }
            return retVal;
        }
    }
}
