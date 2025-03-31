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
        static int processCount = 0;
        private void OnAddProcess(object sender, RoutedEventArgs e)
        {
            TaskMonitor.AddTask((CancellationToken token) =>
            {
                for (int i = 0; i < 6; i++)
                {
                    try
                    {
                        Task.Delay(10000, token).Wait(token);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, $"#{++processCount} Long running (1 minutes) process");
        }

        private void OnCancelProcess(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is NamedTaskItem taskItem)
            {
                taskItem.CancelTokenSource.Cancel();

                //Or:
                //TaskMonitor.Cancel(taskItem);
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            TaskMonitor.CancelAllTasks();
        }
    }
}
