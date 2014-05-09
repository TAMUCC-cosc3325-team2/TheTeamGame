using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;

namespace TeamGame
{
    public static class Extensions
    {
        public static Vector2 Position(this MouseState mouse)
        {
            return new Vector2(mouse.X, mouse.Y);
        }
        public static Point PPosition(this MouseState mouse)
        {
            return new Point(mouse.X, mouse.Y);
        }
        public static bool IsClicked(this Microsoft.Xna.Framework.Input.ButtonState state)
        {
            return state == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        }
        public static Vector2 Plus(this Vector2 vector, int x, int y)
        {
            return new Vector2(vector.X + x, vector.Y + y);
        }
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        public static bool Contains(this Rectangle r, Vector2 v)
        {
            return r.Contains((int) v.X, (int) v.Y);
        }
        /// Thanks Hans Passant!
        /// http://stackoverflow.com/questions/4305800/using-custom-colored-cursors-in-a-c-windows-application
        public static Cursor LoadCustomCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            if (hCurs == IntPtr.Zero) throw new Win32Exception();
            var curs = new Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(curs, true);
            return curs;
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);

    }
}
