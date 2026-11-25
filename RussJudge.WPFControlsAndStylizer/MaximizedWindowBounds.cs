using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Keeps a maximized window that has no caption (WindowStyle="None") inside the work area of its monitor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Windows only keeps a maximized window clear of the Taskbar when the window has a caption.  A window without
    /// one is sized to the whole monitor (plus its resize border), which covers the Taskbar and pushes the outer few
    /// pixels of the window off screen.  DialogWindowStyle and StandardWindowStyle both use WindowStyle="None".
    /// </para>
    /// <para>
    /// The fix is applied where Windows asks the window to confirm its new position (WM_WINDOWPOSCHANGING), so it
    /// covers every way a window can become maximized: the title bar button, a double click on the title bar, the
    /// system menu, dragging to the top of the screen, Win+Up, restoring a minimized window, and
    /// <see cref="Window.WindowState"/> being set in code or XAML.  Nothing about the window's style or template is
    /// touched, so the content of the window is not unloaded and reloaded.
    /// </para>
    /// </remarks>
    internal static class MaximizedWindowBounds
    {
        // One delegate for every window: the hook is static, so it keeps no window alive.
        private static readonly HwndSourceHook Hook = WndProc;

        /// <summary>
        /// Starts watching the window.  Safe to call before the window has been shown.
        /// </summary>
        internal static void Attach(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            if (handle != IntPtr.Zero)
            {
                AddHook(handle);
            }
            else
            {
                window.SourceInitialized += OnSourceInitialized;
            }
        }

        private static void OnSourceInitialized(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                window.SourceInitialized -= OnSourceInitialized;
                AddHook(new WindowInteropHelper(window).Handle);
            }
        }

        private static void AddHook(IntPtr handle)
        {
            HwndSource.FromHwnd(handle)?.AddHook(Hook);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_WINDOWPOSCHANGING && lParam != IntPtr.Zero)
            {
                ConstrainToWorkArea(hwnd, lParam);
            }
            // Never mark the message as handled: WPF and WindowChrome still need to see it.
            return IntPtr.Zero;
        }

        private static void ConstrainToWorkArea(IntPtr hwnd, IntPtr windowPos)
        {
            int style = GetWindowLong(hwnd, GWL_STYLE);
            if ((style & WS_MAXIMIZE) == 0 || (style & WS_MINIMIZE) != 0)
            {
                return;
            }
            if ((style & WS_CAPTION) == WS_CAPTION)
            {
                // The window has a real caption (the style is not applied): Windows already does the right thing.
                return;
            }

            // WINDOWPOS: HWND hwnd; HWND hwndInsertAfter; int x; int y; int cx; int cy; UINT flags;
            int xOffset = 2 * IntPtr.Size;
            int flagsOffset = xOffset + 16;
            int flags = Marshal.ReadInt32(windowPos, flagsOffset);
            if ((flags & (SWP_NOMOVE | SWP_NOSIZE)) == (SWP_NOMOVE | SWP_NOSIZE))
            {
                return;
            }

            // Work out where Windows is about to put the window, to find the monitor it is being maximized on.
            RECT proposed;
            if ((flags & (SWP_NOMOVE | SWP_NOSIZE)) == 0)
            {
                proposed = default;
            }
            else if (!GetWindowRect(hwnd, out proposed))
            {
                return;
            }
            int width = proposed.Right - proposed.Left;
            int height = proposed.Bottom - proposed.Top;
            if ((flags & SWP_NOMOVE) == 0)
            {
                proposed.Left = Marshal.ReadInt32(windowPos, xOffset);
                proposed.Top = Marshal.ReadInt32(windowPos, xOffset + 4);
            }
            if ((flags & SWP_NOSIZE) == 0)
            {
                width = Marshal.ReadInt32(windowPos, xOffset + 8);
                height = Marshal.ReadInt32(windowPos, xOffset + 12);
            }
            proposed.Right = proposed.Left + width;
            proposed.Bottom = proposed.Top + height;

            IntPtr monitor = MonitorFromRect(ref proposed, MONITOR_DEFAULTTONEAREST);
            MONITORINFO info = new() { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
            if (monitor == IntPtr.Zero || !GetMonitorInfo(monitor, ref info))
            {
                return;
            }

            RECT bounds = info.rcWork;
            if (proposed.Left > bounds.Left || proposed.Top > bounds.Top
                || proposed.Right < bounds.Right || proposed.Bottom < bounds.Bottom)
            {
                // Windows is not asking for the whole monitor, so this is not the oversized maximize that needs
                // correcting: the window is being restored, snapped or moved, or MaxWidth/MaxHeight limit its size.
                return;
            }
            LeaveRoomForAutoHideTaskbar(ref bounds, ref info);

            Marshal.WriteInt32(windowPos, xOffset, bounds.Left);
            Marshal.WriteInt32(windowPos, xOffset + 4, bounds.Top);
            Marshal.WriteInt32(windowPos, xOffset + 8, bounds.Right - bounds.Left);
            Marshal.WriteInt32(windowPos, xOffset + 12, bounds.Bottom - bounds.Top);
            Marshal.WriteInt32(windowPos, flagsOffset, flags & ~(SWP_NOMOVE | SWP_NOSIZE));
        }

        /// <summary>
        /// An auto-hide Taskbar reserves no work area, so a window that fills the monitor would stop it from sliding
        /// back in.  Leave a sliver of the monitor free on the edge that an auto-hide Taskbar is docked to.
        /// </summary>
        private static void LeaveRoomForAutoHideTaskbar(ref RECT bounds, ref MONITORINFO info)
        {
            APPBARDATA state = new() { cbSize = Marshal.SizeOf(typeof(APPBARDATA)) };
            if ((SHAppBarMessage(ABM_GETSTATE, ref state).ToInt64() & ABS_AUTOHIDE) == 0)
            {
                return;
            }

            RECT monitor = info.rcMonitor;
            bool isPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;
            if (bounds.Left == monitor.Left && HasAutoHideBar(ABE_LEFT, monitor, isPrimary))
            {
                bounds.Left += AutoHideTaskbarReveal;
            }
            if (bounds.Top == monitor.Top && HasAutoHideBar(ABE_TOP, monitor, isPrimary))
            {
                bounds.Top += AutoHideTaskbarReveal;
            }
            if (bounds.Right == monitor.Right && HasAutoHideBar(ABE_RIGHT, monitor, isPrimary))
            {
                bounds.Right -= AutoHideTaskbarReveal;
            }
            if (bounds.Bottom == monitor.Bottom && HasAutoHideBar(ABE_BOTTOM, monitor, isPrimary))
            {
                bounds.Bottom -= AutoHideTaskbarReveal;
            }
        }

        private static bool HasAutoHideBar(int edge, RECT monitor, bool isPrimaryMonitor)
        {
            APPBARDATA data = new() { cbSize = Marshal.SizeOf(typeof(APPBARDATA)), uEdge = edge, rc = monitor };
            if (SHAppBarMessage(ABM_GETAUTOHIDEBAREX, ref data) != IntPtr.Zero)
            {
                return true;
            }
            // Windows 7 does not know the per-monitor query and only has a Taskbar on the primary monitor.
            return isPrimaryMonitor && SHAppBarMessage(ABM_GETAUTOHIDEBAR, ref data) != IntPtr.Zero;
        }

        #region Win32

        private const int WM_WINDOWPOSCHANGING = 0x0046;

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZE = 0x01000000;
        private const int WS_MINIMIZE = 0x20000000;
        private const int WS_CAPTION = 0x00C00000;

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;

        private const int MONITOR_DEFAULTTONEAREST = 2;
        private const int MONITORINFOF_PRIMARY = 1;

        private const int ABM_GETSTATE = 0x4;
        private const int ABM_GETAUTOHIDEBAR = 0x7;
        private const int ABM_GETAUTOHIDEBAREX = 0xB;
        private const int ABS_AUTOHIDE = 0x1;
        private const int ABE_LEFT = 0;
        private const int ABE_TOP = 1;
        private const int ABE_RIGHT = 2;
        private const int ABE_BOTTOM = 3;

        // Pixels left free for an auto-hide Taskbar; the same amount other desktop applications leave.
        private const int AutoHideTaskbarReveal = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromRect(ref RECT lprc, int dwFlags);

        [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        #endregion
    }
}
