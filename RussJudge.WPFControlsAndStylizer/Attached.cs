using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace RussJudge.WPFControlsAndStylizer
{
    public class Attached : DependencyObject
    {



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
                // Assume 'this' is a window or a control that has access to the resources.
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/RussJudge.WPFControlsAndStylizer;component/StylizerDictionary.xaml", UriKind.Absolute)
                };
                
                // Retrieve the ControlTemplate by key
                ControlTemplate template = (ControlTemplate)resourceDictionary["PlaceholderTextBoxTemplate"];

                // Assign the template to your TextBox
                element.Template = template;
            }
        }

        public static object GetPlaceholder(UIElement target) =>
            (object)target.GetValue(PlaceholderProperty);

        // Declare a set accessor method.
        public static void SetPlaceholder(UIElement target, object value) =>
            target.SetValue(PlaceholderProperty, value);






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



        private static readonly DependencyProperty IsStylizedAppliedProperty =
           DependencyProperty.RegisterAttached(
           "IsStylizedApplied",
           typeof(bool),
           typeof(Attached),
           new FrameworkPropertyMetadata(defaultValue: false,
               flags: FrameworkPropertyMetadataOptions.None));


        public static readonly DependencyProperty IsStylizedProperty =
            DependencyProperty.RegisterAttached(
            "IsStylized",
            typeof(bool),
            typeof(Attached),
            new FrameworkPropertyMetadata(defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                propertyChangedCallback: OnIsStylizedChanged));

        

        private static void OnIsStylizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window element && !GetIsStylizedApplied(element))
            {
                var titleContent = GetTitleContent(element);
                var isStylized = GetIsStylized(element);
                if (titleContent != null || isStylized)
                {
                    AddSystemCommandBindings(element);
                    SetIsStylizedApplied(element, true);
                }
            }
        }
        private static void AddSystemCommandBindings(Window win)
        {
            win.CommandBindings.Add(
                new CommandBinding(SystemCommands.CloseWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    win.Close();
                }));

            win.CommandBindings.Add(
                new CommandBinding(SystemCommands.MaximizeWindowCommand,
                    (object sender, ExecutedRoutedEventArgs e) =>
                    {
                        SystemCommands.MaximizeWindow(win);
                    }, (object sender, CanExecuteRoutedEventArgs e) =>
                    {
                        e.CanExecute = win.ResizeMode == ResizeMode.CanResize || win.ResizeMode == ResizeMode.CanResizeWithGrip;
                    })
                );

            win.CommandBindings.Add(
                new CommandBinding(SystemCommands.MinimizeWindowCommand,
                    (object sender, ExecutedRoutedEventArgs e) =>
                    {
                        SystemCommands.MinimizeWindow(win);
                    }, (object sender, CanExecuteRoutedEventArgs e) =>
                    {
                        e.CanExecute = win.ResizeMode != ResizeMode.NoResize;
                    })
                );


            win.CommandBindings.Add(
                new CommandBinding(SystemCommands.RestoreWindowCommand,
                    (object sender, ExecutedRoutedEventArgs e) =>
                    {
                        SystemCommands.RestoreWindow(win);
                    }, (object sender, CanExecuteRoutedEventArgs e) =>
                    {
                        e.CanExecute = win.ResizeMode == ResizeMode.CanResize || win.ResizeMode == ResizeMode.CanResizeWithGrip;
                    })
                );



            win.StateChanged += Win_StateChanged;



            win.CommandBindings.Add(
                new CommandBinding(SystemCommands.ShowSystemMenuCommand,
                    (object sender, ExecutedRoutedEventArgs e) =>
                    {
                        if (e.OriginalSource is not FrameworkElement element)
                            return;

                        var point = win.WindowState == WindowState.Maximized ? new Point(0, element.ActualHeight)
                            : new Point(win.Left + win.BorderThickness.Left, element.ActualHeight + win.Top + win.BorderThickness.Top);
                        point = element.TransformToAncestor(win).Transform(point);
                        SystemCommands.ShowSystemMenu(win, point);

                    })
                );
        }
        static bool fixingMax = false;
        private static void Win_StateChanged(object? sender, EventArgs e)
        {
            if (sender is Window win)
            {
                if (!fixingMax && win.WindowState == WindowState.Maximized)
                {
                    fixingMax = true;
                    win.WindowState = WindowState.Normal;
                    win.WindowState = WindowState.Maximized;
                    fixingMax = false;
                }
            }
        }
        public static bool GetIsStylized(UIElement target) =>
            (bool)target.GetValue(IsStylizedProperty);

        // Declare a set accessor method.
        public static void SetIsStylized(UIElement target, bool value) =>
            target.SetValue(IsStylizedProperty, value);

        private static bool GetIsStylizedApplied(UIElement target) =>
            (bool)target.GetValue(IsStylizedAppliedProperty);

        // Declare a set accessor method.
        private static void SetIsStylizedApplied(UIElement target, bool value) =>
            target.SetValue(IsStylizedAppliedProperty, value);
    }
}
