using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;

namespace TeamGame.Puzzles
{
    class AwaitingParticipants : IPuzzle
    {

        public Rectangle drawRegion;
        public Player player;
        public bool awaitingParticipants, t1p1, t1p2, t1p3, t1p4, t2p1, t2p2, t2p3, t2p4, t3p1, t3p2, t3p3, t3p4, t4p1, t4p2, t4p3, t4p4;
        //public Matrix matrix { get { return Matrix.CreateTranslation(drawRegion.Location.X, drawRegion.Location.Y, 0); } }

        SoundEffectInstance buttonBlip, statusZero;

        bool statusZeroPlayed;


        public Texture2D awaitingPlayer;

        public AwaitingParticipants(Game game, Player player)
            : base(game, player)
        {
            t1p1 = t1p2 = t1p3 = t1p4 = t2p1 = t2p2 = t2p3 = t2p4 = t3p1 = t3p2 = t3p3 = t3p4 = t4p1 = t4p2 = t4p3 = t4p4 = false;
            awaitingParticipants = true;
        }



        public override void Initialize() 
        {
            awaitingPlayer = Game.Content.Load<Texture2D>("art/awaitingPlayer");
        }
        public override void Update(GameTime gameTime) 
        { 
            foreach (Player p in Enum.GetValues(typeof(Player)))
            {
                switch (p)
                {
                    case Player.t1p1:
                    case Player.t1p2:
                    case Player.t1p3:
                    case Player.t1p4:
                    case Player.t2p1:
                    case Player.t2p2:
                    case Player.t2p3:
                    case Player.t2p4:
                    case Player.t3p1:
                    case Player.t3p2:
                    case Player.t3p3:
                    case Player.t3p4:
                    case Player.t4p1:
                    case Player.t4p2:
                    case Player.t4p3:
                    case Player.t4p4:
                    case Player.None:
                        awaitingParticipants = true;
                        break;
                }

            }
            if (!awaitingParticipants)
                PuzzleOver(true);

        }
        public override void Encode(NetOutgoingMessage msg) 
        { 

        }
        public override void Decode(NetIncomingMessage msg) 
        { 

        }
        public override void PuzzleOver(bool Correct) 
        {
            Game.Components.Remove(this);
            byte randomPuzzle = (byte)(Game1.random.Next(2, 6));
            Game1.pStates[this.player].puzzle = randomPuzzle.CreateFromID(this.Game, this.player);
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            foreach (Player p in Enum.GetValues(typeof(Player)))
                if(p != Player.None)
                    spriteBatch.Draw(awaitingPlayer, p.GetRelativeRegion(), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
