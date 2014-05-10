using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace TeamGame.Puzzles
{
    class Transition : IPuzzle
    {
        int countUpdates = 0;

        public Transition(Game game, Player player)
            : base(game, player)
        {
            
        }

        public override void Initialize()
        {

        }

        public override void Draw(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return;

            countUpdates += 1;

            if (countUpdates > 150)
                PuzzleOver(true);

        }

        public new void PuzzleOver(bool p)
        {
            Game.Components.Remove(this);
            byte randomPuzzle = (byte) (Game1.random.Next(1, 3) * 2);
            Game1.pStates[this.player].puzzle = randomPuzzle.CreateFromID(this.Game, this.player);
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            
        }

        public override void Decode(NetIncomingMessage msg)
        {
            
        }
    }
}
