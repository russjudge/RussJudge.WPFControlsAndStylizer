using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Attached properties that support the styles in StylizerDictionary.xaml.
    /// </summary>
    public class Attached : DependencyObject
    {
        #region Placeholder

        /// <summary>
        /// Content shown inside an empty TextBox that does not have the focus.
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
            "Placeholder",
            typeof(object),
            typeof(Attached),
            new FrameworkPropertyMetadata(defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                propertyChangedCallback: OnPlaceholderPropertyChanged));

        private static void OnPlaceholderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox element)
            {
                // The template is loaded once and shared.  Assigning the instance that is already in place would make
                // the TextBox rebuild its visual tree for nothing, so only assign it the first time.
                ControlTemplate template = StylizerResources.PlaceholderTextBoxTemplate;
                if (!ReferenceEquals(element.Template, template))
                {
                    element.Template = template;
                }
            }
        }

        public static object GetPlaceholder(UIElement target) =>
            (object)target.GetValue(PlaceholderProperty);

        // Declare a set accessor method.
        public static void SetPlaceholder(UIElement target, object value) =>
            target.SetValue(PlaceholderProperty, value);

        #endregion

        #region TitleContent

        /// <summary>
        /// Content shown in the title bar, left of the title, of a window that uses DialogWindowStyle or StandardWindowStyle.
        /// </summary>
        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.RegisterAttached(
            "TitleContent",
            typeof(object),
            typeof(Attached),
            new FrameworkPropertyMetadata(defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.AffectsRender, OnTitleContentChanged));

        private static void OnTitleContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && !GetIsStylizedApplied(window))
            {
                SetIsStylized(window, true);
            }
        }

        public static object? GetTitleContent(UIElement target) =>
            (object?)target.GetValue(TitleContentProperty);

        // Declare a set accessor method.
        public static void SetTitleContent(UIElement target, object value) =>
            target.SetValue(TitleContentProperty, value);

        #endregion

        #region IsStylized

        private static readonly DependencyProperty IsStylizedAppliedProperty =
           DependencyProperty.RegisterAttached(
           "IsStylizedApplied",
           typeof(bool),
           typeof(Attached),
           new FrameworkPropertyMetadata(defaultValue: false,
               flags: FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Makes the title bar buttons of DialogWindowStyle and StandardWindowStyle work, and keeps the window from
        /// covering the Taskbar when it is maximized.
        /// </summary>
        /// <remarks>
        /// Both styles set this property themselves, so it no longer has to be set on each window.  Setting it
        /// explicitly (or setting <see cref="TitleContentProperty"/>) still works and does no harm.
        /// </remarks>
        public static readonly DependencyProperty IsStylizedProperty =
            DependencyProperty.RegisterAttached(
            "IsStylized",
            typeof(bool),
            typeof(Attached),
            new FrameworkPropertyMetadata(defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                propertyChangedCallback: OnIsStylizedChanged));

        public static bool GetIsStylized(UIElement target) =>
            (bool)target.GetValue(IsStylizedProperty);

        // Declare a set accessor method.
        public static void SetIsStylized(UIElement target, bool value) =>
            target.SetValue(IsStylizedProperty, value);

        private static void OnIsStylizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && !GetIsStylizedApplied(window)
                && (GetIsStylized(window) || GetTitleContent(window) != null))
            {
                SetIsStylizedApplied(window, true);
                AddSystemCommandBindings(window);
                MaximizedWindowBounds.Attach(window);
            }
        }

        private static bool GetIsStylizedApplied(UIElement target) =>
            (bool)target.GetValue(IsStylizedAppliedProperty);

        // Declare a set accessor method.
        private static void SetIsStylizedApplied(UIElement target, bool value) =>
            target.SetValue(IsStylizedAppliedProperty, value);

        #endregion

        #region WindowStyle (obsolete)

        /// <summary>
        /// No longer used.  Earlier versions stored the style of a window here while it was being maximized.
        /// </summary>
        [Obsolete("No longer used: maximizing a stylized window no longer removes and re-applies its style.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.RegisterAttached(
                "WindowStyle",
                typeof(Style),
                typeof(Attached),
                new FrameworkPropertyMetadata(defaultValue: null,
                    flags: FrameworkPropertyMetadataOptions.AffectsRender));

#pragma warning disable CS0618 // The accessors of an obsolete property have to keep using it.
        [Obsolete("No longer used: maximizing a stylized window no longer removes and re-applies its style.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Style GetWindowStyle(Window target) =>
            (Style)target.GetValue(WindowStyleProperty);

        // Declare a set accessor method.
        [Obsolete("No longer used: maximizing a stylized window no longer removes and re-applies its style.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetWindowStyle(Window target, Style value) =>
            target.SetValue(WindowStyleProperty, value);
#pragma warning restore CS0618

        #endregion

        #region Title bar commands

        // The handlers are static and take the window from "sender", so the same delegates serve every window.
        private static void AddSystemCommandBindings(Window win)
        {
            win.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu));
        }

        private static void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as Window)?.Close();
        }

        private static void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Window win)
            {
                SystemCommands.MinimizeWindow(win);
            }
        }

        private static void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Window win)
            {
                SystemCommands.RestoreWindow(win);
            }
        }

        private static void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Window win)
            {
                SystemCommands.MaximizeWindow(win);
            }
        }

        private static void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is Window win && win.ResizeMode != ResizeMode.NoResize;
        }

        private static void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is Window win
                && (win.ResizeMode == ResizeMode.CanResize || win.ResizeMode == ResizeMode.CanResizeWithGrip);
        }

        private static void OnShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is not Window win || e.OriginalSource is not FrameworkElement element)
            {
                return;
            }

            CompositionTarget? target = PresentationSource.FromVisual(element)?.CompositionTarget;
            if (target == null)
            {
                return;
            }

            // Open the menu just below the element that was clicked.  PointToScreen answers in physical pixels on
            // whichever monitor the window is on (maximized or not); ShowSystemMenu expects device independent units.
            Point screenPoint = element.PointToScreen(new Point(0, element.ActualHeight));
            SystemCommands.ShowSystemMenu(win, target.TransformFromDevice.Transform(screenPoint));
        }

        #endregion
    }
}
