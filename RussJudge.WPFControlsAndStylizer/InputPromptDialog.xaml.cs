using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Interaction logic for InputPromptWindow.xaml
    /// </summary>
    public partial class InputPromptDialog : Window
    {
        /// <summary>
        /// Display a prompt window for accepting a string from the user.
        /// </summary>
        /// <param name="prompt">The prompt for obtaining the string value from the user.</param>
        /// <param name="title">The title to use on the window.</param>
        /// <param name="defaultValue">The default value to enter for the user.</param>
        /// <returns>Null if the user cancels, otherwise the string the user entered.</returns>
        public static string? ShowDialog(string prompt, string title, string defaultValue = "")
        {
            InputPromptDialog dialog = new()
            {
                Title = title,
                Value = defaultValue,
                Prompt = prompt,
            };
            if (dialog.ShowDialog() == true)
            {
                return dialog.Value;
            }
            else
            {
                return null;
            }
        }
        private InputPromptDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register(nameof(Prompt), typeof(string),
            typeof(InputPromptDialog));


        public string Prompt
        {
            get
            {
                return (string)this.GetValue(PromptProperty);
            }
            set
            {
                this.SetValue(PromptProperty, value);
            }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string),
            typeof(InputPromptDialog));


        public string Value
        {
            get
            {
                return (string)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
