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
        None, 
        t1p1, t1p2, t2p1, t2p2, 
        t1p4, t1p3, t2p4, t2p3, 
        t3p1, t3p2, t4p1, t4p2, 
        t3p4, t3p3, t4p4, t4p3
    }
    public enum Team
    {
        None, t1, t2, t3, t4
    }

    public static class PlayerExtensions
    {
        public static byte rotations = 0;
        public static bool individualColors = true;

        public static Team TeamOf(this Player p)
        {
            return (Team) (Enum.GetName(typeof(Player), p)[1] - 48);
        }
        public static byte ID(this Player p)
        {
            return (byte) (Enum.GetName(typeof(Player), p)[3] - '0');
        }
        public static Player FromTeamAndID(Team tid, int pid)
        {
            return (Player)Enum.Parse(typeof(Player), Enum.GetName(typeof(Team), tid) + "p" + pid);
        }

        /// <summary>
        /// Returns the color the player appears to himself as.
        /// </summary>
        /// <param name="p">Any player</param>
        /// <returns>One of RGBY</returns>
        public static Color RealColor(this Player p)
        {
            if (!individualColors)
                p = (Player)p.TeamOf();
            switch (p.ID())
            {
                case 1:
                    return Color.Red;
                case 2:
                    return Color.FromNonPremultiplied(25, 255, 10, 255);
                case 3:
                    return Color.Blue;
                case 4:
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

            for (int i = 0; i < rotations; i++)
                p = p.ClockwisePlayer();

            return new Rectangle(col * rw + (col + 1) * border + (((int)p.TeamOf() == 2 || (int)p.TeamOf() == 4) ? border : 0),
                                     row * rh + (row + 1) * border + (((int)p.TeamOf() == 3 || (int)p.TeamOf() == 4) ? border : 0),
                                     rw,
                                     rh);

        }

        public static Rectangle GetRegion(this Team t)
        {
            int border = 4; // two between teams
            int rw = 504, rh = 354;

            switch (t)
            {
                case Team.t1:
                    return new Rectangle(border, border, rw, rh);
                case Team.t2:
                    return new Rectangle(3 * border + rw, border, rw, rh);
                case Team.t3:
                    return new Rectangle(border, 3 * border + rh, rw, rh);
                case Team.t4:
                    return new Rectangle(3 * border + rw, 3 * border + rh, rw, rh);
                default:
                    return new Rectangle(350, 350, rw, rh);
            }
        }

        public static Player ClockwisePlayer(this Player p)
        {
            return (Player) Enum.Parse(typeof(Player), p.TeamOf() + "p" + ((((int) Enum.GetName(typeof(Player), p)[3]) % 4) + 1));
        }
        public static float ClockwiseAngle(this Player p)
        {
            for (int i = 0; i < rotations; i++)
                p = p.ClockwisePlayer();

            switch (Enum.GetName(typeof(Player), p)[3])
            {
                case '1':
                    return 0;
                case '2':
                    return (float) (Math.PI * 0.5);
                case '3':
                    return (float) (Math.PI);
                default:
                    return (float) (Math.PI * 1.5);
            }
        }

        public static List<Player> TeamList(this Player p)
        {
            List<Player> l = new List<Player>(4);
            for (int i = 1; i < 5; i++)
                l.Add(FromTeamAndID(p.TeamOf(), i));

            return l;
        }
    }
}
