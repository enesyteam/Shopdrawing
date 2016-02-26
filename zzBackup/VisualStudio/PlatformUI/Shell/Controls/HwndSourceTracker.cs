using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    internal static class HwndSourceTracker
    {
        private static Dictionary<HwndSource, int> trackedSources;

        static HwndSourceTracker()
        {
            HwndSourceTracker.trackedSources = new Dictionary<HwndSource, int>();
        }

        public static IDisposable AddSource(HwndSource source)
        {
            if (HwndSourceTracker.trackedSources.ContainsKey(source))
            {
                Dictionary<HwndSource, int> item = HwndSourceTracker.trackedSources;
                Dictionary<HwndSource, int> hwndSources = item;
                HwndSource hwndSource = source;
                item[hwndSource] = hwndSources[hwndSource] + 1;
            }
            else
            {
                HwndSourceTracker.trackedSources[source] = 1;
                source.AddHook(new HwndSourceHook(HwndSourceTracker.HwndSource_MessageHook));
            }
            return new HwndSourceTracker.HwndSourceHookTracker(source);
        }

        private static IntPtr HwndSource_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 7:
                    {
                        HwndSource hwndSource = HwndSource.FromHwnd(hwnd);
                        FocusHelper.ClearRestoreFocusWindowIf(hwndSource, (IntPtr currentValue) => !HwndSourceTracker.IsValidRestoreFocusWindowForSource(hwndSource, currentValue));
                        break;
                    }
                case 8:
                    {
                        if (!HwndSourceTracker.IsNonChildDescendant(wParam, hwnd))
                        {
                            break;
                        }
                        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback((object oFocusedKeyboardElement) =>
                        {
                            FocusHelper.SetRestoreFocusFields(HwndSource.FromHwnd(hwnd), oFocusedKeyboardElement, IntPtr.Zero);
                            return null;
                        }), Keyboard.FocusedElement);
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        private static bool IsNonChildDescendant(IntPtr window, IntPtr parent)
        {
            while (window != IntPtr.Zero && window != parent)
            {
                IntPtr intPtr = NativeMethods.GetParent(window);
                if (intPtr == parent && (NativeMethods.GetWindowLong(window, NativeMethods.GWL.STYLE) & 1073741824) == 0)
                {
                    return HwndSource.FromHwnd(window) == null;
                }
                window = intPtr;
            }
            return false;
        }

        private static bool IsValidRestoreFocusWindowForSource(HwndSource source, IntPtr hwndRestoreFocus)
        {
            if (hwndRestoreFocus == IntPtr.Zero || !NativeMethods.IsWindowEnabled(hwndRestoreFocus) || !NativeMethods.IsWindowVisible(hwndRestoreFocus))
            {
                return false;
            }
            return NativeMethods.IsChild(source.Handle, hwndRestoreFocus);
        }

        private static void RemoveSource(HwndSource source)
        {
            Dictionary<HwndSource, int> item = HwndSourceTracker.trackedSources;
            Dictionary<HwndSource, int> hwndSources = item;
            HwndSource hwndSource = source;
            item[hwndSource] = hwndSources[hwndSource] - 1;
            if (HwndSourceTracker.trackedSources[source] == 0)
            {
                HwndSourceTracker.trackedSources.Remove(source);
                source.RemoveHook(new HwndSourceHook(HwndSourceTracker.HwndSource_MessageHook));
            }
        }

        [CompilerGenerated]
        // <>c__DisplayClass3
        private sealed class u003cu003ec__DisplayClass3
        {
            public IntPtr hwnd;

            public u003cu003ec__DisplayClass3()
            {
            }

            // <HwndSource_MessageHook>b__0
            public object u003cHwndSource_MessageHooku003eb__0(object oFocusedKeyboardElement)
            {
                HwndSource hwndSource = HwndSource.FromHwnd(this.hwnd);
                FocusHelper.SetRestoreFocusFields(hwndSource, oFocusedKeyboardElement, IntPtr.Zero);
                return null;
            }
        }

        [CompilerGenerated]
        // <>c__DisplayClass5
        private sealed class u003cu003ec__DisplayClass5
        {
            // CS$<>8__locals4
            public HwndSourceTracker.u003cu003ec__DisplayClass3 CSu0024u003cu003e8__locals4;

            public HwndSource source;

            public u003cu003ec__DisplayClass5()
            {
            }

            // <HwndSource_MessageHook>b__1
            public bool u003cHwndSource_MessageHooku003eb__1(IntPtr currentValue)
            {
                return !HwndSourceTracker.IsValidRestoreFocusWindowForSource(this.source, currentValue);
            }
        }

        private class HwndSourceHookTracker : DisposableObject
        {
            private HwndSource Source
            {
                get;
                set;
            }

            public HwndSourceHookTracker(HwndSource source)
            {
                this.Source = source;
            }

            protected override void DisposeManagedResources()
            {
                HwndSourceTracker.RemoveSource(this.Source);
            }
        }
    }
}