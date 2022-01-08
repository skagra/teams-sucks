using System;
using System.Diagnostics; 
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TeamsSucks
{
    class TeamsController
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref SearchData data);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static IntPtr SearchForWindow(string wndclass, string title)
        {
            SearchData sd = new SearchData { Wndclass = wndclass, Title = title };
            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnd;
        }

        public static bool EnumProc(IntPtr hWnd, ref SearchData data)
        {
            // Check classname and title
            // This is different from FindWindow() in that the code below allows partial matches
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);
          //  Console.WriteLine("CN:"+sb);
            //if (sb.ToString().StartsWith(data.Wndclass))
            //{
                sb = new StringBuilder(1024);
                GetWindowText(hWnd, sb, sb.Capacity);

                if (sb.ToString().StartsWith("Meeting"))
                {
                    Console.WriteLine(sb.ToString());
                    data.hWnd = hWnd;

                    uint processID = 0;
                    uint threadID = GetWindowThreadProcessId(hWnd, out processID);

                    Console.WriteLine(processID);

                    return false;    // Found the wnd, halt enumeration
                }
            //}
            return true;
        }

        public class SearchData
        {
            // You can put any dicks or Doms in here...
            public string Wndclass;
            public string Title;
            public IntPtr hWnd;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, ref SearchData data);



        static void MainX(string[] args)
        {

            SearchForWindow("IEFrame", "pinvoke.net: EnumWindows");

            Process[] processlist = Process.GetProcesses();

            foreach (var p in processlist)
            {
                if (p.ProcessName.StartsWith("Teams") && !String.IsNullOrWhiteSpace(p.MainWindowTitle))
                {
                    Console.WriteLine(p.ProcessName + ":" +p.Id);


                }

                // Supply p.id and use GetWindwoThreadProcessId

                //var handle = processlist.
                //    Where(p => p.ProcessName == "Teams").
                //    First(p => p.MainWindowTitle.StartsWith("Meeting")).
                //    MainWindowHandle;

                //Console.WriteLine(handle);

                //bool oldHandle = SetForegroundWindow(handle);

                //Console.WriteLine(oldHandle);

                //keybd_event(0x10, 0, 0, IntPtr.Zero);
                //keybd_event(0x11, 0, 0, IntPtr.Zero);
                //keybd_event(0x4D, 0, 0, IntPtr.Zero);
                //keybd_event(0x4D, 0, 0x0002, IntPtr.Zero);
                //keybd_event(0x11, 0, 0x0002, IntPtr.Zero);
                //keybd_event(0x10, 0, 0x0002, IntPtr.Zero);
            }
        }
    }
}
