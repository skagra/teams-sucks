using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TeamsSucks
{
    public class TeamsController
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, ref IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        public void ToggleMute()
        {
            var hWnd=SearchForWindow();
            if (hWnd!=(IntPtr)0) {
                
                keybd_event(VK_ALT, 0,  0, IntPtr.Zero);
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, 3);
                keybd_event(VK_ALT, 0, KEYUP, IntPtr.Zero);

                keybd_event(VK_SHIFT, 0, 0, IntPtr.Zero);
                keybd_event(VK_CTRL, 0, 0, IntPtr.Zero);
                keybd_event(VK_M, 0, 0, IntPtr.Zero);
                keybd_event(VK_M, 0, KEYUP, IntPtr.Zero);
                keybd_event(VK_CTRL, 0, KEYUP, IntPtr.Zero);
                keybd_event(VK_SHIFT, 0, KEYUP, IntPtr.Zero);

            }
        }

        public void ToggleCamera()
        {
            var hWnd=SearchForWindow();
            if (hWnd!=(IntPtr)0) {
                
                keybd_event(VK_ALT, 0,  0, IntPtr.Zero);
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, 3);
                keybd_event(VK_ALT, 0, KEYUP, IntPtr.Zero);

                keybd_event(VK_SHIFT, 0, 0, IntPtr.Zero);
                keybd_event(VK_CTRL, 0, 0, IntPtr.Zero);
                keybd_event(VK_O, 0, 0, IntPtr.Zero);
                keybd_event(VK_O, 0, KEYUP, IntPtr.Zero);
                keybd_event(VK_CTRL, 0, KEYUP, IntPtr.Zero);
                keybd_event(VK_SHIFT, 0, KEYUP, IntPtr.Zero);

            }
        }

        public void BringToFront()
        {
            var hWnd=SearchForWindow();
            if (hWnd!=(IntPtr)0) {
                
                keybd_event(VK_ALT, 0,  0, IntPtr.Zero);
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, 3);
                keybd_event(VK_ALT, 0, KEYUP, IntPtr.Zero);
            }
        }

        public int FindTeamsProcess()
        {
            int processId = 0;

            foreach (var p in Process.GetProcesses())
            {
                if (
                    p.ProcessName.StartsWith("Teams")
                    && !String.IsNullOrWhiteSpace(p.MainWindowTitle)
                )
                {
                    Console.WriteLine(
                        p.ProcessName
                            + ":"
                            + p.Id
                            + ":"
                            + p.MainWindowTitle
                            + ":"
                            + p.MainWindowHandle
                    );
                    processId = p.Id;
                }
            }
            return processId;
        }

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CTRL = 0x11;
        private const byte VK_ALT = 0x12; 
        private const byte VK_M = 0x4D; // Mute
        private const byte VK_O = 0x4F; // Camera

        private const int KEYUP = 0x2;

        private const int SW_SHOWMAXIMIZED = 3;

        private bool EnumProc(IntPtr hWnd, ref IntPtr lParam)
        {
            var sb = new StringBuilder(1024);
            GetWindowText(hWnd, sb, sb.Capacity);
            
            //  GetWindowThreadProcessId() - can use to check this is teams
            if (sb.ToString().StartsWith("Meeting"))
            {
                Console.WriteLine($"Window title: {sb.ToString()}");
                lParam = hWnd;
                return false;
            }

            return true;
        }

        private IntPtr SearchForWindow()
        {
            IntPtr hWnd = (IntPtr)0;
            EnumWindows(new EnumWindowsProc(EnumProc), ref hWnd);

            Console.WriteLine(hWnd);

            return hWnd;
        }
    }
}
