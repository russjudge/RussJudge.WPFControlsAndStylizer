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

    [ValueConversion(typeof(object), typeof(Visibility))]
    internal class NullToVisibilityConverter : IValueConverter
    {
        // A converter parameter is a constant in XAML, so each distinct parameter is parsed once and then reused
        // instead of being split and parsed again on every conversion (for a TextBox, that is every keystroke).
        private static readonly ConcurrentDictionary<string, VisibilityPair> ParsedParameters = new();

        public static Visibility? StringToVisibility(string value)
        {
            return Enum.TryParse(value, out Visibility visibility) ? visibility : null;
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

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is string parm)
            {
                VisibilityPair options = ParsedParameters.GetOrAdd(parm, p => new VisibilityPair(ParmsToVisibility(0, p.Split('|'))));
                return (value == null || (value is string val && string.IsNullOrWhiteSpace(val))) ? options.First : options.Second;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The two outcomes of a visibility converter, already boxed so that a conversion does not allocate.
    /// An outcome that could not be parsed is <see cref="DependencyProperty.UnsetValue"/>.
    /// </summary>
    internal sealed class VisibilityPair(Tuple<Visibility?, Visibility?> options)
    {
        public object First { get; } = Box(options.Item1);
        public object Second { get; } = Box(options.Item2);

        private static object Box(Visibility? visibility) => visibility ?? DependencyProperty.UnsetValue;
    }
}
