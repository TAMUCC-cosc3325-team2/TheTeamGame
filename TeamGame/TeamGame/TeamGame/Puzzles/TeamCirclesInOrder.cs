using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TeamGame.Puzzles
{
    /// <summary>
    /// This "team" puzzle is actually deceptively just four
    /// individual puzzles that all four teammates play at once
    /// </summary>
    class TeamCirclesInOrder : TPuzzle
    {
        

        public TeamCirclesInOrder(Game game, Player player)
            : base(game, player)
        {

        }



    }
}
