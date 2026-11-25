using System.Collections.Concurrent;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Note: This converter is available in the Nuget RussJudge.WPFValueConverters.
    /// Included here so that this Library is not dependent on RussJudge.WPFValueConverters.
    /// </summary>
    internal class ValueMatchToVisibilityConverter : IValueConverter
    {
        // Each distinct converter parameter is parsed once.  See NullToVisibilityConverter.
        private static readonly ConcurrentDictionary<string, ParsedParameter> ParsedParameters = new();

        private sealed class ParsedParameter(string[] parms)
        {
            public string Operator { get; } = parms[0];
            public string CompareValue { get; } = parms[1];
            public VisibilityPair Options { get; } = new(NullToVisibilityConverter.ParmsToVisibility(2, parms));
        }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else
            {
                if (parameter is string parm)
                {
                    ParsedParameter parsed = ParsedParameters.GetOrAdd(parm, p => new ParsedParameter(p.Split('|')));

                    return OperatorTest(value, parsed.Operator, parsed.CompareValue) ? parsed.Options.First : parsed.Options.Second;
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
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
