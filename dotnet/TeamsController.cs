using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NLog;

// TODO: Add teams is running check
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

      [DllImport("user32.dll")]
      private static extern int ShowWindow(IntPtr hWnd, uint Msg);

      [DllImport("user32.dll", SetLastError = true)]
      static extern bool BringWindowToTop(IntPtr hWnd);

      [DllImport("user32.dll")]
      private static extern IntPtr GetForegroundWindow();

      private const byte VK_SHIFT = 0x10;
      private const byte VK_CTRL = 0x11;
      private const byte VK_M = 0x4D; // Mute
      private const byte VK_O = 0x4F; // Camera
      private const int KEYUP = 0x2;
      private const int SW_SHOWMAXIMIZED = 3;

      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      public TeamsController()
      {
      }

      public bool ToggleMute()
      {
         bool ok = false;

         var allTeamsWindows = GetAllTeamsWindows();
         IntPtr activeWindow = GetForegroundWindow();

         ok = allTeamsWindows.Contains(activeWindow);
         if (!ok)
         {
            ok = CycleTeamsWindows();
         }

         if (ok)
         {
            keybd_event(VK_SHIFT, 0, 0, IntPtr.Zero);
            keybd_event(VK_CTRL, 0, 0, IntPtr.Zero);
            keybd_event(VK_M, 0, 0, IntPtr.Zero);
            keybd_event(VK_M, 0, KEYUP, IntPtr.Zero);
            keybd_event(VK_CTRL, 0, KEYUP, IntPtr.Zero);
            keybd_event(VK_SHIFT, 0, KEYUP, IntPtr.Zero);
         }
         else
         {
            _logger.Info("Current window is not a Teams window");
         }

         return ok;
      }

      public bool ToggleCamera()
      {
         bool ok = false;

         var allTeamsWindows = GetAllTeamsWindows();
         IntPtr activeWindow = GetForegroundWindow();

         ok = allTeamsWindows.Contains(activeWindow);
         if (!ok)
         {
            ok = CycleTeamsWindows();
         }

         if (ok)
         {
            keybd_event(VK_SHIFT, 0, 0, IntPtr.Zero);
            keybd_event(VK_CTRL, 0, 0, IntPtr.Zero);
            keybd_event(VK_O, 0, 0, IntPtr.Zero);
            keybd_event(VK_O, 0, KEYUP, IntPtr.Zero);
            keybd_event(VK_CTRL, 0, KEYUP, IntPtr.Zero);
            keybd_event(VK_SHIFT, 0, KEYUP, IntPtr.Zero);
         }
         else
         {
            _logger.Info("Current window is not a Teams window");
         }

         return ok;
      }

      public bool CycleTeamsWindows()
      {
         bool ok = false;

         var hWnd = GetNextWindow();
         if (hWnd != (IntPtr)0)
         {
            ShowWindow(hWnd, SW_SHOWMAXIMIZED);
            BringWindowToTop(hWnd);
            SetForegroundWindow(hWnd);
            ok = true;
         }
         else
         {
            _logger.Info("Teams is not running");
         }

         return ok;
      }

      private IntPtr GetTeamsMainWindowHandle()
      {
         IntPtr result = (IntPtr)0;

         Process[] procs = Process.GetProcesses(".");
         foreach (Process proc in procs)
         {
            if (proc.ProcessName == "Teams" &&
                proc.MainModule?.ModuleName == "Teams.exe")
            {

               result = proc.MainWindowHandle;
               break;
            }
         }

         return result;
      }

      private List<IntPtr> GetAllTeamsWindows()
      {
         var teamsWindows = new List<IntPtr>();

         bool EnumProc(IntPtr hWnd, ref IntPtr lParam)
         {
            var sb = new StringBuilder(1024);
            GetWindowText(hWnd, sb, sb.Capacity);

            if (sb.ToString().EndsWith("| Microsoft Teams"))
            {
               lParam = hWnd;

               teamsWindows.Add(hWnd);

               return true;
            }

            return true;
         }

         IntPtr hWnd = (IntPtr)0;
         EnumWindows(new EnumWindowsProc(EnumProc), ref hWnd);

         return teamsWindows;
      }


      private IntPtr GetNextWindow()
      {
         IntPtr result = (IntPtr)0;

         var mainWindow = GetTeamsMainWindowHandle();

         if (mainWindow != (IntPtr)0)
         {
            var allTeamsWindows = GetAllTeamsWindows();
            allTeamsWindows.Sort();

            if (allTeamsWindows.Count() > 0)
            {
               if (allTeamsWindows.Count() == 1)
               {
                  result = mainWindow;
               }
               else
               {
                  var indexOfMainWindow = allTeamsWindows.Select((handle, index) => (handle, index)).First(hi => hi.handle == mainWindow).index;

                  if (indexOfMainWindow == allTeamsWindows.Count - 1)
                  {
                     result = allTeamsWindows[0];
                  }
                  else
                  {
                     result = allTeamsWindows[indexOfMainWindow + 1];
                  }
               }
            }
         }

         return result;
      }
   }
}

