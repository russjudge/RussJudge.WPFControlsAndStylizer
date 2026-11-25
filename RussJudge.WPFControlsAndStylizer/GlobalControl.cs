using System.Windows;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Application wide state that the controls in this library bind to.
    /// </summary>
    public class GlobalControl : DependencyObject
    {
        private GlobalControl()
        {

        }

        private static GlobalControl? _current;

        /// <summary>
        /// The one GlobalControl of the application.  It always belongs to the UI thread, whichever thread uses it first.
        /// </summary>
        public static GlobalControl Current
        {
            get
            {
                GlobalControl? current = Volatile.Read(ref _current);
                if (current == null)
                {
                    // See TaskMonitor.Current: no lock is held while waiting for the UI thread.
                    GlobalControl created = StylizerResources.CreateOnUiThread(() => new GlobalControl());
                    current = Interlocked.CompareExchange(ref _current, created, null) ?? created;
                }
                return current;
            }
            set
            {
                Volatile.Write(ref _current, value);
            }
        }

        public static readonly DependencyProperty IsActiveProperty =
           DependencyProperty.Register(nameof(IsActive), typeof(bool),
           typeof(GlobalControl), new PropertyMetadata(false, OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GlobalControl)d)._isActive = (bool)e.NewValue;
        }

        // Copy of IsActive that can be read from any thread.
        private volatile bool _isActive;

        /// <summary>
        /// True while the application is busy.  Every <see cref="ActivityIndicator"/> that uses the default style
        /// shows and spins while this is true.
        /// </summary>
        /// <remarks>
        /// Can be read and set from any thread.  When set from a thread other than the UI thread, the change is
        /// passed on to the UI thread and bindings see it a moment later.
        /// </remarks>
        public bool IsActive
        {
            get
            {
                return CheckAccess() ? (bool)this.GetValue(IsActiveProperty) : _isActive;
            }
            set
            {
                if (CheckAccess())
                {
                    this.SetValue(IsActiveProperty, value);
                }
                else
                {
                    _isActive = value;
                    Dispatcher.BeginInvoke(new Action(() => this.SetValue(IsActiveProperty, value)));
                }
            }
        }
    }
}
