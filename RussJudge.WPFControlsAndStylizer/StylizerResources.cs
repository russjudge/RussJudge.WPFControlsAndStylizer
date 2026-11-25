using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Shared access to StylizerDictionary.xaml and to the UI thread for the code in this library.
    /// </summary>
    internal static class StylizerResources
    {
        // A relative pack URI resolves against the application without needing the "pack:" scheme to be registered yet.
        private static readonly Uri DictionaryUri =
            new("/RussJudge.WPFControlsAndStylizer;component/StylizerDictionary.xaml", UriKind.Relative);

        // The dictionary holds brushes that belong to the thread that loaded them, so keep one copy per UI thread.
        [ThreadStatic]
        private static ResourceDictionary? _dictionary;

        [ThreadStatic]
        private static ControlTemplate? _placeholderTextBoxTemplate;

        /// <summary>
        /// StylizerDictionary.xaml, loaded once per UI thread instead of once per use.
        /// </summary>
        internal static ResourceDictionary Dictionary =>
            _dictionary ??= new ResourceDictionary { Source = DictionaryUri };

        /// <summary>
        /// The template that <see cref="Attached.PlaceholderProperty"/> applies to a TextBox.
        /// </summary>
        internal static ControlTemplate PlaceholderTextBoxTemplate =>
            _placeholderTextBoxTemplate ??= (ControlTemplate)Dictionary["PlaceholderTextBoxTemplate"];

        /// <summary>
        /// Lets a window that ships in this library find the styles it uses when the application has not merged
        /// StylizerDictionary.xaml into its own resources.  Call before InitializeComponent.
        /// </summary>
        /// <remarks>
        /// Nothing is added when the application already supplies the styles, so colors that the application
        /// overrides keep taking precedence.
        /// </remarks>
        internal static void EnsureAvailable(FrameworkElement element)
        {
            if (element.TryFindResource("DialogWindowStyle") == null)
            {
                element.Resources.MergedDictionaries.Add(Dictionary);
            }
        }

        /// <summary>
        /// Creates an object on the application's UI thread, no matter which thread asks for it.
        /// </summary>
        /// <remarks>
        /// A DependencyObject belongs to the thread that creates it.  The singletons in this library are bound to from
        /// XAML, so they have to belong to the UI thread even when a worker thread happens to be the first to use them.
        /// </remarks>
        internal static T CreateOnUiThread<T>(Func<T> factory)
        {
            Dispatcher? dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess() || dispatcher.HasShutdownStarted)
            {
                return factory();
            }
            return dispatcher.Invoke(factory);
        }
    }
}
