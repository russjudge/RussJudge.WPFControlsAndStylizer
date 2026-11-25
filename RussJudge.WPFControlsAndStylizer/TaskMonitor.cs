using System.Collections.ObjectModel;
using System.Windows;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Starts background tasks and keeps a bindable list of the ones that are still running.
    /// </summary>
    public class TaskMonitor : DependencyObject
    {
        private TaskMonitor()
        {
            BackgroundProcesses = new ObservableCollection<NamedTaskItem>();
        }

        private static TaskMonitor? _current;

        /// <summary>
        /// The one TaskMonitor of the application.  It always belongs to the UI thread, whichever thread uses it first.
        /// </summary>
        public static TaskMonitor Current
        {
            get
            {
                TaskMonitor? current = Volatile.Read(ref _current);
                if (current == null)
                {
                    // No lock is held while waiting for the UI thread, so two threads racing here cannot deadlock.
                    // Should both create an instance, the first one stored wins and the other is dropped.
                    TaskMonitor created = StylizerResources.CreateOnUiThread(() => new TaskMonitor());
                    current = Interlocked.CompareExchange(ref _current, created, null) ?? created;
                }
                return current;
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
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

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
            if (CheckAccess())
            {
                Register(item);
            }
            else
            {
                Dispatcher.Invoke(() => Register(item));
            }

            // Hand the clean-up to the UI thread without waiting for it: the worker thread that finished the task is
            // released straight away instead of being blocked until the UI thread gets around to it.
            task.ContinueWith(
                _ => Dispatcher.BeginInvoke(new Action(() => Unregister(item))),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        private void Register(NamedTaskItem item)
        {
            IsActive = true;
            if (SyncIsActiveWithGlobalControl)
            {
                GlobalControl.Current.IsActive = true;
            }
            BackgroundProcesses.Add(item);
        }

        private void Unregister(NamedTaskItem item)
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
            item.CancelTokenSource.Dispose();
        }

        /// <summary>
        /// Cancel all background tasks that are currently part of the monitor.
        /// </summary>
        public static void CancelAllTasks()
        {
            TaskMonitor monitor = Current;
            if (monitor.CheckAccess())
            {
                monitor.CancelAll();
            }
            else
            {
                monitor.Dispatcher.Invoke(monitor.CancelAll);
            }
        }

        private void CancelAll()
        {
            // Cancel a copy of the list: a task that reacts to the cancellation must not be able to change the
            // collection while it is being walked.
            foreach (var item in BackgroundProcesses.ToArray())
            {
                Cancel(item);
            }
        }

        /// <summary>
        /// Cancel the specified background task. For this to have any effect, the delegate Task must process the CancellationToken.
        /// </summary>
        /// <param name="item">The task to cancel.  Nothing happens when the task has already finished.</param>
        public static void Cancel(NamedTaskItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            try
            {
                item.CancelTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // The task has already finished and its CancellationTokenSource is gone: there is nothing to cancel.
            }
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
