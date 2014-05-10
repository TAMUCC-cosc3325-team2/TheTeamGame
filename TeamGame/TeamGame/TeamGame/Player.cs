using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TeamGame
{
    public enum Player
    {
        // None, t1p1, t2p1, t3p1, t4p1, t1p2, t2p2, t3p2, t4p2, t1p3, t2p3, t3p3, t4p3, t1p4, t2p4, t3p4, t4p4
        None, t1p1, t1p2, t2p1, t2p2, t1p4, t1p3, t2p4, t2p3, t3p1, t3p2, t4p1, t4p2, t3p4, t3p3, t4p4, t4p3
    }

    public static class PlayerExtensions
    {
        public static int TeamOf(this Player p)
        {
            return Enum.GetName(typeof(Player), p)[1] - 48;
        }

        /// <summary>
        /// Returns the color the player appears to himself as.
        /// </summary>
        /// <param name="p">Any player</param>
        /// <returns>One of RGBY</returns>
        public static Color RealColor(this Player p)
        {
            switch (Enum.GetName(typeof(Player), p)[3])
            {
                case '1':
                    return Color.Red;
                case '2':
                    return Color.LawnGreen;
                case '3':
                    return Color.Blue;
                case '4':
                    return Color.Yellow;
                default:
                    return Color.Orange;
            }
        }
        /// <summary>
        /// Returns the color of the player considering which team he's on
        /// </summary>
        /// <param name="p">Any player</param>
        /// <returns>RGBY or Non-team color</returns>
        public static Color ColorOf(this Player p)
        {
            if (p.TeamOf() != Game1.localPlayer.TeamOf())
                return Color.Beige;
            return p.RealColor();
            
        }
        public static string ColorName(this Player p)
        {
            //if (p.TeamOf() != Game1.localPlayer.TeamOf())
            //    return new String("Grey");
            switch (Enum.GetName(typeof(Player), p)[3])
            {
                case '1':
                    return "Red";
                case '2':
                    return "Green";
                case '3':
                    return "Blue";
                case '4':
                    return "Yellow";
                default:
                    return "None";
            }

        }

        public static bool IsLocal(this Player p)
        {
            return (p == Game1.localPlayer);
        }

        public static Rectangle GetRegion(this Player p)
        {
            int border = 4; // two borders between teams
            int rw = 250, rh = 175;

            int row = (int) (p - 1) / 4;
            int col = (int) (p - 1) % 4;

            return new Rectangle(col * rw + (col + 1) * border + ((p.TeamOf() == 2 || p.TeamOf() == 4) ? border : 0),
                                 row * rh + (row + 1) * border + ((p.TeamOf() == 3 || p.TeamOf() == 4) ? border : 0),
                                 rw,
                                 rh);

        }
    }
}
