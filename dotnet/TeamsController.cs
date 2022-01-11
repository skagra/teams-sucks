using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace TeamsSucks
{
    public class TeamsController
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);



        public delegate bool EnumWindowsCProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsCProc lpEnumFunc, IntPtr lParam);

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowsCProc childProc = new EnumWindowsCProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        /// <summary>
        /// Callback method to be used when enumerating windows.
        /// </summary>
        /// <param name="handle">Handle of the next window</param>
        /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
        /// <returns>True to continue the enumeration, false to bail</returns>
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
            //}

        }

        public void ToggleMute()
        {
            var hWnd = SearchForWindow();
            if (hWnd != (IntPtr)0)
            {

            //    BringWindowToTop(hWnd);
            //// SetActiveWindow(hWnd);
            //SetForegroundWindow(hWnd);

                //keybd_event(VK_ALT, 0, 0, IntPtr.Zero);
                //SetForegroundWindow(hWnd);
                //ShowWindow(hWnd, 3);
                //keybd_event(VK_ALT, 0, KEYUP, IntPtr.Zero);

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
            var hWnd = SearchForWindow();
            if (hWnd != (IntPtr)0)
            {

                //keybd_event(VK_ALT, 0, 0, IntPtr.Zero);
                //SetForegroundWindow(hWnd);
                //ShowWindow(hWnd, 3);
                //keybd_event(VK_ALT, 0, KEYUP, IntPtr.Zero);

                //BringWindowToTop(hWnd);
                //// SetActiveWindow(hWnd);
                //SetForegroundWindow(hWnd);

                keybd_event(VK_SHIFT, 0, 0, IntPtr.Zero);
                keybd_event(VK_CTRL, 0, 0, IntPtr.Zero);
                keybd_event(VK_O, 0, 0, IntPtr.Zero);
                keybd_event(VK_O, 0, KEYUP, IntPtr.Zero);
                keybd_event(VK_CTRL, 0, KEYUP, IntPtr.Zero);
                keybd_event(VK_SHIFT, 0, KEYUP, IntPtr.Zero);

            }
        }

        //public void BringToFront()
        //{
        //    var hWnd = SearchForWindow();
        //    if (hWnd != (IntPtr)0)
        //    {

        //        keybd_event(VK_ALT, 0, 0, IntPtr.Zero);
        //        SetForegroundWindow(hWnd);
        //        ShowWindow(hWnd, 3);
        //        keybd_event(VK_ALT, 0, KEYUP, IntPtr.Zero);
        //    }
        //}

        public void BringToFront()
        {
            var hWnd = SearchForWindow();
            if (hWnd != (IntPtr)0)
            {

                BringWindowToTop(hWnd);
               // SetActiveWindow(hWnd);
                SetForegroundWindow(hWnd);
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

       
        private IntPtr GetTeamsMainWindowHandle()
        {
            IntPtr result = (IntPtr)0;

            Process[] procs = Process.GetProcesses(".");
            foreach (Process proc in procs)
            {
                if (proc.ProcessName == "Teams")
                {
                    Console.WriteLine(proc.MainModule?.ModuleName);
                   
                    if (proc.MainWindowHandle!=(IntPtr)0)
                    {
                        result = proc.MainWindowHandle;
                    }
                }
            }

            return result;
        }

        private List<IntPtr> TeamsWindows;

        private bool EnumProc(IntPtr hWnd, ref IntPtr lParam)
        {
            var sb = new StringBuilder(1024);
            GetWindowText(hWnd, sb, sb.Capacity);

            //  GetWindowThreadProcessId() - can use to check this is teams
            if (sb.ToString().EndsWith("| Microsoft Teams"))
            {
               // Console.WriteLine($"Window title: {sb.ToString()}");
                lParam = hWnd;

                Console.WriteLine("Window name="+sb+" hWnd=" + hWnd);

                TeamsWindows.Add(hWnd);


                //StringBuilder ClassName = new StringBuilder(256);
                ////Get the window class name
                //GetClassName(hWnd, ClassName, ClassName.Capacity);
                //if (ClassName.ToString() == "Chrome_WidgetWin_1")
                //{
                //    Console.WriteLine("Window Name: "+sb + " ClassName:" + ClassName);

                //    var children = GetChildWindows(hWnd);
                //    foreach (IntPtr cw in children)
                //    {
                //        //Console.WriteLine(cw);
                //        GetClassName(cw, ClassName, ClassName.Capacity);
                //        Console.WriteLine("Child window class name:" + ClassName);
                //        GetWindowText(cw, sb, sb.Capacity);
                //        Console.WriteLine("Child Window Name: "+ sb);
                //    }

                //    Console.WriteLine();

                //}


                return true;
            }

            return true;
        }

        private void ShowWindHierarchy(IntPtr handle)
        {
            var sb = new StringBuilder(1024);

            
            GetWindowText(handle, sb, sb.Capacity);
            Console.WriteLine("----> " + handle + ":" + sb);

            var children=GetChildWindows(handle);
            foreach (var child in children)
            {
                ShowWindHierarchy(child);
            }
        }

        private IntPtr SearchForWindow()
        {
            IntPtr mainWindow=GetTeamsMainWindowHandle();

            TeamsWindows = new List<IntPtr>();

            IntPtr hWnd = (IntPtr)0;
            EnumWindows(new EnumWindowsProc(EnumProc), ref hWnd);

            Console.WriteLine("RESULTS -->");
            Console.WriteLine("Main window: " + mainWindow);
            Console.WriteLine("All Windows: " + String.Join(", ", TeamsWindows));
           // Console.WriteLine("Iterating over child windows tree");

            //foreach (IntPtr handle in TeamsWindows) {
            //    ShowWindHierarchy(handle);
            //}

            Console.WriteLine("<--- RESULTS");

            
            var result= TeamsWindows.Where(h => h != mainWindow).FirstOrDefault();

            Console.WriteLine("And the winner is " + result);

            return result;
        }
    }
}
