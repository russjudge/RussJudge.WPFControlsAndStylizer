using RussJudge.WPFControlsAndStylizer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        private void OnChangeStyleToDialogWindowStyle(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = SizeToContent.Manual;
            this.Style = (Style)Application.Current.Resources["DialogWindowStyle"];
            Title = "Main Window Example (Style=DialogWindowStyle)";
            Attached.SetIsStylized(this, true);
            this.InvalidateMeasure();
            this.UpdateLayout();
            this.SizeToContent = SizeToContent.Height;

        }

        private void OnChangeStyleToStandardWindowStyle(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = SizeToContent.Manual;
            this.Style = (Style)Application.Current.Resources["StandardWindowStyle"];
            Title = "Main Window Example (Style=StandardWindowStyle)";
            Attached.SetIsStylized(this, true);
            this.InvalidateMeasure();
            this.UpdateLayout();
            this.SizeToContent = SizeToContent.Height;
        }

        private void OnOpenDialog(object sender, RoutedEventArgs e)
        {
            (new DialogWindow()).ShowDialog();
        }

        private void OnOpenStandard(object sender, RoutedEventArgs e)
        {
            (new StandardWindow()).Show();
        }

        private void OnOpenStandardNoTitleContent(object sender, RoutedEventArgs e)
        {
            (new StandardWindowNoTitleContent()).Show();
        }

        private void OnAbout(object sender, RoutedEventArgs e)
        {
            About.DisplayAbout();
        }
    }
}