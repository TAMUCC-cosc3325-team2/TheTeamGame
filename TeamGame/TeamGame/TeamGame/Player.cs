using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TeamGame
{
    public enum Player
    {
        None, t1p1, t1p2, t1p3, t1p4, t2p1, t2p2, t2p3, t2p4, t3p1, t3p2, t3p3, t3p4, t4p1, t4p2, t4p3, t4p4
    }

    public static class PlayerE
    {
        public static int TeamOf(this Player p)
        {
            return ((int)p - 1) / 4;
        }

        public static Color ColorOf(this Player p)
        {
            if (p.TeamOf() != Game1.localPlayer.TeamOf())
                return Color.Beige;
            switch ((int) p % 4)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.LawnGreen;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.Yellow;
                default:
                    return Color.Teal;
            }
            
        }

        public static bool IsLocal(this Player p)
        {
            return (p == Game1.localPlayer);
        }

        public static Rectangle GetRegion(this Player p)
        {
            int border = 12;
            int rw = 250, rh = 175;

            switch (p)
            {
                case Player.t1p1:
                    return new Rectangle(0, 0, rw, rh);
                case Player.t1p2:
                    return new Rectangle(rw + border, 0, rw, rh);
                case Player.t1p3:
                    return new Rectangle(rw + border, rh + border, rw, rh);
                case Player.t1p4:
                    return new Rectangle(0, rh + border, rw, rh);
                case Player.t2p1:
                    return new Rectangle(0, 2 * rh + border, rw, rh);
                case Player.t2p2:
                    return new Rectangle(2 * rw + border, 0, rw, rh);
                case Player.t2p3:
                    return new Rectangle(2 * rw + border, 2 * rh + border, rw, rh);
                case Player.t2p4:
                    return new Rectangle(0, 2 * rh + border, rw, rh);
                case Player.t3p1:
                    return new Rectangle(0, 2 * rh + border, rw, rh);
                case Player.t3p2:
                    return new Rectangle(2 * rw + border, 0, rw, rh);
                case Player.t3p3:
                    return new Rectangle(2 * rw + border, 2 * rh + border, rw, rh);
                case Player.t3p4:
                    return new Rectangle(0, 2 * rh + border, rw, rh);
                case Player.t4p1:
                    return new Rectangle(0, 2 * rh + border, rw, rh);
                case Player.t4p2:
                    return new Rectangle(2 * rw + border, 0, rw, rh);
                case Player.t4p3:
                    return new Rectangle(2 * rw + border, 2 * rh + border, rw, rh);
                case Player.t4p4:
                    return new Rectangle(0, 2 * rh + border, rw, rh);
                default:
                    return new Rectangle(300, 300, 50, 50);
            }

        }
    }
}
