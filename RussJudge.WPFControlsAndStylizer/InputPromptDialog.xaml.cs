using System.Windows;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// A small dialog that asks the user for a string.  Open it with <see cref="ShowDialog(string, string, string)"/>.
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
            // The window uses DialogWindowStyle; make sure it can be found even when the application did not
            // merge StylizerDictionary.xaml into its resources.
            StylizerResources.EnsureAvailable(this);

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

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Let the user type straight away; typing replaces the default value.
            // This is done here rather than in Loaded because Loaded is raised before the binding has put the
            // default value into the TextBox, and setting the text afterwards would clear the selection again.
            ValueTextBox.Focus();
            ValueTextBox.SelectAll();
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
