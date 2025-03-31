using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace RussJudge.WPFControlsAndStylizer
{
    public class TaskMonitor : DependencyObject
    {
        private TaskMonitor()
        {
            BackgroundProcesses = [];

        }
        private static TaskMonitor? _current;
        public static TaskMonitor Current
        {
            get
            {
                _current ??= new();
                return _current;
            }
        }
        /// <summary>
        /// Add a delegate action as a background Task.
        /// </summary>
        /// <param name="action">The delegate to run in the background.  the delegate must accept a CancellationToken as a parameter.</param>
        /// <param name="description">A description for the delegate action.</param>
        public static void AddTask(Action<CancellationToken> action,
            string description)
        {
            Current.InternalAddTask(action, description);
        }
        private void InternalAddTask(
            Action<CancellationToken> action,
            string description)
        {

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken token = cancellationTokenSource.Token;
            void cancellableAction()
            {
                try
                {
                    // Check if cancellation is requested before starting
                    token.ThrowIfCancellationRequested();
                    // Execute the original action
                    action(token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"{description} canceled.");
                    throw; // Re-throw to properly propagate cancellation
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error running {description}: {ex}");
                    throw;
                }
            }
            var task = Task.Run(cancellableAction, token);

            NamedTaskItem item = new(description, task, cancellationTokenSource);
            Dispatcher.Invoke(() =>
            {
                IsActive = true;
                if (SyncIsActiveWithGlobalControl)
                {
                    GlobalControl.Current.IsActive = true;
                }
                BackgroundProcesses.Add(item);
            });


            task.ContinueWith(t =>
            {

                Dispatcher.Invoke(() =>
                {
                    BackgroundProcesses.Remove(item);
                    if (BackgroundProcesses.Count == 0)
                    {
                        IsActive = false;
                        if (SyncIsActiveWithGlobalControl)
                        {
                            GlobalControl.Current.IsActive = false;
                        }
                    }
                });
                cancellationTokenSource.Dispose();
            }, TaskContinuationOptions.ExecuteSynchronously);

        }
        /// <summary>
        /// Cancel all background tasks that are currently part of the monitor.
        /// </summary>
        public static void CancelAllTasks()
        {
            Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in Current.BackgroundProcesses)
                {
                    Cancel(item);
                }
            });
        }
        /// <summary>
        /// Cancel the specified background task. For this to have any effect, the delegate Task must process the CancellationToken.
        /// </summary>
        /// <param name="item"></param>
        public static void Cancel(NamedTaskItem item)
        {
            item.CancelTokenSource.Cancel();
        }

        public static readonly DependencyProperty BackgroundProcessesProperty =
            DependencyProperty.Register(nameof(BackgroundProcesses), typeof(ObservableCollection<NamedTaskItem>),
            typeof(TaskMonitor));

        /// <summary>
        /// A bindable collection of BackgroundProcesses of type NamedTaskItem.
        /// </summary>
        public ObservableCollection<NamedTaskItem> BackgroundProcesses
        {
            get
            {
                return (ObservableCollection<NamedTaskItem>)this.GetValue(BackgroundProcessesProperty);
            }
            set
            {
                this.SetValue(BackgroundProcessesProperty, value);
            }
        }

        public static readonly DependencyProperty IsActiveProperty =
           DependencyProperty.Register(nameof(IsActive), typeof(bool),
           typeof(TaskMonitor), new PropertyMetadata(false));


        /// <summary>
        /// A Flag to indicate that there are background processes running.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (bool)this.GetValue(IsActiveProperty);
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }


        public static readonly DependencyProperty SyncIsActiveWithGlobalControlProperty =
           DependencyProperty.Register(nameof(SyncIsActiveWithGlobalControl), typeof(bool),
           typeof(TaskMonitor), new PropertyMetadata(false));


        /// <summary>
        /// A flag to indicate whether or not to set GlobalControl.Current.IsActive to match TaskMonitor.Current.IsActive.
        /// </summary>
        /// <remarks>
        /// This is a one-way synchronization.  Only when TaskMonitor.Current.IsActive changed will GlobalControl.Current.IsActive be changed.
        /// If GlobalControl.Current.IsActive is changed directly, TaskMonitor.Current.IsActive will NOT be changed.
        /// </remarks>
        public bool SyncIsActiveWithGlobalControl
        {
            get
            {
                return (bool)this.GetValue(SyncIsActiveWithGlobalControlProperty);
            }
            set
            {
                this.SetValue(SyncIsActiveWithGlobalControlProperty, value);
            }
        }

    }
}
