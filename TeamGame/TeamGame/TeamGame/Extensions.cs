using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

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
        public static bool IsClicked(this ButtonState state)
        {
            return state == ButtonState.Pressed;
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
    }
}
