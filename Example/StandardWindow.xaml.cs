using RussJudge.WPFControlsAndStylizer;
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

namespace Example
{
    /// <summary>
    /// Interaction logic for StandardWindow.xaml
    /// </summary>
    public partial class StandardWindow : Window
    {
        public StandardWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnShowActivity(object sender, RoutedEventArgs e)
        {
            GlobalControl.Current.IsActive = !GlobalControl.Current.IsActive;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
